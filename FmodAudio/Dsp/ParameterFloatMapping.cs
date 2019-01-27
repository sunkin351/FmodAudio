using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterFloatMapping
    {
        public ParameterFloatMappingType Type;
        public ParameterFloatMappingPiecewiseLinear PiecewiseLinearMapping;
    }
}
