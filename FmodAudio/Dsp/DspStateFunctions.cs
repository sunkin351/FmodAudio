using System;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStateFunctions
    {
        public readonly delegate* stdcall<uint, MemoryType, IntPtr, IntPtr> Alloc;
        public readonly delegate* stdcall<IntPtr, uint, MemoryType, IntPtr, IntPtr> Realloc;
        public readonly delegate* stdcall<IntPtr, MemoryType, IntPtr, void> Free;

        public readonly delegate* stdcall<DspState*, int*, Result> GetSampleRate;
        public readonly delegate* stdcall<DspState*, uint*, Result> GetBlockSize;
        public readonly DspStateDFTFunctions* DFT;
        public readonly DspStatePanFunctions* Pan;
        public readonly delegate* stdcall<DspState*, SpeakerMode*, SpeakerMode*, Result> GetSpeakerMode;
        public readonly delegate* stdcall<DspState*, ulong*, uint*, uint*, Result> GetDSPClock;
        public readonly delegate* stdcall<DspState*, int*, Attributes3D*, Result> GetListenerAttributes;
        /// <summary>
        /// It is recommended to use your own logging methods
        /// </summary>
        public readonly delegate* stdcall<DebugFlags, byte*, int, byte*, byte*, void> LogFuncPtr;
        internal readonly delegate* stdcall<DspState*, IntPtr*, Result> GetUserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStateDFTFunctions
    {
        public readonly delegate* stdcall<DspState*, int, float*, Fmod_Complex*, float*, int, Result> FFTReal;
        public readonly delegate* stdcall<DspState*, int, Fmod_Complex*, float*, float*, int, Result> InverseFFTReal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStatePanFunctions
    {
        public readonly delegate* stdcall<DspState*, SpeakerMode, float, float, float*, Result> SumMonoMatrix;
        public readonly delegate* stdcall<DspState*, SpeakerMode, float, float, float, int, float*, Result> SumSterioMatrix;
        public readonly delegate* stdcall<DspState*, SpeakerMode, SpeakerMode, float, float, float, float, float, int, float*, PanSurroundFlags, Result> SumSurroundMatrix;
        public readonly delegate* stdcall<DspState*, SpeakerMode, float, float, float, float, int, float*, Result> SumMonoToSurroundMatrix;
        public readonly delegate* stdcall<DspState*, SpeakerMode, float, float, float, float, float, int, float*, Result> SumSterioToSurroundMatrix;
        public readonly delegate* stdcall<DspState*, Effects.Pan3DRolloffType, float, float, float, float*, Result> GetRolloffGain;
    }

}
