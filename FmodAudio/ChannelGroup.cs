using System;
using System.Numerics;

using FmodAudio.Base;
#nullable enable

namespace FmodAudio
{
    public sealed unsafe class ChannelGroup : ChannelControl, IDisposable
    {
        public static implicit operator ChannelGroup?(ChannelGroupHandle handle)
        {
            return handle.IsNull() ? null : new ChannelGroup(handle);
        }

        public static implicit operator ChannelGroupHandle(ChannelGroup? channelGroup)
        {
            return channelGroup is null ? default : channelGroup.Handle;
        }

        private new ChannelGroupHandle Handle => new ChannelGroupHandle(base.Handle);

        private string? _name = null;

        internal ChannelGroup(ChannelGroupHandle handle) : base(handle)
        {
        }

        public override bool IsPlaying
        {
            get
            {
                FmodBool value;
                library.ChannelGroup_IsPlaying(Handle, &value).CheckResult();

                return value;
            }
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            library.ChannelGroup_Release(Handle).CheckResult();
        }

        public int GroupCount
        {
            get
            {
                library.ChannelGroup_GetNumGroups(Handle, out int value).CheckResult();

                return value;
            }
        }

        public ChannelGroupHandle ParentGroup
        {
            get
            {
                ChannelGroupHandle handle;

                library.ChannelGroup_GetParentGroup(Handle, &handle).CheckResult();

                return handle;
            }
        }

        public unsafe string Name
        {
            get
            {
                if (_name == null)
                {
                    const int len = Fmod.MaxInteropNameStringLength;
                    var ptr = stackalloc byte[len];

                    library.ChannelGroup_GetName(Handle, ptr, len).CheckResult();

                    _name = FmodHelpers.BufferToString(ptr, len);
                }

                return _name;
            }
        }

        public int ChannelCount
        {
            get
            {
                library.ChannelGroup_GetNumChannels(Handle, out int value).CheckResult();

                return value;
            }
        }

