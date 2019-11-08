using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Codec
{
    public struct CodecWaveFormat
    {
        public IntPtr Name;
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
}
