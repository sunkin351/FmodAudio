using System;
using System.IO;

using FmodAudio;

using Examples.Base;

namespace Examples
{
    public class MusicPlayer : Example
    {
        private Sound sound;
        private Channel channel;

        public MusicPlayer() : base("Fmod Music Player")
        {
            RegisterCommand(ConsoleKey.Spacebar, () =>
            {
                channel.Paused = !channel.Paused;
            });
            RegisterCommand(ConsoleKey.UpArrow, () =>
            {
                float volume = channel.Volume;

                volume += 0.1f;

                if (volume > 1f)
                    volume = 1f;

                channel.Volume = volume;
            });
            RegisterCommand(ConsoleKey.DownArrow, () =>
            {
                float volume = channel.Volume;

                volume -= 0.1f;

                if (volume < 0.1f)
                    volume = 0.1f;

                channel.Volume = volume;
            });
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(2);

            var filepath = RetrieveFilePath();

            sound = System.CreateSound(filepath, Mode.CreateStream);

            channel = System.PlaySound(sound);
        }

        public override void Run()
        {
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

            do
            {
                OnUpdate();
                System.Update();

                DrawTime((int)channel.GetPosition(TimeUnit.MS), lenms);

                Sleep(10);
            }
            while (!ShouldExit && channel.IsPlaying);
        }

        public override void Dispose()
        {
            if (sound != default)
                sound.Dispose();

            base.Dispose();
        }

        private string RetrieveFilePath()
        {
            var chars = Path.GetInvalidPathChars();

            ConsoleHelpers.Draw("Type in audio file path:");

        Retry:
            ConsoleHelpers.SetCursorRow(4);

            Console.Write(":> ");

            var Out = Console.Out;

            var path = Console.ReadLine();

            if (path.IndexOfAny(chars) >= 0)
            {
                ConsoleHelpers.SetCursorRow(2);
                ConsoleHelpers.Draw("Invalid Path Characters!");
                Sleep(3000);

                goto Retry;
            }

            if (!File.Exists(path))
            {
                ConsoleHelpers.SetCursorRow(2);
                ConsoleHelpers.Draw("File does not Exist!");
                Sleep(3000);

                goto Retry;
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            return path;
        }
    }
}
