using System;

namespace FmodAudio.Dsp
{
    public unsafe delegate Result DspStateCallback(DspState* state);
    public unsafe delegate Result DspReadCallback(DspState* state, float* inbuffer, float* outbuffer, uint length, int inchannels, ref int outchannels);
    public unsafe delegate Result DspProcessCallback(DspState* state, uint length, DspBufferArray* inBufferArray, DspBufferArray* outBufferArray, bool inputsIdle, ProcessOperation operation);
    public unsafe delegate Result DspSetPositionCallback(DspState* state, uint pos);

    public unsafe delegate Result DspSetParamFloatCallback(DspState* state, int index, float value);
    public unsafe delegate Result DspSetParamIntCallback(DspState* state, int index, int value);
    public unsafe delegate Result DspSetParamBoolCallback(DspState* state, int index, bool value);
    public unsafe delegate Result DspSetParamDataCallback(DspState* state, int index, IntPtr data, uint dataLength);

    public unsafe delegate Result DspGetParamFloatCallback(DspState* state, int index, out float value, IntPtr valueString);
    public unsafe delegate Result DspGetParamIntCallback(DspState* state, int index, out int value, IntPtr valueString);
    public unsafe delegate Result DspGetParamBoolCallback(DspState* state, int index, out bool value, IntPtr valueString);
    public unsafe delegate Result DspGetParamDataCallback(DspState* state, int index, out IntPtr data, out uint dataLength, IntPtr valueString);

    public unsafe delegate Result DspShouldIProcessCallback(DspState* state, bool inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode);

    public unsafe delegate Result DspSystemMixCallback(DspState* state, int stage);
}
