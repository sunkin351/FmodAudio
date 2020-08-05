#pragma warning disable CS0660, CS0661
using System;

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

        public static explicit operator SystemHandle(IntPtr rawHandle)
        {
            return new SystemHandle(rawHandle);
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

        public static explicit operator SoundHandle(IntPtr handle)
        {
            return new SoundHandle(handle);
        }
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

        public static explicit operator SoundGroupHandle(IntPtr rawHandle)
        {
            return new SoundGroupHandle(rawHandle);
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

        public static bool operator ==(ChannelHandle left, ChannelHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(ChannelHandle left, ChannelHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator ChannelHandle(IntPtr rawHandle)
        {
            return new ChannelHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct ChannelGroupHandle : IHandleType<IntPtr>, IEquatable<ChannelGroupHandle>
    {
        public IntPtr Handle { get; }

        internal ChannelGroupHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(ChannelGroupHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is ChannelGroupHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(ChannelGroupHandle left, ChannelGroupHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(ChannelGroupHandle left, ChannelGroupHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator ChannelGroupHandle(IntPtr rawHandle)
        {
            return new ChannelGroupHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct DspHandle : IHandleType<IntPtr>, IEquatable<DspHandle>
    {
        public IntPtr Handle { get; }

        internal DspHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(DspHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is DspHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(DspHandle left, DspHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(DspHandle left, DspHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator DspHandle(IntPtr rawHandle)
        {
            return new DspHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct DspConnectionHandle : IHandleType<IntPtr>, IEquatable<DspConnectionHandle>
    {
        public IntPtr Handle { get; }

        internal DspConnectionHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(DspConnectionHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is DspConnectionHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(DspConnectionHandle left, DspConnectionHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(DspConnectionHandle left, DspConnectionHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator DspConnectionHandle(IntPtr rawHandle)
        {
            return new DspConnectionHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct GeometryHandle : IHandleType<IntPtr>, IEquatable<GeometryHandle>
    {
        public IntPtr Handle { get; }

        internal GeometryHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(GeometryHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is GeometryHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(GeometryHandle left, GeometryHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(GeometryHandle left, GeometryHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator GeometryHandle(IntPtr rawHandle)
        {
            return new GeometryHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct Reverb3DHandle : IHandleType<IntPtr>, IEquatable<Reverb3DHandle>
    {
        public IntPtr Handle { get; }

        internal Reverb3DHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(Reverb3DHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is Reverb3DHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(Reverb3DHandle left, Reverb3DHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Reverb3DHandle left, Reverb3DHandle right)
        {
            return left.Handle != right.Handle;
        }

        public static explicit operator Reverb3DHandle(IntPtr rawHandle)
        {
            return new Reverb3DHandle(rawHandle);
        }
    }

    [WrapperType]
    public struct PluginHandle : IHandleType<uint>, IEquatable<PluginHandle>
    {
        public uint Handle { get; }

        internal PluginHandle(uint handle)
        {
            Handle = handle;
        }

        public bool Equals(PluginHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is PluginHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(PluginHandle left, PluginHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(PluginHandle left, PluginHandle right)
        {
            return left.Handle != right.Handle;
        }
    }

    [WrapperType]
    public struct SyncPointHandle : IHandleType<IntPtr>, IEquatable<SyncPointHandle>
    {
        public IntPtr Handle { get; }

        internal SyncPointHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public bool Equals(SyncPointHandle other) => Handle == other.Handle;

        public override bool Equals(object obj) => obj is SyncPointHandle other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handle);

        public static bool operator ==(SyncPointHandle left, SyncPointHandle right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(SyncPointHandle left, SyncPointHandle right)
        {
            return left.Handle != right.Handle;
        }
    }
}
