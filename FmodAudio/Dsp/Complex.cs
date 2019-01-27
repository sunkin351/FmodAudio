using System.Runtime.InteropServices;

namespace FmodAudio.Dsp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex
    {
        public float Real;
        public float Imag;
    }
}
