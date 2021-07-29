using System;
using System.IO;
using System.Runtime.InteropServices;

using FmodAudio.Base;

namespace FmodAudio
{
    public static unsafe class Fmod
    {
        #region Constants
        internal const string Deprecation_Message = "Newer versions of Fmod have deprecated this, and will eventually remove it entirely.";

        internal const int HeaderVersion = 0x00020201;

        public const int MaxAllowedSystemObjects = 8;

        internal const int MaxInteropNameStringLength = 200;

        public const int Dsp_GetParam_ValueStr_Length = 32;

        public const uint PluginSDKVersion = 110;

        public const uint CodecPluginVersion = 1;

        public const uint OutputPluginVersion = 5;

        #endregion

        public static FmodVersion BindingVersion => new FmodVersion(HeaderVersion);

        /// <summary>
        /// Affects string interop marshalling. If the buffer turns out to be too small, this will decide whether to throw an exception, or continue with the truncated string.
        /// </summary>
        public static bool AllowStringTruncation { get; set; }

        #region Pre-Native Setup

        private static string? location;

        // <summary>
        // Subscribe to this to log when fatal errors occur.String passed is the error message.
        // </summary>
        //public static event Action<string, string> FatalError;

        public static string DefaultLibraryName { get; } = SelectDefaultLibraryName();

        public static string SelectDefaultLibraryName(bool loggingEnabled = false)
        {
            string name;

            if (OperatingSystem.IsWindows())
            {
                name = "fmod.dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                name = "libfmod.so";
            }
            else if (OperatingSystem.IsMacOS())
            {
                name = "libfmod.dylib";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            if (loggingEnabled)
            {
                int idx = name.IndexOf('.');

                name = string.Concat(name.AsSpan(0, idx), "L", name.AsSpan(idx));
            }

            return name;
        }

        /// <summary>
        ///     This method allows you to specify the location to the FMOD native library file.
        ///     
        ///     Do take care to call this before calling any other FMOD api method, or else this will do nothing.
        /// </summary>
        /// <param name="path">
        ///     Can Specify either a path directly to the library file (allows you to specify a different file name),
        ///     or a path to the directory containing the library file (File name MUST be "fmod.dll", "libfmod.so", or "libfmod.dylib" depending on the platform).
        /// </param>
        public static void SetLibraryLocation(string path)
        {
            if (File.Exists(path))
            {
                location = path;
                return;
            }

            if (Directory.Exists(path))
            {
                var loc = Path.Combine(path, DefaultLibraryName);

                if (!File.Exists(loc))
                {
                    throw new ArgumentException($"Unable to find {DefaultLibraryName} in path '{path}'", nameof(path));
                }

                location = loc;
                return;
            }

            throw new ArgumentException("Invalid Path to Library", nameof(path));
        }

        #endregion

        #region Native loading and System instantiation

        private static readonly Lazy<FmodLibrary> nativeLibrary = new Lazy<FmodLibrary>(() =>
        {
            return new FmodLibrary(NativeLibrary.Load(location ?? DefaultLibraryName));
        });

        public static FmodLibrary Library => nativeLibrary.Value;

        internal static readonly object CreationSyncObject = new object();

        private static DebugCallback? DebugCallbackReference;

        [UnmanagedCallersOnly]
        private static Result DebugCallbackMarshaller(DebugFlags flags, byte* file, int line, byte* func, byte* message)
        {
            if (DebugCallbackReference is null)
                return Result.Ok;

            try
            {
                var fileString = FmodHelpers.PtrToStringUnknownSize(file);
                var funcString = FmodHelpers.PtrToStringUnknownSize(func);
                var messageString = FmodHelpers.PtrToStringUnknownSize(message);

                return DebugCallbackReference.Invoke(flags, fileString, line, funcString, messageString);
            }
            catch(FmodException fe)
            {
                return fe.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }
        }

        /// <summary>
        /// This overload assumes there is a single callback
        /// </summary>
        public static void InitializeDebug(DebugFlags flags, DebugMode mode, DebugCallback? callback)
        {
            if (callback is null)
            {
                InitializeDebug(flags, mode, null, null);
            }
            else
            {
                DebugCallbackReference = callback;

                delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, Result> callbackPointer = &DebugCallbackMarshaller;

                InitializeDebug(flags, mode, callbackPointer, null);
            }
        }

        public static void InitializeDebug(DebugFlags flags, DebugMode mode, string? filename = null)
        {
            InitializeDebug(flags, mode, null, filename);
        }

        /// <inheritdoc cref="FmodLibrary.Debug_Initialize(DebugFlags, DebugMode, delegate* unmanaged{DebugFlags, byte*, int, byte*, byte*, Result}, byte*)"/>
        public static void InitializeDebug(DebugFlags flags, DebugMode mode, delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, Result> callback, string? filename)
        {
            Library.Debug_Initialize(flags, mode, callback, filename).CheckResult();
        }

        public static unsafe bool DiskBusy
        {
            get
            {
                FmodBool value;
                Library.File_GetDiskBusy(&value).CheckResult();

                return value;
            }   
        }

        /// <inheritdoc cref="FmodLibrary.File_SetDiskBusy(FmodBool)"/>
        public static void SetDiskBusy(bool busy)
        {
            Library.File_SetDiskBusy(busy).CheckResult();
        }

        public unsafe static FmodSystem CreateSystem()
        {
            SystemHandle system;

            lock (CreationSyncObject)
            {
                Library.System_Create(&system).CheckResult();
            }

            return system;
        }

        public static class Memory
        {
            public static void Initialize(IntPtr poolmem,
                                          int poollen,
                                          delegate* unmanaged<uint, MemoryType, IntPtr, IntPtr> useralloc,
                                          delegate* unmanaged<IntPtr, uint, MemoryType, IntPtr, IntPtr> userrealloc,
                                          delegate* unmanaged<IntPtr, MemoryType, IntPtr, void> userfree,
                                          MemoryType memtypeflags)
            {
                Library.Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags).CheckResult();
            }

            public static void GetStats(out int currentlyAllocated, out int maxAllocated, bool blocking)
            {
                Library.Memory_GetStats(out currentlyAllocated, out maxAllocated, blocking).CheckResult();
            }
        }

