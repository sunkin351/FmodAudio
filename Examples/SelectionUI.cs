using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Examples
{
    using Base;

    public class SelectionUI
    {
        private readonly Example[] List;
        private readonly char[] lineBuffer;
        private int currentSelection = 0;

        public SelectionUI(Example[] list)
        {
            List = list;
            lineBuffer = new char[ConsoleHelpers.ColumnCount];
        }

        private static void ClearScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        public Example Select()
        {
            ClearScreen();

            Span<char> buffer = lineBuffer;

            buffer[0] = ' ';
            buffer[2] = ':';

            Span<char> nameBuffer = buffer.Slice(4);

            ConsoleHelpers.Draw("Select An Example to Run:");

            while (true)
            {
                for (int i = 0; i < List.Length; ++i)
                {
                    var example = List[i];

                    buffer[1] = (i == currentSelection) ? '>' : (char)0;

                    WriteTitleToBuffer(example.Title, nameBuffer);

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

                            return List[currentSelection];

                        case ConsoleKey.Escape:
                            return null;
                    }
                }

                ConsoleHelpers.OnUpdate(1);
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
