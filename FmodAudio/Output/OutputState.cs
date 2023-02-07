namespace FmodAudio.Output;

public unsafe struct OutputState
{
    public void* PluginData;
    public readonly delegate* unmanaged<OutputState*, void*, uint, Result> ReadFromMixer;
    public readonly delegate* unmanaged<uint, uint, byte*, int, void*> Alloc;
    public readonly delegate* unmanaged<void*, byte*, int, Result> Free;
    public readonly delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, Result> Log;
    public readonly delegate* unmanaged<OutputState*, int, void*, uint, Result> CopyPort;
    public readonly delegate* unmanaged<OutputState*, Result> RequestReset;
}
