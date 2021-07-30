#pragma warning disable CA1815, CA1034

using System;
using System.Text;
using FmodAudio.Base;

namespace FmodAudio.Codec
{
    public unsafe struct CodecDescriptionStruct
    {
        public uint ApiVersion;
        public byte* Name;
        public FmodVersion Version;
        public FmodBool DefaultAsStream;
        public TimeUnit TimeUnits;

        public delegate* unmanaged<CodecState*, Mode, CreateSoundInfoStruct*, Result> Open;
        public delegate* unmanaged<CodecState*, Result> Close;
        public delegate* unmanaged<CodecState*, void*, uint, uint*, Result> Read;
        public delegate* unmanaged<CodecState*, uint*, TimeUnit, Result> GetLength;
        public delegate* unmanaged<CodecState*, int, uint, TimeUnit, Result> SetPosition;
        public delegate* unmanaged<CodecState*, uint*, TimeUnit, Result> GetPosition;
        public delegate* unmanaged<CodecState*, int, IntPtr, Result> SoundCreate;
        public delegate* unmanaged<CodecState*, int, CodecWaveFormat*, Result> GetWaveFormat;
    }
}
