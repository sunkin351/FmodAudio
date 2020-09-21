using System;
using System.Numerics;
using System.Threading.Tasks;
using FmodAudio.Base;

#nullable enable

namespace FmodAudio
{
    public sealed unsafe class Channel : ChannelControl
    {
        private new ChannelHandle Handle => new ChannelHandle(base.Handle);

        public static implicit operator Channel?(ChannelHandle handle)
        {
            return handle.IsNull() ? null : new Channel(handle);
        }

        public static implicit operator ChannelHandle(Channel? channel)
        {
            return channel is null ? default : channel.Handle;
        }

        internal Channel(ChannelHandle handle) : base(handle)
        {
        }

        public override bool IsPlaying
        {
            get
            {
                FmodBool value;
                var res = library.Channel_IsPlaying(Handle, &value);

                switch (res)
                {
                    case Result.Ok:
                        return value;
                    case Result.Err_Invalid_Handle:
                    case Result.Err_Channel_Stolen:
                        return false;
                    default:
                        res.CheckResult();
                        return false;
                }
            }
        }

        #region Channel-Specific Control Functionality

        public float Frequency
        {
            get
            {
                float value;
                library.Channel_GetFrequency(Handle, &value).CheckResult();

                return value;
            }

            set
            {
                library.Channel_SetFrequency(Handle, value).CheckResult();
            }
        }

        public int Priority
        {
            get
            {
                int value;
                library.Channel_GetPriority(Handle, &value).CheckResult();

                return value;
            }

            set
            {
                library.Channel_SetPriority(Handle, value).CheckResult();
            }
        }

        public void SetPosition(TimeUnit unit, uint position)
        {
            library.Channel_SetPosition(Handle, position, unit).CheckResult();
        }

        public uint GetPosition(TimeUnit unit)
        {
            library.Channel_GetPosition(Handle, out uint position, unit).CheckResult();

            return position;
        }
        
        public ChannelGroup ChannelGroup
        {
            get
            {
                ChannelGroupHandle handle;
                library.Channel_GetChannelGroup(Handle, &handle).CheckResult();

                return handle;
            }

            set
            {
                library.Channel_SetChannelGroup(Handle, value).CheckResult();
            }
        }

        public int LoopCount
        {
            get
            {
                int value;
                library.Channel_GetLoopCount(Handle, &value).CheckResult();

                return value;
            }

            set
            {
                library.Channel_SetLoopCount(Handle, value).CheckResult();
            }
        }

        public void SetLoopPoints(TimeUnit startUnit, uint loopStart, TimeUnit endUnit, uint loopEnd)
        {
            library.Channel_SetLoopPoints(Handle, loopStart, startUnit, loopEnd, endUnit).CheckResult();
        }

        public void GetLoopPoints(TimeUnit startUnit, out uint loopStart, TimeUnit endUnit, out uint loopEnd)
        {
            library.Channel_GetLoopPoints(Handle, out loopStart, startUnit, out loopEnd, endUnit).CheckResult();
        }

        #endregion

        #region Information Only Functions

        public bool IsVirtual
        {
            get
            {
                FmodBool value;
                library.Channel_IsVirtual(Handle, &value).CheckResult();

                return value;
            }
        }

        public Sound CurrentSound
        {
            get
            {
                SoundHandle handle;
                library.Channel_GetCurrentSound(Handle, &handle).CheckResult();

                return handle;
            }
        }

        public int Index
        {
            get
            {
                int value;
                library.Channel_GetIndex(Handle, &value).CheckResult();

                return value;
            }
        }

        public override void Stop()
        {
            library.Channel_Stop(Handle).CheckResult();
        }

