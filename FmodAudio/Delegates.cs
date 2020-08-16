using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate Result DebugCallback(DebugFlags flags, string file, int line, string func, string message);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SystemCallback(IntPtr system, SystemCallbackType type, IntPtr commanddata1, IntPtr commanddata2, IntPtr userdata);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void ChannelCallback(IntPtr channel, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SoundNonBlockCallback(IntPtr soundraw, Result Result);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SoundPCMReadCallback(IntPtr soundraw, IntPtr data, uint datalen);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result SoundPCMSetPosCallback(IntPtr soundraw, int subsound, uint position, TimeUnit postype);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate Result FileOpenCallback(IntPtr name, uint* filesize, IntPtr* handle, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result FileCloseCallback(IntPtr handle, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate Result FileReadCallback(IntPtr handle, IntPtr buffer, uint sizebytes, uint* bytesread, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate Result FileSeekCallback(IntPtr handle, uint pos, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate Result FileAsyncReadCallback(AsyncReadInfo* info, IntPtr userdata);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate Result FileAsyncCancelCallback(AsyncReadInfo* info, IntPtr userdata);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr MemoryAllocCallback(uint size, MemoryType type, IntPtr sourcestr);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr MemoryReallocCallback(IntPtr ptr, uint size, MemoryType type, IntPtr sourcestr);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void MemoryFreeCallback(IntPtr ptr, MemoryType type, IntPtr sourcestr);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate float _3DRolloffCallback(IntPtr channelraw, float distance);
}
