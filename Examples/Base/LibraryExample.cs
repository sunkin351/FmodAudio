using FmodAudio;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples.Base
{
    public abstract class Example : IDisposable
    {
        protected FmodSystem System;

        public string Title { get; }

        private readonly Dictionary<ConsoleKey, Action> Commands = new Dictionary<ConsoleKey, Action>();

        protected bool ShouldExit { get; private set; }

        private int ConsoleUpdateStart;

        protected Example(string title)
        {
            Title = title;
        }

        public virtual void Initialize()
        {
            ShouldExit = false;
            ConsoleUpdateStart = 0;

            System = Fmod.CreateSystem();
            TestVersion(System);
        }

        public abstract void Run();

        protected void SetConsoleUpdateStart()
        {
            ConsoleUpdateStart = Console.CursorTop + 1;
        }

        protected void OnUpdate()
        {
            ConsoleHelpers.SetCursorRow(ConsoleUpdateStart);
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
                throw new NotSupportedException($"FMOD Library version {version} is less than Binding version {Fmod.BindingVersion}");
            }
        }

        public virtual void Dispose()
        {
            System?.Dispose();
        }

        protected void RegisterCommand(ConsoleKey key, Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!Commands.TryAdd(key, action))
            {
                throw new InvalidOperationException("An action has already been registered to keybind " + key.ToString());
            }
        }

        protected void ProcessInput()
        {
            while (Console.KeyAvailable)
            {
                var info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.Escape)
                {
                    ShouldExit = true;
                    return;
                }

                if (Commands.TryGetValue(info.Key, out var action))
                {
                    action();
                }
            }
        }
    }
}
