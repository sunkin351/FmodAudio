#pragma warning disable CS0660, CS0661

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace FmodAudio
{
    using Base;

    public readonly struct Reverb3D : IDisposable
    {
        public static implicit operator Reverb3D(Reverb3DHandle handle)
        {
            return new Reverb3D(handle);
        }

        public static implicit operator Reverb3DHandle(Reverb3D reverb)
        {
            return reverb.Handle;
        }

        private static readonly FmodLibrary library = Fmod.Library;
        private readonly Reverb3DHandle Handle;

        internal Reverb3D(Reverb3DHandle handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            library.Reverb3D_Release(Handle).CheckResult();
        }

        public void Set3DAttributes(in Vector3 position, float minDistance, float maxdistance)
        {
            library.Reverb3D_Set3DAttributes(Handle, in position, minDistance, maxdistance).CheckResult();
        }

        public void Get3DAttributes(out Vector3 position, out float minDistance, out float maxDistance)
        {
            library.Reverb3D_Get3DAttributes(Handle, out position, out minDistance, out maxDistance).CheckResult();
        }

        public void SetProperties(in ReverbProperties properties)
        {
            library.Reverb3D_SetProperties(Handle, in properties).CheckResult();
        }

        public void GetProperties(out ReverbProperties properties)
        {
            library.Reverb3D_GetProperties(Handle, out properties).CheckResult();
        }

        public bool Active
        {
            get
            {
                library.Reverb3D_GetActive(Handle, out FmodBool value).CheckResult();
                return value;
            }

            set
            {
                library.Reverb3D_SetActive(Handle, value).CheckResult();
            }
        }

        internal unsafe IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.Reverb3D_GetUserData(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.Reverb3D_SetUserData(Handle, value).CheckResult();
            }
        }

        public static bool operator ==(Reverb3D l, Reverb3D r)
        {
            return l.Handle == r.Handle;
        }

        public static bool operator !=(Reverb3D l, Reverb3D r)
        {
            return l.Handle != r.Handle;
        }
    }
}
