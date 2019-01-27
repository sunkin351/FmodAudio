using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterFloatMappingPiecewiseLinear
    {
        public int PointCount;
        public IntPtr PointParamValues;
        public IntPtr pointPositions;
    }
}
