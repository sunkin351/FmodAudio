using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterFloatMapping
    {
        public ParameterFloatMappingType Type;
        public ParameterFloatMappingPiecewiseLinear PiecewiseLinearMapping;
    }
}
