#pragma warning disable CA1815

using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DspBufferArray
    {
        public int BufferCount;
        public int* BufferChannelCount;
        public ChannelMask* BufferChannelMask;
        public IntPtr* Buffers;
        public SpeakerMode SpeakerMode;
    }
}
