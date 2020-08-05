#pragma warning disable CA1815, CS0649

using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Codec
{
    using FmodAudio.Base;

    public unsafe struct CodecState
    {
        public int SubsoundCount;
        public CodecWaveFormat* WaveFormat;
        public IntPtr PluginData;

        public IntPtr FileHandle;
        public uint FileSize;

        private readonly delegate* stdcall<IntPtr, byte*, uint, uint*, IntPtr, Result> fileRead;
        private readonly delegate* stdcall<IntPtr, uint, IntPtr, Result> fileSeek;
        private readonly delegate* stdcall<CodecState*, TagType, byte*, byte*, uint, TagDataType, int, Result> metaData;

        public int WaveFormatVersion;

        public uint FileRead(IntPtr fileHandle, byte* buffer, uint bufferLength, IntPtr userData)
        {
            uint bytesRead = 0;
            fileRead(fileHandle, buffer, bufferLength, &bytesRead, userData).CheckResult();
            return bytesRead;
        }

        public uint FileRead(byte* buffer, uint bufferLength, IntPtr userData)
        {
            return FileRead(FileHandle, buffer, bufferLength, userData);
        }

        public uint FileRead(IntPtr fileHandle, Span<byte> buffer, IntPtr userData)
        {
            fixed (byte* bufferPtr = buffer)
            {
                return FileRead(fileHandle, bufferPtr, (uint)buffer.Length, userData);
            }
        }

        public uint FileRead(Span<byte> buffer, IntPtr userData)
        {
            return FileRead(FileHandle, buffer, userData);
        }

        public void FileSeek(IntPtr fileHandle, uint pos, IntPtr userData)
        {
            fileSeek(fileHandle, pos, userData).CheckResult();
        }

        public void FileSeek(uint pos, IntPtr userData)
        {
            FileSeek(FileHandle, pos, userData);
        }

        public void MetaData(CodecState* state, TagType tagType, byte* name, byte* data, uint dataLen, TagDataType dataType, FmodBool unique)
        {
            metaData(state, tagType, name, data, dataLen, dataType, unique.value).CheckResult();
        }

        public void MetaData(CodecState* state, TagType tagType, string name, Span<byte> data, TagDataType dataType, FmodBool unique)
        {
            fixed (byte* dataPtr = data, pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                metaData(state, tagType, pName, dataPtr, (uint)data.Length, dataType, unique.value);
            }
        }
    }
}
