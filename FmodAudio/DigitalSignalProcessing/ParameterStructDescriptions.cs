#pragma warning disable CA1815

using System;
using System.Runtime.InteropServices;
using FmodAudio.Base;

namespace FmodAudio.DigitalSignalProcessing
{
    public struct ParameterFloatMappingPiecewiseLinear
    {
        public int PointCount;
        public IntPtr PointParamValues;
        public IntPtr pointPositions;
    }

    public struct ParameterFloatMapping
    {
        public ParameterFloatMappingType Type;
        public ParameterFloatMappingPiecewiseLinear PiecewiseLinearMapping;
    }

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

    public struct BoolDescription
    {
        public FmodBool DefaultValue;

        /// <summary>
        /// [Optional] Names for true and false respectively. There should be 2 strings.
        /// </summary>
        public IntPtr ValueNames;
    }

    public struct DataDescription
    {
        public int DataType;

        public DataDescription(int dataType)
        {
            DataType = dataType;
        }
    }

    public unsafe struct ParameterDescriptionStruct
    {
        public DSPParameterType Type;

        public fixed byte NameBuffer[16];

        public fixed byte LabelBuffer[16];

        /// <summary>
        /// byte* UTF8 null terminated string
        /// </summary>
        public byte* Description;

        public DescriptionUnion DescUnion;

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
