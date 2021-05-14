using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.DigitalSignalProcessing.Effects
{
    public enum DspOscillator
    {
        Type,
        Rate
    }

    public enum DspLowpass
    {
        Cutoff,
        Resonance
    }

    public enum DspITLowpass
    {
        Cutoff,
        Resonance
    }

    public enum DspHighpass
    {
        Cutoff,
        Resonance
    }

    public enum DspEcho
    {
        Delay,
        Feedback,
        DryLevel,
        WetLevel
    }

    public enum DspFader
    {
        Gain,
        OverallGain
    }

    public enum DspFlange
    {
        Mix,
        Depth,
        Rate
    }

    public enum DspDistortion
    {
        Level
    }

    public enum DspNormalize
    {
        FadeTime,
        ThreshHold,
        MaxAmp
    }

    public enum DspLimiter
    {
        ReleaseTime,
        Ceiling,
        MaximizerGain,
        Mode
    }

    public enum ParamEq
    {
        Center,
        Bandwidth,
        Gain
    }

    public enum DspMultibandEq
    {
        A_Filter,
        A_Frequency,
        A_Q,
        A_Gain,
        B_Filter,
        B_Frequency,
        B_Q,
        B_Gain,
        C_Filter,
        C_Frequency,
        C_Q,
        C_Gain,
        D_Filter,
        D_Frequency,
        D_Q,
        D_Gain,
        E_Filter,
        E_Frequency,
        E_Q,
        E_Gain
    }

    public enum DspMultibandEqFilterType
    {
        Lowpass_12DB,
        Lowpass_24DB,
        Lowpass_48DB,
        Highpass_12DB,
        Highpass_24DB,
        Highpass_48DB,
        Lowshelf,
        Highshelf,
        Peaking,
        Bandpass,
        Notch,
        Allpass
    }

    public enum DspPitchShift
    {
        Pitch,
        FFTSize,
        Overlap,
        MaxChannels
    }

    public enum DspChorus
    {
        Mix,
        Rate,
        Depth
    }

    //public enum Pan3DRolloffType
    //{
    //    LinearSquared,
    //    Linear,
    //    Inverse,
    //    InverseTapered,
    //    Custom
    //}

    public enum DspITEcho
    {
        WetDryMix,
        Feedback,
        LeftDelay,
        RightDelay,
        PanDelay
    }

    public enum DspCompressor
    {
        ThreshHold,
        Ratio,
        Attack,
        Release,
        GainMakeup,
        UseSideChain,
        Linked
    }

    public enum DspSfxReverb
    {
        DecayTime,
        EarlyDelay,
        LateDelay,
        HFReference,
        HFDecayRatio,
        Diffusion,
        Density,
        LowShelfFrequency,
        LowShelfGain,
        HighCut,
        EarlyLateMix,
        WetLevel,
        DryLevel
    }

    public enum DspLowpassSimple
    {
        Cutoff
    }

    public enum DspDelay
    {
        CH0,
        CH1,
        CH2,
        CH3,
        CH4,
        CH5,
        CH6,
        CH7,
        CH8,
        CH9,
        CH10,
        CH11,
        CH12,
        CH13,
        CH14,
        CH15,
        MaxDelay
    }

    public enum DspTremolo
    {
        Frequency,
        Depth,
        Shape,
        Skew,
        Duty,
        Square,
        Phase,
        Spread
    }

    public enum DspSend
    {
        ReturnId,
        Level
    }

    public enum DspReturn
    {
        ReturnId,
        Level
    }

    public enum DspHighpassSimple
    {
        Cutoff
    }

    public enum DspPan2DStereoModeType
    {
        Distributed,
        Discrete
    }

    public enum DspPanModeType
    { 
        Mono,
        Sterio,
        Surround
    }

    public enum DspPan3DRolloffType
    {
        LinearSquared,
        Linear,
        Inverse,
        InverseTapered,
        Custom
    }

    public enum DspPan3DExtentModeType
    {
        Auto,
        User,
        Off
    }

    public enum DspPan
    {
        Mode,
        _2DStereoPosition,
        _2DDirection,
        _2DExtent,
        _2DRotational,
        _2DLFELevel,
        _2DSterioMode,
        _2DStereoSeparation,
        _2DStereoAxis,
        EnabledSpeakers,
        _3DPosition,
        _3DRolloff,
        _3DMinDistance,
        _3DMaxDistance,
        _3DExtentMode,
        _3DSoundSize,
        _3DMinExtent,
        _3DPanBlend,
        FLEUpmixEnabled,
        OverallGain,
        SurroundSpeakerMode,
        _2DHeightBlend
    }

    public enum DspThreeEqCrossoverSlope
    {
        _12DB,
        _24DB,
        _48DB
    }

    public enum DspThreeEq
    {
        LowGain,
        MidGain,
        HighGain,
        LowCrossover,
        HighCrossover,
        CrossoverSlope
    }

    public enum DspFFTWindow
    {
        Rect,
        Triangle,
        Hamming,
        Hanning,
        Blackman,
        BlackmanHarris
    }

    public enum DspFFT
    {
        WindowSize,
        WindowType,
        SpectrumData,
        DominantFreq
    }

    public enum DspEnvelopeFollower
    {
        Attack,
        Release,
        Envelope,
        UseSideChain
    }

    public enum DspConvolutionReverb
    {
        Param_IR,
        Param_Wet,
        Param_Dry,
        Param_Linked
    }

    public enum DspChannelMixOutput
    {
        Default,
        AllMono,
        AllStereo,
        AllQuad,
        All5Point1,
        All7Point1,
        AllLFE,
        All7Point1Point4
    }

    public enum DspChannelMix
    {
        OutputGrouping,
        Gain_CH0,
        Gain_CH1,
        Gain_CH2,
        Gain_CH3,
        Gain_CH4,
        Gain_CH5,
        Gain_CH6,
        Gain_CH7,
        Gain_CH8,
        Gain_CH9,
        Gain_CH10,
        Gain_CH11,
        Gain_CH12,
        Gain_CH13,
        Gain_CH14,
        Gain_CH15,
        Gain_CH16,
        Gain_CH17,
        Gain_CH18,
        Gain_CH19,
        Gain_CH20,
        Gain_CH21,
        Gain_CH22,
        Gain_CH23,
        Gain_CH24,
        Gain_CH25,
        Gain_CH26,
        Gain_CH27,
        Gain_CH28,
        Gain_CH29,
        Gain_CH30,
        Gain_CH31,
        Output_CH0,
        Output_CH1,
        Output_CH2,
        Output_CH3,
        Output_CH4,
        Output_CH5,
        Output_CH6,
        Output_CH7,
        Output_CH8,
        Output_CH9,
        Output_CH10,
        Output_CH11,
        Output_CH12,
        Output_CH13,
        Output_CH14,
        Output_CH15,
        Output_CH16,
        Output_CH17,
        Output_CH18,
        Output_CH19,
        Output_CH20,
        Output_CH21,
        Output_CH22,
        Output_CH23,
        Output_CH24,
        Output_CH25,
        Output_CH26,
        Output_CH27,
        Output_CH28,
        Output_CH29,
        Output_CH30,
        Output_CH31
    }

    public enum DspTransceiver
    {
        Transmit,
        Gain,
        Channel,
        TransmitSpeakerMode
    }

    public enum DspObjectPan
    {
        _3DPosition,
        _3DRolloff,
        _3DMinDistance,
        _3dMaxDistance,
        _3DExtentMode,
        _3DSoundSize,
        _3DMinExtent,
        OverallGain,
        OutputGain
    }
}