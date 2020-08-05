using System;

using FmodAudio.Base;
using FmodAudio.DigitalSignalProcessing.Interop;

#nullable enable

namespace FmodAudio.DigitalSignalProcessing
{
    public unsafe partial class Dsp
    {
        public static implicit operator Dsp?(DspHandle handle)
        {
            return handle.IsNull() ? null : new Dsp(handle);
        }

        public static implicit operator DspHandle(Dsp? dsp)
        {
            return dsp is null ? default : dsp.Handle;
        }

        protected readonly FmodLibrary library = Fmod.Library;
        protected readonly DspHandle Handle;


        internal Dsp(DspHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Retrieves the system object that was used in this DSP's creation
        /// </summary>
        public FmodSystem SystemObject
        {
            get
            {
                SystemHandle handle;
                library.DSP_GetSystemObject(Handle, &handle).CheckResult();

                return handle;
            }
        }

        internal IntPtr UserData
        {
            get 
            {
                IntPtr userData;
                library.DSP_GetUserData(Handle, &userData).CheckResult();

                return userData;
            }
            set
            {
                library.DSP_SetUserData(Handle, value).CheckResult();
            }
        }

        /// <summary>
        /// Retrieves the number of DSP units in the input list.
        /// </summary>
        /// <remarks>
        /// This will flush the DSP queue (which blocks against the mixer) to ensure the input list is correct, avoid this during time sensitive operations.
        /// </remarks>
        public int InputCount
        {
            get
            {
                library.DSP_GetNumInputs(Handle, out int value).CheckResult();
                return value;
            }
        }

        /// <summary>
        /// Retrieves the number of DSP units in the output list.
        /// </summary>
        /// <remarks>
        /// This will flush the DSP queue (which blocks against the mixer) to ensure the input list is correct, avoid this during time sensitive operations.
        /// </remarks>
        public int OutputCount
        {
            get
            {
                library.DSP_GetNumOutputs(Handle, out int value).CheckResult();
                return value;
            }
        }

        /// <summary>
        /// Gets or sets whether this DSP is active. When inactive, processing of this dsp and all its inputs is stopped.
        /// </summary>
        public bool Active
        {
            get
            {
                FmodBool value;
                library.DSP_GetActive(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.DSP_SetActive(Handle, value).CheckResult();
            }
        }

        /// <summary>
        /// Gets or sets whether this DSP is bypassed. If true, this DSP will not be processed, while continuing to process its inputs.
        /// </summary>
        public bool Bypass
        {
            get
            {
                FmodBool value;
                library.DSP_GetBypass(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.DSP_SetBypass(Handle, value).CheckResult();
            }
        }

        /// <summary>
        /// Retrieves the number of parameters exposed by this unit.
        /// </summary>
        /// <remarks>
        /// Use this to enumerate all parameters of a DSP unit with <see cref="Dsp.GetParameterInfo(int)"/>
        /// </remarks>
        public virtual int ParameterCount
        {
            get
            {
                int count;

                library.DSP_GetNumParameters(this.Handle, &count).CheckResult();

                return count;
            }
        }

        /// <summary>
        /// Retrieves the pre-defined type of this FMOD registered DSP unit.
        /// </summary>
        /// <remarks>
        /// This is only valid for built in FMOD effects. Any user plugins will simply return <see cref="DSPType.Unknown"/>
        /// </remarks>
        public DSPType Type
        {
            get
            {
                library.DSP_GetType(this.Handle, out var type).CheckResult();
                return type;
            }
        }

        /// <summary>
        /// Retrieves the idle state of this DSP.
        /// </summary>
        /// <remarks>
        /// A DSP is considered idle when it stops receiving input signal and all internal processing of stored input has been exhausted.
        /// </remarks>
        public bool Idle
        {
            get
            {
                FmodBool value;
                library.DSP_GetIdle(Handle, &value).CheckResult();
                return value;
            }
        }

        /// <summary>
        /// Adds a DSP unit as an input to this object.
        /// </summary>
        /// <param name="input">DSP unit to be added</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DspConnection AddInput(DspHandle input, DSPConnectionType type = DSPConnectionType.Standard)
        {
            library.DSP_AddInput(Handle, input, out DspConnectionHandle connection, type).CheckResult();

            return connection;
        }

        public void DisconnectFrom(DspHandle dsp, DspConnectionHandle connection = default)
        {
            library.DSP_DisconnectFrom(Handle, dsp, connection).CheckResult();
        }

        public void DisconnectAll(bool inputs, bool outputs)
        {
            library.DSP_DisconnectAll(Handle, inputs, outputs).CheckResult();
        }

        public (Dsp Dsp, DspConnection Connection) GetInput(int index)
        {
            library.DSP_GetInput(Handle, index, out DspHandle input, out DspConnectionHandle connection).CheckResult();

            return ((Dsp)input!, (DspConnection)connection);
        }

        public (Dsp Dsp, DspConnection Connection) GetOutput(int index)
        {
            library.DSP_GetOutput(Handle, index, out DspHandle output, out DspConnectionHandle connection).CheckResult();

            return ((Dsp)output!, (DspConnection)connection);
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

        public void SetParameterData(int index, void* data, uint length)
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
            library.DSP_GetParameterBool(Handle, index, out FmodBool value, null, 0).CheckResult();
            return value;
        }

        public void* GetParameterData(int index, out uint length)
        {
            CheckParamIndex(index);
            library.DSP_GetParameterData(Handle, index, out void* data, out length).CheckResult();
            return data;
        }

        public virtual ParameterDescription GetParameterInfo(int index)
        {
            ParameterDescriptionStruct* ptr;
            library.DSP_GetParameterInfo(Handle, index, &ptr).CheckResult();

            return ParameterDescription.CreateFromPointer(ptr);
        }

        public int GetDataParameterIndex(int dataType)
        {
            int index;
            library.DSP_GetDataParameterIndex(Handle, dataType, &index).CheckResult();
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

        public void GetMeteringEnabled(out FmodBool inputEnabled, out FmodBool outputEnabled)
        {
            library.DSP_GetMeteringEnabled(Handle, out inputEnabled, out outputEnabled).CheckResult();
        }

        public void GetMeteringInfo(out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo)
        {
            library.DSP_GetMeteringInfo(Handle, out inputInfo, out outputInfo).CheckResult();
        }
    }
}
