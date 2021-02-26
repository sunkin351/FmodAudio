using System;

namespace FmodAudio.Base.SGAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InteropMethodAttribute : Attribute
    {
        /// <summary>
        /// If True, source generator will elegantly handle this method not being loaded from the Native Library.
        /// </summary>
        public bool Guard { get; set; } = false;

        public InteropMethodAttribute(bool guard = false) => Guard = guard;
    }
}
