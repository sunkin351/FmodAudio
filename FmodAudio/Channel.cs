using System;

namespace FmodAudio
{
    public sealed class Channel : ChannelControl
    {
        internal Channel(FmodSystem system, IntPtr handle) : base(system, handle)
        {
        }

        #region Channel-Specific Control Functionality

        public float Frequency
        {
            get
            {
                library.Channel_GetFrequency(Handle, out float value).CheckResult();

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
                library.Channel_GetPriority(Handle, out int value).CheckResult();

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
                library.Channel_GetChannelGroup(Handle, out IntPtr handle).CheckResult();

                return SystemObject.GetChannelGroup(handle);
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
                library.Channel_GetLoopCount(Handle, out int value).CheckResult();

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
                library.Channel_GetCurrentSound(Handle, out IntPtr handle).CheckResult();

                return SystemObject.GetSound(handle);
            }
        }

        public int Index
        {
            get
            {
                library.Channel_GetIndex(Handle, out int value).CheckResult();

                return value;
            }
        }

        #endregion
    }
}
