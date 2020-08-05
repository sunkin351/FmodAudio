#pragma warning disable CA1815, CA1034

using System;
using System.Text;
using FmodAudio.Base;

namespace FmodAudio.Codec
{
    public unsafe struct CodecDescriptionStruct
    {
        public byte* Name;
        public FmodVersion Version;
        public FmodBool DefaultAsStream;
        public TimeUnit TimeUnits;
        public delegate* stdcall<CodecState*, Mode, CreateSoundInfoStruct*, Result> Open;
        public delegate* stdcall<CodecState*, Result> Close;
        public delegate* stdcall<CodecState*, void*, uint, uint*, Result> Read;
        public delegate* stdcall<CodecState*, uint*, TimeUnit, Result> GetLength;
        public delegate* stdcall<CodecState*, int, uint, TimeUnit, Result> SetPosition;
        public delegate* stdcall<CodecState*, uint*, TimeUnit, Result> GetPosition;
        public delegate* stdcall<CodecState*, int, IntPtr, Result> SoundCreate;
        public delegate* stdcall<CodecState*, int, CodecWaveFormat*, Result> GetWaveFormat;
    }
}
