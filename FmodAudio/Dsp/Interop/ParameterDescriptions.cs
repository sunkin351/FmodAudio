using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Dsp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FloatDescription
    {
        public float Min;
        public float Max;
        public float DefaultValue;
        public ParameterFloatMapping Mapping;

        public FloatDescription(float min, float max, float defaultValue)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            Mapping = default;
        }

        public FloatDescription(float min, float max, float defaultValue, ref ParameterFloatMapping mapping)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            Mapping = mapping;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IntDescription
    {
        public int Min;

        public int Max;

        public int DefaultValue;

        public int Infinity;

        /// <summary>
        /// [Optional] Names for each value. There should be as many strings as there are possible values (max - min + 1).
        /// </summary>
        public IntPtr ValueNames;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BoolDescription
    {
        public int DefaultValue;

        /// <summary>
        /// [Optional] Names for true and false respectively. There should be 2 strings.
        /// </summary>
        public IntPtr ValueNames;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DataDescription
    {
        public int DataType;

        public DataDescription(int dataType)
        {
            DataType = dataType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ParameterDescriptionStruct
    {
        public DSPParameterType Type;

        public fixed byte NameBuffer[16];

        public fixed byte LabelBuffer[16];

        /// <summary>
        /// byte* UTF8 null terminated string
        /// </summary>
        public IntPtr Description;

        internal DescriptionUnion DescUnion;

        [StructLayout(LayoutKind.Explicit)]
        public struct DescriptionUnion
        {
            [FieldOffset(0)]
            public FloatDescription FloatDescription;

            [FieldOffset(0)]
            public IntDescription IntDescription;

            [FieldOffset(0)]
            public BoolDescription BoolDescription;

            [FieldOffset(0)]
            public DataDescription DataDescription;
        }
    }
}
