using System;
using System.IO;
using System.Runtime.InteropServices;

using FmodAudio.Base;

namespace FmodAudio
{
    public static unsafe class Fmod
    {
        internal const string Deprecation_Message = "Newer versions of Fmod have deprecated this, and will eventually remove it entirely.";

        public static FmodVersion BindingVersion => new FmodVersion(0x00020100);

        private static readonly FmodVersion MinimumSupportedVersion = new FmodVersion(0x00020100);

        public static uint PluginSDKVersion => 110;

        public const int MaxAllowedSystemObjects = 8;

        internal const int MaxInteropNameStringLength = 200;

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
            FmodVersion version;

            var library = Library;

            lock (CreationSyncObject)
            {
                library.System_Create(&system).CheckResult();

                library.System_GetVersion(system, &version).CheckResult();

                if (version < MinimumSupportedVersion)
                {
                    library.System_Release(system).CheckResult();

                    throw new InvalidOperationException("Native API version is " + version.ToString() + ", but only " + MinimumSupportedVersion.ToString() + " and above is supported by FmodAudio 2.0.");
                }
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
                if (useralloc == null || userfree == null)
                    throw new ArgumentNullException();

                Library.Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags).CheckResult();
            }

            public static void GetStats(out int currentlyAllocated, out int maxAllocated, bool blocking)
            {
                Library.Memory_GetStats(out currentlyAllocated, out maxAllocated, blocking).CheckResult();
            }
        }

        #endregion
    }
}
