using System;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Whether the last value represents infinity
        /// </summary>
        public bool GoesToInf;

        /// <summary>
        /// [Optional] Names for each value. There should be as many strings as there are possible values (max - min + 1).
        /// </summary>
        public IntPtr ValueNames;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterDescBool
    {
        public bool DefaultValue;

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
        public static ParameterDescription CreateDataDescription(string name, string label, ParameterDataType dataType)
        {
            return CreateDataDescription(name, label, (int)dataType);
        }

        public static ParameterDescription CreateDataDescription(string name, string label, int dataType)
        {
            ParameterDescription desc = default;
            desc.Type = ParameterType.Data;
            desc.Name = name;
            desc.Label = label;
            desc.DescData = new ParameterDescData(dataType);
            return desc;
        }

        public static ParameterDescription CreateFloatDescription(string name, string label, float min, float max, float defaultValue)
        {
            ParameterDescription desc = default;
            desc.Type = ParameterType.Float;
            desc.Name = name;
            desc.Label = label;
            desc.DescData = new ParameterDescFloat(min, max, defaultValue);
            return desc;
        }

        public ParameterType Type;
        
        private fixed byte NameBuffer[16];
        
        private fixed byte LabelBuffer[16];

        /// <summary>
        /// byte* encoding null terminated UTF8 string
        /// </summary>
        public IntPtr Description;

        public DescriptionUnion DescData;
        

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

                if (buf[0] != 0)
                    buf.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    Encoding.UTF8.GetBytes(value.AsSpan(), buf);
                }
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

                if (buf[0] != 0)
                    buf.Clear();

                if (!string.IsNullOrEmpty(value))
                {
                    Encoding.UTF8.GetBytes(value.AsSpan(), buf);
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
