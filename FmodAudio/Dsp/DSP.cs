using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace FmodAudio.Dsp
{
    using global::FmodAudio.Interop;
    public sealed class DSP : HandleBase
    {
        private readonly INativeLibrary library;

        public FmodSystem SystemObject { get; }
        internal DspDescription Description;
        int? ParamCount;
        readonly bool OwnsHandle;

        internal DSP(FmodSystem sys, IntPtr handle, bool ownsHandle = true) : base(handle)
        {
            SystemObject = sys;
            library = sys.library;
            OwnsHandle = ownsHandle;

            if (!ownsHandle)
            {
                GC.SuppressFinalize(this);
            }
        }

        protected override void ReleaseImpl()
        {
            if (OwnsHandle)
            {
                this.DisconnectAll(true, true);
                SystemObject.ReleaseDSP(Handle);
            }
        }

        public int InputCount
        {
            get
            {
                library.DSP_GetNumInputs(Handle, out int value).CheckResult();
                return value;
            }
        }

        public int OutputCount
        {
            get
            {
                library.DSP_GetNumOutputs(Handle, out int value).CheckResult();
                return value;
            }
        }

        public bool Active
        {
            get
            {
                library.DSP_GetActive(Handle, out bool value).CheckResult();
                return value;
            }
            set
            {
                library.DSP_SetActive(Handle, value).CheckResult();
            }
        }

        public bool Bypass
        {
            get
            {
                library.DSP_GetBypass(Handle, out bool value).CheckResult();
                return value;
            }
            set
            {
                library.DSP_SetBypass(Handle, value).CheckResult();
            }
        }

        public int ParameterCount
        {
            get
            {
                if (ParamCount is null)
                {
                    library.DSP_GetNumParameters(Handle, out int paramCount).CheckResult();
                    ParamCount = paramCount;
                }
                
                return ParamCount.Value;
            }
        }

        public DSPType Type
        {
            get
            {
                library.DSP_GetType(Handle, out var type).CheckResult();
                return type;
            }
        }

        public bool Idle
        {
            get
            {
                library.DSP_GetIdle(Handle, out bool value).CheckResult();
                return value;
            }
        }

        public IntPtr UserData
        {
            get
            {
                library.DSP_GetUserData(Handle, out IntPtr value).CheckResult();
                return value;
            }

            set
            {
                library.DSP_SetUserData(Handle, value).CheckResult();
            }
        }


        public DSPConnection AddInput(DSP target, DSPConnectionType type = DSPConnectionType.Standard)
        {
            library.DSP_AddInput(Handle, target.Handle, out IntPtr connection, type).CheckResult();

            return new DSPConnection(SystemObject, connection);
        }

        public void DisconnectFrom(DSP dsp, DSPConnection connection = null)
        {
            library.DSP_DisconnectFrom(Handle, dsp.Handle, connection?.Handle ?? default).CheckResult();
        }

        public void DisconnectAll(bool inputs, bool outputs)
        {
            library.DSP_DisconnectAll(Handle, inputs, outputs).CheckResult();
        }

        public (DSP, DSPConnection) GetInput(int index)
        {
            library.DSP_GetInput(Handle, index, out IntPtr input, out IntPtr connection).CheckResult();

            var dsp = SystemObject.GetDSP(input);
            var con = new DSPConnection(SystemObject, connection);
            return (dsp, con);
        }

        public (DSP, DSPConnection) GetOutput(int index)
        {
            library.DSP_GetOutput(Handle, index, out IntPtr output, out IntPtr connection).CheckResult();

            var dsp = SystemObject.GetDSP(output);
            var con = new DSPConnection(SystemObject, connection);
            return (dsp, con);
        }

        public void SetWetDryMix(float prewet, float postwet, float dry)
        {
            library.DSP_SetWetDryMix(Handle, prewet, postwet, dry).CheckResult();
        }

        public void GetWetDryMix(out float prewet, out float postwet, out float dry)
        {
            library.DSP_GetWetDryMix(Handle, out prewet, out postwet, out dry).CheckResult();
        }

        public void SetChannelFormat(ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode)
        {
            library.DSP_SetChannelFormat(Handle, channelmask, numchannels, source_speakermode).CheckResult();
        }

        public void GetChannelFormat(out ChannelMask channelmask, out int numchannels, out SpeakerMode source_speakermode)
        {
            library.DSP_GetChannelFormat(Handle, out channelmask, out numchannels, out source_speakermode).CheckResult();
        }

        public void GetOutputChannelFormat(ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            library.DSP_GetOutputChannelFormat(Handle, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode).CheckResult();
        }

        public void Reset()
        {
            library.DSP_Reset(Handle).CheckResult();
        }

        public void SetParameterFloat(int index, float value)
        {
            library.DSP_SetParameterFloat(Handle, index, value).CheckResult();
        }

        public void SetParameterInt(int index, int value)
        {
            library.DSP_SetParameterInt(Handle, index, value).CheckResult();
        }

        public void SetParameterBool(int index, bool value)
        {
            library.DSP_SetParameterBool(Handle, index, value).CheckResult();
        }

        public void SetParameterData(int index, IntPtr data, uint length)
        {
            library.DSP_SetParameterData(Handle, index, data, length).CheckResult();
        }

        public unsafe float GetParameterFloat(int index)
        {
            if ((uint)index >= (uint)ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            library.DSP_GetParameterFloat(Handle, index, out float value, null, 0).CheckResult();
            return value;
        }

        public unsafe int GetParameterInt(int index)
        {
            if ((uint)index >= (uint)ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            library.DSP_GetParameterInt(Handle, index, out int value, null, 0).CheckResult();
            return value;
        }

        public unsafe bool GetParameterBool(int index)
        {
            if ((uint)index >= (uint)ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            library.DSP_GetParameterBool(Handle, index, out bool value, null, 0).CheckResult();
            return value;
        }

        public IntPtr GetParameterData(int index, out uint length)
        {
            length = 0;

            if ((uint)index >= (uint)ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            library.DSP_GetParameterData(Handle, index, out IntPtr data, out length, IntPtr.Zero, 0).CheckResult();
            return data;
        }

        public unsafe ref ParameterDescription GetParameterInfo(int index)
        {
            library.DSP_GetParameterInfo(Handle, index, out IntPtr ptr).CheckResult();

            return ref Unsafe.AsRef<ParameterDescription>((void*)ptr);
        }

        public void GetParameterInfo(int index, out ParameterDescription description)
        {
            description = GetParameterInfo(index);
        }

        public int GetDataParameterIndex(int dataType)
        {
            library.DSP_GetDataParameterIndex(Handle, dataType, out int index).CheckResult();
            return index;
        }

        public int GetDataParameterIndex(ParameterDataType dataType)
        {
            return GetDataParameterIndex((int)dataType);
        }

        public void ShowConfigDialog(IntPtr hwnd, bool show)
        {
            library.DSP_ShowConfigDialog(Handle, hwnd, show).CheckResult();
        }

        public unsafe void GetInfo(Span<char> nameBuffer, out FmodVersion version, out int channels, out int configWidth, out int configHeight)
        {
            byte* namePtr = stackalloc byte[32];
            library.DSP_GetInfo(Handle, (IntPtr)namePtr, out version, out channels, out configWidth, out configHeight).CheckResult();

            var buf = new Span<byte>(namePtr, 32);

            int idx = buf.IndexOf(byte.MinValue);

            if (idx == 0)
                return;
            if (idx > 0)
            {
                buf = buf.Slice(0, idx);
            }

            Encoding.UTF8.GetChars(buf, nameBuffer);
        }

        public void SetMeteringEnabled(bool inputEnabled, bool outputEnabled)
        {
            library.DSP_SetMeteringEnabled(Handle, inputEnabled, outputEnabled).CheckResult();
        }

        public void GetMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
        {
            library.DSP_GetMeteringEnabled(Handle, out inputEnabled, out outputEnabled).CheckResult();
        }

        public void GetMeteringInfo(out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo)
        {
            library.DSP_GetMeteringInfo(Handle, out inputInfo, out outputInfo).CheckResult();
        }
    }
}
