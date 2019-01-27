using System;

namespace FmodAudio
{
    using Interop;
    public class SoundGroup : HandleBase
    {
        static IFmodLibrary library { get => NativeLibrary.Library; }

        public FmodSystem SystemObject { get; }
        internal bool IsMaster = false;

        internal SoundGroup(FmodSystem sys, IntPtr inst) : base(inst)
        {
            SystemObject = sys;
        }

        protected override void ReleaseImpl()
        {
            SystemObject.ReleaseSoundGroup(Handle, IsMaster);
        }

        public int MaxAudible
        {
            get
            {
                var res = library.SoundGroup_GetMaxAudible(Handle, out int value);

                FmodSystem.CheckResult(res);

                return value;
            }
            set
            {
                var res = library.SoundGroup_SetMaxAudible(Handle, value);

                FmodSystem.CheckResult(res);
            }
        }

        public SoundGroupBehavior MaxAudibleBehavior
        {
            get
            {
                var res = library.SoundGroup_GetMaxAudibleBehavior(Handle, out SoundGroupBehavior value);

                FmodSystem.CheckResult(res);

                return value;
            }
            set
            {
                var res = library.SoundGroup_SetMaxAudibleBehavior(Handle, value);

                FmodSystem.CheckResult(res);
            }
        }

        public float MuteFadeSpeed
        {
            get
            {
                var res = library.SoundGroup_GetMuteFadeSpeed(Handle, out float value);

                FmodSystem.CheckResult(res);

                return value;
            }
            set
            {
                var res = library.SoundGroup_SetMuteFadeSpeed(Handle, value);

                FmodSystem.CheckResult(res);
            }
        }

        public float Volume
        {
            get
            {
                var res = library.SoundGroup_GetVolume(Handle, out float value);

                FmodSystem.CheckResult(res);

                return value;
            }
            set
            {
                var res = library.SoundGroup_SetVolume(Handle, value);

                FmodSystem.CheckResult(res);
            }
        }

        public unsafe string Name
        {
            get
            {
                const int len = FmodSystem.MaxInteropNameStringLength;
                var ptr = stackalloc byte[len];

                var res = library.SoundGroup_GetName(Handle, ptr, len);

                FmodSystem.CheckResult(res);

                return Helpers.PtrToString(ptr, len);
            }
        }

        public int SoundCount
        {
            get
            {
                var res = library.SoundGroup_GetNumSounds(Handle, out int value);

                FmodSystem.CheckResult(res);

                return value;
            }
        }

        public int PlayingCount
        {
            get
            {
                var res = library.SoundGroup_GetNumPlaying(Handle, out int value);
                FmodSystem.CheckResult(res);

                return value;
            }
        }

        public IntPtr UserData
        {
            get
            {
                var res = library.SoundGroup_GetUserData(Handle, out IntPtr value);
                FmodSystem.CheckResult(res);

                return value;
            }
            set
            {
                var res = library.Sound_SetUserData(Handle, value);
                FmodSystem.CheckResult(res);
            }
        }

        public void Stop()
        {
            var res = library.SoundGroup_Stop(Handle);

            FmodSystem.CheckResult(res);
        }
    }
}
