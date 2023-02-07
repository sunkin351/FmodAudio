namespace FmodAudio.Codec;

public unsafe struct CodecWaveFormat
{
    public byte* Name;
    public SoundFormat Format;
    public int Channels;
    public int Frequency;
    public uint LengthBytes;
    public uint LengthPCM;
    public uint PCMBlockSize;
    public int LoopStart;
    public int LoopEnd;
    public Mode Mode;
    public ChannelMask ChannelMask;
    public ChannelOrder ChannelOrder;
    public float PeakVolume;
}
