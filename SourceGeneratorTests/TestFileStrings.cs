using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGeneratorTests
{
    public static class TestFileStrings
    {
        public const string VTableAttribute = @"
using System;

namespace FmodAudio.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VTableAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Struct)]
    internal class WrapperTypeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class InteropMethodAttribute : Attribute
    {
    }
}
";

        public const string SimpleTestFile = @"
using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Base
{
    [WrapperType]
    public struct SystemHandle
    {
        public IntPtr Handle { get; }
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
}
";
    }
}
