using System;
using System.Runtime.InteropServices;

namespace FmodAudio;

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

/// <summary>
/// Callback to allocate a block of memory.
/// </summary>
/// <param name="size">
/// Size of the memory block to be allocated and returned. Units: <see cref="byte"/>
/// </param>
/// <param name="type">
/// Type of memory allocation.
/// </param>
/// <param name="sourcestr">
/// String with the FMOD source code filename and line number in it. Only valid in logging versions of FMOD. (UTF-8 string)
/// </param>
/// <returns>
/// An aligned pointer, of 16 byte alignment is recommended for performance reasons.
/// </returns>
[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public unsafe delegate void* MemoryAllocCallback(uint size, MemoryType type, byte* sourcestr);

/// <summary>
/// Callback to re-allocate a block of memory to a different size.
/// </summary>
/// <param name="ptr">
/// Block of memory to be resized. If this is null, then a new block of memory is allocated and no memory is freed.
/// </param>
/// <param name="size">
/// Size of the memory to be reallocated. Units: <see cref="byte"/>
/// </param>
/// <param name="type">
/// Memory allocation type
/// </param>
/// <param name="sourcestr">
/// String with the FMOD source code filename and line number in it. Only valid in logging versions of FMOD. (UTF-8 string)
/// </param>
/// <returns>
/// An aligned pointer, of 16 byte alignment is recommended for performance reasons.
/// </returns>
/// <remarks>
/// When allocating new memory, the contents of the old memory block must be preserved.
/// </remarks>
[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public unsafe delegate void* MemoryReallocCallback(void* ptr, uint size, MemoryType type, byte* sourcestr);

/// <summary>
/// Callback to free a block of memory.
/// </summary>
/// <param name="ptr">
/// Pre-existing block of memory to be freed.
/// </param>
/// <param name="type">
/// Type of memory to be freed.
/// </param>
/// <param name="sourcestr">
/// String with the FMOD source code filename and line number in it. Only valid in logging versions of FMOD. (UTF-8 string)
/// </param>
[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public unsafe delegate void MemoryFreeCallback(void* ptr, MemoryType type, byte* sourcestr);

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public delegate float _3DRolloffCallback(nint channelraw, float distance);
