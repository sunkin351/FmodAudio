using System;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParameterFloatMappingPiecewiseLinear
    {
        public int PointCount;
        public IntPtr PointParamValues;
        public IntPtr pointPositions;
    }
}
