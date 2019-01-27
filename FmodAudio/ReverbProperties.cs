using System.Runtime.InteropServices;

namespace FmodAudio
{
//#pragma warning disable 414
    [StructLayout(LayoutKind.Sequential)]
    public struct ReverbProperties
    {                            /*        MIN     MAX    DEFAULT   DESCRIPTION */
        public float DecayTime;         /* [r/w]  0.0    20000.0 1500.0  Reverberation decay time in ms                                        */
        public float EarlyDelay;        /* [r/w]  0.0    300.0   7.0     Initial reflection delay time                                         */
        public float LateDelay;         /* [r/w]  0.0    100     11.0    Late reverberation delay time relative to initial reflection          */
        public float HFReference;       /* [r/w]  20.0   20000.0 5000    Reference high frequency (hz)                                         */
        public float HFDecayRatio;      /* [r/w]  10.0   100.0   50.0    High-frequency to mid-frequency decay time ratio                      */
        public float Diffusion;         /* [r/w]  0.0    100.0   100.0   Value that controls the echo density in the late reverberation decay. */
        public float Density;           /* [r/w]  0.0    100.0   100.0   Value that controls the modal density in the late reverberation decay */
        public float LowShelfFrequency; /* [r/w]  20.0   1000.0  250.0   Reference low frequency (hz)                                          */
        public float LowShelfGain;      /* [r/w]  -36.0  12.0    0.0     Relative room effect level at low frequencies                         */
        public float HighCut;           /* [r/w]  20.0   20000.0 20000.0 Relative room effect level at high frequencies                        */
        public float EarlyLateMix;      /* [r/w]  0.0    100.0   50.0    Early reflections level relative to room effect                       */
        public float WetLevel;          /* [r/w]  -80.0  20.0    -6.0    Room effect level (at mid frequencies)
                                  * */
        #region wrapperinternal
        public ReverbProperties(float decayTime, float earlyDelay, float lateDelay, float hfReference,
            float hfDecayRatio, float diffusion, float density, float lowShelfFrequency, float lowShelfGain,
            float highCut, float earlyLateMix, float wetLevel)
        {
            DecayTime = decayTime;
            EarlyDelay = earlyDelay;
            LateDelay = lateDelay;
            HFReference = hfReference;
            HFDecayRatio = hfDecayRatio;
            Diffusion = diffusion;
            Density = density;
            LowShelfFrequency = lowShelfFrequency;
            LowShelfGain = lowShelfGain;
            HighCut = highCut;
            EarlyLateMix = earlyLateMix;
            WetLevel = wetLevel;
        }
        #endregion
    }
//#pragma warning restore 414
}
