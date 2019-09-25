using System.Runtime.InteropServices;
using System.Numerics;

namespace FmodAudio
{
    /// <summary>
    /// Structure describing a position, velocity and orientation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Attributes3D
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Forward;
        public Vector3 Up;
    }
}
