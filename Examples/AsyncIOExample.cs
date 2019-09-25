using FmodAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Examples
{
    using Base;

    public unsafe class AsyncIOExample : Example
    {
        //Static variables
        static readonly object SyncPoint = new object();
        static readonly LinkedList<IntPtr> AsyncReads = new LinkedList<IntPtr>();
        static readonly Dictionary<long, FileStream> Handles = new Dictionary<long, FileStream>();
        static long FileHandleInc = 1;
        static bool ThreadContinue = true;
        static readonly Thread AsyncThread = new Thread(ProcessThread);
        
        public override void Run()
        {
            FmodSystem system = Fmod.CreateSystem();

            TestVersion(system);

            AsyncThread.Start();

            system.Init(32, InitFlags.Normal);

            system.SetStreamBufferSize(32768, TimeUnit.RAWBytes);

            system.SetFileSystem(MyOpen, MyClose, MyRead, MySeek, MyAsyncRead, MyAsyncCancel, 2048); //Experimental

            var sound = system.CreateStream(MediaPath("wave.mp3"), Mode.Loop_Normal | Mode._2D | Mode.IgnoreTags);
            var channel = system.PlaySound(sound);

            do
            {
                OnUpdate();

                if (sound != null)
                {
                    sound.GetOpenState(out _, out _, out bool starving, out _);

                    if (starving)
                    {
                        Log("Starving");
                    }

                    channel.Mute = starving;
                }

                if (!Commands.IsEmpty)
                {
                    while (Commands.TryDequeue(out Button btn))
                    {
                        if (btn == Button.Quit)
                            goto Exit;

                        if (sound != null && btn == Button.Action1)
                        {
                            sound.Release();
                            sound = null;
                            Log("Released Sound");
                        }
                    }
                }

                system.Update();

                DrawText("==================================================");
                DrawText("Async IO Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to release playing stream");
                DrawText("Press Esc to quit");
                DrawText();
                DrawLog();

                Sleep(50);
            }
            while (true);

            Exit:
            sound?.Dispose(); //Dispose if not null

            ThreadContinue = false;

            AsyncThread.Join();

            system.Dispose();
        }

        const int BufferLength = 5;
        static LinkedList<string> LogBuffer = new LinkedList<string>();

        static void Log(string line)
        {
            lock (LogBuffer)
            {
                if (LogBuffer.Count >= BufferLength)
                {
                    LogBuffer.RemoveFirst();
                }

                LogBuffer.AddLast(line);
            }
        }

        void DrawLog()
        {
            lock (LogBuffer)
            {
                foreach (var item in LogBuffer)
                {
                    DrawText(item);
                }
            }
        }
        
        static IntPtr AllocateHandle(FileStream stream)
        {
            long tmp = FileHandleInc++;
            lock(Handles)
            {
                Handles.Add(tmp, stream);
            }
            return new IntPtr(tmp);
        }

        static bool GetStream(IntPtr handle, out FileStream stream)
        {
            lock(Handles)
                return Handles.TryGetValue((long)handle, out stream);
        }

        static void ReleaseHandle(IntPtr handle)
        {
            lock(Handles)
                Handles.Remove((long)handle);
        }

        static Result MyOpen(string Filename, out uint filesize, out IntPtr handle, IntPtr userdata)
        {
            filesize = 0;
            handle = IntPtr.Zero;

            FileStream stream;
            try
            {
                stream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            }
            catch(FileNotFoundException)
            {
                return Result.Err_File_NotFound;
            }
            catch
            {
                return Result.Err_File_Bad;
            }
            
            filesize = (uint)stream.Length;

            handle = AllocateHandle(stream);

            return Result.Ok;
        }

        static Result MyClose(IntPtr handle, IntPtr userdata)
        {
            if (!GetStream(handle, out var stream))
            {
                return Result.Err_Invalid_Handle;
            }

            stream.Dispose();

            ReleaseHandle(handle);

            return Result.Ok;
        }

        static Result MyRead(IntPtr handle, IntPtr buffer, uint sizeBytes, out uint bytesRead, IntPtr userdata)
        {
            bytesRead = 0;

            if (!GetStream(handle, out var stream))
            {
                return Result.Err_Invalid_Handle;
            }
            
            bytesRead = (uint)stream.Read(new Span<byte>(buffer.ToPointer(), (int)sizeBytes));

            if (bytesRead < sizeBytes)
            {
                return Result.Err_File_EOF;
            }
            else
            {
                return Result.Ok;
            }
        }

        static Result MySeek(IntPtr handle, uint pos, IntPtr userdata)
        {
            if (!GetStream(handle, out var stream))
            {
                return Result.Err_Invalid_Handle;
            }

            try
            {
                stream.Seek(pos, SeekOrigin.Begin);
            }
            catch
            {
                return Result.Err_Internal;
            }

            return Result.Ok;
        }

        static Result MyAsyncRead(IntPtr info, IntPtr userdata)
        {
            Debug.Assert(info != IntPtr.Zero);
            lock(SyncPoint) //Alternatively, you can use the AsyncReads object as the syncpoint
            {
                Debug.Assert(!AsyncReads.Contains(info));
                AsyncReads.AddLast(info);
            }

            return Result.Ok;
        }

        static unsafe Result MyAsyncCancel(IntPtr info, IntPtr userdata)
        {
            Debug.Assert(info != IntPtr.Zero);
            lock (SyncPoint)
            {
                //Find the pending IO Request and remove it
                var tmp = AsyncReads.Find(info);
                if (tmp != null)
                {
                    AsyncReads.Remove(tmp);

                    //Signal FMOD to wake up, this operation has been cancelled
                    ((AsyncReadInfo*)info)->InvokeDoneCallback(Result.Err_File_DiskEjected);
                    return Result.Err_File_DiskEjected;
                }
            }
            //IO request not found, it must have completed already
            return Result.Ok;
        }

        static void ProcessThread()
        {
            while (ThreadContinue)
            {
                IntPtr info = IntPtr.Zero;
                lock(SyncPoint)
                {
                    if (AsyncReads.Count > 0)
                    {
                        info = AsyncReads.First.Value;
                        AsyncReads.RemoveFirst();
                    }
                }

                if (info != IntPtr.Zero)
                {
                    AsyncReadInfo* ptr = (AsyncReadInfo*)info;

                    Debug.Assert(ptr->Buffer != IntPtr.Zero, "The FMOD library failed to provide a buffer for the file data");

                    if (!GetStream(ptr->Handle, out var stream))
                    {
                        ptr->InvokeDoneCallback(Result.Err_Invalid_Handle);
                        continue;
                    }

                    try
                    {
                        stream.Seek(ptr->Offset, SeekOrigin.Begin);

                        var buffer = new Span<byte>(ptr->Buffer.ToPointer(), (int)ptr->SizeBytes);

                        ptr->BytesRead = (uint)stream.Read(buffer);
                    }
                    catch(Exception e)
                    {
                        ptr->InvokeDoneCallback(Result.Err_Internal);
                        //TODO Implement Error Logging
                        continue;
                    }

                    ptr->InvokeDoneCallback(Result.Ok);
                }
                else
                {
                    Sleep(10); //Example only: Use your native filesystem synchronisation to wait for more requests
                }
            }
        }
        
    }
}
