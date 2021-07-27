using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FmodAudio.Codec
{
    public unsafe readonly struct CodecStateFunctions
    {
        public readonly delegate* unmanaged<CodecState*, TagType, byte*, void*, uint, TagDataType, int, Result> Metadata;
        public readonly delegate* unmanaged<uint, uint, byte*, int, void*> Alloc;
        public readonly delegate* unmanaged<void*, byte*, int, void> Free;
        /// <summary>
        /// Made public for future language compatibility.
        /// Uses varargs calling convention, making it incompatible with C# versions less than or equal to 9
        /// and potentially many future versions as well
        /// </summary>
        public readonly nint Log;

        public readonly delegate* unmanaged<CodecState*, byte*, uint, uint*, Result> FileRead;
        public readonly delegate* unmanaged<CodecState*, uint, CodecSeekMethod, Result> FileSeek;
        public readonly delegate* unmanaged<CodecState*, uint*, Result> FileTell;
        public readonly delegate* unmanaged<CodecState*, uint*, Result> FileSize;
    }
}
