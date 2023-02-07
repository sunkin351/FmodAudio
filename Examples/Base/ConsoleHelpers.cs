using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Examples.Base;

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
    static readonly StringBuilder LineBuffer = new();
    static readonly TextWriter ConsoleOut = Console.Out;

    static bool isInitialized = false;

    public static int ColumnCount => Console.WindowWidth;

    static ConsoleHelpers()
    {
        Debug.Assert(sizeof(char) == 2);
    }

    public static void Initialize()
    {
        if (isInitialized)
            return;

        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Green;

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

        Console.SetCursorPosition(0, 0);
    }
    
    public static void Draw()
    {
        int x = Console.WindowWidth;

        LineBuffer.Append(' ', x);

        ConsoleOut.Write(LineBuffer);
        LineBuffer.Clear();
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
        int x = Console.WindowWidth;

        if (input.Length >= x)
        {
            LineBuffer.Append(input.Slice(0, x));
        }
        else
        {
            LineBuffer.Append(input);
            LineBuffer.Append(' ', x - input.Length);
        }

        LineBuffer.Replace('\0', ' ');

        ConsoleOut.Write(LineBuffer);

        LineBuffer.Clear();
    }
}
