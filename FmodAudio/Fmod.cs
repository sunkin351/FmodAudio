#pragma warning disable IDE0052, CA1034
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using AdvancedDLSupport;

namespace FmodAudio
{
    using Interop;

    public static class Fmod
    {
        public static FmodVersion BindingVersion => new FmodVersion(0x00020005);

        public static uint PluginSDKVersion => 110;

        public const int MaxAllowedSystemObjects = 8;

        #region Pre-Native Setup

        private static string defaultLibName;
        private static string location;

        public static NativeLibrary Library => nativeLibrary.Value;

        internal static UserDataMethods UserDataMethods => userDataMethods.Value;

        // <summary>
        // Subscribe to this to log when fatal errors occur.String passed is the error message.
        // </summary>
        //public static event Action<string, string> FatalError;

        public static string DefaultLibraryName
        {
            get
            {
                if (defaultLibName is null)
                {
                    string ext;

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        ext = ".dll";
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        ext = ".so";
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        ext = ".dylib";
                    }
                    else
                    {
                        throw new PlatformNotSupportedException();
                    }

                    defaultLibName = string.Concat("fmod", ext);
                }

                return defaultLibName;
            }
        }

        /// <summary>
        ///     This method allows you to specify the location to the FMOD native library file.
        ///     
        ///     Do take care to call this before calling any other FMOD api method, or else this will do nothing.
        /// </summary>
        /// <param name="path">
        ///     Can Specify either a path directly to the library file (allows you to specify a different file name),
        ///     or a path to the directory containing the library file (File name MUST be "fmod.dll", "fmod.so", or "fmod.dylib" depending on the platform).
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

        private const ImplementationOptions BuilderOptions =
                    ImplementationOptions.EnableOptimizations |
                    ImplementationOptions.EnableDllMapSupport |
                    ImplementationOptions.UseIndirectCalls |
                    ImplementationOptions.UseLazyBinding;

        private static readonly NativeLibraryBuilder builder = new NativeLibraryBuilder(BuilderOptions);

        private static readonly Lazy<NativeLibrary> nativeLibrary = new Lazy<NativeLibrary>(delegate ()
        {
            return builder.ActivateClass<NativeLibrary, INativeLibrary>(location ?? DefaultLibraryName);
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<UserDataMethods> userDataMethods = new Lazy<UserDataMethods>(delegate ()
        {
            return builder.ActivateClass<UserDataMethods, IUserDataMethods>(location ?? DefaultLibraryName);
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        internal static readonly object CreationSyncObject = new object();
        private static readonly object InitSyncObject = new object();

        private static DebugCallback DebugCallbackReference;

        public static void InitializeDebug(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename)
        {
            Library.Debug_Initialize(flags, mode, callback, filename).CheckResult();
            DebugCallbackReference = callback;
        }

        public static unsafe bool DiskBusy
        {
            get
            {
                Library.File_GetDiskBusy(out bool res).CheckResult();

                return res;
            }

            set
            {
                Library.File_SetDiskBusy(value).CheckResult();
            }    
        }

        internal static readonly ConcurrentDictionary<IntPtr, WeakReference<FmodSystem>> SystemLookup = new ConcurrentDictionary<IntPtr, WeakReference<FmodSystem>>();

        internal static bool TryGetSystem(IntPtr handle, out FmodSystem system)
        {
            if (SystemLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out system))
                {
                    return true;
                }

                goto InvalidState;
            }

            system = null;
            return false;

            InvalidState:
            throw new InvalidOperationException();
        }

        internal static FmodSystem GetSystem(IntPtr handle)
        {
            if (!TryGetSystem(handle, out var system))
            {
                throw new ArgumentException("Invalid system handle");
            }

            return system;
        }

        public unsafe static FmodSystem CreateSystem()
        {
            IntPtr handle;

            lock (CreationSyncObject)
            {
                Library.System_Create(&handle).CheckResult();
            }

            var sys = new FmodSystem(Library, handle);

            var tmp = SystemLookup.TryAdd(handle, new WeakReference<FmodSystem>(sys));

            Debug.Assert(tmp);

            return sys;
        }

        public static class Memory
        {
            private static MemoryAllocCallback userAlloc;
            private static MemoryReallocCallback userRealloc;
            private static MemoryFreeCallback userFree;

            public static void Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags)
            {
                Library.Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags).CheckResult();

                userAlloc = useralloc;
                userRealloc = userrealloc;
                userFree = userfree;
            }

            public static void GetStats(out int currentlyAllocated, out int maxAllocated, bool blocking)
            {
                Library.Memory_GetStats(out currentlyAllocated, out maxAllocated, blocking).CheckResult();
            }
        }

        #endregion
    }
}
