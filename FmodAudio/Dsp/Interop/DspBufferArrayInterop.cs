using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DspBufferArrayInterop
    {
        public int BufferCount;
        public IntPtr BufferChannelCount;
        public IntPtr BufferChannelMask;
        public IntPtr Buffers;
        public SpeakerMode SpeakerMode;
    }
}
