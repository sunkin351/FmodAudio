#pragma warning disable CA1815

using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DspBufferArray
    {
        public int BufferCount;
        public int* BufferChannelCount;
        public ChannelMask* BufferChannelMask;
        public float** Buffers;
        public SpeakerMode SpeakerMode;
    }
}
