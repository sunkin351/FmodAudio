using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable

namespace FmodAudio
{
    using Base;

    public unsafe abstract class ChannelControl
    {
        protected readonly FmodLibrary library = Fmod.Library;

        protected readonly IntPtr Handle;

        protected ChannelControl(ChannelHandle handle)
        {
            Debug.Assert(this is Channel);
            Handle = handle.Handle;
        }

        protected ChannelControl(ChannelGroupHandle handle)
        {
            Debug.Assert(this is ChannelGroup);
            Handle = handle.Handle;
        }

        #region General Control Functionality for Channels and ChannelGroups

        public abstract void Stop();

        public abstract bool Paused { get; set; }

        public abstract float Volume { get; set; }

        public abstract bool VolumeRamp { get; set; }

        public abstract float Audibility { get; }

        public abstract float Pitch { get; set; }

        public abstract bool Mute { get; set; }

        public abstract void GetReverbProperties(int instance, out float wet);

        public abstract void SetReverbProperties(int instance, float wet);

        public abstract float LowPassGain { get; set; }

        public abstract Mode Mode { get; set; }

        public abstract void SetCallback(delegate* unmanaged<IntPtr, ChannelControlType, ChannelControlCallbackType, IntPtr, IntPtr, void> callback);

        public abstract bool IsPlaying { get; }


        #endregion

        #region Panning and Level adjustment

        public abstract void SetPan(float pan);

        public abstract void SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

        public abstract unsafe void SetMixLevelsInput(ReadOnlySpan<float> mixLevels);

        public abstract unsafe void SetMixMatrix(float* matrix, int outChannels, int inChannels, int inChannelHop = 0);

        public unsafe void SetMixMatrix(ReadOnlySpan<float> matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            if (outChannels * inChannels > matrix.Length)
                throw new ArgumentException("");

            fixed (float* pMatrix = matrix)
            {
                SetMixMatrix(pMatrix, outChannels, inChannels, inChannelHop);
            }
        }

        public abstract unsafe void GetMixMatrix(float* matrix, int* outChannels, int* inChannels, int inChannelHop = 0);

        public unsafe bool TryGetMixMatrix(Span<float> matrix, out int outChannels, out int inChannels, int inChannelHop = 0)
        {
            int Out, In;

            GetMixMatrix(null, &Out, &In, inChannelHop);

            if (matrix.Length < Out * In)
            {
                outChannels = 0;
                inChannels = 0;
                return false;
            }

            fixed(float* mat = matrix)
            {
                GetMixMatrix(mat, &Out, &In, inChannelHop);
            }

            outChannels = Out;
            inChannels = In;
            return true;
        }

        #endregion

        #region Clock based functionality

        public abstract void GetDSPClock(out ulong dspClock, out ulong parentClock);

        public abstract void SetDelay(ulong dspclock_start, ulong dspclock_end, FmodBool stopchannels);

        public void SetDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels = true)
        {
            SetDelay(dspclock_start, dspclock_end, (FmodBool)stopchannels);
        }

        public abstract void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopchannels);

        public void GetDelay(out ulong dspclock_start, out ulong dspclock_end)
        {
            GetDelay(out dspclock_start, out dspclock_end, out _);
        }

        public abstract void AddFadePoint(ulong dspClock, float volume);

        public abstract void SetFadePointRamp(ulong dspClock, float volume);

        public abstract void RemoveFadePoints(ulong dspClockStart, ulong dspClockEnd);

        public abstract unsafe void GetFadePoints(uint* count, ulong* dspClocks, float* volumes);

        public unsafe bool TryGetFadePoints(out ulong[]? dspClocks, out float[]? volumes)
        {
            Unsafe.SkipInit(out dspClocks);
            Unsafe.SkipInit(out volumes);

            uint pointCount = 0;
            GetFadePoints(&pointCount, null, null);

            if (pointCount == 0)
            {
                return false;
            }

            var clocks = new ulong[pointCount];
            var vols = new float[pointCount];

            fixed(ulong* t0 = clocks)
            fixed(float* t1 = vols)
            {
                GetFadePoints(&pointCount, t0, t1);
            }

            dspClocks = clocks;
            volumes = vols;
            return true;
        }

        #endregion

        #region DSP Effects

        public abstract DspHandle GetDSP(int index);

        public abstract DspHandle GetDSP(ChannelControlDSPIndex index);

        public abstract void AddDSP(int index, DspHandle dsp);

        public abstract void AddDSP(ChannelControlDSPIndex index, DspHandle dsp);

        public abstract void RemoveDSP(DspHandle dsp);

        public abstract int DspCount { get; }

        public abstract void SetDSPIndex(DspHandle dsp, int index);

        public abstract int GetDSPIndex(DspHandle dsp);

        #endregion

        #region 3D Effects

        public abstract void Set3DAttributes(in Vector3 pos, in Vector3 vel, in Vector3 altPanPos);

        public abstract void Get3DAttributes(out Vector3 pos, out Vector3 vel, out Vector3 altPanPos);

        public abstract void Set3DMinMaxDistance(float minDistance, float maxDistance);

        public abstract void Get3DMinMaxDistance(out float minDistance, out float maxDistance);

        public abstract void Set3DConeSettings(float insideConeAngle, float outsideConeAngle, float outsideVolume);

        public abstract void Get3DConeSettings(out float insideConeAngle, out float outsideConeAngle, out float outsideVolume);

        public abstract void Set3DConeOrientation(in Vector3 orientation);

        public abstract void Get3DConeOrientation(out Vector3 orientation);

        public abstract unsafe void Get3DCustomRolloff(out Vector3* points, out int count);

        public abstract unsafe void Set3DCustomRolloff(Vector3* points, int count);

        public abstract void Set3DOcclusion(float directOcclusion, float reverbOcclusion);

        public abstract void Get3DOcclusion(out float directOcclusion, out float reverbOcclusion);

        /// <summary>
        /// value is the Spread Angle.
        /// </summary>
        public abstract float Spread { get; set; }

        public abstract float Level3D { get; set; }

        public abstract float DopplerLevel3D { get; set; }

        public abstract void Set3DDistanceFilter(FmodBool custom, float customLevel, float centerFreq);

        public abstract void Get3DDistanceFilter(out FmodBool custom, out float customLevel, out float centerFreq);

        internal abstract unsafe IntPtr UserData { get; set; }

        #endregion
    }
}
