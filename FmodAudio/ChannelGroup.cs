using System;

namespace FmodAudio
{
    using Dsp;

    public sealed class ChannelGroup : ChannelControl
    {
        internal bool IsMaster;

        internal ChannelGroup(FmodSystem sys, IntPtr handle) : base(sys, handle)
        {
        }

        protected override void ReleaseImpl()
        {
            SystemObject.ReleaseChannelGroup(Handle, IsMaster);
        }

        public int GroupCount
        {
            get
            {
                library.ChannelGroup_GetNumGroups(Handle, out int value).CheckResult();

                return value;
            }
        }

        public ChannelGroup ParentGroup
        {
            get
            {
                library.ChannelGroup_GetParentGroup(Handle, out IntPtr handle).CheckResult();

                return SystemObject.GetChannelGroup(handle);
            }
        }

        public unsafe string Name
        {
            get
            {
                const int len = FmodSystem.MaxInteropNameStringLength;
                var ptr = stackalloc byte[len];

                var res = library.ChannelGroup_GetName(Handle, ptr, len);

                return FmodHelpers.PtrToString(ptr, len);
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

        public DSPConnection AddGroup(ChannelGroup group, bool propagateDSPClock = true)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group));

            library.ChannelGroup_AddGroup(Handle, group.Handle, propagateDSPClock, out IntPtr dspConnectionHandle).CheckResult();

            return new DSPConnection(SystemObject, dspConnectionHandle);
        }
        
        public ChannelGroup GetGroup(int index)
        {
            library.ChannelGroup_GetGroup(Handle, index, out IntPtr handle).CheckResult();

            return SystemObject.GetChannelGroup(handle);
        }

        public Channel GetChannel(int index)
        {
            library.ChannelGroup_GetChannel(Handle, index, out IntPtr handle).CheckResult();

            return new Channel(SystemObject, handle);
        }
    }
}
