using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio.Dsp
{
    using global::FmodAudio.Interop;
    public sealed class DSPConnection : HandleBase
    {
        static IFmodLibrary library { get => NativeLibrary.Library; }

        private readonly FmodSystem system;

        internal DSPConnection(FmodSystem sys, IntPtr handle) : base(handle)
        {
            system = sys;
        }
        
        public DSP Input
        {
            get
            {
                library.DSPConnection_GetInput(Handle, out IntPtr handle).CheckResult();

                return system.GetDSP(handle);
            }
        }

        public DSP Output
        {
            get
            {
                library.DSPConnection_GetOutput(Handle, out IntPtr handle).CheckResult();

                return system.GetDSP(handle);
            }
        }

        public float Mix
        {
            get
            {
                library.DSPConnection_GetMix(Handle, out float value).CheckResult();
                return value;
            }

            set
            {
                library.DSPConnection_SetMix(Handle, value).CheckResult();
            }
        }

        public DSPConnectionType Type
        {
            get
            {
                library.DSPConnection_GetType(Handle, out DSPConnectionType type).CheckResult();
                return type;
            }
        }

        public IntPtr UserData
        {
            get
            {
                library.DSPConnection_GetUserData(Handle, out IntPtr value).CheckResult();
                return value;
            }

            set
            {
                library.DSPConnection_SetUserData(Handle, value).CheckResult();
            }
        }
    }
}
