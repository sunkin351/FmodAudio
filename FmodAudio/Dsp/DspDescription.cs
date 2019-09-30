using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace FmodAudio.Dsp
{
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

        public ReadOnlySpan<ParameterDescription> ParameterDescriptions
        {
            get
            {
                return descManager is null ? default : descManager.Array;
            }
        }

        public int ParameterCount => Struct.ParameterCount;

        public void SetParameterDescriptions(ReadOnlySpan<ParameterDescription> paramDescriptions)
        {
            if (paramDescriptions.IsEmpty)
            {
                descManager = null;
                Struct.ParameterCount = 0;
                Struct.ParameterDescriptions = null;
            }
            else
            {
                descManager = new ParameterDescriptionManager(paramDescriptions);
                Struct.ParameterCount = paramDescriptions.Length;
                Struct.ParameterDescriptions = (ParameterDescription**)descManager.PointerArray;
            }
        }

        public DspSetParamFloatCallback SetParamFloat { get => Struct.SetParamFloat; set => Struct.SetParamFloat = value; }

        public DspSetParamIntCallback SetParamInt { get => Struct.SetParamInt; set => Struct.SetParamInt = value; }

        public DspSetParamBoolCallback SetParamBool { get => Struct.SetParamBool; set => Struct.SetParamBool = value; }

        public DspSetParamDataCallback SetParamData { get => Struct.SetParamData; set => Struct.SetParamData = value; }

        public DspGetParamFloatCallback GetParamFloat { get => Struct.GetParamFloat; set => Struct.GetParamFloat = value; }

        public DspGetParamIntCallback GetParamInt { get => Struct.GetParamInt; set => Struct.GetParamInt = value; }

        public DspGetParamBoolCallback GetParamBool { get => Struct.GetParamBool; set => Struct.GetParamBool = value; }

        public DspGetParamDataCallback GetParamData { get => Struct.GetParamData; set => Struct.GetParamData = value; }

        
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
            public ParameterDescription** ParameterDescriptions;

            public DspSetParamFloatCallback SetParamFloat;
            public DspSetParamIntCallback SetParamInt;
            public DspSetParamBoolCallback SetParamBool;
            public DspSetParamDataCallback SetParamData;

            public DspGetParamFloatCallback GetParamFloat;
            public DspGetParamIntCallback GetParamInt;
            public DspGetParamBoolCallback GetParamBool;
            public DspGetParamDataCallback GetParamData;

            public DspShouldIProcessCallback ShouldIProcess;

            public IntPtr UserData;

            public DspStateCallback SystemRegister;
            public DspStateCallback SystemDeregister;
            public DspSystemMixCallback SystemMix;

            /// <summary>
            /// [w] Name of the unit to be displayed in the network.
            /// </summary>
            
            private Span<byte> GetNameBufferSpan()
            {
                return MemoryMarshal.CreateSpan(ref NameBuffer[0], 31);
            }

            public string Name
            {
                get
                {
                    return Helpers.MemoryToString(GetNameBufferSpan());
                }

                set
                {
                    var span = GetNameBufferSpan();

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
            private IntPtr ptrArray, descArray;
            private int length;

            public ParameterDescriptionManager(ReadOnlySpan<ParameterDescription> arr)
            {
                ptrArray = Memory.AllocateUnsafe(IntPtr.Size * arr.Length);
                descArray = Memory.AllocateUnsafe(Unsafe.SizeOf<ParameterDescription>() * arr.Length);
                length = arr.Length;

                InitPtrArr(ptrArray, descArray, arr.Length);

                arr.CopyTo(new Span<ParameterDescription>((void*)descArray, arr.Length));
            }

            ~ParameterDescriptionManager()
            {
                Memory.FreeUnsafe(ptrArray);
                Memory.FreeUnsafe(descArray);
            }

            public IntPtr PointerArray => ptrArray;

            public ReadOnlySpan<ParameterDescription> Array => new ReadOnlySpan<ParameterDescription>((void*)descArray, length);

            private static void InitPtrArr(IntPtr arr, IntPtr descPtr, int size)
            {
                int ParamDescLen = Unsafe.SizeOf<ParameterDescription>();

                Span<IntPtr> ptrArr = new Span<IntPtr>((void*)arr, size);

                for (int i = 0; i < size; ++i)
                {
                    ptrArr[i] = descPtr + (ParamDescLen * i);
                }
            }
        }
    }
}
