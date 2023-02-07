#pragma warning disable CA1815

using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct DSPMeteringInfo
{
    public int SampleCount;
    public fixed float peakLevel[32];
    public fixed float rmsLevel[32];
    public short ChannelCount;
}
