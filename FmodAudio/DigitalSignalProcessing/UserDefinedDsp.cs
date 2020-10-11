#pragma warning disable CA1031, IDE1006, CS0612
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FmodAudio.Base;

#nullable enable

namespace FmodAudio.DigitalSignalProcessing
{
    public unsafe abstract class UserDefinedDsp : Dsp
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
        private static readonly DspDescriptionStruct DspCreateStructure;

        static UserDefinedDsp()
        {
            
            MethodReferences.SetParamFloat = _setParamFloat;
            MethodReferences.SetParamInt = _setParamInt;
            MethodReferences.SetParamBool = _setParamBool;
            MethodReferences.SetParamData = _setParamData;

            //MethodReferences.GetParamFloat = _getParamFloat;
            //MethodReferences.GetParamInt = _getParamInt;
            //MethodReferences.GetParamBool = _getParamBool;
            //MethodReferences.GetParamData = _getParamData;

            DspCreateStructure.Create =  (delegate* unmanaged<DspState*, Result>)(delegate* <DspState*, Result>)&_createMethod;
            DspCreateStructure.Release = (delegate* unmanaged<DspState*, Result>)(delegate* <DspState*, Result>)&_releaseMethod;
            DspCreateStructure.Reset =   (delegate* unmanaged<DspState*, Result>)(delegate* <DspState*, Result>)&_resetMethod;
            DspCreateStructure.Read = (delegate* unmanaged<DspState*, float*, float*, uint, int, int*, Result>)(delegate* <DspState*, float*, float*, uint, int, int*, Result>)&_readMethod;
            DspCreateStructure.Process = (delegate* unmanaged<DspState*, uint, DspBufferArray*, DspBufferArray*, int, ProcessOperation, Result>)(delegate* <DspState*, uint, DspBufferArray*, DspBufferArray*, int, ProcessOperation, Result>)& _processMethod;
            DspCreateStructure.SetPosition = (delegate* unmanaged<DspState*, uint, Result>)(delegate*<DspState*, uint, Result>)&_setPositionMethod;

            //DspCreateStructure.SetParamFloat = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamFloat);
            //DspCreateStructure.SetParamInt = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamInt);
            //DspCreateStructure.SetParamBool = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamBool);
            //DspCreateStructure.SetParamData = Marshal.GetFunctionPointerForDelegate(MethodReferences.SetParamData);

            //DspCreateStructure.GetParamFloat = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamFloat);
            //DspCreateStructure.GetParamInt = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamInt);
            //DspCreateStructure.GetParamBool = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamBool);
            //DspCreateStructure.GetParamData = Marshal.GetFunctionPointerForDelegate(MethodReferences.GetParamData);

            DspCreateStructure.ShouldIProcess =   (delegate* unmanaged<DspState*, int, uint, ChannelMask, int, SpeakerMode, Result>)(delegate*<DspState*, int, uint, ChannelMask, int, SpeakerMode, Result>)&_shouldIProcessMethod;

            DspCreateStructure.SystemRegister =   (delegate* unmanaged<DspState*, Result>)(delegate* <DspState*, Result>)&_systemRegisterMethod;
            DspCreateStructure.SystemDeregister = (delegate* unmanaged<DspState*, Result>)(delegate* <DspState*, Result>)&_systemDeregisterMethod;
            
            DspCreateStructure.SystemMix =        (delegate* unmanaged<DspState*, int, Result>)(delegate* <DspState*, int, Result>)&_systemMixMethod;
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
        private static Result _readMethod(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, int* outChannels)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Read(state, inBuffer, outBuffer, length, inChannels, ref *outChannels);
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

        [UnmanagedCallersOnly]
        private static Result _processMethod(DspState* state, uint length, DspBufferArray* inBufferArray, DspBufferArray* outBufferArray, int inputsIdle, ProcessOperation operation)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.Process(state, length, inBufferArray, outBufferArray, inputsIdle != 0, operation);
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
        private static Result _getParamFloat(DspState* state, int index, float* value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterFloat(state, index, out *value, strValue);
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

        [UnmanagedCallersOnly]
        private static Result _getParamInt(DspState* state, int index, int* value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.GetParameterInt(state, index, out *value, strValue);
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

        [UnmanagedCallersOnly]
        private static Result _getParamBool(DspState* state, int index, FmodBool* value, IntPtr strValue)
        {
            value = default;
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                var res = dsp.GetParameterBool(state, index, out bool val, strValue);

                *value = val;

                return res;
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

        [UnmanagedCallersOnly]
        private static Result _getParamData(DspState* state, int index, IntPtr* data, uint* length, IntPtr strValue)
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
                return dsp.GetParameterData(state, index, out *data, out *length, strValue);
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

        [UnmanagedCallersOnly]
        private static Result _shouldIProcessMethod(DspState* state, int inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode)
        {
            var dsp = GetDSPObject(state);

            if (dsp is null)
            {
                return Result.Err_Internal;
            }

            try
            {
                return dsp.ShouldIProcess(state, inputsIdle != 0, length, inMask, inChannels, speakerMode);
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        [UnmanagedCallersOnly]
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

        private readonly ParameterDescriptionList? DescriptionList;

        public override int ParameterCount => DescriptionList?.List.Length ?? 0;

        protected UserDefinedDsp(SystemHandle sys, string name, FmodVersion pluginVersion, int inputBufferCount, int outputBufferCount, ParameterDescription[]? descriptions, DSPProcessType processType)
            : base(default)
        {
            var createStruct = DspCreateStructure; //Value Type Copy

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

            if (descriptions != null)
            {
                DescriptionList = new ParameterDescriptionList(descriptions);

                DescriptionList.GetPointerAndCount(out createStruct.ParameterDescriptions, out createStruct.ParameterCount);
            }

            createStruct.UserData = (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak);

            fixed (DspHandle* pHandle = &Handle)
            {
                library.System_CreateDSP(sys, &createStruct, pHandle).CheckResult();
            }
        }

        public sealed override ParameterDescription GetParameterInfo(int index)
        {
            if ((uint)index >= (uint)ParameterCount)
            {
                throw new IndexOutOfRangeException();
            }

            return DescriptionList!.List[index];
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
            Span<float> inSpan = new Span<float>(inBuffer, (int)length * inChannels),
                outSpan = new Span<float>(outBuffer, (int)length * inChannels);

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
