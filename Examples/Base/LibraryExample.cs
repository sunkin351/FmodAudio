using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FmodAudio;

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

            ProcessInput();
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
            if (System != default)
                System.Dispose();
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

        private static int RenderTime(int ms, Span<char> buffer)
        {
            if (ms < 0)
                throw new ArgumentOutOfRangeException(nameof(ms));

            int total = 0;

            var seconds = Math.DivRem(ms, 1000, out ms);

            var minutes = Math.DivRem(seconds, 60, out seconds);

            int count;
            if (minutes != 0)
            {
                minutes.TryFormat(buffer, out count);

                buffer[count] = ':';
                buffer = buffer.Slice(count + 1);

                total += count + 1;
            }

            seconds.TryFormat(buffer, out count);

            buffer[count] = '.';
            buffer = buffer.Slice(count + 1);

            total += count + 1;

            ms.TryFormat(buffer, out count);

            total += count;

            return total;
        }

        protected static void DrawTime(int ms, int totalms)
        {
            const string PreText = "Time: ";

            Span<char> buffer = stackalloc char[50];

            PreText.AsSpan().CopyTo(buffer);

            int count = RenderTime(ms, buffer.Slice(PreText.Length));

            buffer[PreText.Length + count] = '/';
            count += 1;

            count += RenderTime(totalms, buffer.Slice(PreText.Length + count));

            DrawText(buffer.Slice(0, PreText.Length + count));
        }

        protected ref struct BufferedStringBuilder
        {
            private readonly Span<char> Buffer;
            private int contentLength;

            public BufferedStringBuilder(Span<char> buffer)
            {
                Buffer = buffer;
                contentLength = 0;
            }

            public int WriteToBuffer(ReadOnlySpan<char> input)
            {
                int availableRoom = Buffer.Length - contentLength;

                if (availableRoom == 0)
                {
                    return 0;
                }

                if (input.Length > availableRoom)
                {
                    input.Slice(0, availableRoom).CopyTo(Buffer.Slice(contentLength));
                    contentLength += availableRoom;
                    return availableRoom;
                }

                input.CopyTo(Buffer.Slice(contentLength));
                contentLength += input.Length;
                return input.Length;
            }

            public bool WriteToBuffer(char input)
            {
                int availableRoom = Buffer.Length - contentLength;
                if (availableRoom == 0)
                {
                    return false;
                }

                Buffer[contentLength] = input;
                contentLength += 1;
                return true;
            }

            public int WriteToBuffer(int input)
            {
                int availableRoom = Buffer.Length - contentLength;
                if (availableRoom == 0)
                {
                    return 0;
                }

                input.TryFormat(Buffer.Slice(contentLength), out int written);
                contentLength += written;
                return written;
            }

            public void Clear() => contentLength = 0;

            public ReadOnlySpan<char> CurrentContent => Buffer.Slice(0, contentLength);
        }
    }
}
