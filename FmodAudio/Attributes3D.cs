using System.Runtime.InteropServices;

namespace FmodAudio
{
    /// <summary>
    /// Structure describing a position, velocity and orientation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Attributes3D
    {
        public Vector Position;
        public Vector Velocity;
        public Vector Forward;
        public Vector Up;
    }
}
