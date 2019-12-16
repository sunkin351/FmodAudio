using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct FmodVersion : IComparable<FmodVersion>, IEquatable<FmodVersion>
    {
        private readonly uint EncodedVersion;

        public int Major { get => (int)(EncodedVersion >> 16); }
        public int Minor { get => (int)((EncodedVersion >> 8) & 0xff); }
        public int Patch { get => (int)(EncodedVersion & 0xff); }

        internal FmodVersion(uint encodedVersion)
        {
            EncodedVersion = encodedVersion;
        }

        public FmodVersion(int Major, int Minor, int Patch)
        {
            uint tmp = (uint)Major << 16;
            tmp |= ((uint)Minor & 0xff) << 8;
            tmp |= (uint)Patch & 0xff;
            EncodedVersion = tmp;
        }

        public int CompareTo(FmodVersion other)
        {
            return EncodedVersion.CompareTo(other.EncodedVersion);
        }

        public bool Equals(FmodVersion other)
        {
            return EncodedVersion == other.EncodedVersion;
        }

        public override bool Equals(object obj)
        {
            return obj is FmodVersion tmp && this.Equals(tmp);
        }

        public override int GetHashCode()
        {
            return (int)EncodedVersion;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public static bool operator <(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator >(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator <=(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        public static bool operator >=(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator ==(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        public static bool operator !=(FmodVersion lhs, FmodVersion rhs)
        {
            return lhs.CompareTo(rhs) != 0;
        }
    }
}
