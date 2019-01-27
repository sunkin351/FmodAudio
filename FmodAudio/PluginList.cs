using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginList
    {
        PluginType Type;
        IntPtr Description;
    }
}
