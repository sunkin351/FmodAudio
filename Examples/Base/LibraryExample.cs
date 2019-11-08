using FmodAudio;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples.Base
{
    public abstract class Example
    {
        protected ConcurrentQueue<Button> Commands => ConsoleHelpers.CommandQueue;
        
        public virtual string Title => "Fmod Example";

        public abstract void Run();

        protected static void OnUpdate()
        {
            ConsoleHelpers.OnUpdate();
        }
        
        protected static void DrawText()
        {
            ConsoleHelpers.Draw();
        }

        protected static void DrawText(ReadOnlySpan<char> input)
        {
            ConsoleHelpers.Draw(input);
        }

        protected static void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        protected static string MediaPath(string file)
        {
            return Path.Combine("../../../media/", file);
        }

        protected static void TestVersion(FmodSystem system)
        {
            var version = system.Version;
            
            if (version < Fmod.BindingVersion)
            {
                throw new NotSupportedException($"FMOD Library version {version} is less than Binding version {FmodSystem.BindingVersion}");
            }
        }
    }
}
