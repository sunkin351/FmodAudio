#pragma warning disable CA1815, CA1034

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

using FmodAudio.Base;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DspDescriptionStruct
    {
        /// <summary>
        /// The Plugin SDK version this plugin is built for.
        /// </summary>
        public uint PluginSDKVersion;

        /// <summary>
        /// Name of the unit to be displayed in the network
        /// </summary>
        public fixed byte NameBuffer[32];

        /// <summary>
        /// Plugin writer's version number
        /// </summary>
        public FmodVersion Version;

        public int InputBufferCount;
        public int OutputBufferCount;

        /// <summary>
        /// Delegate Definition: delegate Result DspStateCallback(DspState* state);
        /// </summary>
        public delegate* unmanaged<DspState*, Result> Create;

        /// <summary>
        /// Delegate Definition: delegate Result DspStateCallback(DspState* state);
        /// </summary>
        public delegate* unmanaged<DspState*, Result> Release;

        /// <summary>
        /// Delegate Definition: delegate Result DspStateCallback(DspState* state);
        /// </summary>
        public delegate* unmanaged<DspState*, Result> Reset;

        /// <summary>
        /// 
        /// </summary>
        public delegate* unmanaged<DspState*, float*, float*, uint, int, int*, Result> Read;
        
        public delegate* unmanaged<DspState*, uint, DspBufferArray*, DspBufferArray*, int, ProcessOperation, Result> Process;
        public delegate* unmanaged<DspState*, uint, Result> SetPosition;

        public int ParameterCount;
        public ParameterDescriptionStruct** ParameterDescriptions;

        public delegate* unmanaged<DspState*, int, float, Result> SetParamFloat;
        public delegate* unmanaged<DspState*, int, int, Result> SetParamInt;
        public delegate* unmanaged<DspState*, int, int, Result> SetParamBool;
        public delegate* unmanaged<DspState*, int, void*, uint, Result> SetParamData;

        public delegate* unmanaged<DspState*, int, float*, byte*, Result> GetParamFloat;
        public delegate* unmanaged<DspState*, int, int*, byte*, Result> GetParamInt;
        public delegate* unmanaged<DspState*, int, FmodBool*, byte*, Result> GetParamBool;
        public delegate* unmanaged<DspState*, int, void**, uint*, byte*, Result> GetParamData;

        public delegate* unmanaged<DspState*, int, uint, ChannelMask, int, SpeakerMode, Result> ShouldIProcess;

        public IntPtr UserData;

        public delegate* unmanaged<DspState*, Result> SystemRegister;
        public delegate* unmanaged<DspState*, Result> SystemDeregister;
        public delegate* unmanaged<DspState*, int, Result> SystemMix;

        /// <summary>
        /// [w] Name of the unit to be displayed in the network.
        /// Note: will only use the first 31 bytes of the 32-byte buffer. (Enforcing a null-terminated string)
        /// </summary>
        public string Name
        {
            get
            {
                return FmodHelpers.BufferToString(MemoryMarshal.CreateSpan(ref NameBuffer[0], 31));
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
                    var status = Utf8.FromUtf16(value, span, out _, out _);

                    if (status == OperationStatus.InvalidData || status == OperationStatus.NeedMoreData)
                    {
                        throw new ArgumentException("Invalid String Data");
                    }
                }
            }
        }
    }
}
