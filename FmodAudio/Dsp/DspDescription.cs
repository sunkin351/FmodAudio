#pragma warning disable CA1815

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace FmodAudio.Dsp
{
    using Dsp.Interop;
    [StructLayout(LayoutKind.Sequential)]
    public unsafe sealed class DspDescription
    {
        internal Structure Struct = default;

        private DspStateCallback Create;
        private DspStateCallback Release;
        private DspStateCallback Reset;
        private DspReadCallback Read;
        private DspProcessCallback Process;
        private DspSetPositionCallback SetPosition;

        private ParameterDescriptionManager descManager;

        private DspSetParamFloatCallback SetParamFloat;
        private DspSetParamIntCallback SetParamInt;
        private DspSetParamBoolCallback SetParamBool;
        private DspSetParamDataCallback SetParamData;
        
        private DspGetParamFloatCallback GetParamFloat;
        private DspGetParamIntCallback GetParamInt;
        private DspGetParamBoolCallback GetParamBool;
        private DspGetParamDataCallback GetParamData;

        private DspShouldIProcessCallback ShouldIProcess;
        private DspStateCallback SystemRegister;
        private DspStateCallback SystemDeregister;
        private DspSystemMixCallback SystemMix;

        public FmodVersion PluginSDKVersion { get => Struct.PluginSDKVersion; set => Struct.PluginSDKVersion = value; }

        public string Name { get => Struct.Name; set => Struct.Name = value; }

        public FmodVersion Version { get => Struct.Version; set => Struct.Version = value; }

        public int InputBufferCount { get => Struct.InputBufferCount; set => Struct.InputBufferCount = value; }

        public int OutputBufferCount { get => Struct.OutputBufferCount; set => Struct.OutputBufferCount = value; }

        public DspStateCallback CreateCallback
        {
            get => Create;
            set => Helpers.UpdateCallback(value, out Create, out Struct.Create);
        }

        public DspStateCallback ReleaseCallback
        {
            get => Release;
            set => Helpers.UpdateCallback(value, out Release, out Struct.Create);
        }

        public DspStateCallback ResetCallback
        {
            get => Reset;
            set => Helpers.UpdateCallback(value, out Reset, out Struct.Reset);
        }

        public DspReadCallback ReadCallback
        {
            get => Read;
            set => Helpers.UpdateCallback(value, out Read, out Struct.Read);
        }

        public DspProcessCallback ProcessCallback
        {
            get => Process;
            set => Helpers.UpdateCallback(value, out Process, out Struct.Process);
        }

        public DspSetPositionCallback SetPositionCallback
        {
            get => SetPosition;
            set => Helpers.UpdateCallback(value, out SetPosition, out Struct.SetPosition);
        }

        private ParameterDescription[] descriptions;

        public int ParameterCount => descriptions.Length;

        public IReadOnlyList<ParameterDescription> ParameterDescriptions => descriptions;

        public DspDescription SetParameterDescriptions(params ParameterDescription[] descriptions)
        {
            if (descriptions is null || descriptions.Length == 0)
            {
                descManager = null;
                descriptions = null;
                Struct.ParameterCount = 0;
                Struct.ParameterDescriptions = null;
            }
            else
            {
                if (descriptions.Any(param => param == null))
                {
                    throw new ArgumentException("Elements in array cannot be null.");
                }

                this.descriptions = descriptions;
                descManager = new ParameterDescriptionManager(descriptions);
                Struct.ParameterCount = descManager.Length;
                Struct.ParameterDescriptions = (ParameterDescriptionStruct**)descManager.PointerArray;
            }

            return this;
        }

        public DspSetParamFloatCallback SetParamFloatCallback
        {
            get => SetParamFloat;
            set => Helpers.UpdateCallback(value, out SetParamFloat, out Struct.SetParamFloat);
        }

        public DspSetParamIntCallback SetParamIntCallback
        {
            get => SetParamInt;
            set => Helpers.UpdateCallback(value, out SetParamInt, out Struct.SetParamInt);
        }

        public DspSetParamBoolCallback SetParamBoolCallback
        {
            get => SetParamBool;
            set => Helpers.UpdateCallback(value, out SetParamBool, out Struct.SetParamBool);
        }

        public DspSetParamDataCallback SetParamDataCallback
        {
            get => SetParamData;
            set => Helpers.UpdateCallback(value, out SetParamData, out Struct.SetParamData);
        }

        public DspGetParamFloatCallback GetParamFloatCallback
        {
            get => GetParamFloat;
            set => Helpers.UpdateCallback(value, out GetParamFloat, out Struct.GetParamFloat);
        }

        public DspGetParamIntCallback GetParamIntCallback
        {
            get => GetParamInt;
            set => Helpers.UpdateCallback(value, out GetParamInt, out Struct.GetParamInt);
        }

        public DspGetParamBoolCallback GetParamBoolCallback
        {
            get => GetParamBool;
            set => Helpers.UpdateCallback(value, out GetParamBool, out Struct.GetParamBool);
        }

        public DspGetParamDataCallback GetParamDataCallback
        {
            get => GetParamData;
            set => Helpers.UpdateCallback(value, out GetParamData, out Struct.GetParamData);
        }

        public DspShouldIProcessCallback ShouldIProcessCallback
        {
            get => ShouldIProcess;
            set => Helpers.UpdateCallback(value, out ShouldIProcess, out Struct.ShouldIProcess);
        }

        public DspStateCallback SystemRegisterCallback
        {
            get => SystemRegister;
            set => Helpers.UpdateCallback(value, out SystemRegister, out Struct.SystemRegister);
        }

        public DspStateCallback SystemDeregisterCallback
        {
            get => SystemDeregister;
            set => Helpers.UpdateCallback(value, out SystemDeregister, out Struct.SystemDeregister);
        }

        public DspSystemMixCallback SystemMixCallback
        {
            get => SystemMix;
            set => Helpers.UpdateCallback(value, out SystemMix, out Struct.SystemMix);
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Structure
        {
            /// <summary>
            /// The Plugin SDK version this plugin is built for.
            /// </summary>
            public FmodVersion PluginSDKVersion;

            /// <summary>
            /// Name of the unit to be displayed in the network
            /// </summary>
            private fixed byte NameBuffer[32];

            /// <summary>
            /// Plugin writer's version number
            /// </summary>
            public FmodVersion Version;

            public int InputBufferCount;
            public int OutputBufferCount;

            // TODO: Implement all of the following delegates as "function pointers", when the language feature becomes available.
            // Source: https://github.com/dotnet/csharplang/blob/master/proposals/function-pointers.md

            public IntPtr Create;
            public IntPtr Release;
            public IntPtr Reset;
            public IntPtr Read;
            public IntPtr Process;
            public IntPtr SetPosition;

            public int ParameterCount;
            public ParameterDescriptionStruct** ParameterDescriptions;

            public IntPtr SetParamFloat;
            public IntPtr SetParamInt;
            public IntPtr SetParamBool;
            public IntPtr SetParamData;

            public IntPtr GetParamFloat;
            public IntPtr GetParamInt;
            public IntPtr GetParamBool;
            public IntPtr GetParamData;

            public IntPtr ShouldIProcess;

            public IntPtr UserData;

            public IntPtr SystemRegister;
            public IntPtr SystemDeregister;
            public IntPtr SystemMix;

            /// <summary>
            /// [w] Name of the unit to be displayed in the network.
            /// </summary>
            public string Name
            {
                get
                {
                    return Helpers.MemoryToString(MemoryMarshal.CreateSpan(ref NameBuffer[0], 31));
                }

                set
                {
                    var span = MemoryMarshal.CreateSpan(ref NameBuffer[0], 31);

                    if (span[0] != 0)
                    {
                        span.Clear();
                    }

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        Encoding.UTF8.GetBytes(value.AsSpan(), span);
                    }
                }
            }
        }

        internal DspDescription Clone()
        {
            return (DspDescription)this.MemberwiseClone();
        }

        public delegate Result DspStateCallback(DspState* state);
        public delegate Result DspReadCallback(DspState* state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels);
        public delegate Result DspProcessCallback(DspState* state, uint length, ref DspBufferArray inBufferArray, ref DspBufferArray outBufferArray, bool inputsIdle, ProcessOperation operation);
        public delegate Result DspSetPositionCallback(DspState* state, uint pos);

        public delegate Result DspSetParamFloatCallback(DspState* state, int index, float value);
        public delegate Result DspSetParamIntCallback(DspState* state, int index, int value);
        public delegate Result DspSetParamBoolCallback(DspState* state, int index, bool value);
        public delegate Result DspSetParamDataCallback(DspState* state, int index, IntPtr data, uint dataLength);

        public delegate Result DspGetParamFloatCallback(DspState* state, int index, out float value, IntPtr valueString);
        public delegate Result DspGetParamIntCallback(DspState* state, int index, out int value, IntPtr valueString);
        public delegate Result DspGetParamBoolCallback(DspState* state, int index, out bool value, IntPtr valueString);
        public delegate Result DspGetParamDataCallback(DspState* state, int index, out IntPtr data, out uint dataLength, IntPtr valueString);

        public delegate Result DspShouldIProcessCallback(DspState* state, bool inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode);

        public delegate Result DspSystemMixCallback(DspState* state, int stage);

        private sealed class ParameterDescriptionManager
        {
            private readonly IntPtr ptrArray, descArray;
            private readonly int length;

            public ParameterDescriptionManager(ParameterDescription[] arr)
            {
                ptrArray = Memory.AllocateUnsafe(IntPtr.Size * arr.Length);
                descArray = Memory.AllocateUnsafe(Unsafe.SizeOf<ParameterDescriptionStruct>() * arr.Length);
                length = arr.Length;

                InitPtrArr(ptrArray, descArray, arr.Length);

                var span = new Span<ParameterDescriptionStruct>(descArray.ToPointer(), arr.Length);

                for (int i = 0; i < arr.Length; ++i)
                {
                    span[i] = arr[i].internalDescription;
                }
            }

            ~ParameterDescriptionManager()
            {
                Memory.FreeUnsafe(ptrArray);
                Memory.FreeUnsafe(descArray);
            }

            public IntPtr PointerArray => ptrArray;
            public int Length => length;

            private static void InitPtrArr(IntPtr arr, IntPtr descPtr, int size)
            {
                int ParamDescLen = Unsafe.SizeOf<ParameterDescriptionStruct>();

                Span<IntPtr> ptrArr = new Span<IntPtr>((void*)arr, size);

                for (int i = 0; i < size; ++i)
                {
                    ptrArr[i] = descPtr + (ParamDescLen * i);
                }
            }
        }
    }
}
