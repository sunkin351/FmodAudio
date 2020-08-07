using System;
using System.Runtime.CompilerServices;
using FmodAudio.Base;

namespace FmodAudio.DigitalSignalProcessing
{
    public readonly struct DspConnection
    {
        public static implicit operator DspConnection(DspConnectionHandle handle)
        {
            return new DspConnection(handle);
        }

        public static implicit operator DspConnectionHandle(DspConnection connection)
        {
            return connection.Handle;
        }

        private static readonly FmodLibrary library = Fmod.Library;

        public readonly DspConnectionHandle Handle;

        internal DspConnection(DspConnectionHandle handle)
        {
            Handle = handle;
        }
        
        public Dsp Input
        {
            get
            {
                library.DSPConnection_GetInput(Handle, out DspHandle handle).CheckResult();

                return handle;
            }
        }

        public Dsp Output
        {
            get
            {
                library.DSPConnection_GetOutput(Handle, out DspHandle handle).CheckResult();

                return handle;
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

        public unsafe void SetMixMatrix(float* matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            library.DSPConnection_SetMixMatrix(Handle, matrix, outChannels, inChannels, inChannelHop).CheckResult();
        }

        public unsafe void SetMixMatrix(ReadOnlySpan<float> matrix, int outChannels, int inChannels, int inChannelHop = 0)
        {
            if (matrix.Length < outChannels * inChannels)
            {
                throw new ArgumentOutOfRangeException("Matrix length is too small!");
            }

            fixed (float* pMatrix = matrix)
            {
                SetMixMatrix(pMatrix, outChannels, inChannels, inChannelHop);
            }
        }

        public unsafe void GetMixMatrix(float* matrix, out int outChannels, out int inChannels, int inChannelHop = 0)
        {
            library.DSPConnection_GetMixMatrix(Handle, matrix, out outChannels, out inChannels, inChannelHop).CheckResult();
        }

        public unsafe bool TryGetMixMatrix(Span<float> matrix, out int outChannels, out int inChannels, int inChannelHop = 0)
        {
            Unsafe.SkipInit(out outChannels);
            Unsafe.SkipInit(out inChannels);

            int Out, In;

            library.DSPConnection_GetMixMatrix(Handle, null, &Out, &In, inChannelHop).CheckResult();

            if (matrix.Length < Out * In)
            {
                return false;
            }

            fixed (float* pMatrix = matrix)
            {
                library.DSPConnection_GetMixMatrix(Handle, pMatrix, &Out, &In, inChannelHop).CheckResult();
            }

            outChannels = Out;
            inChannels = In;

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is DspConnection other && this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Handle);
        }

        public static bool operator ==(DspConnection left, DspConnection right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(DspConnection left, DspConnection right)
        {
            return left.Handle != right.Handle;
        }
    }
}
