using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Examples.Base
{
    public enum Button
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

    public static class ConsoleHelpers
    {
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

        const int ColumnCount = 50;
        const int RowCount = 25;

        static bool isInitialized = false;
        static readonly char[] LineBuffer = new char[ColumnCount];
        static int LastLineLen = 0;

        static Thread InputThread = new Thread(ThreadMethod);
        static bool ThreadContinue;

        public static readonly ConcurrentQueue<Button> CommandQueue = new ConcurrentQueue<Button>();

        public static void Initialize(string Title)
        {
            if (isInitialized)
                return;

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = Title;

            const int TrueRowCount = RowCount + 1;

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

            ThreadContinue = true;
            InputThread.Start();
        }

        public static void OnExit()
        {
            ThreadContinue = false;

            if (!InputThread.Join(100))
            {
                InputThread.Abort();
            }
        }

        public static void OnUpdate()
        {
            Console.SetCursorPosition(0, 0);
        }

        public static void OnError()
        {
            Console.SetCursorPosition(0, 0);
            Console.SetBufferSize(100, 100);
        }
        
        public static void Draw()
        {
            if (LastLineLen > 0)
            {
                Array.Clear(LineBuffer, 0, LastLineLen);
                LastLineLen = 0;
            }

            Console.Write(LineBuffer);
        }

        public static void Draw(ReadOnlySpan<char> input)
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

        private static void ThreadMethod()
        {
            while(ThreadContinue)
            {
                while (Console.KeyAvailable)
                {
                    if (Keys.TryGetValue(Console.ReadKey(true).Key, out Button btn))
                    {
                        CommandQueue.Enqueue(btn);

                        if (btn == Button.Quit)
                        {
                            return;
                        }
                    }
                }

                Thread.Sleep(50);
            }
        }
    }
}
