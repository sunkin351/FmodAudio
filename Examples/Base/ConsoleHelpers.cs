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
        public const int ColumnCount = 50;
        public const int RowCount = 25;

        static bool isInitialized = false;
        static readonly char[] LineBuffer = new char[ColumnCount];
        static int LastLineLen = 0;

        public static void Initialize()
        {
            if (isInitialized)
                return;

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;

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
        }

        public static void OnExit()
        {
        }

        public static void OnUpdate()
        {
            Console.SetCursorPosition(0, 0);
        }

        public static void OnUpdate(int Row)
        {
            Console.SetCursorPosition(0, Row);
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

        public static void Draw(string input)
        {
            int len = input.Length;

            if (len == LineBuffer.Length)
            {
                Console.Write(input);
            }

            Draw(input.AsSpan());
        }

        public static void Draw(char[] input)
        {
            int len = input.Length;

            if (len == LineBuffer.Length)
            {
                Console.Write(input);
                return;
            }

            Draw(input.AsSpan());
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
    }
}
