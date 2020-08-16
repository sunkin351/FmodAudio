using System.Numerics;

namespace FmodAudio.Output
{
    public unsafe readonly struct Object3DInfo
    {
        public readonly float* Buffer;
        public readonly uint BufferLength;
        public readonly Vector3 Position;
        public readonly float Gain;
        public readonly float Spread;
        public readonly float Priority;
    }
}
