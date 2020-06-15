using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using FmodAudio.Interop;

#nullable enable

namespace FmodAudio.Dsp
{
    public unsafe abstract partial class DSP : HandleBase
    {
        internal static NativeLibrary library = Fmod.Library;

        internal static DSP FromHandle(IntPtr handle)
        {
            IntPtr value;
            Fmod.UserDataMethods.DSP_GetUserData(handle, &value).CheckResult();

            if (value != default)
            {
                var gchandle = (GCHandle)value;

                if (gchandle.IsAllocated && gchandle.Target is DSP dsp)
                {
                    return dsp;
            }
            }

            library.DSP_GetSystemObject(handle, &value).CheckResult();

            var system = FmodSystem.FromHandle(value);

            return new SystemDefinedDsp(system, handle);
        }

        public FmodSystem SystemObject { get; }

        internal DSP(FmodSystem sys)
        {
            SystemObject = sys;
        }

        internal DSP(FmodSystem sys, IntPtr handle) : base (handle)
        {
            this.SystemObject = sys;
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

        public abstract int ParameterCount { get; }

        public DSPType Type
        {
            get
            {
                library.DSP_GetType(this.Handle, out var type).CheckResult();
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

        /// <summary>
        /// Adds a DSP unit as an input to this object.
        /// </summary>
        /// <param name="input">DSP unit to be added</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DSPConnection AddInput(DSP input, DSPConnectionType type = DSPConnectionType.Standard)
        {
            library.DSP_AddInput(Handle, input.Handle, out IntPtr connection, type).CheckResult();

            return new DSPConnection(connection);
        }

        public void DisconnectFrom(DSP dsp, DSPConnection connection = default)
        {
            library.DSP_DisconnectFrom(Handle, dsp.Handle, connection.Handle).CheckResult();
        }

        public void DisconnectAll(bool inputs, bool outputs)
        {
            library.DSP_DisconnectAll(Handle, inputs, outputs).CheckResult();
        }

        public (DSP Dsp, DSPConnection Connection) GetInput(int index)
        {
            library.DSP_GetInput(Handle, index, out IntPtr input, out IntPtr connection).CheckResult();

            var dsp = FromHandle(input);
            var con = new DSPConnection(connection);
            return (dsp, con);
        }

        public (DSP Dsp, DSPConnection Connection) GetOutput(int index)
        {
            library.DSP_GetOutput(Handle, index, out IntPtr output, out IntPtr connection).CheckResult();

            var dsp = FromHandle(output);
            var con = new DSPConnection(connection);
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

        private void CheckParamIndex(int index)
        {
            if ((uint)index >= (uint)this.ParameterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public void SetParameterFloat(int index, float value)
        {
            CheckParamIndex(index);
            library.DSP_SetParameterFloat(Handle, index, value).CheckResult();
        }

        public void SetParameterInt(int index, int value)
        {
            CheckParamIndex(index);
            library.DSP_SetParameterInt(Handle, index, value).CheckResult();
        }

        public void SetParameterBool(int index, bool value)
        {
            CheckParamIndex(index);
            library.DSP_SetParameterBool(Handle, index, value).CheckResult();
        }

        public void SetParameterData(int index, IntPtr data, uint length)
        {
            CheckParamIndex(index);
            library.DSP_SetParameterData(Handle, index, data, length).CheckResult();
        }

        public unsafe float GetParameterFloat(int index)
        {
            CheckParamIndex(index);
            library.DSP_GetParameterFloat(Handle, index, out float value, null, 0).CheckResult();
            return value;
        }

        public unsafe int GetParameterInt(int index)
        {
            CheckParamIndex(index);

            int value;
            library.DSP_GetParameterint(Handle, index, &value).CheckResult();
            return value;
        }

        public unsafe bool GetParameterBool(int index)
        {
            CheckParamIndex(index);
            library.DSP_GetParameterBool(Handle, index, out bool value, null, 0).CheckResult();
            return value;
        }

        public IntPtr GetParameterData(int index, out uint length)
        {
            CheckParamIndex(index);
            library.DSP_GetParameterData(Handle, index, out IntPtr data, out length).CheckResult();
            return data;
        }

        public abstract ParameterDescription GetParameterInfo(int index);

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

        public unsafe void GetInfo(out string name, out FmodVersion version, out int channels, out int configWidth, out int configHeight)
        {
            library.DSP_GetInfo(Handle, out name, out version, out channels, out configWidth, out configHeight);
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
