using System.Runtime.InteropServices;

namespace FmodAudio;

[StructLayout(LayoutKind.Sequential)]
public struct PluginList
{
    public PluginType Type;
    public nint Description;
}
