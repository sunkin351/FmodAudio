using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;

namespace FmodAudio
{
    internal static class Helpers
    {
        static readonly SortedDictionary<Result, string> ErrorMessageLookup = new SortedDictionary<Result, string>()
        {
            {Result.Err_Already_Locked, "The specified resource is already locked." },
            {Result.Err_BadCommand, "Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound.lock() on a streaming sound)." },
            {Result.Err_Channel_Alloc, "Error trying to allocate a channel." },
            {Result.Err_Channel_Stolen, "The specified channel has been reused to play another sound." },
            {Result.Err_DMA, "DMA Failure. See debug output for more information." },
            {Result.Err_DSP_Connection, "DSP connection error. Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts." },
            {Result.Err_DSP_DontProcess, "DSP return code from a DSP process query callback. Tells mixer not to call the process callback and therefore not consume CPU. Use this to optimize the DSP graph." },
            {Result.Err_DSP_Format, "DSP Format error. A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map." },
            {Result.Err_DSP_InUse, "DSP is already in the mixer's DSP network. It must be removed before being reinserted or released." },
            {Result.Err_DSP_NotFound, "DSP connection error. Couldn't find the DSP unit specified." },
            {Result.Err_DSP_Reserved, "DSP operation error. Cannot perform operation on this DSP as it is reserved by the system." },
            {Result.Err_DSP_Silence, "DSP return code from a DSP process query callback. Tells mixer silence would be produced from read, so go idle and not consume CPU. Use this to optimize the DSP graph." },
            {Result.Err_DSP_Type, "DSP operation cannot be performed on a DSP of this type." },
            {Result.Err_File_Bad, "Error loading file." },
            {Result.Err_File_CouldNotSeek, "Couldn't perform seek operation. This is a limitation of the medium (ie netstreams) or the file format." },
            {Result.Err_File_DiskEjected, "Media was ejected while reading." },
            {Result.Err_File_EOF, "End of file unexpectedly reached while trying to read essential data (truncated?)." },
            {Result.Err_File_EndOfData, "End of current chunk reached while trying to read data." },
            {Result.Err_File_NotFound, "File not found." },
            {Result.Err_Format, "Unsupported file or audio format." },
            {Result.Err_Header_Mismatch, "There is a version mismatch between the FMOD header and either the FMOD Studio library or the FMOD Low Level library." },
            {Result.Err_HTTP, "A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere." },
            {Result.Err_HTTP_Access, "The specified resource requires authentication or is forbidden." },
            {Result.Err_HTTP_Proxy_Auth, "Proxy authentication is required to access the specified resource." },
            {Result.Err_HTTP_Server_Error, "A HTTP server error occurred." },
            {Result.Err_HTTP_Timeout, "The HTTP request timed out." },
            {Result.Err_Initialization, "FMOD was not initialized correctly to support this function." },
            {Result.Err_Initialized, "Cannot call this command after System.Init()." },
            {Result.Err_Internal, "An error occurred that wasn't supposed to.  Contact support." },
            {Result.Err_Invalid_Float, "Value passed in was a NaN, Inf or denormalized float." },
            {Result.Err_Invalid_Handle, "An invalid object handle was used." },
            {Result.Err_Invalid_Param, "An invalid parameter was passed to this function." },
            {Result.Err_Invalid_Position, "An invalid seek position was passed to this function." },
            {Result.Err_Invalid_Speaker, "An invalid speaker was passed to this function based on the current speaker mode." },
            {Result.Err_Invalid_String, "An invalid string was passed to this function." },
            {Result.Err_Invalid_SyncPoint, "The syncpoint did not come from this sound handle." },
            {Result.Err_Invalid_Thread, "Tried to call a function on a thread that is not supported." },
            {Result.Err_Invalid_Vector, "The vectors passed in are not unit length, or perpendicular." },
            {Result.Err_MaxAudible, "Reached maximum audible playback count for this sound's soundgroup." },
            {Result.Err_Memory, "Not enough memory or resources." },
            {Result.Err_Memory_CantPoint, "Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used." },
            {Result.Err_Needs3D, "Tried to call a command on a 2d sound when the command was meant for 3d sound." },
            {Result.Err_NeedsHardware, "Tried to use a feature that requires hardware support." },
            {Result.Err_Net_Connect, "Couldn't connect to the specified host." },
            {Result.Err_Net_Socket_Error, "A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere." },
            {Result.Err_Net_Url, "The specified URL couldn't be resolved." },
            {Result.Err_Net_Would_Block, "Operation on a non-blocking socket could not complete immediately." },
            {Result.Err_NotReady, "Operation could not be performed because specified sound/DSP connection is not ready." },
            {Result.Err_Output_Allocated, "Error initializing output device, but more specifically, the output device is already in use and cannot be reused." },
            {Result.Err_Output_CreateBuffer, "Error creating hardware sound buffer." },
            {Result.Err_Output_DriverCall, "A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted." },
            {Result.Err_Output_Format, "Soundcard does not support the specified format." },
            {Result.Err_Output_Init, "Error initializing output device." },
            {Result.Err_Output_NoDrivers, "The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails." },
            {Result.Err_Plugin, "An unspecified error has been returned from a plugin." },
            {Result.Err_Plugin_Missing, "A requested output, dsp unit type or codec was not available." },
            {Result.Err_Plugin_Resource, "A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback)" },
            {Result.Err_Plugin_Version, "A plugin was built with an unsupported SDK version." },
            {Result.Err_Record, "An error occurred trying to initialize the recording device." },
            {Result.Err_Record_Disconnected, "The specified recording driver has been disconnected." },
            {Result.Err_Reverb_ChannelGroup, "Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection." },
            {Result.Err_Reverb_Instance, "Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist." },
            {Result.Err_SubSounds, "The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have. The operation may also not be able to be performed on a parent sound." },
            {Result.Err_SubSound_Allocated, "This subsound is already being used by another sound, you cannot have more than one parent to a sound. Null out the other parent's entry first." },
            {Result.Err_SubSound_CantMove, "Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file." },
            {Result.Err_TagNotFound, "The specified tag could not be found or there are no tags." },
            {Result.Err_TooManyChannels, "The sound created exceeds the allowable input channel count. This can be increased using the 'maxinputchannels' parameter in System.SetSoftwareFormat()." },
            {Result.Err_TooManySamples, "The length provided exceeds the allowable limit." },
            {Result.Err_Truncated, "The retrieved string is too long to fit in the supplied buffer and has been truncated." },
            

        };

