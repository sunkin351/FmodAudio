using System;

namespace FmodAudio.Output
{
    public enum OutputMethod : int
    { 
        Mix_Direct = 0,
        Polling = 1,
        Mix_Buffered = 2
    }

    public unsafe struct OutputDescriptionStruct
    {
        public FmodVersion ApiVersion;
        public byte* Name;
        public FmodVersion Version;
        public OutputMethod Method;
        public delegate* stdcall<OutputState*, int*, Result> GetDriverCount;
        public delegate* stdcall<OutputState*, int, byte*, int, Guid*, int*, SpeakerMode*, int*, Result> GetDriverInfo;
        public delegate* stdcall<OutputState*, int, InitFlags, int*, SpeakerMode*, int*, SoundFormat*, int, int, void*, Result> Init;
        public delegate* stdcall<OutputState*, Result> Start;
        public delegate* stdcall<OutputState*, Result> Stop;
        public delegate* stdcall<OutputState*, Result> Close;
        public delegate* stdcall<OutputState*, Result> Update;
        public delegate* stdcall<OutputState*, IntPtr*, Result> GetHandle;
        public delegate* stdcall<OutputState*, uint*, Result> GetPosition;
        public delegate* stdcall<OutputState*, uint, uint, IntPtr*, IntPtr*, uint*, uint*, Result> Lock;
        public delegate* stdcall<OutputState*, IntPtr, IntPtr, uint, uint, Result> Unlock;
        public delegate* stdcall<OutputState*, Result> Mixer;
        public delegate* stdcall<OutputState*, int*, Result> Object3DGetInfo;
        public delegate* stdcall<OutputState*, IntPtr*, Result> Object3DAlloc;
        public delegate* stdcall<OutputState*, IntPtr, Result> Object3DFree;
        public delegate* stdcall<OutputState*, IntPtr, Object3DInfo*, Result> Object3DUpdate;
        public delegate* stdcall<OutputState*, uint, ulong, int*, int*, int*, SoundFormat, Result> OpenPort;
        public delegate* stdcall<OutputState*, int, Result> ClosePort;
        public delegate* stdcall<OutputState*, Result> DeviceListChanged;
    }
}
