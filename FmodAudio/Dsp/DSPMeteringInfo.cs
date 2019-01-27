using System;
using System.Runtime.CompilerServices;
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

        //Warning: Not .NET Framework Friendly
        public Span<float> PeakLevel => new Span<float>(Unsafe.AsPointer(ref peakLevel[0]), 32);
        public Span<float> RMSLevel => new Span<float>(Unsafe.AsPointer(ref rmsLevel[0]), 32);
    }
}
