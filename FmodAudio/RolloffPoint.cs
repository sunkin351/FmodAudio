#pragma warning disable CS0414

namespace FmodAudio
{
    /// <summary>
    /// Represents a 3D Rolloff point
    /// </summary>
    public struct RolloffPoint
    {
        public float Distance;
        public float Volume;
        private uint Padding; //Padding is according to the documentation for a rolloff point

        public RolloffPoint(float distance, float volume)
        {
            Distance = distance;
            Volume = volume;
            Padding = 0;
        }
    }
}
