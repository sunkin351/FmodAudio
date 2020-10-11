using System;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStateFunctions
    {
        public readonly delegate* unmanaged<uint, MemoryType, IntPtr, IntPtr> Alloc;
        public readonly delegate* unmanaged<IntPtr, uint, MemoryType, IntPtr, IntPtr> Realloc;
        public readonly delegate* unmanaged<IntPtr, MemoryType, IntPtr, void> Free;

        public readonly delegate* unmanaged<DspState*, int*, Result> GetSampleRate;
        public readonly delegate* unmanaged<DspState*, uint*, Result> GetBlockSize;
        public readonly DspStateDFTFunctions* DFT;
        public readonly DspStatePanFunctions* Pan;
        public readonly delegate* unmanaged<DspState*, SpeakerMode*, SpeakerMode*, Result> GetSpeakerMode;
        public readonly delegate* unmanaged<DspState*, ulong*, uint*, uint*, Result> GetDSPClock;
        public readonly delegate* unmanaged<DspState*, int*, Attributes3D*, Result> GetListenerAttributes;
        /// <summary>
        /// It is recommended to use your own logging methods
        /// </summary>
        public readonly delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, void> LogFuncPtr;
        internal readonly delegate* unmanaged<DspState*, IntPtr*, Result> GetUserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStateDFTFunctions
    {
        public readonly delegate* unmanaged<DspState*, int, float*, Fmod_Complex*, float*, int, Result> FFTReal;
        public readonly delegate* unmanaged<DspState*, int, Fmod_Complex*, float*, float*, int, Result> InverseFFTReal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct DspStatePanFunctions
    {
        public readonly delegate* unmanaged<DspState*, SpeakerMode, float, float, float*, Result> SumMonoMatrix;
        public readonly delegate* unmanaged<DspState*, SpeakerMode, float, float, float, int, float*, Result> SumSterioMatrix;
        public readonly delegate* unmanaged<DspState*, SpeakerMode, SpeakerMode, float, float, float, float, float, int, float*, PanSurroundFlags, Result> SumSurroundMatrix;
        public readonly delegate* unmanaged<DspState*, SpeakerMode, float, float, float, float, int, float*, Result> SumMonoToSurroundMatrix;
        public readonly delegate* unmanaged<DspState*, SpeakerMode, float, float, float, float, float, int, float*, Result> SumSterioToSurroundMatrix;
        public readonly delegate* unmanaged<DspState*, Effects.Pan3DRolloffType, float, float, float, float*, Result> GetRolloffGain;
    }

}
