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

        private static readonly Dictionary<ConsoleKey, Button> Keys = new Dictionary<ConsoleKey, Button>()
        {
            {ConsoleKey.D1, Button.Action1 },
            {ConsoleKey.D2, Button.Action2 },
            {ConsoleKey.D3, Button.Action3 },
            {ConsoleKey.D4, Button.Action4 },
            {ConsoleKey.LeftArrow, Button.Left },
            {ConsoleKey.RightArrow, Button.Right },
            {ConsoleKey.UpArrow, Button.Up },
            {ConsoleKey.DownArrow, Button.Down },
            {ConsoleKey.Spacebar, Button.More },
            {ConsoleKey.Escape, Button.Quit }
        };

        protected readonly ConcurrentQueue<Button> Commands = new ConcurrentQueue<Button>();
        private Thread ConsoleInputThread;
        private bool ConsoleThreadContinue = true;

        protected Example()
        {
            int WinWidth = Console.WindowWidth;
            int WinHeight = Console.WindowHeight;

            if (WinWidth < ColumnCount)
            {
                Console.SetBufferSize(ColumnCount, WinHeight);
                Console.SetWindowSize(ColumnCount, WinHeight);
            }
            else
            {
                Console.SetWindowSize(ColumnCount, WinHeight);
                Console.SetBufferSize(ColumnCount, WinHeight);
            }

            const int TrueRowCount = RowCount + 1;

            if (WinHeight < TrueRowCount)
            {
                Console.SetBufferSize(ColumnCount, TrueRowCount);
                Console.SetWindowSize(ColumnCount, TrueRowCount);
            }
            else
            {
                Console.SetWindowSize(ColumnCount, TrueRowCount);
                Console.SetBufferSize(ColumnCount, TrueRowCount);
            }

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = ExampleTitle;

            ConsoleInputThread = new Thread(ConsoleThread);
            ConsoleInputThread.Start();
        }

        protected virtual string ExampleTitle => "Fmod Example";

        public abstract void Run();

        public virtual void Dispose()
        {
            ConsoleThreadContinue = false;

            if (!ConsoleInputThread.Join(100))
            {
                ConsoleInputThread.Abort();
            }
        }
        
        private readonly char[] LineBuffer = new char[ColumnCount];
        private int LastLineLen = 0;

        protected void OnUpdate()
        {
            Console.SetCursorPosition(0, 0);
        }

        protected void DrawText(string input)
        {
            DrawText(input.AsSpan());
        }

        protected void DrawText(ReadOnlySpan<char> input)
        {
            int len = input.Length;

            if (len > ColumnCount)
            {
                input = input.Slice(0, ColumnCount);
                len = ColumnCount;
            }

            Span<char> buffer = LineBuffer;

            input.CopyTo(buffer);

            if (len < LastLineLen)
            {
                buffer.Slice(len, ColumnCount - len).Clear();
            }

            LastLineLen = len;
            Console.Write(LineBuffer);
        }

        protected void DrawText()
        {
            if (LastLineLen > 0)
            {
                Array.Clear(LineBuffer, 0, LastLineLen);
                LastLineLen = 0;
            }

            Console.Write(LineBuffer);
        }

        protected static void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        private void ConsoleThread()
        {
            Button button = default;
            do
            {
                while(Console.KeyAvailable)
                {
                    if (Keys.TryGetValue(Console.ReadKey(true).Key, out button))
                    {
                        Commands.Enqueue(button);
                        if (button == Button.Quit)
                            return;
                    }
                }

                Sleep(50);

            } while (ConsoleThreadContinue);
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

        protected enum Button
        {
            Action1 = 1,
            Action2,
            Action3,
            Action4,
            Left,
            Right,
            Up,
            Down,
            More,
            Pause,
            Quit
        }
    }
}
