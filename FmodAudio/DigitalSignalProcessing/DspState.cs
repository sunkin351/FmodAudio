using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
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
            return Functions->Alloc(size, type, sourceString);
        }

        public IntPtr Reallocate(IntPtr ptr, uint size, MemoryType type, IntPtr sourceString)
        {
            return Functions->Realloc(ptr, size, type, sourceString);
        }

        public void Free(IntPtr ptr, MemoryType type, IntPtr sourceString)
        {
            Functions->Free(ptr, type, sourceString);
        }

        public Result GetSampleRate(int* sampleRate)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetSampleRate(ThisPtr, sampleRate);
        }

        public Result GetSampleRate(out int sampleRate)
        {
            fixed (int* pSampleRate = &sampleRate)
            {
                return GetSampleRate(pSampleRate);
            }
        }

        public Result GetBlockSize(uint* size)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetBlockSize(ThisPtr, size);
        }

        public Result GetBlockSize(out uint size)
        {
            fixed (uint* pSize = &size)
            {
                return GetBlockSize(pSize);
            }
        }

        public Result GetSpeakerMode(SpeakerMode* mixer, SpeakerMode* output)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetSpeakerMode(ThisPtr, mixer, output);
        }

        public Result GetSpeakerMode(out SpeakerMode mixer, out SpeakerMode output)
        {
            fixed (SpeakerMode* pMixer = &mixer, pOutput = &output)
            {
                return GetSpeakerMode(pMixer, pOutput);
            }
        }

        public Result GetDspClock(ulong* clock, uint* offset, uint* length)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetDSPClock(ThisPtr, clock, offset, length);
        }

        public Result GetDspClock(out ulong clock, out uint offset, out uint length)
        {
            fixed (ulong* pClock = &clock)
            fixed (uint* pOffset = &offset, pLength = &length)
            {
                return GetDspClock(pClock, pOffset, pLength);
            }
        }

        public Result ListenerAttributes(int* listenerCount, Attributes3D* attributes)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            return Functions->GetListenerAttributes(ThisPtr, listenerCount, attributes);
        }

        public Result ListenerAttributes(out int listenerCount, out Attributes3D attributes)
        {
            fixed (int* pListenerCount = &listenerCount)
            fixed (Attributes3D* pAttributes = &attributes)
            {
                return ListenerAttributes(pListenerCount, pAttributes);
            }
        }

        internal Result GetUserData(out IntPtr userData)
        {
            // This struct is expected to always live within unmanaged memory, or on the stack. Meaning, it won't be moved by the GC
            var ThisPtr = (DspState*)Unsafe.AsPointer(ref this);

            fixed (IntPtr* pUserData = &userData)
                return Functions->GetUserData(ThisPtr, pUserData);
        }
    }
}
