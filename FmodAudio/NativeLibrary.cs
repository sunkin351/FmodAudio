using AdvancedDLSupport;
using System;
using System.Runtime.InteropServices;

namespace FmodAudio
{
    using Interop;

    internal static class NativeLibrary
    {
        public static IFmodLibrary Library { get; }

        static NativeLibrary()
        {
            const ImplementationOptions options = 
                ImplementationOptions.EnableOptimizations | 
                ImplementationOptions.EnableDllMapSupport | 
                ImplementationOptions.UseIndirectCalls;

            string libExt;

            //This section of code could probably use some improvement
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libExt = ".dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libExt = ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                libExt = ".dylib";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            NativeLibraryBuilder builder = new NativeLibraryBuilder(options);

            Library = builder.ActivateInterface<IFmodLibrary>("fmod" + (Environment.Is64BitProcess ? "64" : string.Empty) + libExt);
        }

    }
}
