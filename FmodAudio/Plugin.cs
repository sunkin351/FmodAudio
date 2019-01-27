using System;
using System.Runtime.InteropServices;
namespace FmodAudio
{
    /// <summary>
    /// Type safety for the plugin handle
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Plugin : IEquatable<Plugin>
    {
        internal readonly uint Handle;

        internal Plugin(uint handle)
        {
            Handle = handle;
        }

        public bool Equals(Plugin other)
        {
            return Handle == other.Handle;
        }

        public override bool Equals(object obj)
        {
            return obj is Plugin plug && this.Equals(plug);
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public static bool operator==(Plugin l, Plugin r)
        {
            return l.Handle == r.Handle;
        }

        public static bool operator!=(Plugin l, Plugin r)
        {
            return l.Handle != r.Handle;
        }
    }
}
