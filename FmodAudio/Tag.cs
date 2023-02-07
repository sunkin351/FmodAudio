using System.Runtime.InteropServices;

using FmodAudio.Base;

namespace FmodAudio;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct Tag
{
    public readonly TagType Type;        /* [r] The type of this tag. */
    public readonly TagDataType DataType;/* [r] The type of data that this tag contains */
    private readonly byte* _name;       /* [r] The name of this tag i.e. "TITLE", "ARTIST" etc. */
    public readonly void* Data;         /* [r] Pointer to the tag data - its format is determined by the datatype member */
    public readonly uint DataLen;        /* [r] Length of the data contained in this tag */
    public readonly FmodBool Updated;       /* [r] True if this tag has been updated since last being accessed with Sound::getTag */

    public string Name
    {
        get
        {
            return FmodHelpers.PtrToStringUnknownSize(_name);
        }
    }
}
