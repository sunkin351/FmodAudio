using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Examples.Base;

using FmodAudio;
using FmodAudio.Base;

namespace Examples
{
    internal class FileHandleManager
    {
        private readonly LinkedList<Item> ItemList = new();

        private readonly ConcurrentDictionary<long, FileStream> Handles = new();
        private long FileHandleInc = 1;

        public FileHandleManager()
        {
        }

        public IntPtr CreateHandle(FileStream stream)
        {
            long handle = Interlocked.Increment(ref FileHandleInc);

            Handles.TryAdd(handle, stream);

            return (IntPtr)handle;
        }

        public bool DisposeHandle(IntPtr handle)
        {
            if (Handles.TryRemove((long)handle, out var stream))
            {
                stream.Dispose();

                return true;
            }

            return false;
        }

        public bool TryGetStream(IntPtr handle, out FileStream stream)
        {
            return Handles.TryGetValue((long)handle, out stream);
        }

        public void CreateAsyncOperation(AsyncReadInfoPointer asyncInfo)
        {
            var cancelationSource = new CancellationTokenSource();

            LinkedListNode<Item> node;

            lock (ItemList)
            {
                node = ItemList.AddLast(new Item
                {
                    InfoPtr = asyncInfo,
                    CancelationSource = cancelationSource
                });
            }

            var token = cancelationSource.Token;

            var asyncOp = Task.Run(async () =>
            {
                if (token.IsCancellationRequested)
                {
                    asyncInfo.Done(Result.Err_File_DiskEjected);
                    goto RemoveOperation;
                }

                if (!TryGetStream(asyncInfo.Handle, out var stream))
                {
                    asyncInfo.Done(Result.Err_Invalid_Handle);
                    goto RemoveOperation;
                }

                try
                {
                    stream.Seek(asyncInfo.Offset, SeekOrigin.Begin);

                    if (token.IsCancellationRequested)
                    {
                        asyncInfo.Done(Result.Err_File_DiskEjected);

                        goto RemoveOperation;
                    }

                    asyncInfo.BytesRead = (uint)await stream.ReadAsync(asyncInfo.GetMemoryManager().Memory, token);

                    asyncInfo.Done(Result.Ok);
                }
                catch (OperationCanceledException)
                {
                    asyncInfo.Done(Result.Err_File_DiskEjected);
                }
                catch
                {
                    asyncInfo.Done(Result.Err_Internal);
                }

            RemoveOperation:
                RemoveAsyncOperation(asyncInfo);
            });

            node.ValueRef.AsyncOperation = asyncOp;
        }

        public bool CancelAsyncOperation(AsyncReadInfoPointer asyncInfo)
        {
            lock (ItemList)
            {
                var node = this.FindItem(asyncInfo);

                if (node != null)
                {
                    node.ValueRef.CancelationSource.Cancel();
                    return true;
                }

                return false;
            }
        }

        private unsafe LinkedListNode<Item>? FindItem(AsyncReadInfoPointer ptr)
        {
            var node = ItemList.First;

            if (node != null)
            {
                do
                {
                    ref Item item = ref node.ValueRef;

                    if (item.InfoPtr.GetPointer() == ptr.GetPointer())
                    {
                        return node;
                    }
                }
                while ((node = node.Next) != null);
            }

            return null;
        }

        private void RemoveAsyncOperation(AsyncReadInfoPointer asyncInfo)
        {
            lock (ItemList)
            {
                var node = FindItem(asyncInfo);

                this.ItemList.Remove(node);
            }
        }

        private struct Item
        {
            public AsyncReadInfoPointer InfoPtr;
            public Task AsyncOperation;
            public CancellationTokenSource CancelationSource;
        }
    }

    public unsafe class AsyncIOExample : Example
    {
        //Static variables
        private static readonly FileHandleManager Manager = new();

        private Sound sound;

        public AsyncIOExample() : base("Fmod Async IO Example")
        {
            RegisterCommand(ConsoleKey.D1, () =>
            {
                if (sound != default)
                {
                    sound.Release();
                    sound = default;
                    Log("Released Sound");
                }
            });
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32, InitFlags.Normal);

