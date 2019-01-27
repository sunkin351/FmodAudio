using System;
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
    }
}
