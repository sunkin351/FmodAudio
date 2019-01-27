using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace FmodAudio.Dsp
{
    

    [StructLayout(LayoutKind.Sequential)]
    public unsafe class DspDescription
    {
        private Structure iStruct = default;

        public ref Structure Struct => ref iStruct;

        public FmodVersion PluginSDKVersion { get => iStruct.PluginSDKVersion; set => iStruct.PluginSDKVersion = value; }

        public string Name { get => iStruct.Name; set => iStruct.Name = value; }

        public FmodVersion Version { get => iStruct.Version; set => iStruct.Version = value; }

        public int InputBufferCount { get => iStruct.InputBufferCount; set => iStruct.InputBufferCount = value; }

        public int OutputBufferCount { get => iStruct.OutputBufferCount; set => iStruct.OutputBufferCount = value; }

        public DspStateCallback Create { get => iStruct.Create; set => iStruct.Create = value; }

        public DspStateCallback Release { get => iStruct.Release; set => iStruct.Release = value; }

        public DspStateCallback Reset { get => iStruct.Reset; set => iStruct.Reset = value; }

        public DspReadCallback Read { get => iStruct.Read; set => iStruct.Read = value; }

        public DspProcessCallback Process { get => iStruct.Process; set => iStruct.Process = value; }

        public DspSetPositionCallback SetPosition { get => iStruct.SetPosition; set => iStruct.SetPosition = value; }

        public int ParameterCount { get => iStruct.ParameterCount; set => iStruct.ParameterCount = value; }

        public ParameterDescription** ParameterDescriptions { get => iStruct.ParameterDescriptions; set => iStruct.ParameterDescriptions = value; }

        public DspSetParamFloatCallback SetParamFloat { get => iStruct.SetParamFloat; set => iStruct.SetParamFloat = value; }

        public DspSetParamIntCallback SetParamInt { get => iStruct.SetParamInt; set => iStruct.SetParamInt = value; }

        public DspSetParamBoolCallback SetParamBool { get => iStruct.SetParamBool; set => iStruct.SetParamBool = value; }

        public DspSetParamDataCallback SetParamData { get => iStruct.SetParamData; set => iStruct.SetParamData = value; }

        public DspGetParamFloatCallback GetParamFloat { get => iStruct.GetParamFloat; set => iStruct.GetParamFloat = value; }

        public DspGetParamIntCallback GetParamInt { get => iStruct.GetParamInt; set => iStruct.GetParamInt = value; }

        public DspGetParamBoolCallback GetParamBool { get => iStruct.GetParamBool; set => iStruct.GetParamBool = value; }

        public DspGetParamDataCallback GetParamData { get => iStruct.GetParamData; set => iStruct.GetParamData = value; }

        
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

            // TODO: Implement all of the following delegates as "static delegate", if and when the language feature becomes available.
            // Source: https://github.com/dotnet/csharplang/issues/80

            public DspStateCallback Create;
            public DspStateCallback Release;
            public DspStateCallback Reset;
            public DspReadCallback Read;
            public DspProcessCallback Process;
            public DspSetPositionCallback SetPosition;

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
                return new Span<byte>(Unsafe.AsPointer(ref NameBuffer[0]), 31);
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
    }
}
