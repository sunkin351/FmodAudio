using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace FmodAudio
{
    using Interop;

    public class Reverb3D : HandleBase
    {
        private readonly NativeLibrary library;

        internal Reverb3D(IntPtr handle, NativeLibrary library) : base(handle)
        {
            this.library = library;
        }

        protected override void ReleaseImpl()
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
                library.Reverb3D_GetActive(Handle, out bool value).CheckResult();
                return value;
            }

            set
            {
                library.Reverb3D_SetActive(Handle, value).CheckResult();
            }
        }

        [Obsolete]
        public unsafe IntPtr UserData
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
    }
}