        public static string GetErrorMessage(Result res)
        {
            if (!ErrorMessageLookup.TryGetValue(res, out string message))
            {
                message = "Unknown Error";
            }
            return message;
        }
        
        public static void CheckResult(this Result result)
        {
            if (result == Result.Ok || result == Result.Err_DSP_DontProcess || (result == Result.Err_Truncated && FmodSystem.AllowStringTruncation))
            {
                return;
            }

            string message = GetErrorMessage(result);
            switch (result)
            {
                case Result.Err_Invalid_Handle:
                    throw new ObjectDisposedException(message, new FmodException(result));
                case Result.Err_File_NotFound:
                    throw new FileNotFoundException(message);
                case Result.Err_File_Bad:
                    throw new FileLoadException(message, new FmodException(result));
                case Result.Err_Unsupported:
                    throw new InvalidOperationException(message);
                case Result.Err_Invalid_Param:
                    throw new ArgumentException(message);
                default:
                    throw new FmodException(result, message);
            }
        }
        
        public static Memory.SaferPointer AllocateCustomRolloff(ReadOnlySpan<Vector3> rolloff)
        {
            if (rolloff.IsEmpty)
                return null;

            var ptr = Memory.Allocate(rolloff.Length * Unsafe.SizeOf<Vector3>());
            
            rolloff.CopyTo(ptr.AsSpan<Vector3>());

            return ptr;
        }

        public static unsafe string PtrToString(IntPtr buffer, int buflen)
        {
            return PtrToString((byte*)buffer, buflen);
        }

