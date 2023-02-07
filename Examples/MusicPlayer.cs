using System;
using System.IO;

using FmodAudio;

using Examples.Base;

namespace Examples;

public class MusicPlayer : Example
{
    private ChannelGroup masterChannel;

    public MusicPlayer() : base("Fmod Music Player")
    {
        RegisterCommand(ConsoleKey.Spacebar, () =>
        {
            masterChannel.Paused = !masterChannel.Paused;
        });
        RegisterCommand(ConsoleKey.UpArrow, () =>
        {
            float volume = masterChannel.Volume;

            volume += 0.1f;

            if (volume > 1f)
                volume = 1f;

            masterChannel.Volume = volume;
        });
        RegisterCommand(ConsoleKey.DownArrow, () =>
        {
            float volume = masterChannel.Volume;

            volume -= 0.1f;

            if (volume < 0.1f)
                volume = 0.1f;

            masterChannel.Volume = volume;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        System.Init(2);

        masterChannel = System.MasterChannelGroup;
    }

    public unsafe override void Run()
    {
        while (true)
        {
            this.ResetConsoleUpdateStart();

            var filepath = RetrieveFilePath();

            if (filepath == null)
            {
                return;
            }

            using var sound = System.CreateSound(filepath, Mode.CreateStream);

            DrawText("==================================================");
            DrawText("Tiny Music Player Example");
            DrawText("");
            DrawText("==================================================");
            DrawText("Press Esc to exit.");
            DrawText("Press Spacebar to toggle pause.");
            DrawText("Up and Down arrow keys for volume control.");
            DrawText();

            DrawText("Now playing " + sound.Name);

            SetConsoleUpdateStart();

            int lenms = (int)sound.GetLength(TimeUnit.MS);
            var channel = System.PlaySound(sound);

            while (true)
            {
                if (ShouldEndExample)
                {
                    return;
                }

                OnUpdate();
                System.Update();

                uint pos;

                if (Fmod.Library.Channel_GetPosition(channel, &pos, TimeUnit.MS) == Result.Ok)
                {
                    DrawTime((int)pos, lenms);
                }
                else
                {
                    break;
                }

                Sleep(10);
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    private string? RetrieveFilePath()
    {
        Console.Clear();

        var chars = Path.GetInvalidPathChars();

        ConsoleHelpers.SetCursorRow(0);
        ConsoleHelpers.Draw("Type in audio file path:");

        Span<char> buffer = stackalloc char[200];

        buffer[0] = ':';
        buffer[1] = '>';
        buffer[2] = ' ';

        var pathBuilder = new BufferedStringBuilder(buffer.Slice(3));

        while (true)
        {
            ConsoleHelpers.SetCursorRow(4);
            ConsoleHelpers.Draw(buffer.Slice(0, 3 + pathBuilder.Written));

            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                pathBuilder.Written -= 1;
                continue;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                var pathSpan = pathBuilder.CurrentContent;

                if (pathSpan.IndexOfAny(chars) >= 0)
                {
                    ConsoleHelpers.SetCursorRow(2);
                    ConsoleHelpers.Draw("Invalid Path Characters!");
                    Sleep(3000);
                    continue;
                }

                var path = pathSpan.ToString();

                if (!File.Exists(path))
                {
                    ConsoleHelpers.SetCursorRow(2);
                    ConsoleHelpers.Draw("File does not Exist!");
                    Sleep(3000);
                    continue;
                }

                Console.Clear();
                Console.SetCursorPosition(0, 0);

                return path;
            }

            if (key.KeyChar >= ' ')
            {
                pathBuilder.WriteToBuffer(key.KeyChar);
            }
        }
    }
}
