#pragma warning disable CA1031, IDE1006, CS0612
using System;
using System.Runtime.InteropServices;

#nullable enable

namespace FmodAudio.Dsp
{
    public unsafe abstract class UserDefinedDsp : DSP
    {
        protected internal enum DSPProcessType
        {
            /// <summary>
            /// Specifies that the functions Read() and ShouldIProcess() are to be used by FMOD in Digital Signal Processing
            /// </summary>
            Read,

            /// <summary>
            /// Specifies that the function Process() is to be used by FMOD in Digital Signal Processing
            /// </summary>
            Process
        }

        private struct InteropMethods
        {
            public DspStateCallback Create;
            public DspStateCallback Release;
            public DspStateCallback Reset;
            public DspReadCallback Read;
            public DspProcessCallback Process;
            public DspSetPositionCallback SetPosition;

            public DspSetParamFloatCallback SetParamFloat;
            public DspSetParamIntCallback SetParamInt;
            public DspSetParamBoolCallback SetParamBool;
            public DspSetParamDataCallback SetParamData;

            public DspGetParamFloatCallback GetParamFloat;
            public DspGetParamIntCallback GetParamInt;
            public DspGetParamBoolCallback GetParamBool;
            public DspGetParamDataCallback GetParamData;

            public DspShouldIProcessCallback ShouldIProcess;
            public DspStateCallback SystemRegister;
            public DspStateCallback SystemDeregister;
            public DspSystemMixCallback SystemMix;

        }

        private static readonly InteropMethods MethodReferences;
        private static readonly DspDescription.Structure DspCreateStructure;

        static UserDefinedDsp()
        {
            MethodReferences.Create = _createMethod;
            MethodReferences.Release = _releaseMethod;
            MethodReferences.Reset = _resetMethod;
            MethodReferences.Read = _readMethod;
            MethodReferences.Process = _processMethod;
            MethodReferences.SetPosition = _setPositionMethod;
            
            MethodReferences.SetParamFloat = _setParamFloat;
            MethodReferences.SetParamInt = _setParamInt;
            MethodReferences.SetParamBool = _setParamBool;
            MethodReferences.SetParamData = _setParamData;

            MethodReferences.GetParamFloat = _getParamFloat;
            MethodReferences.GetParamInt = _getParamInt;
            MethodReferences.GetParamBool = _getParamBool;
            MethodReferences.GetParamData = _getParamData;

            MethodReferences.ShouldIProcess = _shouldIProcessMethod;
            MethodReferences.SystemRegister = _systemRegisterMethod;
            MethodReferences.SystemDeregister = _systemDeregisterMethod;
            MethodReferences.SystemMix = _systemMixMethod;

            DspCreateStructure.Create = Marshal.GetFunctionPointerForDelegate(MethodReferences.Create);
            DspCreateStructure.Release = Marshal.GetFunctionPointerForDelegate(MethodReferences.Release);
            DspCreateStructure.Reset = Marshal.GetFunctionPointerForDelegate(MethodReferences.Reset);
            DspCreateStructure.Read = Marshal.GetFunctionPointerForDelegate(MethodReferences.Read);
            DspCreateStructure.Process = Marshal.GetFunctionPointerForDelegate(MethodReferences.Process);
            DspCreateStructure.SetPosition = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetPosition);

