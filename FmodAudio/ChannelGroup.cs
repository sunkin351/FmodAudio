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

        public override bool Paused { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool VolumeRamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override float Audibility => throw new NotImplementedException();

        public override float Pitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool Mute { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void GetReverbProperties(int instance, out float wet)
        {
            throw new NotImplementedException();
        }

        public override void SetReverbProperties(int instance, float wet)
        {
            throw new NotImplementedException();
        }

        public override float LowPassGain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Mode Mode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override unsafe void SetCallback(delegate* stdcall<IntPtr, ChannelControlType, ChannelControlCallbackType, IntPtr, IntPtr, void> callback)
        {
            throw new NotImplementedException();
        }

        public override void SetPan(float pan)
        {
            throw new NotImplementedException();
        }

        public override void SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            throw new NotImplementedException();
        }

        public override void SetMixLevelsInput(ReadOnlySpan<float> mixLevels)
        {
            throw new NotImplementedException();
        }

        public override void SetMixMatrix(float* matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            throw new NotImplementedException();
        }

        public override unsafe void GetMixMatrix(float* matrix, int* outChannels, int* inChannels, int inChannelHop = 0)
        {
            throw new NotImplementedException();
        }

        public override void GetDSPClock(out ulong dspClock, out ulong parentClock)
        {
            throw new NotImplementedException();
        }

        public override void SetDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels = true)
        {
            throw new NotImplementedException();
        }

        public override void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            throw new NotImplementedException();
        }

        public override void AddFadePoint(ulong dspClock, float volume)
        {
            throw new NotImplementedException();
        }

        public override void SetFadePointRamp(ulong dspClock, float volume)
        {
            throw new NotImplementedException();
        }

        public override void RemoveFadePoints(ulong dspClockStart, ulong dspClockEnd)
        {
            throw new NotImplementedException();
        }

        public override unsafe void GetFadePoints(uint* count, ulong* dspClocks, float* volumes)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void AddDSP(ChannelControlDSPIndex index, DspHandle dsp)
        {
            throw new NotImplementedException();
        }

        public override void RemoveDSP(DspHandle dsp)
        {
            throw new NotImplementedException();
        }

        public override int DspCount => throw new NotImplementedException();

        public override void SetDSPIndex(DspHandle dsp, int index)
        {
            throw new NotImplementedException();
        }

        public override int GetDSPIndex(DspHandle dsp)
        {
            throw new NotImplementedException();
        }

        public override void Set3DAttributes(in Vector3 pos, in Vector3 vel, in Vector3 altPanPos)
        {
            throw new NotImplementedException();
        }

        public override void Get3DAttributes(out Vector3 pos, out Vector3 vel, out Vector3 altPanPos)
        {
            throw new NotImplementedException();
        }

        public override void Set3DMinMaxDistance(float minDistance, float maxDistance)
        {
            throw new NotImplementedException();
        }

        public override void Get3DMinMaxDistance(out float minDistance, out float maxDistance)
        {
            throw new NotImplementedException();
        }

        public override void Set3DConeSettings(float insideConeAngle, float outsideConeAngle, float outsideVolume)
        {
            throw new NotImplementedException();
        }

        public override void Get3DConeSettings(out float insideConeAngle, out float outsideConeAngle, out float outsideVolume)
        {
            throw new NotImplementedException();
        }

        public override void Set3DConeOrientation(in Vector3 orientation)
        {
            throw new NotImplementedException();
        }

        public override void Get3DConeOrientation(out Vector3 orientation)
        {
            throw new NotImplementedException();
        }

        public override unsafe void Get3DCustomRolloff(out Vector3* points, out int count)
        {
            throw new NotImplementedException();
        }

        public override unsafe void Set3DCustomRolloff(Vector3* points, int count)
        {
            throw new NotImplementedException();
        }

        public override void Set3DOcclusion(float directOcclusion, float reverbOcclusion)
        {
            throw new NotImplementedException();
        }

        public override void Get3DOcclusion(out float directOcclusion, out float reverbOcclusion)
        {
            throw new NotImplementedException();
        }

        public override float Spread { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float Level3D { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float DopplerLevel3D { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Set3DDistanceFilter(FmodBool custom, float customLevel, float centerFreq)
        {
            throw new NotImplementedException();
        }

        public override void Get3DDistanceFilter(out FmodBool custom, out float customLevel, out float centerFreq)
        {
            throw new NotImplementedException();
        }

        internal override IntPtr UserData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
