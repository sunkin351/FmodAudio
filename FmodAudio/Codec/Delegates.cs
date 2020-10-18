using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Codec
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecOpenCallback(CodecState* state, Mode usermode, CreateSoundInfoStruct* createSoundInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecCloseCallback(CodecState* state);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecReadCallback(CodecState* state, void* buffer, uint samplesIn, uint* samplesOut);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecGetLengthCallback(CodecState* state, uint* length, TimeUnit lengthType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecSetPositionCallback(CodecState* state, int subsound, uint position, TimeUnit posType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecGetPositionCallback(CodecState* state, uint* position, TimeUnit posType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecSoundCreateCallback(CodecState* state, int subsound, IntPtr SoundHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public unsafe delegate Result CodecGetWaveFormatCallback(CodecState* state, int index, CodecWaveFormat* waveFormat);
}
