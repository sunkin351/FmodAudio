#pragma warning disable CS0660, CS0661
using System;

namespace FmodAudio.Base;

public readonly record struct SystemHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal SystemHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator SystemHandle(nint rawHandle)
    {
        return new SystemHandle(rawHandle);
    }
}

public readonly record struct SoundHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal SoundHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator SoundHandle(nint handle)
    {
        return new SoundHandle(handle);
    }
}

public readonly record struct SoundGroupHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal SoundGroupHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator SoundGroupHandle(nint rawHandle)
    {
        return new SoundGroupHandle(rawHandle);
    }
}

public readonly record struct ChannelHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal ChannelHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator ChannelHandle(nint rawHandle)
    {
        return new ChannelHandle(rawHandle);
    }
}

public readonly record struct ChannelGroupHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal ChannelGroupHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator ChannelGroupHandle(nint rawHandle)
    {
        return new ChannelGroupHandle(rawHandle);
    }
}

public readonly record struct DspHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal DspHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator DspHandle(nint rawHandle)
    {
        return new DspHandle(rawHandle);
    }
}

public readonly record struct DspConnectionHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal DspConnectionHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator DspConnectionHandle(nint rawHandle)
    {
        return new DspConnectionHandle(rawHandle);
    }
}

public readonly record struct GeometryHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal GeometryHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator GeometryHandle(nint rawHandle)
    {
        return new GeometryHandle(rawHandle);
    }
}

public readonly record struct Reverb3DHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal Reverb3DHandle(nint handle)
    {
        Handle = handle;
    }

    public static explicit operator Reverb3DHandle(nint rawHandle)
    {
        return new Reverb3DHandle(rawHandle);
    }
}

public readonly record struct PluginHandle : IHandleType<uint>
{
    public uint Handle { get; }

    internal PluginHandle(uint handle)
    {
        Handle = handle;
    }
}

public readonly record struct SyncPointHandle : IHandleType<nint>
{
    public nint Handle { get; }

    internal SyncPointHandle(nint handle)
    {
        Handle = handle;
    }
}