        public override bool Paused
        {
            get 
            {
                FmodBool value;
                library.Channel_GetPaused(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.Channel_SetPaused(Handle, value).CheckResult();
            }
        }

        public override float Volume
        {
            get
            {
                float value;
                library.Channel_GetVolume(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetVolume(Handle, value).CheckResult();
            }
        }
        
        public override bool VolumeRamp
        {
            get
            {
                FmodBool value;
                library.Channel_GetVolumeRamp(Handle, &value).CheckResult();
                return value;
            }
            
            set
            {
                library.Channel_SetVolumeRamp(Handle, value).CheckResult();
            }
        }

        public override float Audibility
        {
            get
            {
                float value;
                library.Channel_GetAudibility(Handle, &value).CheckResult();
                return value;
            }
        }

        public override float Pitch
        {
            get
            {
                float value;
                library.Channel_GetPitch(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetPitch(Handle, value).CheckResult();
            }
        }
        
        public override bool Mute
        {
            get
            {
                FmodBool value;
                library.Channel_GetMute(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetMute(Handle, value).CheckResult();
            }
        }

        public override void GetReverbProperties(int instance, out float wet)
        {
            library.Channel_GetReverbProperties(Handle, instance, out wet).CheckResult();
        }

        public override void SetReverbProperties(int instance, float wet)
        {
            library.Channel_SetReverbProperties(Handle, instance, wet).CheckResult();
        }

        public override float LowPassGain
        {
            get
            {
                library.Channel_GetLowPassGain(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetLowPassGain(Handle, value).CheckResult();
            }
        }

        public override Mode Mode
        {
            get
            {
                Mode value;
                library.Channel_GetMode(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetMode(Handle, value).CheckResult();
            }
        }

        public override unsafe void SetCallback(delegate* unmanaged<IntPtr, ChannelControlType, ChannelControlCallbackType, IntPtr, IntPtr, void> callback)
        {
            library.Channel_SetCallback(Handle, callback).CheckResult();
        }

        public override void SetPan(float pan)
        {
            library.Channel_SetPan(Handle, pan).CheckResult();
        }

        public override void SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            library.Channel_SetMixLevelsOutput(Handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright).CheckResult();
        }

        public override void SetMixLevelsInput(ReadOnlySpan<float> mixLevels)
        {
            library.Channel_SetMixLevelsInput(Handle, mixLevels).CheckResult();
        }

        public override void SetMixMatrix(float* matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            library.Channel_SetMixMatrix(Handle, matrix, outChannels, inChannels, inChannelHop).CheckResult();
        }

        public override unsafe void GetMixMatrix(float* matrix, int* outChannels, int* inChannels, int inChannelHop = 0)
        {
            library.Channel_GetMixMatrix(Handle, matrix, outChannels, inChannels, inChannelHop).CheckResult();
        }

        public override void GetDSPClock(out ulong dspClock, out ulong parentClock)
        {
            library.Channel_GetDSPClock(Handle, out dspClock, out parentClock).CheckResult();
        }

        public override void SetDelay(ulong dspclock_start, ulong dspclock_end, FmodBool stopChannels)
        {
            library.Channel_SetDelay(Handle, dspclock_start, dspclock_end, stopChannels).CheckResult();
        }

        public override void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopChannels)
        {
            library.Channel_GetDelay(Handle, out dspclock_start, out dspclock_end, out stopChannels).CheckResult();
        }

        public override void AddFadePoint(ulong dspClock, float volume)
        {
            library.Channel_AddFadePoint(Handle, dspClock, volume).CheckResult();
        }

        public override void SetFadePointRamp(ulong dspClock, float volume)
        {
            library.Channel_SetFadePointRamp(Handle, dspClock, volume).CheckResult();
        }

        public override void RemoveFadePoints(ulong dspClockStart, ulong dspClockEnd)
        {
            library.Channel_RemoveFadePoints(Handle, dspClockStart, dspClockEnd).CheckResult();
        }

        public override unsafe void GetFadePoints(uint* count, ulong* dspClocks, float* volumes)
        {
            library.Channel_GetFadePoints(Handle, count, dspClocks, volumes).CheckResult();
        }

        public override DspHandle GetDSP(int index)
        {
            throw new NotImplementedException();
        }

        public override DspHandle GetDSP(ChannelControlDSPIndex index)
        {
            throw new NotImplementedException();
        }

        public override void AddDSP(int index, DspHandle dsp)
        {
            library.Channel_AddDSP(Handle, index, dsp).CheckResult();
        }

        public override void AddDSP(ChannelControlDSPIndex index, DspHandle dsp)
        {
            library.Channel_AddDSP(Handle, index, dsp).CheckResult();
        }

        public override void RemoveDSP(DspHandle dsp)
        {
            library.Channel_RemoveDSP(Handle, dsp).CheckResult();
        }

        public override int DspCount
        {
            get
            {
                int value;
                library.Channel_GetNumDSPs(Handle, &value).CheckResult();
                return value;
            }
        }

        public override void SetDSPIndex(DspHandle dsp, int index)
        {
            library.Channel_SetDSPIndex(Handle, dsp, index).CheckResult();
        }

        public override int GetDSPIndex(DspHandle dsp)
        {
            int value;
            library.Channel_GetDSPIndex(Handle, dsp, &value).CheckResult();
            return value;
        }

        public override void Set3DAttributes(in Vector3 pos, in Vector3 vel, in Vector3 altPanPos)
        {
            library.Channel_Set3DAttributes(Handle, in pos, in vel, in altPanPos).CheckResult();
        }

        public override void Get3DAttributes(out Vector3 pos, out Vector3 vel, out Vector3 altPanPos)
        {
            library.Channel_Get3DAttributes(Handle, out pos, out vel, out altPanPos).CheckResult();
        }

        public override void Set3DMinMaxDistance(float minDistance, float maxDistance)
        {
            library.Channel_Set3DMinMaxDistance(Handle, minDistance, maxDistance).CheckResult();
        }

        public override void Get3DMinMaxDistance(out float minDistance, out float maxDistance)
        {
            library.Channel_Get3DMinMaxDistance(Handle, out minDistance, out maxDistance).CheckResult();
        }

        public override void Set3DConeSettings(float insideConeAngle, float outsideConeAngle, float outsideVolume)
        {
            library.Channel_Set3DConeSettings(Handle, insideConeAngle, outsideConeAngle, outsideVolume).CheckResult();
        }

        public override void Get3DConeSettings(out float insideConeAngle, out float outsideConeAngle, out float outsideVolume)
        {
            library.Channel_Get3DConeSettings(Handle, out insideConeAngle, out outsideConeAngle, out outsideVolume).CheckResult();
        }

        public override void Set3DConeOrientation(in Vector3 orientation)
        {
            library.Channel_Set3DConeOrientation(Handle, in orientation).CheckResult();
        }

        public override void Get3DConeOrientation(out Vector3 orientation)
        {
            library.Channel_Get3DConeOrientation(Handle, out orientation).CheckResult();
        }

        public override unsafe void Get3DCustomRolloff(out Vector3* points, out int count)
        {
            library.Channel_Get3DCustomRolloff(Handle, out points, out count).CheckResult();
        }

        public override unsafe void Set3DCustomRolloff(Vector3* points, int count)
        {
            library.Channel_Set3DCustomRolloff(Handle, points, count).CheckResult();
        }

        public override void Set3DOcclusion(float directOcclusion, float reverbOcclusion)
        {
            library.Channel_Set3DOcclusion(Handle, directOcclusion, reverbOcclusion).CheckResult();
        }

        public override void Get3DOcclusion(out float directOcclusion, out float reverbOcclusion)
        {
            library.Channel_Get3DOcclusion(Handle, out directOcclusion, out reverbOcclusion).CheckResult();
        }

        public override float Spread
        {
            get
            {
                library.Channel_Get3DSpread(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_Set3DSpread(Handle, value).CheckResult();
            }
        }

        public override float Level3D
        {
            get
            {
                library.Channel_Get3DLevel(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_Set3DLevel(Handle, value).CheckResult();
            }
        }

        public override float DopplerLevel3D
        {
            get
            {
                library.Channel_Get3DDopplerLevel(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_Set3DDopplerLevel(Handle, value).CheckResult();
            }
        }

        public override void Set3DDistanceFilter(FmodBool custom, float customLevel, float centerFreq)
        {
            library.Channel_Set3DDistanceFilter(Handle, custom, customLevel, centerFreq).CheckResult();
        }

        public override void Get3DDistanceFilter(out FmodBool custom, out float customLevel, out float centerFreq)
        {
            library.Channel_Get3DDistanceFilter(Handle, out custom, out customLevel, out centerFreq).CheckResult();
        }

        internal override IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.Channel_GetUserData(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Channel_SetUserData(Handle, value).CheckResult();
            }
        }

        #endregion

        public static bool operator ==(Channel? l, Channel? r)
        {
            if (ReferenceEquals(l, r))
                return true;

            if (l is null || r is null)
                return false;

            return l.Handle == r.Handle;
        }

        public static bool operator !=(Channel? l, Channel? r)
        {
            return !(l == r);
        }
    }
}
