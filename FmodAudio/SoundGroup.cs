using System;

#nullable enable

namespace FmodAudio
{
    using Base;
    public unsafe struct SoundGroup : IDisposable
    {
        public static implicit operator SoundGroup(SoundGroupHandle handle)
        {
            return new SoundGroup(handle);
        }

        public static implicit operator SoundGroupHandle(SoundGroup soundGroup)
        {
            return soundGroup.Handle;
        }

        private static readonly FmodLibrary library = Fmod.Library;

        private readonly SoundGroupHandle Handle;

        internal SoundGroup(SoundGroupHandle handle)
        {
            Handle = handle;
        }

        public SystemHandle SystemObject
        {
            get
            {
                library.SoundGroup_GetSystemObject(Handle, out SystemHandle system);
                return system;
            }
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
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

        public string Name
        {
            get
            {
                const int len = Fmod.MaxInteropNameStringLength;
                var ptr = stackalloc byte[len];

                library.SoundGroup_GetName(Handle, ptr, len).CheckResult();

                return FmodHelpers.BufferToString(ptr, len);
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

        internal IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.SoundGroup_GetUserData(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.SoundGroup_SetUserData(Handle, value).CheckResult();
            }
        }

        public void Stop()
        {
            library.SoundGroup_Stop(Handle).CheckResult();
        }
    }
}
