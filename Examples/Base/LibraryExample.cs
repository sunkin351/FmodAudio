using FmodAudio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples.Base
{
    public abstract class Example : IExample
    {
        protected const int ColumnCount = 50;
        protected const int RowCount = 25;

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
            return "../../../media/" + file;
        }

        protected static void TestVersion(FmodSystem system)
        {
            var version = system.Version;
            
            if (version < FmodSystem.BindingVersion)
            {
                throw new NotSupportedException($"FMOD Library version {version} doesn't match Binding version {FmodSystem.BindingVersion}");
            }
        }

        protected static float Clamp(float min, float max, float value)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;

            return value;
        }
    }
}
