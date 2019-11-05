using System;
using System.Numerics;

namespace FmodAudio
{
    using Dsp;
    using Interop;

    public abstract class ChannelControl : HandleBase
    {
        protected readonly NativeLibrary library;
        public FmodSystem SystemObject { get; }

        protected ChannelControl(FmodSystem sys, IntPtr handle) : base(handle)
        {
            SystemObject = sys;
            library = sys.library;
        }

        internal ChannelControl(FmodSystem sys, IntPtr handle, Memory.SaferPointer customRolloff) : this(sys, handle)
        {
            CustomRolloff = customRolloff;
        }

        #region General Control Functionality for Channels and ChannelGroups

        public void Stop()
        {
            library.ChannelGroup_Stop(Handle).CheckResult();
        }

        public bool Paused
        {
            get
            {
                library.ChannelGroup_GetPaused(Handle, out bool value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetPaused(Handle, value).CheckResult();
            }
        }

        public float Volume
        {
            get
            {
                library.ChannelGroup_GetVolume(Handle, out float value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetVolume(Handle, value).CheckResult();
            }
        }

        public bool VolumeRamp
        {
            get
            {
                library.ChannelGroup_GetVolumeRamp(Handle, out bool value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetVolumeRamp(Handle, value).CheckResult();
            }
        }

        public float Audibility
        {
            get
            {
                library.ChannelGroup_GetAudibility(Handle, out float value).CheckResult();

                return value;
            }
        }

        public float Pitch
        {
            get
            {
                library.ChannelGroup_GetPitch(Handle, out float value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetPitch(Handle, value).CheckResult();
            }
        }

        public bool Mute
        {
            get
            {
                library.ChannelGroup_GetMute(Handle, out bool value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetMute(Handle, value).CheckResult();
            }
        }

        public void GetReverbProperties(int instance, out float wet)
        {
            library.ChannelGroup_GetReverbProperties(Handle, instance, out wet).CheckResult();
        }

        public void SetReverbProperties(int instance, float wet)
        {
            library.ChannelGroup_SetReverbProperties(Handle, instance, wet).CheckResult();
        }

        public float LowPassGain
        {
            get
            {
                library.ChannelGroup_GetLowPassGain(Handle, out float value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetLowPassGain(Handle, value).CheckResult();
            }
        }

        public Mode Mode
        {
            get
            {
                library.ChannelGroup_GetMode(Handle, out Mode value).CheckResult();

                return value;
            }

            set
            {
                library.ChannelGroup_SetMode(Handle, value).CheckResult();
            }
        }
        
        ChannelCallbackInternal InternalCallback;
        ChannelCallback Callback;

        public void SetCallback(ChannelCallback callback)
        {
            if (ReferenceEquals(callback, Callback))
            {
                return;
            }

            ChannelCallbackInternal cb = delegate (IntPtr rawchannel, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2)
            {
                if (rawchannel != Handle)
                {
                    return Result.Err_Internal;
                }

                Result result = Result.Ok;

                try
                {
                    callback(this, controltype, type, commanddata1, commanddata2);
                }
                catch (FmodException e)
                {
                    result = e.Result ?? Result.Err_Internal;
                }
                catch (Exception e)
                {
                    result = (e.InnerException as FmodException)?.Result ?? Result.Err_Internal;
                }

                return result;
            };
            
            library.ChannelGroup_SetCallback(Handle, cb).CheckResult();

            InternalCallback = cb;
            Callback = callback;
        }

        public bool IsPlaying
        {
            get
            {
                var res = library.ChannelGroup_IsPlaying(Handle, out bool value);

                if (res != Result.Ok)
                {
                    if (res == Result.Err_Invalid_Handle)
                        return false;

                    res.CheckResult();
                }

                return value;
            }
        }

        #endregion

        #region Panning and Level adjustment

        public void SetPan(float pan)
        {
            library.ChannelGroup_SetPan(Handle, pan).CheckResult();
        }

        public void SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            library.ChannelGroup_SetMixLevelsOutput(Handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright).CheckResult();
        }

        public unsafe void SetMixLevelsInput(Span<float> mixLevels)
        {
            Result res;

            fixed(float* levels = mixLevels)
                res = library.ChannelGroup_SetMixLevelsInput(Handle, levels, mixLevels.Length);

            res.CheckResult();
        }

        public unsafe void SetMixMatrix(ReadOnlySpan<float> matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            Result res;

            if(matrix.Length < (outChannels * inChannels))
            {
                throw new ArgumentOutOfRangeException($"{nameof(outChannels)}, {nameof(inChannels)}", "matrix length needs to be at least 'outChannels * inChannels'");
            }

            fixed(float* mx = matrix)
            {
                res = library.ChannelGroup_SetMixMatrix(Handle, mx, outChannels, inChannels, inChannelHop);
            }

            res.CheckResult();
        }

        public unsafe void GetMixMatrix(out float[] matrix, out int outChannels, out int inChannels, int inChannelHop = 0)
        {
            matrix = null;
            outChannels = 0;
            inChannels = 0;

            library.ChannelGroup_GetMixMatrix(Handle, null, out outChannels, out inChannels, inChannelHop).CheckResult();

            var tmp = new float[outChannels * inChannels];

            Result res;

            fixed(float* mat = tmp)
            {
                res = library.ChannelGroup_GetMixMatrix(Handle, mat, out outChannels, out inChannels, inChannelHop);
            }

            res.CheckResult();
            matrix = tmp;
        }

        #endregion

        #region Clock based functionality

        public void GetDSPClock(out ulong dspClock, out ulong parentClock)
        {
            library.ChannelGroup_GetDSPClock(Handle, out dspClock, out parentClock).CheckResult();
        }

        public void SetDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels)
        {
            library.ChannelGroup_SetDelay(Handle, dspclock_start, dspclock_end, stopchannels).CheckResult();
        }

        public void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            library.ChannelGroup_GetDelay(Handle, out dspclock_start, out dspclock_end, out stopchannels).CheckResult();
        }

        public void AddFadePoint(ulong dspClock, float volume)
        {
            library.ChannelGroup_AddFadePoint(Handle, dspClock, volume).CheckResult();
        }

        public void SetFadePointRamp(ulong dspClock, float volume)
        {
            library.ChannelGroup_SetFadePointRamp(Handle, dspClock, volume).CheckResult();
        }

        public void RemoveFadePoints(ulong dspClockStart, ulong dspClockEnd)
        {
            library.ChannelGroup_RemoveFadePoints(Handle, dspClockStart, dspClockEnd).CheckResult();
        }

        public unsafe void GetFadePoints(out ulong[] dspClocks, out float[] volumes)
        {
            dspClocks = null;
            volumes = null;

            uint pointCount = 0;
            library.ChannelGroup_GetFadePoints(Handle, ref pointCount, null, null).CheckResult();

            if (pointCount == 0)
                return;

            var clocks = new ulong[pointCount];
            var vols = new float[pointCount];
            Result res;

            fixed(ulong* t0 = clocks)
            fixed(float* t1 = vols)
            {
                res = library.ChannelGroup_GetFadePoints(Handle, ref pointCount, t0, t1);
            }

            res.CheckResult();

            dspClocks = clocks;
            volumes = vols;
        }

        #endregion

        #region DSP Effects

        public DSP GetDSP(int index)
        {
            library.ChannelGroup_GetDSP(Handle, index, out IntPtr handle).CheckResult();

            return SystemObject.GetDSP(handle);
        }

        public void AddDSP(int index, DSP dsp)
        {
            if (dsp is null)
                throw new ArgumentNullException(nameof(dsp));

            library.ChannelGroup_AddDSP(Handle, index, dsp.Handle);
        }

        public void RemoveDSP(DSP dsp)
        {
            if (dsp is null)
                throw new ArgumentNullException(nameof(dsp));

            library.ChannelGroup_RemoveDSP(Handle, dsp.Handle).CheckResult();
        }

        public int DSPCount
        {
            get
            {
                library.ChannelGroup_GetNumDSPs(Handle, out int value).CheckResult();
                return value;
            }
        }

        public void SetDSPIndex(DSP dsp, int index)
        {
            if (dsp is null)
                throw new ArgumentNullException(nameof(dsp));

            library.ChannelGroup_SetDSPIndex(Handle, dsp.Handle, index).CheckResult();
        }

        public int GetDSPIndex(DSP dsp)
        {
            if (dsp is null)
                throw new ArgumentNullException(nameof(dsp));

            library.ChannelGroup_GetDSPIndex(Handle, dsp.Handle, out int index).CheckResult();
            return index;
        }

        #endregion

        #region 3D Effects

        public void Set3DAttributes(ref Vector3 pos, ref Vector3 vel, ref Vector3 altPanPos)
        {
            library.ChannelGroup_Set3DAttributes(Handle, ref pos, ref vel, ref altPanPos).CheckResult();
        }

        public void Set3DAttributes(ref Vector3 pos, ref Vector3 vel)
        {
            Vector3 altPanPos = default;
            Set3DAttributes(ref pos, ref vel, ref altPanPos);
        }

        public void Get3DAttributes(out Vector3 pos, out Vector3 vel, out Vector3 altPanPos)
        {
            library.ChannelGroup_Get3DAttributes(Handle, out pos, out vel, out altPanPos).CheckResult();
        }

        public void Set3DMinMaxDistance(float minDistance, float maxDistance)
        {
            library.ChannelGroup_Set3DMinMaxDistance(Handle, minDistance, maxDistance).CheckResult();
        }

        public void Get3DMinMaxDistance(out float minDistance, out float maxDistance)
        {
            library.ChannelGroup_Get3DMinMaxDistance(Handle, out minDistance, out maxDistance).CheckResult();
        }

        public void Set3DConeSettings(float insideConeAngle, float outsideConeAngle, float outsideVolume)
        {
            library.ChannelGroup_Set3DConeSettings(Handle, insideConeAngle, outsideConeAngle, outsideVolume).CheckResult();
        }

        public void Get3DConeSettings(out float insideConeAngle, out float outsideConeAngle, out float outsideVolume)
        {
            library.ChannelGroup_Get3DConeSettings(Handle, out insideConeAngle, out outsideConeAngle, out outsideVolume).CheckResult();
        }

        public void Set3DConeOrientation(ref Vector3 orientation)
        {
            library.ChannelGroup_Set3DConeOrientation(Handle, ref orientation).CheckResult();
        }

        public void Get3DConeOrientation(out Vector3 orientation)
        {
            library.ChannelGroup_Get3DConeOrientation(Handle, out orientation).CheckResult();
        }

        private Memory.SaferPointer CustomRolloff = null;

        public void Set3DCustomRolloff(ReadOnlySpan<Vector3> rolloff)
        {
            if (this is Channel)
                throw new NotSupportedException();

            var ptr = FmodHelpers.AllocateCustomRolloff(rolloff);
            int count = 0;

            if (ptr != null)
            {
                count = rolloff.Length;
            }
            else if (CustomRolloff == null)
            {
                return;
            }

            library.ChannelGroup_Set3DCustomRolloff(Handle, ptr, count).CheckResult();

            CustomRolloff = ptr;
        }

        public unsafe ReadOnlySpan<Vector3> Get3DCustomRolloff()
        {
            library.ChannelGroup_Get3DCustomRolloff(Handle, out IntPtr points, out int pointCount).CheckResult();

            if (points == IntPtr.Zero)
                return default;

            return new ReadOnlySpan<Vector3>((void*)points, pointCount);
        }

        public void Set3DOcclusion(float directOcclusion, float reverbOcclusion)
        {
            library.ChannelGroup_Set3DOcclusion(Handle, directOcclusion, reverbOcclusion).CheckResult();
        }

        public void Get3DOcclusion(out float directOcclusion, out float reverbOcclusion)
        {
            library.ChannelGroup_Get3DOcclusion(Handle, out directOcclusion, out reverbOcclusion).CheckResult();
        }

        /// <summary>
        /// value is the Spread Angle.
        /// </summary>
        public float Spread
        {
            get
            {
                library.ChannelGroup_Get3DSpread(Handle, out float Angle).CheckResult();

                return Angle;
            }
            set
            {
                library.ChannelGroup_Set3DSpread(Handle, value).CheckResult();
            }
        }

        public float Level3D
        {
            get
            {
                library.ChannelGroup_Get3DLevel(Handle, out float level).CheckResult();

                return level;
            }
            set
            {
                library.ChannelGroup_Set3DLevel(Handle, value).CheckResult();
            }
        }

        public float DopplerLevel3D
        {
            get
            {
                library.ChannelGroup_Get3DDopplerLevel(Handle, out float level).CheckResult();

                return level;
            }
            set
            {
                library.ChannelGroup_Set3DDopplerLevel(Handle, value).CheckResult();
            }
        }

        public void Set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
        {
            library.ChannelGroup_Set3DDistanceFilter(Handle, custom, customLevel, centerFreq).CheckResult();
        }

        public void Get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
        {
            library.ChannelGroup_Get3DDistanceFilter(Handle, out custom, out customLevel, out centerFreq).CheckResult();
        }

        public IntPtr UserData
        {
            get
            {
                library.ChannelGroup_GetUserData(Handle, out IntPtr value).CheckResult();

                return value;
            }
            set
            {
                library.ChannelGroup_SetUserData(Handle, value).CheckResult();
            }
        }

        #endregion
    }
}
