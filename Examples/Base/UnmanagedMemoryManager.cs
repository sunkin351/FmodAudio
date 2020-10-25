using System;
using System.Buffers;

namespace Examples.Base
{
    internal unsafe sealed class UnmanagedBufferMemoryManager : MemoryManager<byte>
    {
        private byte* memoryPointer;
        private int memoryLength;

        public UnmanagedBufferMemoryManager(byte* ptr, int len)
        {
            memoryPointer = ptr;
            memoryLength = len;
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override Span<byte> GetSpan()
        {
            return new Span<byte>(memoryPointer, memoryLength);
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            return new MemoryHandle(memoryPointer + elementIndex);
        }

        public override void Unpin()
        {
        }
    }
}
