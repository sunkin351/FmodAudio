using System;

#nullable enable

namespace FmodAudio
{
    using System.Runtime.InteropServices;
    using Dsp;

    public sealed unsafe class ChannelGroup : ChannelControl
    {
        internal static ChannelGroup FromNewHandle(FmodSystem system, IntPtr handle, bool ownsHandle)
        {
            return FromHandle(handle) ?? new ChannelGroup(system, handle, ownsHandle);
        }

        internal static ChannelGroup? FromHandle(IntPtr handle)
        {
            IntPtr ptr;
            Fmod.UserDataMethods.ChannelGroup_GetUserData(handle, &ptr).CheckResult();

            if (ptr != default)
            {
                GCHandle gcHandle = (GCHandle)ptr;

                if (gcHandle.IsAllocated && gcHandle.Target is ChannelGroup group)
                {
                    return group;
                }
            }

            return null;
        }

        internal string? _name = null;

        internal ChannelGroup(FmodSystem sys, IntPtr handle, bool ownsHandle) : base(sys, handle, ownsHandle)
        {
        }

        protected override void ReleaseImpl()
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

        public ChannelGroup ParentGroup
        {
            get
            {
                library.ChannelGroup_GetParentGroup(Handle, out IntPtr handle).CheckResult();

                return FromNewHandle(SystemObject, handle, false);
            }
        }

        public unsafe string Name
        {
            get
            {
                if (_name == null)
                {
                    const int len = FmodSystem.MaxInteropNameStringLength;
                    var ptr = stackalloc byte[len];

                    library.ChannelGroup_GetName(Handle, ptr, len).CheckResult();

                    _name = FmodHelpers.PtrToString(ptr, len);
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

        public DSPConnection AddGroup(ChannelGroup group, bool propagateDSPClock = true)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group));

            library.ChannelGroup_AddGroup(Handle, group.Handle, propagateDSPClock, out IntPtr dspConnectionHandle).CheckResult();

            return new DSPConnection(dspConnectionHandle);
        }
        
        public ChannelGroup GetGroup(int index)
        {
            library.ChannelGroup_GetGroup(Handle, index, out IntPtr handle).CheckResult();

            return FromNewHandle(SystemObject, handle, false);
        }

        public Channel GetChannel(int index)
        {
            library.ChannelGroup_GetChannel(Handle, index, out IntPtr handle).CheckResult();

            return new Channel(SystemObject, handle);
        }
    }
}
