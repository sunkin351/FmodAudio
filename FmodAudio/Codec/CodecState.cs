#pragma warning disable CA1815, CS0649

using System;

using FmodAudio.Base;

namespace FmodAudio.Codec;

public unsafe struct CodecState
{
    public nint PluginData;
    public readonly CodecWaveFormat* WaveFormat;
    public readonly CodecStateFunctions* Functions;
    public int SubsoundCount;


    public static void MetaData(CodecState* state, TagType tagType, byte* name, byte* data, uint dataLen, TagDataType dataType, FmodBool unique)
    {
        state->Functions->Metadata(state, tagType, name, data, dataLen, dataType, unique.value).CheckResult();
    }

    public static void MetaData(CodecState* state, TagType tagType, string name, Span<byte> data, TagDataType dataType, FmodBool unique)
    {
        fixed (byte* dataPtr = data, pName = FmodHelpers.ToUTF8NullTerminated(name))
        {
            MetaData(state, tagType, pName, dataPtr, (uint)data.Length, dataType, unique);
        }
    }

    public static void* Allocate(CodecState* state, uint size, uint alignment)
    {
        return state->Functions->Alloc(size, alignment, null, 0);
    }

    public static void Free(CodecState* state, void* ptr)
    {
        state->Functions->Free(ptr, null, 0);
    }

    public static uint FileRead(CodecState* state, byte* buffer, uint bufferLength)
    {
        uint bytesRead = 0;
        state->Functions->FileRead(state, buffer, bufferLength, &bytesRead).CheckResult();
        return bytesRead;
    }

    public static uint FileRead(CodecState* state, Span<byte> buffer)
    {
        fixed (byte* bufferPtr = buffer)
        {
            return FileRead(state, bufferPtr, (uint)buffer.Length);
        }
    }

    public static void FileSeek(CodecState* state, uint pos, CodecSeekMethod seekMethod)
    {
        state->Functions->FileSeek(state, pos, seekMethod).CheckResult();
    }

    public static uint FileTell(CodecState* state)
    {
        uint pos = 0;
        state->Functions->FileTell(state, &pos).CheckResult();
        return pos;
    }

    public static uint FileSize(CodecState* state)
    {
        uint size = 0;
        state->Functions->FileSize(state, &size).CheckResult();
        return size;
    }
}
