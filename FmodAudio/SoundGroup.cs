using System;

namespace FmodAudio
{
    using Interop;
    public class SoundGroup : HandleBase
    {
        private readonly NativeLibrary library;

        public FmodSystem SystemObject { get; }
        internal bool IsMaster = false;

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
                library.SoundGroup_GetMaxAudible(Handle, out int value).CheckResult();

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
                library.SoundGroup_GetMaxAudibleBehavior(Handle, out SoundGroupBehavior value).CheckResult();

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
                library.SoundGroup_GetMuteFadeSpeed(Handle, out float value).CheckResult();

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
                library.SoundGroup_GetVolume(Handle, out float value).CheckResult();

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
                const int len = FmodSystem.MaxInteropNameStringLength;
                var ptr = stackalloc byte[len];

                library.SoundGroup_GetName(Handle, ptr, len).CheckResult();

                return FmodHelpers.PtrToString(ptr, len);
            }
        }

        public int SoundCount
        {
            get
            {
                library.SoundGroup_GetNumSounds(Handle, out int value).CheckResult();

                return value;
            }
        }

        public int PlayingCount
        {
            get
            {
                library.SoundGroup_GetNumPlaying(Handle, out int value).CheckResult();

                return value;
            }
        }

        public IntPtr UserData
        {
            get
            {
                library.SoundGroup_GetUserData(Handle, out IntPtr value).CheckResult();

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
