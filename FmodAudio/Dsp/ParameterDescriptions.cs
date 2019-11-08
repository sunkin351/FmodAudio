using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FmodAudio.Dsp
{
    using Dsp.Interop;
    public abstract class ParameterDescription
    {
        internal ParameterDescriptionStruct internalDescription;

        internal static unsafe ParameterDescription CreateFromPointer(ParameterDescriptionStruct* ptr)
        {
            ParameterDescriptionStruct* structPtr = (ParameterDescriptionStruct*)ptr;

            return structPtr->Type switch
            {
                DSPParameterType.Float => new FloatParameterDescription(structPtr),
                DSPParameterType.Int => new IntParameterDescription(structPtr),
                DSPParameterType.Bool => new BoolParameterDescription(structPtr),
                DSPParameterType.Data => new DataParameterDescription(structPtr),
                _ => throw new InvalidOperationException("Unknown Parameter type encountered")
            };
        }

        protected unsafe ParameterDescription(DSPParameterType type, string name, string label)
        {
            internalDescription.Type = type;

            StringToBuffer(name, MemoryMarshal.CreateSpan(ref internalDescription.NameBuffer[0], 15));

            StringToBuffer(label, MemoryMarshal.CreateSpan(ref internalDescription.LabelBuffer[0], 15));

            Name = name;
            Label = label;
        }

        internal unsafe ParameterDescription(ParameterDescriptionStruct* ptr)
        {
            internalDescription = *ptr;

            Name = FmodHelpers.MemoryToString(MemoryMarshal.CreateSpan(ref internalDescription.NameBuffer[0], 15));
            Label = FmodHelpers.MemoryToString(MemoryMarshal.CreateSpan(ref internalDescription.LabelBuffer[0], 15));
        }

        public string Name { get; }
        public string Label { get; }

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
    }

    public sealed class IntParameterDescription : ParameterDescription
    {
        public IntParameterDescription(string name, string label, int min, int max, int defaultValue, bool lastValueIsInfinity = false) : base(DSPParameterType.Int, name, label)
        {
            Description.Min = min;
            Description.Max = max;
            Description.DefaultValue = defaultValue;
            Description.Infinity = lastValueIsInfinity ? 1 : 0;
        }

        internal unsafe IntParameterDescription(ParameterDescriptionStruct* ptr) : base(ptr)
        {
        }

        private ref IntDescription Description => ref internalDescription.DescUnion.IntDescription;

        public int Min => Description.Min;

        public int Max => Description.Max;

        public int DefaultValue => Description.DefaultValue;

        public bool LastValueIsInfinity => Description.Infinity != 0;
    }

    public sealed class BoolParameterDescription : ParameterDescription
    {
        bool _managedDefault;

        public BoolParameterDescription(string name, string label, bool defaultValue)
            : base(DSPParameterType.Bool, name, label)
        {
            _managedDefault = defaultValue;
            Description.DefaultValue = defaultValue ? 1 : 0;
        }

        internal unsafe BoolParameterDescription(ParameterDescriptionStruct* ptr) : base(ptr)
        {
            _managedDefault = Description.DefaultValue != 0;
        }

        private ref BoolDescription Description => ref internalDescription.DescUnion.BoolDescription;

        public bool DefaultValue => _managedDefault;
    }

    public sealed class DataParameterDescription : ParameterDescription
    {
        public DataParameterDescription(string name, string label, int dataType)
            : base(DSPParameterType.Data, name, label)
        {
            Description.DataType = dataType;
        }

        public DataParameterDescription(string name, string label, ParameterDataType dataType)
            : this(name, label, (int)dataType)
        {
        }

        internal unsafe DataParameterDescription(ParameterDescriptionStruct* ptr) : base(ptr)
        {
        }

        private ref DataDescription Description => ref internalDescription.DescUnion.DataDescription;

        public ParameterDataType DataType => (ParameterDataType)Description.DataType;
    }

    public sealed class FloatParameterDescription : ParameterDescription
    {
        public FloatParameterDescription(string name, string label, float min, float max, float defaultValue) : base(DSPParameterType.Float, name, label)
        {
            Description.Min = min;
            Description.Max = max;
            Description.DefaultValue = defaultValue;
        }

        internal unsafe FloatParameterDescription(ParameterDescriptionStruct* ptr) : base(ptr)
        {
        }

        private ref FloatDescription Description => ref internalDescription.DescUnion.FloatDescription;

        public float Min => Description.Min;

        public float Max => Description.Max;

        public float DefaultValue => Description.DefaultValue;
    }
}
