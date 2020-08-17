using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

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
        static int LastLineLen = 0;
        static readonly char[] LineBuffer = new char[ColumnCount];
        static readonly TextWriter ConsoleOut = Console.Out;

        static ConsoleHelpers()
        {
            Debug.Assert(sizeof(char) == 2);
            ClearBuffer();
        }

        const char SpaceChar = ' ';
        const ushort SpaceCharUShort = SpaceChar;

        private static unsafe int FillBuffer(ReadOnlySpan<char> input)
        {
            int count = Math.Min(LineBuffer.Length, input.Length);
            int i = 0;

            fixed (char* buffer = LineBuffer, pInput = input)
            {
                if (Sse2.IsSupported && count >= Vector128<ushort>.Count)
                {
                    Vector128<ushort> Space = Vector128.Create(SpaceCharUShort); //Space character

                    do
                    {
                        var data = Sse2.LoadVector128((ushort*)pInput + i);

                        var comp = Vector128<ushort>.Zero;

                        comp = Sse2.CompareEqual(comp, data);

                        if (Sse41.IsSupported)
                        {
                            data = Sse41.BlendVariable(data, Space, comp);
                        }
                        else
                        {
                            comp = Sse2.And(comp, Space);

                            data = Sse2.Or(data, comp); //Elements being replaced are already 0'ed
                        }

                        Sse2.Store((ushort*)buffer + i, data);

                        i += Vector128<ushort>.Count;
                    }
                    while ((count - i) >= Vector128<ushort>.Count);
                }

                while (i < count)
                {
                    char tmp = pInput[i];
                    buffer[i] = tmp == 0 ? ' ' : tmp;

                    i += 1;
                }

                return count;
            }
        }

        private static unsafe void ClearBuffer(int startIdx = 0)
        {
            if (Sse2.IsSupported)
            {
                int count = LineBuffer.Length;

                int i = startIdx;

                fixed (char* buffer = LineBuffer)
                {
                    if (Sse2.IsSupported && count >= Vector128<ushort>.Count)
                    {
                        Vector128<ushort> Space = Vector128.Create(SpaceCharUShort);

                        do
                        {
                            Sse2.Store((ushort*)buffer + i, Space);

                            i += Vector128<ushort>.Count;
                        }
                        while ((count - i) >= Vector128<ushort>.Count);
                    }

                    while (i < count)
                    {
                        buffer[i] = SpaceChar;

                        i += 1;
                    }
                }

                return;
            }

            LineBuffer.AsSpan(startIdx).Fill(SpaceChar);
        }

        private static void SetConsoleDimensions(int width, int height)
        {
            int TrueRowCount = height + 1;

            int WinWidth = Console.WindowWidth;
            int WinHeight = Console.WindowHeight;

            if (WinWidth != width)
            {
                if (WinWidth < width)
                {
                    Console.SetBufferSize(width, WinHeight);
                    Console.SetWindowSize(width, WinHeight);
                }
                else
                {
                    Console.SetWindowSize(width, WinHeight);
                    Console.SetBufferSize(width, WinHeight);
                }
            }

            if (WinHeight != height)
            {
                if (WinHeight <= TrueRowCount)
                {
                    Console.SetBufferSize(width, TrueRowCount);
                    Console.SetWindowSize(width, TrueRowCount);
                }
                else
                {
                    Console.SetWindowSize(width, TrueRowCount);
                    Console.SetBufferSize(width, TrueRowCount);
                }
            }
        }

        public static void Initialize()
        {
            if (isInitialized)
                return;

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;

            SetConsoleDimensions(ColumnCount, RowCount);
            isInitialized = true;
        }

        public static void OnExit()
        {
        }

        public static void SetCursorRow()
        {
            Console.SetCursorPosition(0, 0);
        }

        public static void SetCursorRow(int Row)
        {
            Console.SetCursorPosition(0, Row);
        }

        public static void OnError()
        {
            Console.Clear(); 

            SetConsoleDimensions(100, 50);

            Console.SetCursorPosition(0, 0);
        }
        
        public static void Draw()
        {
            if (LastLineLen > 0)
            {
                ClearBuffer();
                LastLineLen = 0;
            }

            ConsoleOut.Write(LineBuffer);
        }

        public static void Draw(string input)
        {
            Draw(input.AsSpan());
        }

        public static void Draw(char[] input)
        {
            Draw(input.AsSpan());
        }

        public static void Draw(ReadOnlySpan<char> input)
        {
            int len = FillBuffer(input);

            if (len < LastLineLen)
            {
                ClearBuffer(len);
            }

            LastLineLen = len;
            ConsoleOut.Write(LineBuffer);
        }
    }
}
