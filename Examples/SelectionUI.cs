using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples
{
    using Base;

    public class SelectionUI
    {
        private readonly string[] List;
        private readonly char[] lineBuffer;
        private readonly Type[] ExampleTypes;
        private int currentSelection = 0;

        public SelectionUI(string[] list, Type[] exampleTypes)
        {
            List = list;
            ExampleTypes = exampleTypes;
            lineBuffer = new char[ConsoleHelpers.ColumnCount];
        }

        private static void ClearScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        public Type Select()
        {
            Console.Title = "Fmod Example Selection";

            ClearScreen();

            Span<char> buffer = lineBuffer;

            buffer[2] = ':';

            Span<char> nameBuffer = buffer.Slice(4);

            ConsoleHelpers.Draw("Select An Example to Run:");

            while (true)
            {
                for (int i = 0; i < List.Length; ++i)
                {
                    var example = List[i];

                    buffer[1] = (i == currentSelection) ? '>' : default;

                    WriteTitleToBuffer(example, nameBuffer);

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

                            if (tmp < List.Length)
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
                            ClearScreen();

                            return ExampleTypes[currentSelection];

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
    }
}
