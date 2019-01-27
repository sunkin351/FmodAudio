using System;
using System.Diagnostics;

namespace FmodAudio.Dsp
{
    public class DspBufferArray
    {
        public int BufferCount;
        public int[] BufferChannelCount;
        public ChannelMask[] BufferChannelMask;
        public IntPtr[] Buffers;
        public SpeakerMode SpeakerMode;

        internal unsafe DspBufferArray(ref Interop.DspBufferArrayInterop bArr)
        {
            int count = bArr.BufferCount;
            BufferCount = count;
            SpeakerMode = bArr.SpeakerMode;

            if (count > 0)
            {
                Debug.Assert(bArr.BufferChannelCount != IntPtr.Zero && bArr.BufferChannelMask != IntPtr.Zero, "Null variable in DspBufferArrayInterop struct");
                
                BufferChannelCount = new int[count];
                new Span<int>(bArr.BufferChannelCount.ToPointer(), count).CopyTo(BufferChannelCount);

                BufferChannelMask = new ChannelMask[count];
                new Span<ChannelMask>(bArr.BufferChannelMask.ToPointer(), count).CopyTo(BufferChannelMask);

                Buffers = new IntPtr[count];
                new Span<IntPtr>(bArr.Buffers.ToPointer(), count).CopyTo(Buffers);
            }
        }

        public bool IsEmpty => BufferCount == 0;
    }
}
