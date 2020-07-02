using System;

namespace FmodAudio.Output
{
    public enum OutputMethod : int
    { 
        Mix_Direct,
        Polling,
        Mix_Buffered
    }

    public struct OutputDescriptionStruct
    {
        public FmodVersion ApiVersion;
        public IntPtr Name;
        public FmodVersion Version;
        public OutputMethod Polling;
        public IntPtr GetDriverCount;
        public IntPtr GetDriverInfo;
        public IntPtr Init;
        public IntPtr Start;
        public IntPtr Stop;
        public IntPtr Close;
        public IntPtr Update;
        public IntPtr GetHandle;
        public IntPtr GetPosition;
        public IntPtr Lock;
        public IntPtr Unlock;
        public IntPtr Mixer;
        public IntPtr Object3DGetInfo;
        public IntPtr Object3DAlloc;
        public IntPtr Object3DFree;
        public IntPtr Object3DUpdate;
        public IntPtr OpenPort;
        public IntPtr ClosePort;
        public IntPtr DeviceListChanged;
    }
}