        public static unsafe string PtrToString(byte* buffer, int buflen)
        {
            return PtrToString(buffer, buflen, Encoding.UTF8);
        }

        public static unsafe string PtrToString(IntPtr buffer, int buflen, Encoding encoding)
        {
            return PtrToString((byte*)buffer, buflen, encoding);
        }

        public static unsafe string PtrToString(byte* buffer, int buflen, Encoding encoding)
        {
            return MemoryToString(new ReadOnlySpan<byte>(buffer, buflen), encoding);
        }

        public static unsafe string MemoryToString(ReadOnlySpan<byte> buffer)
        {
            return MemoryToString(buffer, Encoding.UTF8);
        }

        public static unsafe string MemoryToString(ReadOnlySpan<byte> buffer, Encoding encoding)
        {
            int size = buffer.IndexOf(byte.MinValue);

            if (size == 0)
                return string.Empty;
            
            if (size < buffer.Length)
            {
                buffer = buffer.Slice(0, size);
            }

            return encoding.GetString(buffer);
        }

        public static byte[] ToUTF8NullTerminated(ReadOnlySpan<char> str)
        {
            Encoding enc = Encoding.UTF8;

            int count = enc.GetByteCount(str);
            byte[] data = new byte[count + 1]; //Extra charactor acts as the null terminator

            enc.GetBytes(str, data.AsSpan(0, count));

            Debug.Assert(data[count] == byte.MinValue);

            return data;
        }

        public static int Abs(int x)
        {
            int mask = x >> 31;
            return (x + mask) ^ mask;
        }

        public static long Abs(long x)
        {
            long mask = x >> 63;
            return (x + mask) ^ mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ArrayClone<T>(this T[] arr)
        {
            if (arr is null)
                return null;

            return (T[])arr.Clone();
        }

        /// <summary>
        /// Converts 'value' into bytes using 'encoding' and stores them in 'pointer'.
        /// If the memory location of 'pointer' is not large enough, its reallocated and the new pointer is stored in 'pointer'.
        /// Returns whether 'pointer' was reallocated.
        /// </summary>
        /// <param name="value">The string value to convert</param>
        /// <param name="encoding">The encoding to use for conversion</param>
        /// <param name="pointer">Unmanaged Memory Block</param>
        /// <returns>if 'pointer' was reallocated</returns>
        public static unsafe bool StringToPointer(string value, Encoding encoding, ref Memory.SaferPointer pointer)
        {
            bool Reallocated = false;

            int count = encoding.GetByteCount(value);
            var localptr = pointer; //Local Variable optimization

            long allocSize = (localptr == null) ? 0 : localptr.AllocationSize;

            int tmp = count + 1;

            if (allocSize < tmp)
            {
                pointer = localptr = Memory.Allocate(tmp);
                Reallocated = true;
            }
            
            var buf = localptr.AsSpan<byte>();
            encoding.GetBytes(value.AsSpan(), buf);
            buf[count] = 0;

            return Reallocated;
        }

        public static unsafe string PtrToStringUnknownSize(IntPtr buffer)
        {
            int count = 0;
            byte* ptr = (byte*)buffer;

            while(ptr[count] != 0)
            {
                count += 1;
            }

            if (count > 0)
            {
                return Encoding.UTF8.GetString(ptr, count);
            }

            return string.Empty;
        }

        [Conditional("DEBUG")]
        public static void VerifySizeof<T>() where T: unmanaged
        {
            int tmp = Unsafe.SizeOf<T>();

            int tmp2 = Marshal.SizeOf<T>();

            Debug.Assert(tmp == tmp2, $"Sizeof Mismatch: sizeof() operator returns {tmp} while Marshal.Sizeof() returns {tmp2} for type {typeof(T).FullName}");
        }

        public static FileOpenCallbackImpl Wrap(this FileOpenCallback callback)
        {
            return delegate (IntPtr Filename, ref uint filesize, ref IntPtr handle, IntPtr userdata)
            {
                return callback(PtrToStringUnknownSize(Filename), out filesize, out handle, userdata);
            };
        }
    }
}
