﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterDescFloat
    {
        public float Min;
        public float Max;
        public float DefaultValue;
        public ParameterFloatMapping Mapping;

        public ParameterDescFloat(float min, float max, float defaultValue)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            Mapping = default;
        }

        public ParameterDescFloat(float min, float max, float defaultValue, ref ParameterFloatMapping mapping)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            Mapping = mapping;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterDescInt
    {
        public int Min, Max, DefaultValue;

        private int _infinity;

        /// <summary>
        /// Whether the last value represents infinity
        /// </summary>
        public bool GoesToInfinity { get => _infinity != 0; set => _infinity = value ? 1 : 0; }

        /// <summary>
        /// [Optional] Names for each value. There should be as many strings as there are possible values (max - min + 1).
        /// </summary>
        public IntPtr ValueNames;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterDescBool
    {
        private int _default;

        public bool DefaultValue { get => _default != 0; set => _default = value ? 1 : 0; }

        /// <summary>
        /// [Optional] Names for true and false respectively. There should be 2 strings.
        /// </summary>
        public IntPtr ValueNames;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterDescData
    {
        public int DataType;

        public ParameterDescData(int dataType)
        {
            DataType = dataType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ParameterDescription
    {
        public static ParameterDescription CreateDataDescription(string name, string label, DSPParameterType dataType)
        {
            return CreateDataDescription(name, label, (int)dataType);
        }

        public static ParameterDescription CreateDataDescription(string name, string label, int dataType)
        {
            ParameterDescription desc = default;
            desc.Type = DSPParameterType.Data;
            desc.Name = name;
            desc.Label = label;
            desc.DescUnion = new ParameterDescData(dataType);
            return desc;
        }

        public static ParameterDescription CreateFloatDescription(string name, string label, float min, float max, float defaultValue)
        {
            ParameterDescription desc = default;
            desc.Type = DSPParameterType.Float;
            desc.Name = name;
            desc.Label = label;
            desc.DescUnion = new ParameterDescFloat(min, max, defaultValue);
            return desc;
        }

        public DSPParameterType Type;
        
        public fixed byte NameBuffer[16];
        
        public fixed byte LabelBuffer[16];

        /// <summary>
        /// byte* encoding null terminated UTF8 string
        /// </summary>
        public IntPtr Description;

        public DescriptionUnion DescUnion;

        /// <summary>
        /// Max string length is 16 UTF8 bytes
        /// </summary>
        public string Name
        {
            get
            {
                return Helpers.MemoryToString(MemoryMarshal.CreateReadOnlySpan(ref NameBuffer[0], 15));
            }

            set
            {
                var buf = MemoryMarshal.CreateSpan(ref NameBuffer[0], 15);

                StringToBuffer(value, buf);
            }
        }

        public string Label
        {
            get
            {
                return Helpers.MemoryToString(MemoryMarshal.CreateReadOnlySpan(ref LabelBuffer[0], 15));
            }

            set
            {
                var buf = MemoryMarshal.CreateSpan(ref LabelBuffer[0], 15);

                StringToBuffer(value, buf);
            }
        }

        private static void StringToBuffer(string value, Span<byte> buffer)
        {
            if (string.IsNullOrEmpty(value))
            {
                buffer[0] = 0;
            }
            else
            {
                int count = Encoding.UTF8.GetBytes(value.AsSpan(), buffer);
                
                if (count < buffer.Length)
                {
                    buffer[count] = 0;
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DescriptionUnion
        {
            [FieldOffset(0)]
            public ParameterDescFloat FloatDescription;

            [FieldOffset(0)]
            public ParameterDescInt IntDescription;

            [FieldOffset(0)]
            public ParameterDescBool BoolDescription;

            [FieldOffset(0)]
            public ParameterDescData DataDescription;

            private DescriptionUnion(ParameterDescFloat desc)
            {
                this = default;
                FloatDescription = desc;
            }

            private DescriptionUnion(ParameterDescInt desc)
            {
                this = default;
                IntDescription = desc;
            }

            private DescriptionUnion(ParameterDescBool desc)
            {
                this = default;
                BoolDescription = desc;
            }

            private DescriptionUnion(ParameterDescData desc)
            {
                this = default;
                DataDescription = desc;
            }

            public static implicit operator DescriptionUnion(ParameterDescFloat desc)
            {
                return new DescriptionUnion(desc);
            }

            public static implicit operator DescriptionUnion(ParameterDescInt desc)
            {
                return new DescriptionUnion(desc);
            }

            public static implicit operator DescriptionUnion(ParameterDescBool desc)
            {
                return new DescriptionUnion(desc);
            }

            public static implicit operator DescriptionUnion(ParameterDescData desc)
            {
                return new DescriptionUnion(desc);
            }
        }
    }
}
