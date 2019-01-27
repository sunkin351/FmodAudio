using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    using Interop;

    [StructLayout(LayoutKind.Sequential)]
    public struct AsyncReadInfo
    {
        public IntPtr Handle;                     /* [r] The file handle that was filled out in the open callback. */
        public uint Offset;                     /* [r] Seek position, make sure you read from this file offset. */
        public uint SizeBytes;                  /* [r] how many bytes requested for read. */
        public int Priority;                   /* [r] 0 = low importance.  100 = extremely important (ie 'must read now or stuttering may occur') */

        public IntPtr Userdata;                   /* [r] User data pointer. */
        public IntPtr Buffer;                     /* [w] Buffer to read file data into. */
        public uint BytesRead;                  /* [w] Fill this in before setting result code to tell FMOD how many bytes were read. */
        private AsyncReadInfoDoneCallback Done;  /* [r] FMOD file system wake up function.  Call this when user file read is finished.  Pass result of file read as a parameter. */

        public void InvokeDoneCallback(Result result)
        {
            Done.Invoke(ref this, result).CheckResult();
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct AsyncReadInfoDoneCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, ref AsyncReadInfo info, Result asyncResult);
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

            public Result Invoke(ref AsyncReadInfo info, Result asyncResult)
            {
                return InvokeImpl(FuncPtr, ref info, asyncResult);
            }

            public bool IsDefault => FuncPtr == IntPtr.Zero;
        }
    }
}
