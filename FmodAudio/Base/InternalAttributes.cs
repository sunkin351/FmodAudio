using System;

namespace FmodAudio.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class VTableAttribute : Attribute
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
