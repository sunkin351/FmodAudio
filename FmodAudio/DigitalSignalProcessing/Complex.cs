using System.Numerics;
using System.Runtime.InteropServices;

namespace FmodAudio.DigitalSignalProcessing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Fmod_Complex
    {
        public float Real;
        public float Imag;

        internal Fmod_Complex(Complex comp)
        {
            Real = (float)comp.Real;
            Imag = (float)comp.Imaginary;
        }

        public static implicit operator Fmod_Complex(Complex complex)
        {
            return new Fmod_Complex(complex);
        }

        public static implicit operator Complex (Fmod_Complex comp)
        {
            return new Complex(comp.Real, comp.Imag);
        }

        public override bool Equals(object? obj)
        {
            return obj is Fmod_Complex comp && comp.Real == Real && comp.Imag == Imag;
        }

        public override int GetHashCode()
        {
            //https://source.dot.net/#System.Runtime.Numerics/System/Numerics/Complex.cs,8d39d13a80a9cb18,references
            int n1 = 99999997;
            int realHash = Real.GetHashCode() % n1;
            int imaginaryHash = Imag.GetHashCode();
            int finalHash = realHash ^ imaginaryHash;
            return finalHash;
        }
    }
}
