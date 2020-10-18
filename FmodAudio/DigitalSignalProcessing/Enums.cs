namespace FmodAudio.DigitalSignalProcessing
{
    /// <summary>
    /// These definitions can be used for creating FMOD defined special effects or DSP units.
    /// </summary>
    public enum DSPType : int
    {
        /// <summary>
        /// This unit was created via a non FMOD plugin so has an unknown purpose.
        /// </summary>
        Unknown,

        /// <summary>
        /// This unit does nothing but take inputs and mix them together then feed the result to the soundcard unit.
        /// </summary>
        Mixer,

        /// <summary>
        /// This unit generates sine/square/saw/triangle or noise tones.
        /// </summary>
        Oscillator,

        /// <summary>
        /// This unit filters sound using a high quality, resonant lowpass filter algorithm but consumes more CPU time.
        /// </summary>
        LowPass,

        /// <summary>
        /// This unit filters sound using a resonant lowpass filter algorithm that is used in Impulse Tracker, but with limited cutoff range (0 to 8060hz).
        /// </summary>
        ITLowPass,

        /// <summary>
        /// This unit filters sound using a resonant highpass filter algorithm.
        /// </summary>
        HighPass,

        /// <summary>
        /// This unit produces an echo on the sound and fades out at the desired rate.
        /// </summary>
        Echo,

        /// <summary>
        /// This unit pans and scales the volume of a unit.
        /// </summary>
        Fader,

        /// <summary>
        /// This unit produces a flange effect on the sound.
        /// </summary>
        Flange,

        /// <summary>
        /// This unit distorts the sound.
        /// </summary>
        Distortion,

        /// <summary>
        /// This unit normalizes or amplifies the sound to a certain level.
        /// </summary>
        Normalize,

        /// <summary>
        /// This unit limits the sound to a certain level.
        /// </summary>
        Limiter,

        /// <summary>
        /// This unit attenuates or amplifies a selected frequency range.
        /// </summary>
        ParamEQ,

        /// <summary>
        /// This unit bends the pitch of a sound without changing the speed of playback.
        /// </summary>
        PitchShift,

        /// <summary>
        /// This unit produces a chorus effect on the sound.
        /// </summary>
        Chorus,

        /// <summary>
        /// This unit allows the use of Steinberg VST plugins
        /// </summary>
        VSTPlugin,

        /// <summary>
        /// This unit allows the use of Nullsoft Winamp plugins
        /// </summary>
        WinAMPPlugin,

        /// <summary>
        /// This unit produces an echo on the sound and fades out at the desired rate as is used in Impulse Tracker.
        /// </summary>
        ITEcho,

        /// <summary>
        /// This unit implements dynamic compression (linked multichannel, wideband)
        /// </summary>
        Compressor,

        /// <summary>
        /// This unit implements SFX reverb
        /// </summary>
        SFXReverb,

        /// <summary>
        /// This unit filters sound using a simple lowpass with no resonance, but has flexible cutoff and is fast.
        /// </summary>
        LowPass_Simple,

        /// <summary>
        /// This unit produces different delays on individual channels of the sound.
        /// </summary>
        Delay,

        /// <summary>
        /// This unit produces a tremolo / chopper effect on the sound.
        /// </summary>
        Tremolo,

        /// <summary>
        /// This unit allows the use of LADSPA standard plugins.
        /// </summary>
        LADSPAPlugin,

        /// <summary>
        /// This unit sends a copy of the signal to a return DSP anywhere in the DSP tree.
        /// </summary>
        Send,

        /// <summary>
        /// This unit receives signals from a number of send DSPs.
        /// </summary>
        Return,

        /// <summary>
        /// This unit filters sound using a simple highpass with no resonance, but has flexible cutoff and is fast.
        /// </summary>
        HighPass_Simple,

        /// <summary>
        /// This unit pans the signal, possibly upmixing or downmixing as well.
        /// </summary>
        Pan,

        /// <summary>
        /// This unit is a three-band equalizer.
        /// </summary>
        Three_EQ,

        /// <summary>
        /// This unit simply analyzes the signal and provides spectrum information back through getParameter.
        /// </summary>
        FFT,

        /// <summary>
        /// This unit analyzes the loudness and true peak of the signal.
        /// </summary>
        Loudness_Meter,

        /// <summary>
        /// This unit tracks the envelope of the input/sidechain signal. Deprecated and will be removed in a future release.
        /// </summary>
        EnvelopeFollower,

        /// <summary>
        /// This unit implements convolution reverb.
        /// </summary>
        ConvolutionReverb,

        /// <summary>
        /// This unit provides per signal channel gain, and output channel mapping to allow 1 multichannel signal made up of many groups of signals to map to a single output signal.
        /// </summary>
        ChannelMix,

        /// <summary>
        /// This unit 'sends' and 'receives' from a selection of up to 32 different slots.
        /// It is like a send/return but it uses global slots rather than returns as the destination.
        /// It also has other features.
        /// Multiple transceivers can receive from a single channel, or multiple transceivers can send to a single channel, or a combination of both.
        /// </summary>
        Transceiver,

        /// <summary>
        /// This unit sends the signal to a 3d object encoder like Dolby Atmos.
        /// </summary>
        ObjectPan,

        /// <summary>
        /// This unit is a flexible five band parametric equalizer.
        /// </summary>
        MultiBand_EQ
    }

    public enum ProcessOperation
    {
        ProcessPerform = 0,
        ProcessQuery
    }

    public enum PanSurroundFlags : int
    {
        Default = 0,
        RotationNotBiased = 1
    }

    public enum DSPParameterType
    {
        Float = 0,
        Int,
        Bool,
        Data
    }

    public enum ParameterFloatMappingType : int
    {
        Linear = 0,
        Auto,
        PiecewiseLinear
    }

    public enum ParameterDataType : int
    {
        OverallGain = 1,
        Attributes3D,
        SideChain,
        FFT,
        Attributes3D_Multi,
        User
    }
}
