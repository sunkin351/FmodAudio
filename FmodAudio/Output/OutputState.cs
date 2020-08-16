namespace FmodAudio.Output
{
    public unsafe struct OutputState
    {
        public void* PluginData;
        public readonly delegate* stdcall<OutputState*, void*, uint, Result> ReadFromMixer;
        public readonly delegate* stdcall<uint, uint, byte*, int, void*> Alloc;
        public readonly delegate* stdcall<void*, byte*, int, Result> Free;
        public readonly delegate* stdcall<DebugFlags, byte*, int, byte*, byte*, Result> Log;
        public readonly delegate* stdcall<OutputState*, int, void*, uint, Result> CopyPort;
        public readonly delegate* stdcall<OutputState*, Result> RequestReset;
    }
}
