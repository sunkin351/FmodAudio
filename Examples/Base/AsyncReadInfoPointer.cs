using System;
using System.Buffers;
using FmodAudio;

namespace Examples.Base
{
    public readonly unsafe struct AsyncReadInfoPointer
    {
        private readonly AsyncReadInfo* Pointer;

        public AsyncReadInfoPointer(AsyncReadInfo* ptr)
        {
            Pointer = ptr;
        }

        public IntPtr Handle { get => Pointer->Handle; }

        public int Offset => (int)Pointer->Offset;

        public uint BytesRead { get => Pointer->BytesRead; set => Pointer->BytesRead = value; }

        public AsyncReadInfo* GetPointer()
        {
            return Pointer;
        }

        public MemoryManager<byte> GetMemoryManager()
        {
            return new UnmanagedBufferMemoryManager((byte*)Pointer->Buffer, (int)Pointer->SizeBytes);
        }

        public void Done(Result res)
        {
            Pointer->Done(res);
        }
    }
}
