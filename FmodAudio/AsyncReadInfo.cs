using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    using Interop;

    [StructLayout(LayoutKind.Sequential)]
    public struct AsyncReadInfo
    {
        public readonly IntPtr Handle;                      /* [r] The file handle that was filled out in the open callback. */
        public readonly uint Offset;                        /* [r] Seek position, make sure you read from this file offset. */
        public readonly uint SizeBytes;                     /* [r] how many bytes requested for read. */
        public readonly int Priority;                       /* [r] 0 = low importance.  100 = extremely important (ie 'must read now or stuttering may occur') */

        public readonly IntPtr Userdata;                    /* [r] User data pointer. */
        public readonly IntPtr Buffer;                      /* [w] Buffer to read file data into. */
        public uint BytesRead;                              /* [w] Fill this in before setting result code to tell FMOD how many bytes were read. */
        private readonly AsyncReadInfoDoneCallback _done;   /* [r] FMOD file system wake up function.  Call this when user file read is finished.  Pass result of file read as a parameter. */

        public void Done(Result result)
        {
            _done.Invoke(ref this, result);
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct AsyncReadInfoDoneCallback
        {
            private delegate void Delegate(IntPtr funcPtr, ref AsyncReadInfo info, Result asyncResult);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            public static explicit operator AsyncReadInfoDoneCallback(IntPtr FuncPtr)
            {
                return new AsyncReadInfoDoneCallback(FuncPtr);
            }

            private readonly IntPtr FuncPtr;

            private AsyncReadInfoDoneCallback(IntPtr FuncPtr)
            {
                this.FuncPtr = FuncPtr;
            }

            public void Invoke(ref AsyncReadInfo info, Result asyncResult)
            {
                InvokeImpl(FuncPtr, ref info, asyncResult);
            }

            public bool IsDefault => FuncPtr == IntPtr.Zero;
        }
    }
}
