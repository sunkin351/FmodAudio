using System;
using System.Buffers;

namespace Examples
{
    using Base;

    public class SelectionUI
    {
        private readonly ExampleInfo[] ExampleList;
        private int currentSelection = 0;

        public SelectionUI(ExampleInfo[] examples)
        {
            ExampleList = examples;
        }

        private static void ClearScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        private const string BeginString = "[ ]:";

        public Type Select()
        {
            Console.Title = "Fmod Example Selection";

            ClearScreen();

            InitialRender();

            while (true)
            {
                bool NothingNew = true;

                do
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

                            NothingNew = false;

                            break;

                        case ConsoleKey.UpArrow:
                            tmp = currentSelection - 1;

                            if (tmp >= 0)
                            {
                                currentSelection = tmp;
                            }

                            NothingNew = false;

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
                while (Console.KeyAvailable || NothingNew);

                ConsoleHelpers.SetCursorRow(1);
                RenderToConsole();
            }
        }

        private void InitialRender()
        {
            ConsoleHelpers.Draw("Select An Example to Run:");

            RenderToConsole();
        }

        private void RenderToConsole()
        {
            char[] lineBuffer = ArrayPool<char>.Shared.Rent(ConsoleHelpers.ColumnCount);

            try
            {
                Span<char> buffer = lineBuffer;

                BeginString.AsSpan().CopyTo(buffer);

                Span<char> nameBuffer = buffer.Slice(BeginString.Length + 1);

                for (int i = 0; i < ExampleList.Length; ++i)
                {
                    ref var example = ref ExampleList[i];

                    if (i != currentSelection)
                    {
                        buffer[1] = ' ';
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
            }
            finally
            {
                ArrayPool<char>.Shared.Return(lineBuffer);
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
