using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudioSourceGenerator.Tests
{
    public static class TestFileStrings
    {
        public const string SimpleTestFile =
@"using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Base
{
    [WrapperType, EqualityBoilerplate]
    public partial struct SystemHandle : IEquatable<SystemHandle>
    {
        public IntPtr Handle { get; }

        public bool Equals(SystemHandle other) => Handle == other.Handle;
    }

    [VTable]
    public unsafe partial class FmodLibrary
    {
        [InteropMethod]
        public partial int System_Create(SystemHandle* system);

        public int System_Create(out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
                return System_Create(pSystem);
        }

        [InteropMethod]
        public partial int System_Release(SystemHandle system);
    }
}";
    }
}
