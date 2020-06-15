using System;

namespace FmodAudio
{
    public sealed unsafe class Channel : ChannelControl
    {
        internal static Channel FromHandle(IntPtr handle)
        {
            Fmod.Library.ChannelGroup_GetSystemObject(handle, out IntPtr sysHandle).CheckResult();

            var system = FmodSystem.FromHandle(sysHandle);

            return new Channel(system, handle);
        }

        internal Channel(FmodSystem system, IntPtr handle) : base(system, handle, false)
        {
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
                IntPtr handle;
                library.Channel_GetChannelGroup(Handle, &handle).CheckResult();

                return handle == default ? null : (ChannelGroup.FromHandle(handle) ?? new ChannelGroup(SystemObject, handle, false));
            }

            set
            {
                library.Channel_SetChannelGroup(Handle, value.Handle).CheckResult();
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
                library.Channel_IsVirtual(Handle, out bool value).CheckResult();

                return value;
            }
        }

        public Sound CurrentSound
        {
            get
            {
                IntPtr handle;
                library.Channel_GetCurrentSound(Handle, &handle).CheckResult();

                return Sound.FromNewHandle(SystemObject, handle, false);
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

        #endregion
    }
}
