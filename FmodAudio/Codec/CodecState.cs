#pragma warning disable CA1815

using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Codec
{
    using FmodAudio.Interop;

    public unsafe struct CodecState
    {
        public int SubsoundCount;
        public CodecWaveFormat* WaveFormat;
        public IntPtr PluginData;

        public IntPtr FileHandle;
        public uint FileSize;

        private readonly FileReadCallback fileRead;
        private readonly FileSeekCallback fileSeek;
        private readonly CodecMetadataCallback metaData;

        public int WaveFormatVersion;

        public uint FileRead(IntPtr fileHandle, byte* buffer, uint bufferLength, IntPtr userData)
        {
            uint bytesRead = 0;
            fileRead.Invoke(fileHandle, buffer, bufferLength, &bytesRead, userData).CheckResult();
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
            fileSeek.Invoke(fileHandle, pos, userData).CheckResult();
        }

        public void FileSeek(uint pos, IntPtr userData)
        {
            FileSeek(FileHandle, pos, userData);
        }

        public void MetaData(CodecState* state, TagType tagType, string name, byte* data, uint dataLen, TagDataType dataType, bool unique)
        {
            metaData.Invoke(state, tagType, name, data, dataLen, dataType, unique).CheckResult();
        }

        public void MetaData(CodecState* state, TagType tagType, string name, Span<byte> data, TagDataType dataType, bool unique)
        {
            fixed (byte* dataPtr = data)
            {
                MetaData(state, tagType, name, dataPtr, (uint)data.Length, dataType, unique);
            }
        }

        private struct FileReadCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, IntPtr fileHandle, byte* buffer, uint bufferlength, uint* bytesRead, IntPtr userData);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(IntPtr fileHandle, byte* buffer, uint bufferlength, uint* bytesRead, IntPtr userData)
            {
                return InvokeImpl(FuncPtr, fileHandle, buffer, bufferlength, bytesRead, userData);
            }

            public bool IsDefault => FuncPtr == default;
        }

        private struct FileSeekCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, IntPtr fileHandle, uint pos, IntPtr userData);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(IntPtr fileHandle, uint pos, IntPtr userData)
            {
                return InvokeImpl(FuncPtr, fileHandle, pos, userData);
            }

            public bool IsDefault => FuncPtr == default;
        }

        private struct CodecMetadataCallback
        {
            private delegate Result Delegate(IntPtr funcPtr, CodecState* state, TagType tagType, byte* name, byte* data, uint dataLen, TagDataType dataType, int unique);
            private static readonly Delegate InvokeImpl = DelegateILGeneration.GenerateCalli<Delegate>();

            private readonly IntPtr FuncPtr;

            public Result Invoke(CodecState* state, TagType tagType, string name, byte* data, uint dataLen, TagDataType dataType, bool unique)
            {
                fixed (byte* namePtr = FmodHelpers.ToUTF8NullTerminated(name))
                {
                    return InvokeImpl(FuncPtr, state, tagType, namePtr, data, dataLen, dataType, unique ? 1 : 0);
                }
            }

            public bool IsDefault => FuncPtr == default;
        }
    }
}
