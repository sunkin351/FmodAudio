using System;

namespace FmodAudio.Dsp
{
    using global::FmodAudio.Interop;
    public sealed class DSPConnection : HandleBase
    {
        private readonly NativeLibrary library;

        private readonly FmodSystem system;

        internal DSPConnection(FmodSystem sys, IntPtr handle) : base(handle)
        {
            system = sys;
            library = sys.library;
        }
        
        public DSP Input
        {
            get
            {
                library.DSPConnection_GetInput(Handle, out IntPtr handle).CheckResult();

                return DSP.GetDSPByHandle(this.system, handle);
            }
        }

        public DSP Output
        {
            get
            {
                library.DSPConnection_GetOutput(Handle, out IntPtr handle).CheckResult();

                return DSP.GetDSPByHandle(this.system, handle);
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

        [Obsolete]
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
        
        public unsafe void SetMixMatrix(Span<float> matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            if (matrix.Length < outChannels * inChannels)
            {
                throw new ArgumentOutOfRangeException("Matrix length is too small!");
            }

            fixed (float* mptr = matrix)
            {
                library.DSPConnection_SetMixMatrix(Handle, mptr, outChannels, inChannels, inChannelHop).CheckResult();
            }
        }

        public unsafe void GetMixMatrix(Span<float> matrix, out int outChannels, out int inChannels, int inChannelHop = 0)
        {
            fixed(float* mptr = matrix)
            {
                library.DSPConnection_GetMixMatrix(Handle, mptr, out outChannels, out inChannels, inChannelHop).CheckResult();
            }
        }
    }
}
