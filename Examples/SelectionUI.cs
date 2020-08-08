using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples
{
    using Base;

    public class SelectionUI
    {
        private readonly ExampleInfo[] ExampleList;
        private readonly char[] lineBuffer;
        private int currentSelection = 0;

        public SelectionUI(ExampleInfo[] examples)
        {
            ExampleList = examples;
            lineBuffer = new char[ConsoleHelpers.ColumnCount];
        }

        private static void ClearScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        public Type Select()
        {
            const string BeginString = "[ ]:";

            Console.Title = "Fmod Example Selection";

            ClearScreen();

            Span<char> buffer = lineBuffer;

            BeginString.AsSpan().CopyTo(buffer);

            Span<char> nameBuffer = buffer.Slice(BeginString.Length + 1);

            ConsoleHelpers.Draw("Select An Example to Run:");

            while (true)
            {
                for (int i = 0; i < ExampleList.Length; ++i)
                {
                    ref var example = ref ExampleList[i];

                    if (i != currentSelection)
                    {
                        buffer[1] = default;
                    }
                    else
                    {
                        if (example.Enabled)
                            buffer[1] = '-';
                        else
                            buffer[1] = 'x';
                    }

                    WriteTitleToBuffer(example.Name, nameBuffer);

                    ConsoleHelpers.Draw(lineBuffer);

                    nameBuffer.Clear();
                }

                while (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(true);

                    int tmp;
                    switch (info.Key)
                    {
                        case ConsoleKey.DownArrow:
                            tmp = currentSelection + 1;

                            if (tmp < ExampleList.Length)
                            {
                                currentSelection = tmp;
                            }
                            break;

                        case ConsoleKey.UpArrow:
                            tmp = currentSelection - 1;

                            if (tmp >= 0)
                            {
                                currentSelection = tmp;
                            }
                            break;

                        case ConsoleKey.Enter:
                            ref var example = ref ExampleList[currentSelection];

                            if (!example.Enabled)
                                break;

                            ClearScreen();

                            return example.ExampleType;

                        case ConsoleKey.Escape:
                            return null;
                    }
                }

                ConsoleHelpers.SetCursorRow(1);
                Thread.Sleep(10);
            }
        }

        private static void WriteTitleToBuffer(string title, Span<char> buffer)
        {
            ReadOnlySpan<char> tmp = title;

            if (tmp.Length > buffer.Length)
            {
                tmp = tmp.Slice(0, buffer.Length);
            }

            tmp.CopyTo(buffer);
        }

        public struct ExampleInfo
        {
            public string Name { get; init; }

            public Type ExampleType { get; init; }

            public bool Enabled { get; init; }
        }
    }
}
