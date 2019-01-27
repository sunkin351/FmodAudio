using System.Runtime.InteropServices;

namespace FmodAudio
{
    /// <summary>
    /// Structure Describing a point in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
