using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio.DigitalSignalProcessing.Effects
{
    public unsafe struct DspLoudnessMeter_InfoType
    {
        public float MomentaryLoudness;
        public float ShorttermLoudness;
        public float IntegratedLoudness;
        public float Loudness10thPercentile;
        public float Loudness95thPercentile;
        public fixed float LoudnessHistogram[66];
        public float MaxTruePeak;
        public float MaxMomentaryLoudness;
    }

    public unsafe struct DspLoudnessMeter_WeightingType
    {
        public fixed float ChannelWeight[32];
    }
}
