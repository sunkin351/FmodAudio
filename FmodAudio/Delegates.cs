using System;

namespace FmodAudio
{   
    public delegate Result DebugCallback(DebugFlags flags, string file, int line, string func, string message);

    public delegate Result SystemCallback(FmodSystem system, SystemCallbackType type, IntPtr commanddata1, IntPtr commanddata2, IntPtr userdata);

    public delegate void ChannelCallback(ChannelControl channel, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2);

    public delegate Result SoundNonBlockCallback(IntPtr soundraw, Result Result);
    public delegate Result SoundPCMReadCallback(IntPtr soundraw, IntPtr data, uint datalen);
    public delegate Result SoundPCMSetPosCallback(IntPtr soundraw, int subsound, uint position, TimeUnit postype);

    public delegate Result FileOpenCallbackImpl(IntPtr name, ref uint filesize, ref IntPtr handle, IntPtr userdata);
    public delegate Result FileOpenCallback(string Filename, out uint fileSize, out IntPtr handle, IntPtr userdata);

    public delegate Result FileCloseCallback(IntPtr handle, IntPtr userdata);
    public delegate Result FileReadCallback(IntPtr handle, IntPtr buffer, uint sizebytes, out uint bytesread, IntPtr userdata);
    public delegate Result FileSeekCallback(IntPtr handle, uint pos, IntPtr userdata);
    public delegate Result FileAsyncReadCallback(IntPtr info, IntPtr userdata);
    public delegate Result FileAsyncCancelCallback(IntPtr info, IntPtr userdata);

    public delegate IntPtr MemoryAllocCallback(uint size, MemoryType type, IntPtr sourcestr);
    public delegate IntPtr MemoryReallocCallback(IntPtr ptr, uint size, MemoryType type, IntPtr sourcestr);
    public delegate void MemoryFreeCallback(IntPtr ptr, MemoryType type, IntPtr sourcestr);

    public delegate float CB_3D_RolloffCallback(IntPtr channelraw, float distance);
}
