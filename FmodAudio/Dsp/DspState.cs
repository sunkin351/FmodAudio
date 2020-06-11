using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    /// <summary>
    /// DSP plugin structure that is passed into each callback.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DspState
    {
        /// <summary>
        /// [r] Handle to the DSP hand the user created.  Not to be modified.
        /// </summary>
        public readonly IntPtr instance;

        /// <summary>
        /// [r/w] Plugin writer created data the output author wants to attach to this object.
        /// </summary>
        public IntPtr plugindata;

        /// <summary>
        /// [r] Specifies which speakers the DSP effect is active on
        /// </summary>
        public readonly ChannelMask ChannelMask;

        /// <summary>
        /// [r] Specifies which speaker mode the signal originated for information purposes, ie in case panning needs to be done differently.
        /// </summary>
        public readonly SpeakerMode SourceSpeakerMode;

        /// <summary>
        /// [r] The mixed result of all incoming sidechains is stored at this pointer address.
        /// </summary>
        public readonly IntPtr sidechaindata;

        /// <summary>
        /// [r] The number of channels of pcm data stored within the sidechain buffer.
        /// </summary>
        public readonly int sidechainchannels;

        /// <summary>
        /// [r] Struct containing callbacks for system level functionality.
        /// </summary>
        public readonly DspStateFunctions* Functions;

        /// <summary>
        /// [r] FMOD::System object index, relating to the System object that created this DSP.
        /// </summary>
        public readonly int SystemObjectIndex;

        public IntPtr Allocate(uint size, MemoryType type, IntPtr sourceString)
        {
            return Functions->Alloc.Invoke(size, type, sourceString);
        }

        public IntPtr Reallocate(IntPtr ptr, uint size, MemoryType type, IntPtr sourceString)
        {
            return Functions->Realloc.Invoke(ptr, size, type, sourceString);
        }

        public void Free(IntPtr ptr, MemoryType type, IntPtr sourceString)
        {
            Functions->Free.Invoke(ptr, type, sourceString);
        }

        public Result GetSampleRate(out int sampleRate)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetSampleRate.Invoke(ThisPtr, out sampleRate);
        }

        public Result GetBlockSize(out uint size)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetBlockSize.Invoke(ThisPtr, out size);
        }

        public Result GetSpeakerMode(out SpeakerMode mixer, out SpeakerMode output)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetSpeakerMode.Invoke(ThisPtr, out mixer, out output);
        }

        public Result GetDspClock(out ulong clock, out uint offset, out uint length)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetDSPClock.Invoke(ThisPtr, out clock, out offset, out length);
        }

        public Result ListenerAttributes(out int listenerCount, out Attributes3D attributes)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetListenerAttributes.Invoke(ThisPtr, out listenerCount, out attributes);
        }

        internal Result GetUserData(out IntPtr userData)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetUserData.Invoke(ThisPtr, out userData);
        }
    }
}
