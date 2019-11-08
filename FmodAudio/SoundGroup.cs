using System;

namespace FmodAudio
{
    using Interop;
    public unsafe class SoundGroup : HandleBase
    {
        private readonly NativeLibrary library;

        public FmodSystem SystemObject { get; }
        internal bool IsMaster = false;
        internal string _name = null;

        internal SoundGroup(FmodSystem sys, IntPtr inst) : base(inst)
        {
            SystemObject = sys;
            library = sys.library;
        }

        protected override void ReleaseImpl()
        {
            SystemObject.ReleaseSoundGroup(Handle, IsMaster);
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

        public IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.SoundGroup_GetUserData(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.Sound_SetUserData(Handle, value).CheckResult();
            }
        }

        public void Stop()
        {
            library.SoundGroup_Stop(Handle).CheckResult();
        }
    }
}