        public DspConnectionHandle AddGroup(ChannelGroup group, bool propagateDSPClock = true)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group));

            library.ChannelGroup_AddGroup(Handle, group.Handle, propagateDSPClock, out DspConnectionHandle dspConnectionHandle).CheckResult();

            return dspConnectionHandle;
        }
        
        public ChannelGroupHandle GetGroup(int index)
        {
            library.ChannelGroup_GetGroup(Handle, index, out ChannelGroupHandle handle).CheckResult();

            return handle;
        }

        public ChannelHandle GetChannel(int index)
        {
            library.ChannelGroup_GetChannel(Handle, index, out ChannelHandle handle).CheckResult();

            return handle;
        }

        public override void Stop()
        {
            library.ChannelGroup_Stop(Handle).CheckResult();
        }

        public override bool Paused
        {
            get
            {
                FmodBool value;
                library.ChannelGroup_GetPaused(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.ChannelGroup_SetPaused(Handle, value).CheckResult();
            }
        }

        public override float Volume
        {
            get
            {
                float value;
                library.ChannelGroup_GetVolume(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetVolume(Handle, value).CheckResult();
            }
        }

        public override bool VolumeRamp
        {
            get
            {
                FmodBool value;
                library.ChannelGroup_GetVolumeRamp(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.ChannelGroup_SetVolumeRamp(Handle, value).CheckResult();
            }
        }

        public override float Audibility
        {
            get
            {
                float value;
                library.ChannelGroup_GetAudibility(Handle, &value).CheckResult();
                return value;
            }
        }

        public override float Pitch
        {
            get
            {
                float value;
                library.ChannelGroup_GetPitch(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetPitch(Handle, value).CheckResult();
            }
        }

        public override bool Mute
        {
            get
            {
                FmodBool value;
                library.ChannelGroup_GetMute(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetMute(Handle, value).CheckResult();
            }
        }

        public override void GetReverbProperties(int instance, out float wet)
        {
            library.ChannelGroup_GetReverbProperties(Handle, instance, out wet).CheckResult();
        }

        public override void SetReverbProperties(int instance, float wet)
        {
            library.ChannelGroup_SetReverbProperties(Handle, instance, wet).CheckResult();
        }

        public override float LowPassGain
        {
            get
            {
                library.ChannelGroup_GetLowPassGain(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetLowPassGain(Handle, value).CheckResult();
            }
        }

        public override Mode Mode
        {
            get
            {
                Mode value;
                library.ChannelGroup_GetMode(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetMode(Handle, value).CheckResult();
            }
        }

        public override unsafe void SetCallback(delegate* unmanaged<IntPtr, ChannelControlType, ChannelControlCallbackType, IntPtr, IntPtr, void> callback)
        {
            library.ChannelGroup_SetCallback(Handle, callback).CheckResult();
        }

        public override void SetPan(float pan)
        {
            library.ChannelGroup_SetPan(Handle, pan).CheckResult();
        }

        public override void SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            library.ChannelGroup_SetMixLevelsOutput(Handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright).CheckResult();
        }

        public override void SetMixLevelsInput(ReadOnlySpan<float> mixLevels)
        {
            library.ChannelGroup_SetMixLevelsInput(Handle, mixLevels).CheckResult();
        }

        public override void SetMixMatrix(float* matrix, int outChannelGroups, int inChannelGroups, int inChannelGroupHop = 0)
        {
            library.ChannelGroup_SetMixMatrix(Handle, matrix, outChannelGroups, inChannelGroups, inChannelGroupHop).CheckResult();
        }

        public override unsafe void GetMixMatrix(float* matrix, int* outChannelGroups, int* inChannelGroups, int inChannelGroupHop = 0)
        {
            library.ChannelGroup_GetMixMatrix(Handle, matrix, outChannelGroups, inChannelGroups, inChannelGroupHop).CheckResult();
        }

        public override void GetDSPClock(out ulong dspClock, out ulong parentClock)
        {
            library.ChannelGroup_GetDSPClock(Handle, out dspClock, out parentClock).CheckResult();
        }

        public override void SetDelay(ulong dspclock_start, ulong dspclock_end, FmodBool stopChannelGroups)
        {
            library.ChannelGroup_SetDelay(Handle, dspclock_start, dspclock_end, stopChannelGroups).CheckResult();
        }

        public override void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopChannelGroups)
        {
            library.ChannelGroup_GetDelay(Handle, out dspclock_start, out dspclock_end, out stopChannelGroups).CheckResult();
        }

        public override void AddFadePoint(ulong dspClock, float volume)
        {
            library.ChannelGroup_AddFadePoint(Handle, dspClock, volume).CheckResult();
        }

        public override void SetFadePointRamp(ulong dspClock, float volume)
        {
            library.ChannelGroup_SetFadePointRamp(Handle, dspClock, volume).CheckResult();
        }

        public override void RemoveFadePoints(ulong dspClockStart, ulong dspClockEnd)
        {
            library.ChannelGroup_RemoveFadePoints(Handle, dspClockStart, dspClockEnd).CheckResult();
        }

        public override unsafe void GetFadePoints(uint* count, ulong* dspClocks, float* volumes)
        {
            library.ChannelGroup_GetFadePoints(Handle, count, dspClocks, volumes).CheckResult();
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
            library.ChannelGroup_AddDSP(Handle, index, dsp).CheckResult();
        }

        public override void AddDSP(ChannelControlDSPIndex index, DspHandle dsp)
        {
            library.ChannelGroup_AddDSP(Handle, index, dsp).CheckResult();
        }

        public override void RemoveDSP(DspHandle dsp)
        {
            library.ChannelGroup_RemoveDSP(Handle, dsp).CheckResult();
        }

        public override int DspCount
        {
            get
            {
                int value;
                library.ChannelGroup_GetNumDSPs(Handle, &value).CheckResult();
                return value;
            }
        }

        public override void SetDSPIndex(DspHandle dsp, int index)
        {
            library.ChannelGroup_SetDSPIndex(Handle, dsp, index).CheckResult();
        }

        public override int GetDSPIndex(DspHandle dsp)
        {
            int value;
            library.ChannelGroup_GetDSPIndex(Handle, dsp, &value).CheckResult();
            return value;
        }

        public override void Set3DAttributes(in Vector3 pos, in Vector3 vel, in Vector3 altPanPos)
        {
            library.ChannelGroup_Set3DAttributes(Handle, in pos, in vel, in altPanPos).CheckResult();
        }

        public override void Get3DAttributes(out Vector3 pos, out Vector3 vel, out Vector3 altPanPos)
        {
            library.ChannelGroup_Get3DAttributes(Handle, out pos, out vel, out altPanPos).CheckResult();
        }

        public override void Set3DMinMaxDistance(float minDistance, float maxDistance)
        {
            library.ChannelGroup_Set3DMinMaxDistance(Handle, minDistance, maxDistance).CheckResult();
        }

        public override void Get3DMinMaxDistance(out float minDistance, out float maxDistance)
        {
            library.ChannelGroup_Get3DMinMaxDistance(Handle, out minDistance, out maxDistance).CheckResult();
        }

        public override void Set3DConeSettings(float insideConeAngle, float outsideConeAngle, float outsideVolume)
        {
            library.ChannelGroup_Set3DConeSettings(Handle, insideConeAngle, outsideConeAngle, outsideVolume).CheckResult();
        }

        public override void Get3DConeSettings(out float insideConeAngle, out float outsideConeAngle, out float outsideVolume)
        {
            library.ChannelGroup_Get3DConeSettings(Handle, out insideConeAngle, out outsideConeAngle, out outsideVolume).CheckResult();
        }

        public override void Set3DConeOrientation(in Vector3 orientation)
        {
            library.ChannelGroup_Set3DConeOrientation(Handle, in orientation).CheckResult();
        }

        public override void Get3DConeOrientation(out Vector3 orientation)
        {
            library.ChannelGroup_Get3DConeOrientation(Handle, out orientation).CheckResult();
        }

        public override unsafe void Get3DCustomRolloff(out Vector3* points, out int count)
        {
            library.ChannelGroup_Get3DCustomRolloff(Handle, out points, out count).CheckResult();
        }

        public override unsafe void Set3DCustomRolloff(Vector3* points, int count)
        {
            library.ChannelGroup_Set3DCustomRolloff(Handle, points, count).CheckResult();
        }

        public override void Set3DOcclusion(float directOcclusion, float reverbOcclusion)
        {
            library.ChannelGroup_Set3DOcclusion(Handle, directOcclusion, reverbOcclusion).CheckResult();
        }

        public override void Get3DOcclusion(out float directOcclusion, out float reverbOcclusion)
        {
            library.ChannelGroup_Get3DOcclusion(Handle, out directOcclusion, out reverbOcclusion).CheckResult();
        }

        public override float Spread
        {
            get
            {
                library.ChannelGroup_Get3DSpread(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_Set3DSpread(Handle, value).CheckResult();
            }
        }

        public override float Level3D
        {
            get
            {
                library.ChannelGroup_Get3DLevel(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_Set3DLevel(Handle, value).CheckResult();
            }
        }

        public override float DopplerLevel3D
        {
            get
            {
                library.ChannelGroup_Get3DDopplerLevel(Handle, out var value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_Set3DDopplerLevel(Handle, value).CheckResult();
            }
        }

        public override void Set3DDistanceFilter(FmodBool custom, float customLevel, float centerFreq)
        {
            library.ChannelGroup_Set3DDistanceFilter(Handle, custom, customLevel, centerFreq).CheckResult();
        }

        public override void Get3DDistanceFilter(out FmodBool custom, out float customLevel, out float centerFreq)
        {
            library.ChannelGroup_Get3DDistanceFilter(Handle, out custom, out customLevel, out centerFreq).CheckResult();
        }

        internal override IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.ChannelGroup_GetUserData(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.ChannelGroup_SetUserData(Handle, value).CheckResult();
            }
        }

        public static bool operator ==(ChannelGroup? l, ChannelGroup? r)
        {
            if (ReferenceEquals(l, r))
                return true;

            if (l is null || r is null)
                return false;

            return l.Handle == r.Handle;
        }

        public static bool operator !=(ChannelGroup? l, ChannelGroup? r)
        {
            return !(l == r);
        }
    }
}
