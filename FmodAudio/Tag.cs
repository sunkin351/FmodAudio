using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Tag
    {
        public TagType Type;         /* [r] The type of this tag. */
        public TagDataType DataType;     /* [r] The type of data that this tag contains */
        private IntPtr Name_Internal;/* [r] The name of this tag i.e. "TITLE", "ARTIST" etc. */
        public IntPtr Data;         /* [r] Pointer to the tag data - its format is determined by the datatype member */
        public uint DataLen;      /* [r] Length of the data contained in this tag */
        public bool Updated;      /* [r] True if this tag has been updated since last being accessed with Sound::getTag */

        public string Name { get { return Marshal.PtrToStringAnsi(Name_Internal); } }
    }
}
