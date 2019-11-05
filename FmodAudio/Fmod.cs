#pragma warning disable IDE0052
using System;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using AdvancedDLSupport;

namespace FmodAudio
{
    using Interop;

    public static class Fmod
    {
        public static FmodVersion BindingVersion => new FmodVersion(0x00020005);

        #region Pre-Native Setup

        private static string defaultLibName;
        private static string location;

        private static NativeLibrary nativeLibrary;

        public static NativeLibrary Library
        {
            get
            {
                EnsureInitialized();

                return nativeLibrary;
            }
        }

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

                    defaultLibName = string.Concat("fmod", Environment.Is64BitProcess ? "64" : string.Empty, ext);
                }

                return defaultLibName;
            }
        }

        public static void SetLibraryLocation(string path)
        {
            if (!Path.GetFileName(path.AsSpan()).StartsWith("fmod", StringComparison.OrdinalIgnoreCase))
            {
                path = Path.Combine(path, DefaultLibraryName);
            }

            location = path;
        }

        #endregion

        #region Native loading and System instantiation

        private static void EnsureInitialized()
        {
            if (nativeLibrary != null) return;

            lock (InitSyncObject)
            {
                if (nativeLibrary != null) return;

                const ImplementationOptions options =
                    ImplementationOptions.EnableOptimizations |
                    ImplementationOptions.EnableDllMapSupport |
                    ImplementationOptions.UseIndirectCalls |
                    ImplementationOptions.UseLazyBinding;

                NativeLibraryBuilder builder = new NativeLibraryBuilder(options);
                
                nativeLibrary = builder.ActivateClass<NativeLibrary, INativeLibrary>(location ?? DefaultLibraryName);
            }
        }

        internal static readonly object CreationSyncObject = new object();
        private static readonly object InitSyncObject = new object();

        private static DebugCallback DebugCallbackReference;

        public static void InitializeDebug(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename)
        {
            EnsureInitialized();

            nativeLibrary.Debug_Initialize(flags, mode, callback, filename).CheckResult();
            DebugCallbackReference = callback;
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

        public static FmodSystem CreateSystem()
        {
            EnsureInitialized();

            IntPtr handle;

            lock (CreationSyncObject)
            {
                nativeLibrary.System_Create(out handle).CheckResult();
            }

            var sys = new FmodSystem(nativeLibrary, handle);

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
                EnsureInitialized();

                nativeLibrary.Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags).CheckResult();

                userAlloc = useralloc;
                userRealloc = userrealloc;
                userFree = userfree;
            }

            public static void GetStats(out int currentlyAllocated, out int maxAllocated, bool blocking)
            {
                EnsureInitialized();

                nativeLibrary.Memory_GetStats(out currentlyAllocated, out maxAllocated, blocking).CheckResult();
            }
        }

        #endregion
    }
}