            System.SetStreamBufferSize(32768, TimeUnit.RAWBytes);

            System.SetFileSystem(&MyOpen, &MyClose, &MyRead, &MySeek, &MyAsyncRead, &MyAsyncCancel, 2048);

            sound = System.CreateStream(MediaPath("wave.mp3"), Mode.Loop_Normal | Mode._2D | Mode.IgnoreTags);
        }

        public override void Run()
        {
            Channel channel = System.PlaySound(sound);

            do
            {
                OnUpdate();

                if (sound != default)
                {
                    sound.GetOpenState(out _, out _, out FmodBool starving, out _);

                    if (starving)
                    {
                        Log("Starving");
                    }

                    channel.Mute = starving;
                }

                System.Update();

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
            while (!ShouldEndExample);
        }

        public override void Dispose()
        {
            if (sound != default)
                sound.Dispose(); //Dispose if not null
            
            base.Dispose();
        }

        const int BufferLength = 5;
        private static readonly LinkedList<string> LogBuffer = new LinkedList<string>();

        internal static void Log(string line)
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

        private static void DrawLog()
        {
            lock (LogBuffer)
            {
                foreach (var item in LogBuffer)
                {
                    DrawText(item);
                }
            }
        }
        
        [UnmanagedCallersOnly]
        private static unsafe Result MyOpen(byte* Filename, uint* filesize, IntPtr* handle, IntPtr userdata)
        {
            *filesize = 0;
            *handle = default;

            Log("Call to MyOpen()");

            string name = FmodHelpers.PtrToStringUnknownSize(Filename);

            FileStream stream;
            try
            {
                stream = new FileStream(name, FileMode.Open, FileAccess.Read);
            }
            catch(FileNotFoundException)
            {
                return Result.Err_File_NotFound;
            }
            catch
            {
                return Result.Err_File_Bad;
            }
            
            *filesize = (uint)stream.Length;

            *handle = Manager.CreateHandle(stream);

            return Result.Ok;
        }

        [UnmanagedCallersOnly]
        private static Result MyClose(IntPtr handle, IntPtr userdata)
        {
            Log("Call to MyClose()");

            return Manager.DisposeHandle(handle) ? Result.Ok : Result.Err_Invalid_Handle;
        }

        [UnmanagedCallersOnly]
        private static Result MyRead(IntPtr handle, byte* buffer, uint sizeBytes, uint* bytesRead, IntPtr userdata)
        {
            *bytesRead = 0;

            Log("Call to MyRead()");

            if (!Manager.TryGetStream(handle, out var stream))
            {
                return Result.Err_Invalid_Handle;
            }

            try
            {
                *bytesRead = (uint)stream.Read(new Span<byte>(buffer, (int)sizeBytes));
            }
            catch (EndOfStreamException)
            {
                return Result.Err_File_EndOfData;
            }
            catch
            {
                return Result.Err_Internal;
            }

            if (*bytesRead < sizeBytes)
            {
                return Result.Err_File_EOF;
            }
            else
            {
                return Result.Ok;
            }
        }

        [UnmanagedCallersOnly]
        private static Result MySeek(IntPtr handle, uint pos, IntPtr userdata)
        {
            Log("Call to MySeek()");

            if (!Manager.TryGetStream(handle, out var stream))
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

        [UnmanagedCallersOnly]
        private static Result MyAsyncRead(AsyncReadInfo* info, IntPtr userdata)
        {
            Log("Call to MyAsyncRead()");

            try
            {
                Manager.CreateAsyncOperation(new AsyncReadInfoPointer(info));
                return Result.Ok;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        [UnmanagedCallersOnly]
        private static unsafe Result MyAsyncCancel(AsyncReadInfo* info, IntPtr userdata)
        {
            Log("Call to MyAsyncCancel()");

            return Manager.CancelAsyncOperation(new AsyncReadInfoPointer(info)) ? Result.Err_File_DiskEjected : Result.Ok;
        }
    }
}
