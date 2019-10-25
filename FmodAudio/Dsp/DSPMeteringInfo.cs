#pragma warning disable CA1815

using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DSPMeteringInfo
    {
        public int NumSamples;
        private fixed float peakLevel[32];
        private fixed float rmsLevel[32];
        public short NumChannels;

        public Span<float> PeakLevel => MemoryMarshal.CreateSpan(ref peakLevel[0], 32);
        public Span<float> RMSLevel => MemoryMarshal.CreateSpan(ref rmsLevel[0], 32);
    }
}
