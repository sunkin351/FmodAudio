using System;

namespace FmodAudio.Output
{
    public enum OutputMethod : int
    { 
        Mix_Direct = 0,
        Mix_Buffered = 1
    }

    public unsafe struct OutputDescriptionStruct
    {
        public FmodVersion ApiVersion;
        public byte* Name;
        public FmodVersion Version;
        public OutputMethod Method;
        public delegate* unmanaged<OutputState*, int*, Result> GetDriverCount;
        public delegate* unmanaged<OutputState*, int, byte*, int, Guid*, int*, SpeakerMode*, int*, Result> GetDriverInfo;
        public delegate* unmanaged<OutputState*, int, InitFlags, int*, SpeakerMode*, int*, SoundFormat*, int, int, void*, Result> Init;
        public delegate* unmanaged<OutputState*, Result> Start;
        public delegate* unmanaged<OutputState*, Result> Stop;
        public delegate* unmanaged<OutputState*, Result> Close;
        public delegate* unmanaged<OutputState*, Result> Update;
        public delegate* unmanaged<OutputState*, IntPtr*, Result> GetHandle;
        public delegate* unmanaged<OutputState*, uint*, Result> GetPosition;
        public delegate* unmanaged<OutputState*, uint, uint, IntPtr*, IntPtr*, uint*, uint*, Result> Lock;
        public delegate* unmanaged<OutputState*, IntPtr, IntPtr, uint, uint, Result> Unlock;
        public delegate* unmanaged<OutputState*, Result> Mixer;
        public delegate* unmanaged<OutputState*, int*, Result> Object3DGetInfo;
        public delegate* unmanaged<OutputState*, IntPtr*, Result> Object3DAlloc;
        public delegate* unmanaged<OutputState*, IntPtr, Result> Object3DFree;
        public delegate* unmanaged<OutputState*, IntPtr, Object3DInfo*, Result> Object3DUpdate;
        public delegate* unmanaged<OutputState*, uint, ulong, int*, int*, int*, SoundFormat, Result> OpenPort;
        public delegate* unmanaged<OutputState*, int, Result> ClosePort;
        public delegate* unmanaged<OutputState*, Result> DeviceListChanged;
    }
}
