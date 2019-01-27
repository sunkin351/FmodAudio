using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio.Dsp
{
    public delegate IntPtr DspAllocFunc(uint size, MemoryType type, IntPtr sourceStr);
    public delegate IntPtr DspReallocFunc(IntPtr ptr, uint size, MemoryType type, IntPtr sourceStr);
    public delegate void DspFreeFunc(IntPtr ptr, MemoryType type, IntPtr sourceStr);
    public delegate void DspLogFunc(DebugFlags flags, IntPtr fileString, int line, IntPtr functionString, IntPtr formatString);
    public delegate Result DspGetSampleRateFunc();
}
