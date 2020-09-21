#pragma warning disable CA1815, CA1034

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

using FmodAudio.DigitalSignalProcessing.Interop;

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

        // TODO: Implement all of the following delegates as "function pointers", when the language feature becomes available.
        // Source: https://github.com/dotnet/csharplang/blob/master/proposals/function-pointers.md

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
        public delegate* unmanaged<DspState*, int, int*, byte*, Result> GetParamBool;
        public delegate* unmanaged<DspState*, int, void**, uint*, byte*, Result> GetParamData;

        public delegate* unmanaged<DspState*, int, uint, ChannelMask, int, int*, Result> ShouldIProcess;

        [Obsolete]
        public IntPtr UserData;

        public delegate* unmanaged<DspState*, Result> SystemRegister;
        public delegate* unmanaged<DspState*, Result> SystemDeregister;
        public delegate* unmanaged<DspState*, int, Result> SystemMix;

        /// <summary>
        /// [w] Name of the unit to be displayed in the network.
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
                    Utf8.FromUtf16(value, span, out _, out _);
                }
            }
        }
    }

    public unsafe sealed class DspDescription
    {
        public uint PluginSDKVersion { get; set; }

        public string Name { get; set; }

        public FmodVersion Version { get; set; }

        public int InputBufferCount { get; set; }

        public int OutputBufferCount { get; set; }

        public DspStateCallback CreateCallback { get; set; }

        public DspStateCallback ReleaseCallback { get; set; }

        public DspStateCallback ResetCallback { get; set; }

        public DspReadCallback ReadCallback { get; set; }

        public DspProcessCallback ProcessCallback { get; set; }

        public DspSetPositionCallback SetPositionCallback { get; set; }

        private ParameterDescription[] descriptions;

        public int ParameterCount => descriptions.Length;

        public IReadOnlyList<ParameterDescription> ParameterDescriptions => descriptions;

        internal ParameterDescriptionManager? descriptionManager;

        public DspDescription SetParameterDescriptions(params ParameterDescription[] descriptions)
        {
            if (descriptions is null || descriptions.Length == 0)
            {
                descriptionManager = null;
                descriptions = null;
            }
            else
            {
                if (descriptions.Any(param => param == null))
                {
                    throw new ArgumentException("Elements in array cannot be null.");
                }

                this.descriptions = descriptions;
                descriptionManager = new ParameterDescriptionManager(descriptions);
            }

            return this;
        }

        public DspSetParamFloatCallback SetParamFloatCallback { get; set; }

        public DspSetParamIntCallback SetParamIntCallback { get; set; }

        public DspSetParamBoolCallback SetParamBoolCallback { get; set; }

        public DspSetParamDataCallback SetParamDataCallback { get; set; }

        public DspGetParamFloatCallback GetParamFloatCallback { get; set; }

        public DspGetParamIntCallback GetParamIntCallback { get; set; }

        public DspGetParamBoolCallback GetParamBoolCallback { get; set; }

        public DspGetParamDataCallback GetParamDataCallback { get; set; }

        public DspShouldIProcessCallback ShouldIProcessCallback { get; set; }

        public DspStateCallback SystemRegisterCallback { get; set; }

        public DspStateCallback SystemDeregisterCallback { get; set; }

        public DspSystemMixCallback SystemMixCallback { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Structure
        {
            /// <summary>
            /// The Plugin SDK version this plugin is built for.
            /// </summary>
            public uint PluginSDKVersion;

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

            [Obsolete]
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
                        Encoding.UTF8.GetBytes(value.AsSpan(), span);
                    }
                }
            }
        }

        internal DspDescription Clone()
        {
            return (DspDescription)this.MemberwiseClone();
        }

        internal sealed class ParameterDescriptionManager
        {
            private readonly IntPtr[] _pointerArray;
            private readonly ParameterDescriptionStruct[] _descriptionArray;

            public ParameterDescriptionManager(ParameterDescription[] descriptions)
            {
                _pointerArray = GC.AllocateArray<IntPtr>(descriptions.Length, pinned: true);
                _descriptionArray = GC.AllocateArray<ParameterDescriptionStruct>(descriptions.Length, pinned: true);

                for (int i = 0; i < descriptions.Length; ++i)
                {
                    ref var tmp = ref _descriptionArray[i];

                    tmp = descriptions[i].internalDescription;

                    _pointerArray[i] = (IntPtr)Unsafe.AsPointer(ref tmp);
                }
            }

            public IntPtr PointerArray => (IntPtr)Unsafe.AsPointer(ref _pointerArray[0]);
            public int Length => _pointerArray.Length;
        }
    }
}
