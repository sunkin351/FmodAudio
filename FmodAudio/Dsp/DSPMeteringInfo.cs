#pragma warning disable CA1815

using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DSPMeteringInfo
    {
        public int SampleCount;
        public fixed float peakLevel[32];
        public fixed float rmsLevel[32];
        public short ChannelCount;
    }
}
