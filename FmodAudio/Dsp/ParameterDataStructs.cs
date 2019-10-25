#pragma warning disable CA1815

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    public struct ParameterOverallGain
    {
        public float LinearGain;
        public float LinearGainAdditive;
    }

    public struct Parameter3DAttributes
    {
        public Attributes3D Relative;
        public Attributes3D Absolute;
    }

    public unsafe struct Parameter3DAttributesMulti
    {
        public int ListenerCount;
        private fixed float Relative3DAttributes[3 * 4 * 8];
        public fixed float Weight[8];
        public Attributes3D Absolute;

        public Span<Attributes3D> Relatives => MemoryMarshal.CreateSpan(ref Unsafe.As<float, Attributes3D>(ref Relative3DAttributes[0]), 8);
    }

    public unsafe struct ParameterSideChain
    {
        public int SidechainEnable;
    }

    /// <summary>
    /// Warning: Do not use if you don't know what you are doing. Compare with the native structure
    /// </summary>
    public unsafe struct ParameterFFT
    {
        public int Length;
        public int ChannelCount;
        private IntPtr Spectrum_FirstElement;

        public Span<IntPtr> Spectrum => MemoryMarshal.CreateSpan(ref Spectrum_FirstElement, 32);
    }
}
