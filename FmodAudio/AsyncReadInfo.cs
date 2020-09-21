using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    using Base;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AsyncReadInfo
    {
        public readonly IntPtr Handle;                      /* [r] The file handle that was filled out in the open callback. */
        public readonly uint Offset;                        /* [r] Seek position, make sure you read from this file offset. */
        public readonly uint SizeBytes;                     /* [r] how many bytes requested for read. */
        public readonly int Priority;                       /* [r] 0 = low importance.  100 = extremely important (ie 'must read now or stuttering may occur') */

        public readonly IntPtr Userdata;                    /* [r] User data pointer. */
        public readonly IntPtr Buffer;                      /* [w] Buffer to read file data into. */
        public uint BytesRead;                              /* [w] Fill this in before setting result code to tell FMOD how many bytes were read. */
        private readonly delegate* unmanaged<AsyncReadInfo*, Result, void> _done;   /* [r] FMOD file system wake up function.  Call this when user file read is finished.  Pass result of file read as a parameter. */

        public void Done(Result result)
        {
            _done((AsyncReadInfo*)Unsafe.AsPointer(ref this), result);
        }
    }
}