            DspCreateStructure.SetParamFloat = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamFloat);
            DspCreateStructure.SetParamInt = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamInt);
            DspCreateStructure.SetParamBool = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamBool);
            DspCreateStructure.SetParamData = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamData);

            DspCreateStructure.GetParamFloat = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamFloat);
            DspCreateStructure.GetParamInt = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamInt);
            DspCreateStructure.GetParamBool = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamBool);
            DspCreateStructure.GetParamData = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamData);

            DspCreateStructure.ShouldIProcess = Marshal.GetFunctionPointerForDelegate(MethodReferences.ShouldIProcess);
            DspCreateStructure.SystemRegister = Marshal.GetFunctionPointerForDelegate(MethodReferences.SystemRegister);
            DspCreateStructure.SystemDeregister = Marshal.GetFunctionPointerForDelegate(MethodReferences.SystemDeregister);
            DspCreateStructure.SystemMix = Marshal.GetFunctionPointerForDelegate(MethodReferences.SystemMix);
        }

        private static UserDefinedDsp? GetDSPObject(DspState* state)
        {
            if (state->plugindata == default)
            {
                var res = state->GetUserData(out state->plugindata);

                if (res != Result.Ok)
                {
                    return null;
                }
            }

            var handle = GCHandle.FromIntPtr(state->plugindata);

            if (!handle.IsAllocated || !(handle.Target is UserDefinedDsp dsp))
            {
                return null;
            }

            return dsp;
        }

        private static Result _createMethod(DspState* state)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Create(state);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _releaseMethod(DspState* state)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Release(state);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
            finally
            {
                GCHandle.FromIntPtr(state->plugindata).Free();
            }
        }

        private static Result _resetMethod(DspState* state)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Reset(state);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _readMethod(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, ref int outChannels)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Read(state, inBuffer, outBuffer, length, inChannels, ref outChannels);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _processMethod(DspState* state, uint length, DspBufferArray* inBufferArray, DspBufferArray* outBufferArray, bool inputsIdle, ProcessOperation operation)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Process(state, length, inBufferArray, outBufferArray, inputsIdle, operation);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _setPositionMethod(DspState* state, uint pos)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SetPosition(state, pos);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _setParamFloat(DspState* state, int index, float value)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SetParameterFloat(state, index, value);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _setParamInt(DspState* state, int index, int value)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SetParameterInt(state, index, value);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _setParamBool(DspState* state, int index, bool value)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SetParameterBool(state, index, value);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _setParamData(DspState* state, int index, IntPtr data, uint length)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SetParameterData(state, index, data, length);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _getParamFloat(DspState* state, int index, out float value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterFloat(state, index, out value, strValue);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _getParamInt(DspState* state, int index, out int value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterInt(state, index, out value, strValue);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _getParamBool(DspState* state, int index, out bool value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterBool(state, index, out value, strValue);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _getParamData(DspState* state, int index, out IntPtr data, out uint length, IntPtr strValue)
        {
            data = default;
            length = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterData(state, index, out data, out length, strValue);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _shouldIProcessMethod(DspState* state, bool inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.ShouldIProcess(state, inputsIdle, length, inMask, inChannels, speakerMode);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _systemRegisterMethod(DspState* state)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SystemRegister(state);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _systemDeregisterMethod(DspState* state)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SystemDeregister(state);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private static Result _systemMixMethod(DspState* state, int stage)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.SystemMix(state, stage);
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        private readonly DspDescription.ParameterDescriptionManager ParameterManager;
        private readonly ParameterDescription[] Descriptions;

        public override int ParameterCount => Descriptions.Length;

        internal override bool ClassManagedGCHandle => true;

        protected UserDefinedDsp(FmodSystem sys, string name, FmodVersion pluginVersion, int inputBufferCount, int outputBufferCount, ParameterDescription[] descriptions, DSPProcessType processType)
            : base(sys, default, true)
        {
            var createStruct = DspCreateStructure;

            createStruct.InputBufferCount = inputBufferCount;
            createStruct.OutputBufferCount = outputBufferCount;
            createStruct.Version = pluginVersion;
            createStruct.PluginSDKVersion = Fmod.PluginSDKVersion;
            createStruct.Name = name;

            switch (processType)
            {
                case DSPProcessType.Read:
                    createStruct.Process = default;
                    break;
                case DSPProcessType.Process:
                    createStruct.Read = default;
                    createStruct.ShouldIProcess = default;
                    break;
                default:
                    throw new ArgumentException("Invalid process type argument", nameof(processType));
            }

            this.ParameterManager = new DspDescription.ParameterDescriptionManager(descriptions);

            createStruct.ParameterCount = ParameterManager.Length;
            createStruct.ParameterDescriptions = (Interop.ParameterDescriptionStruct**)ParameterManager.PointerArray;

            createStruct.UserData = (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak);

            fixed (IntPtr* pHandle = &handle)
            {
                library.System_CreateDSP(sys.Handle, &createStruct, pHandle).CheckResult();
            }

            this.Descriptions = descriptions;
        }

        protected override sealed void ReleaseImpl()
        {
            library.DSP_Release(this.Handle).CheckResult();
        }

        public sealed override ParameterDescription GetParameterInfo(int index)
        {
            if ((uint)index >= (uint)this.Descriptions.Length)
            {
                throw new IndexOutOfRangeException();
            }

            return this.Descriptions[index];
        }

        protected virtual Result Create(DspState* state)
        {
            return Result.Ok;
        }

        protected virtual Result Release(DspState* state)
        {
            return Result.Ok;
        }

        protected abstract Result Reset(DspState* state);

        protected virtual Result Read(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, ref int outChannels)
        {
            Span<float> inSpan = new Span<float>(inBuffer, (int)length),
                outSpan = new Span<float>(outBuffer, (int)length);

            inSpan.CopyTo(outSpan);

            return Result.Ok;
        }

        protected virtual Result Process(DspState* state, uint length, DspBufferArray* inBufferArray, DspBufferArray* outBufferArray, bool inputsIdle, ProcessOperation operation)
        {
            //Needs critique
            return operation == ProcessOperation.ProcessQuery ? Result.Err_DSP_DontProcess : Result.Err_Unimplemented;
        }

        protected virtual Result SetPosition(DspState* state, uint pos)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SetParameterFloat(DspState* state, int index, float value)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SetParameterInt(DspState* state, int index, int value)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SetParameterBool(DspState* state, int index, bool value)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SetParameterData(DspState* state, int index, IntPtr data, uint length)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result GetParameterFloat(DspState* state, int index, out float value, IntPtr valueStr)
        {
            value = default;
            return Result.Err_Unimplemented;
        }

        protected virtual Result GetParameterInt(DspState* state, int index, out int value, IntPtr valueStr)
        {
            value = default;
            return Result.Err_Unimplemented;
        }

        protected virtual Result GetParameterBool(DspState* state, int index, out bool value, IntPtr valueStr)
        {
            value = default;
            return Result.Err_Unimplemented;
        }

        protected virtual Result GetParameterData(DspState* state, int index, out IntPtr value, out uint length, IntPtr valueStr)
        {
            value = default;
            length = default;
            return Result.Err_Unimplemented;
        }

        protected virtual Result ShouldIProcess(DspState* state, bool inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode)
        {
            return Result.Err_DSP_DontProcess;
        }

        protected virtual Result SystemRegister(DspState* state)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SystemDeregister(DspState* state)
        {
            return Result.Err_Unimplemented;
        }

        protected virtual Result SystemMix(DspState* state, int stage)
        {
            return Result.Err_Unimplemented;
        }
    }
}
