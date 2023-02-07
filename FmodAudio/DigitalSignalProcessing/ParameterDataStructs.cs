#pragma warning disable CA1815

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing;

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

public unsafe struct ParameterFFT
{
    static ParameterFFT()
    {
        if (sizeof(SpectrumArray) != IntPtr.Size * 32)
        {
            throw new Exception("The size of ParameterFFT.SpectrumArray is not the size of 32 pointers.");
        }
    }

    public int Length;
    public int ChannelCount;
    private SpectrumArray _spectrum;

    public ReadOnlySpan<float> this[int channel]
    {
        get => GetSpectrum(channel);
    }

    public Span<IntPtr> Spectrum => MemoryMarshal.CreateSpan(ref _spectrum.FirstElement, 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<float> GetSpectrum(int channel)
    {
        if ((uint)channel >= (uint)ChannelCount)
            ThrowHelper_OutOfRange(channel);

        return new ReadOnlySpan<float>((float*)Unsafe.Add(ref _spectrum.FirstElement, channel), Length);
    }

    public void GetSpectrum(int channel, Span<float> buffer)
    {
        GetSpectrum(channel).CopyTo(buffer);
    }

#pragma warning disable CS0649
    private struct SpectrumArray
    {
        public IntPtr FirstElement;
        public IntPtr Element01;
        public IntPtr Element02;
        public IntPtr Element03;
        public IntPtr Element04;
        public IntPtr Element05;
        public IntPtr Element06;
        public IntPtr Element07;
        public IntPtr Element08;
        public IntPtr Element09;
        public IntPtr Element10;
        public IntPtr Element11;
        public IntPtr Element12;
        public IntPtr Element13;
        public IntPtr Element14;
        public IntPtr Element15;
        public IntPtr Element16;
        public IntPtr Element17;
        public IntPtr Element18;
        public IntPtr Element19;
        public IntPtr Element20;
        public IntPtr Element21;
        public IntPtr Element22;
        public IntPtr Element23;
        public IntPtr Element24;
        public IntPtr Element25;
        public IntPtr Element26;
        public IntPtr Element27;
        public IntPtr Element28;
        public IntPtr Element29;
        public IntPtr Element30;
        public IntPtr Element31;
    }
#pragma warning restore CS0649

    private static void ThrowHelper_OutOfRange(int channel)
    {
        throw new ArgumentOutOfRangeException(nameof(channel), channel, "Channel must be between 0 and ChannelCount - 1");
    }
}
