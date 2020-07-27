#pragma warning disable CS0660, CS0661
using System;
using System.Diagnostics.CodeAnalysis;

namespace FmodAudio.Base
{
    [WrapperType]
    public struct SystemHandle : IHandleType<IntPtr>, IEquatable<SystemHandle>
    {
        public IntPtr Handle { get; }

        internal SystemHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(SystemHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is SystemHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(SystemHandle left, SystemHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(SystemHandle left, SystemHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct SoundHandle : IHandleType<IntPtr>, IEquatable<SoundHandle>
    {
        public IntPtr Handle { get; }

        internal SoundHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(SoundHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is SoundHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(SoundHandle left, SoundHandle right) => left.Handle == right.Handle;

        public static bool operator !=(SoundHandle left, SoundHandle right) => left.Handle != right.Handle;
    }

    [WrapperType]
    public struct SoundGroupHandle : IHandleType<IntPtr>, IEquatable<SoundGroupHandle>
    {
        public IntPtr Handle { get; }

        internal SoundGroupHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(SoundGroupHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is SoundGroupHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(SoundGroupHandle left, SoundGroupHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(SoundGroupHandle left, SoundGroupHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct ChannelHandle : IHandleType<IntPtr>, IEquatable<ChannelHandle>
    {
        public IntPtr Handle { get; }

        internal ChannelHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(ChannelHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is ChannelHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);


        public static implicit operator ChannelControlHandle(ChannelHandle handle)
        {
            return new ChannelControlHandle(handle.Handle);
        }

        public static bool operator ==(ChannelHandle left, ChannelHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(ChannelHandle left, ChannelHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct ChannelGroupHandle : IHandleType<IntPtr>
    {
        public IntPtr Handle { get; }

        internal ChannelGroupHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public static implicit operator ChannelControlHandle(ChannelGroupHandle handle)
        {
            return new ChannelControlHandle(handle.Handle);
        }

        public static bool operator ==(ChannelGroupHandle left, ChannelGroupHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(ChannelGroupHandle left, ChannelGroupHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct ChannelControlHandle : IHandleType<IntPtr>
    {
        public IntPtr Handle { get; }

        internal ChannelControlHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public static explicit operator ChannelHandle(ChannelControlHandle handle)
        {
            return new ChannelHandle(handle.Handle);
        }

        public static explicit operator ChannelGroupHandle(ChannelControlHandle handle)
        {
            return new ChannelGroupHandle(handle.Handle);
        }

        public static bool operator ==(ChannelControlHandle left, ChannelControlHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(ChannelControlHandle left, ChannelControlHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct GeometryHandle : IHandleType<IntPtr>
    {
        public IntPtr Handle { get; }

        internal GeometryHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public static bool operator ==(GeometryHandle left, GeometryHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(GeometryHandle left, GeometryHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct Reverb3DHandle : IHandleType<IntPtr>
    {
        public IntPtr Handle { get; }

        internal Reverb3DHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public static bool operator ==(Reverb3DHandle left, Reverb3DHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Reverb3DHandle left, Reverb3DHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct PluginHandle : IHandleType<uint>
    {
        public uint Handle { get; }

        internal PluginHandle(uint handle)
        {
            Handle = handle;
        }

        public static bool operator ==(PluginHandle left, PluginHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(PluginHandle left, PluginHandle right)
        {
            return left.Handle != right.Handle;
        }
    }
}
