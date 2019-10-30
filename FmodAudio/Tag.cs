using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Tag
    {
        public readonly TagType Type;        /* [r] The type of this tag. */
        public readonly TagDataType DataType;/* [r] The type of data that this tag contains */
        private readonly IntPtr _name;       /* [r] The name of this tag i.e. "TITLE", "ARTIST" etc. */
        public readonly IntPtr Data;         /* [r] Pointer to the tag data - its format is determined by the datatype member */
        public readonly uint DataLen;        /* [r] Length of the data contained in this tag */
        private readonly int _updated;       /* [r] True if this tag has been updated since last being accessed with Sound::getTag */

        public string Name { get { return Marshal.PtrToStringAnsi(_name); } }

        public bool Updated { get => _updated != 0; }
    }
}
