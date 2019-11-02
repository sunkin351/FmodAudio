#pragma warning disable CA1815

using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    using FmodAudio.Interop;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStateFunctions
    {
        public readonly DspAllocCallback Alloc;
        public readonly DspReallocCallback Realloc;
        public readonly DspFreeCallback Free;
        public readonly DspGetSampleRateCallback GetSampleRate;
        public readonly DspGetBlockSizeCallback GetBlockSize;
        public readonly DspStateDFTFunctions* DFT;
        public readonly DspStatePanFunctions* Pan;
        public readonly DspGetSpeakerModeCallback GetSpeakerMode;
        public readonly DspGetClockCallback GetDSPClock;
        public readonly DspListenerAttributesCallback GetListenerAttributes;
        /// <summary>
        /// It is recommended to use your own logging methods
        /// </summary>
        public readonly IntPtr LogFuncPtr;
        public readonly DspGetUserDataCallback GetUserData;


        [StructLayout(LayoutKind.Sequential)]
        public readonly struct DspAllocCallback
        {
            private delegate IntPtr Delegate(IntPtr funcPtr, uint size, MemoryType type, IntPtr sourceString);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public IntPtr Invoke(uint size, MemoryType type, IntPtr sourceString)
            {
                return InvokeImpl(FuncPtr, size, type, sourceString);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct DspReallocCallback
        {
            private delegate IntPtr Delegate(IntPtr funcPtr, IntPtr ptr, uint size, MemoryType type, IntPtr sourceString);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public IntPtr Invoke(IntPtr ptr, uint size, MemoryType type, IntPtr sourceString)
            {
                return InvokeImpl(FuncPtr, ptr, size, type, sourceString);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct DspFreeCallback
        {
            private delegate void Delegate(IntPtr funcPtr, IntPtr ptr, MemoryType type, IntPtr sourceString);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public void Invoke(IntPtr ptr, MemoryType type, IntPtr sourceString)
            {
                InvokeImpl(FuncPtr, ptr, type, sourceString);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspGetSampleRateCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out int rate);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out int rate)
            {
                return InvokeImpl(FuncPtr, state, out rate);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspGetBlockSizeCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out uint blockSize);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out uint blockSize)
            {
                return InvokeImpl(FuncPtr, state, out blockSize);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspGetSpeakerModeCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out SpeakerMode mixer, out SpeakerMode output);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out SpeakerMode mixer, out SpeakerMode output)
            {
                return InvokeImpl(FuncPtr, state, out mixer, out output);
            }

            public bool IsDefault => FuncPtr == default;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspGetClockCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out ulong clock, out uint offset, out uint length);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out ulong clock, out uint offset, out uint length)
            {
                return InvokeImpl(FuncPtr, state, out clock, out offset, out length);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspListenerAttributesCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out int listenerCount, out Attributes3D attributes);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out int listenerCount, out Attributes3D attributes)
            {
                return InvokeImpl(FuncPtr, state, out listenerCount, out attributes);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspGetUserDataCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, out IntPtr userData);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, out IntPtr userData)
            {
                return InvokeImpl(FuncPtr, state, out userData);
            }

            public bool IsDefault => FuncPtr == default;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DspStateDFTFunctions
    {
        public readonly DspDFTFFTRealCallback FFTReal;
        public readonly DspDFTIFFTRealCallback InverseFFTReal;

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspDFTFFTRealCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, int size, float* signal, ref Fmod_Complex dft, float* window, int signalHop);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, int size, float* signal, ref Fmod_Complex dft, float* window, int signalHop)
            {
                return InvokeImpl(FuncPtr, state, size, signal, ref dft, window, signalHop);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspDFTIFFTRealCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, int size, ref Fmod_Complex dft, float* signal, float* window, int signalHop);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, int size, ref Fmod_Complex dft, float* signal, float* window, int signalHop)
            {
                return InvokeImpl(FuncPtr, state, size, ref dft, signal, window, signalHop);
            }

            public bool IsDefault => FuncPtr == default;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DspStatePanFunctions
    {
        public readonly DspPanSumMonoMatrixCallback SumMonoMatrix;
        public readonly DspPanSumSterioMatrixCallback SumSterioMatrix;
        public readonly DspPanSumSurroundMatrixCallback SumSurroundMatrix;
        public readonly DspPanSumMonoToSurroundMatrixCallback SumMonoToSurroundMatrix;
        public readonly DspPanSumSterioToSurroundMatrixCallback SumSterioToSurroundMatrix;
        public readonly DspPanGetRolloffGainCallback GetRolloffGain;

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanSumMonoMatrixCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, SpeakerMode mode, float lowFrequencyGain, float overallGain, float* matrix);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, SpeakerMode mode, float lowFrequencyGain, float overallGain, float* matrix)
            {
                return InvokeImpl(FuncPtr, state, mode, lowFrequencyGain, overallGain, matrix);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanSumSterioMatrixCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, SpeakerMode mode, float pan, float lowFrequencyGain, float OverallGain, int matrixHop, float* matrix);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, SpeakerMode mode, float pan, float lowFrequencyGain, float OverallGain, int matrixHop, float* matrix)
            {
                return InvokeImpl(FuncPtr, state, mode, pan, lowFrequencyGain, OverallGain, matrixHop, matrix);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanSumSurroundMatrixCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, SpeakerMode sourceMode, SpeakerMode targetMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix, PanSurroundFlags flags);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, SpeakerMode sourceMode, SpeakerMode targetMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix, PanSurroundFlags flags)
            {
                return InvokeImpl(FuncPtr, state, sourceMode, targetMode, direction, extent, rotation, lowFrequencyGain, overallGain, matrixHop, matrix, flags);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanSumMonoToSurroundMatrixCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, SpeakerMode targetMode, float direction, float extent, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, SpeakerMode targetMode, float direction, float extent, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix)
            {
                return InvokeImpl(FuncPtr, state, targetMode, direction, extent, lowFrequencyGain, overallGain, matrixHop, matrix);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanSumSterioToSurroundMatrixCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, SpeakerMode targetMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, SpeakerMode targetMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, float* matrix)
            {
                return InvokeImpl(FuncPtr, state, targetMode, direction, extent, rotation, lowFrequencyGain, overallGain, matrixHop, matrix);
            }

            public bool IsDefault => FuncPtr == default;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe readonly struct DspPanGetRolloffGainCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, DspState* state, Effects.Pan3DRolloffType rolloff, float distance, float minDistance, float MaxDistance, float* gain);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(DspState* state, Effects.Pan3DRolloffType rolloff, float distance, float minDistance, float MaxDistance, float* gain)
            {
                return InvokeImpl(FuncPtr, state, rolloff, distance, minDistance, MaxDistance, gain);
            }

            public bool IsDefault => FuncPtr == default;
        }

    }

}
