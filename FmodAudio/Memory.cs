using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    internal static class Memory
    {
        public static SaferPointer Allocate(int bytes)
        {
            return new SaferPointer(AllocateUnsafe(bytes), bytes);
        }
        
        public static IntPtr AllocateUnsafe(int bytes)
        {
            return Marshal.AllocHGlobal(bytes);
        }
        
        public static void FreeUnsafe(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        public sealed class SaferPointer
        {
            private readonly IntPtr Ptr;
            public int AllocationSize { get; }

            public SaferPointer(IntPtr pointer, int Size)
            {
                Debug.Assert(pointer != IntPtr.Zero && Size > 0);

                Ptr = pointer;
                AllocationSize = Size;
                GC.AddMemoryPressure(Size);
            }

            ~SaferPointer()
            {
                FreeUnsafe(Ptr);
                GC.RemoveMemoryPressure(AllocationSize);
            }
            
            public unsafe void* ToPointer()
            {
                return Ptr.ToPointer();
            }
            
            public static implicit operator IntPtr (SaferPointer ptr)
            {
                return ptr?.Ptr ?? IntPtr.Zero;
            }
            
            public static unsafe explicit operator void*(SaferPointer ptr)
            {
                return ptr.ToPointer();
            }

            public unsafe int Length<T>() where T: unmanaged
            {
                return AllocationSize / sizeof(T);
            }
            
            public unsafe Span<T> AsSpan<T>() where T: unmanaged
            {
                return new Span<T>(this.ToPointer(), AllocationSize / sizeof(T));
            }
            
            public unsafe Span<T> AsSpan<T>(int start) where T: unmanaged
            {
                int SpanSize = AllocationSize / sizeof(T);

                if ((uint)start > (uint)SpanSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(start));
                }
                
                return new Span<T>((T*)ToPointer() + start, SpanSize - start);
            }

            public unsafe Span<T> AsSpan<T>(int start, int length) where T : unmanaged
            {
                int SpanSize = AllocationSize / sizeof(T);

                if ((uint)start > (uint)SpanSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(start));
                }

                if ((uint)length > (uint)(SpanSize - start))
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }

                return new Span<T>((T*)ToPointer() + start, length);
            }
        }
    }
}