        public static class Thread
        {
            public const long Affinity_GroupDefault = 0x4000000000000000;
            public const long Affinity_GroupA = 0x4000000000000001;
            public const long Affinity_GroupB = 0x4000000000000002;
            public const long Affinity_GroupC = 0x4000000000000003;

            //Default Affinity Values
            public const long Affinity_Mixer               = Affinity_GroupA;
            public const long Affinity_Feeder              = Affinity_GroupC;
            public const long Affinity_Stream              = Affinity_GroupC;
            public const long Affinity_File                = Affinity_GroupC;
            public const long Affinity_NonBlocking         = Affinity_GroupC;
            public const long Affinity_Record              = Affinity_GroupC;
            public const long Affinity_Geometry            = Affinity_GroupC;
            public const long Affinity_Profiler            = Affinity_GroupC;
            public const long Affinity_Studio_Update       = Affinity_GroupB;
            public const long Affinity_Studio_Load_Bank    = Affinity_GroupC;
            public const long Affinity_Studio_Load_Sample  = Affinity_GroupC;
            public const long Affinity_Studio_Convolution1 = Affinity_GroupC;
            public const long Affinity_Studio_Convolution2 = Affinity_GroupC;

            // Core mask, valid up to 1 << 61
            public const long Affinity_Core_All = 0;
            public const long Affinity_Core_0  = (1 << 0);
            public const long Affinity_Core_1  = (1 << 1);
            public const long Affinity_Core_2  = (1 << 2);
            public const long Affinity_Core_3  = (1 << 3);
            public const long Affinity_Core_4  = (1 << 4);
            public const long Affinity_Core_5  = (1 << 5);
            public const long Affinity_Core_6  = (1 << 6);
            public const long Affinity_Core_7  = (1 << 7);
            public const long Affinity_Core_8  = (1 << 8);
            public const long Affinity_Core_9  = (1 << 9);
            public const long Affinity_Core_10 = (1 << 10);
            public const long Affinity_Core_11 = (1 << 11);
            public const long Affinity_Core_12 = (1 << 12);
            public const long Affinity_Core_13 = (1 << 13);
            public const long Affinity_Core_14 = (1 << 14);
            public const long Affinity_Core_15 = (1 << 15);

            public const int Priority_Platform_Min = -32 * 1024;
            public const int Priority_Platform_Max =  32 * 1024;

            //Platform agnostic priorities, map internally to platform specific values
            public const int Priority_Default   = Priority_Platform_Min - 1;
            public const int Priority_Low       = Priority_Platform_Min - 2;
            public const int Priority_Medium    = Priority_Platform_Min - 3;
            public const int Priority_High      = Priority_Platform_Min - 4;
            public const int Priority_Very_High = Priority_Platform_Min - 5;
            public const int Priority_Extreme   = Priority_Platform_Min - 6;
            public const int Priority_Critical  = Priority_Platform_Min - 7;

            //Thread Defaults
            public const int Priority_Mixer                 = Priority_Extreme;
            public const int Priority_Feeder                = Priority_Critical;
            public const int Priority_Stream                = Priority_Very_High;
            public const int Priority_File                  = Priority_High;
            public const int Priority_NonBlocking           = Priority_High;
            public const int Priority_Record                = Priority_High;
            public const int Priority_Geometry              = Priority_Low;
            public const int Priority_Profiler              = Priority_Medium;
            public const int Priority_Studio_Update         = Priority_Medium;
            public const int Priority_Studio_Load_Bank      = Priority_Medium;
            public const int Priority_Studio_Load_Sample    = Priority_Medium;
            public const int Priority_Convolution1          = Priority_Very_High;
            public const int Priority_Convolution2          = Priority_Very_High;

            // Default Thread Stack Sizes
            public const uint StackSize_Default             = 0;
            public const uint StackSize_Mixer               = 80  * 1024;
            public const uint StackSize_Feeder              = 16  * 1024;
            public const uint StackSize_Stream              = 96  * 1024;
            public const uint StackSize_File                = 48  * 1024;
            public const uint StackSize_NonBlocking         = 112 * 1024;
            public const uint StackSize_Record              = 16  * 1024;
            public const uint StackSize_Geometry            = 48  * 1024;
            public const uint StackSize_Profiler            = 128 * 1024;
            public const uint StackSize_Studio_Update       = 96  * 1024;
            public const uint StackSize_Studio_Load_Bank    = 96  * 1024;
            public const uint StackSize_Studio_Load_Sample  = 96  * 1024;
            public const uint StackSize_Convolution1        = 16  * 1024;
            public const uint StackSize_Convolution2        = 16  * 1024;

            public static void SetAttributes(FmodThreadType threadType, ulong affinity, int priority, uint stackSize)
            {
                Library.Thread_SetAttributes(threadType, affinity, priority, stackSize).CheckResult();
            }
        }

        #endregion
    }
}
