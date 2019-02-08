using FmodAudio;
using System;

namespace Examples.Base
{
    public interface IExample
    {
        string Title { get; }
        void Run();
    }
}
