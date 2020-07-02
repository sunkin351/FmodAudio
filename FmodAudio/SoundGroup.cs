using System;

#nullable enable

namespace FmodAudio
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Interop;
    public sealed unsafe class SoundGroup : HandleBase
    {
        internal static SoundGroup? FromHandle(IntPtr handle)
        {
            IntPtr value;
            Fmod.UserDataMethods.SoundGroup_GetUserData(handle, &value).CheckResult();

            if (value != default)
            {
                var gchandle = (GCHandle)value;

                if (gchandle.IsAllocated && gchandle.Target is SoundGroup group)
                {
                    return group;
                }
            }

            return null;
        }

        private readonly NativeLibrary library = Fmod.Library;

        public FmodSystem SystemObject { get; }
        internal string? _name = null;

        internal SoundGroup(FmodSystem sys, IntPtr inst, bool ownsHandle) : base(inst, ownsHandle)
        {
            SystemObject = sys;
        }

        protected override void ReleaseImpl()
        {
            library.SoundGroup_Release(Handle).CheckResult();
        }

        public int MaxAudible
        {
            get
            {
                int value;
                library.SoundGroup_GetMaxAudible(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.SoundGroup_SetMaxAudible(Handle, value).CheckResult();
            }
        }

        public SoundGroupBehavior MaxAudibleBehavior
        {
            get
            {
                SoundGroupBehavior value;
                library.SoundGroup_GetMaxAudibleBehavior(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.SoundGroup_SetMaxAudibleBehavior(Handle, value).CheckResult();
            }
        }

        public float MuteFadeSpeed
        {
            get
            {
                float value;
                library.SoundGroup_GetMuteFadeSpeed(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.SoundGroup_SetMuteFadeSpeed(Handle, value).CheckResult();
            }
        }

        public float Volume
        {
            get
            {
                float value;
                library.SoundGroup_GetVolume(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.SoundGroup_SetVolume(Handle, value).CheckResult();
            }
        }

        public unsafe string Name
        {
            get
            {
                if (_name is null)
                {
                    const int len = FmodSystem.MaxInteropNameStringLength;
                    var ptr = stackalloc byte[len];

                    library.SoundGroup_GetName(Handle, ptr, len).CheckResult();

                    _name = FmodHelpers.PtrToString(ptr, len);
                }
                
                return _name;
            }
        }

        public int SoundCount
        {
            get
            {
                int value;
                library.SoundGroup_GetNumSounds(Handle, &value).CheckResult();
                return value;
            }
        }

        public int PlayingCount
        {
            get
            {
                int value;
                library.SoundGroup_GetNumPlaying(Handle, &value).CheckResult();
                return value;
            }
        }

        internal override IntPtr UserData
        {
            get
            {
                IntPtr value;
                Fmod.UserDataMethods.SoundGroup_GetUserData(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                Fmod.UserDataMethods.SoundGroup_SetUserData(Handle, value).CheckResult();
            }
        }

        public void Stop()
        {
            library.SoundGroup_Stop(Handle).CheckResult();
        }
    }
}
