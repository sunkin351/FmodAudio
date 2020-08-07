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

        public override void SetDelay(ulong dspclock_start, ulong dspclock_end, FmodBool stopChannels)
        {
            throw new NotImplementedException();
        }

        public override void GetDelay(out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopChannels)
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

        #endregion
    }
}
