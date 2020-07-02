using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;

namespace Examples
{
    using Base;

    public class MultipleSpeakerExample : Example
    {
        static readonly string[] SpeakerModeString = new string[] { "default", "raw", "mono", "stereo", "quad", "surround", "5.1", "7.1" };

        static readonly string[] SelectionString = new string[]
        {
            "Mono from front left speaker",
            "Mono from front right speaker",
            "Mono from center speaker",
            "Mono from surround left speaker",
            "Mono from surround right speaker",
            "Mono from rear left speaker",
            "Mono from rear right speaker",
            "Stereo from front speakers",
            "Stereo from front speakers (channel swapped)",
            "Stereo (right only) from center speaker"
        };

        static bool IsSelectionAvailable(SpeakerMode mode, int selection)
        {
            if ((uint)selection > 9u)
                throw new ArgumentOutOfRangeException(nameof(selection));

            if (mode == SpeakerMode.Mono || mode == SpeakerMode.Stereo)
            {
                return !(selection >= 2 && selection <= 6) && selection != 9;
            }

            if (mode == SpeakerMode.Quad)
            {
                return selection != 2 && selection != 5 && selection != 6 && selection != 9;
            }

            if (mode == SpeakerMode.Surround || mode == SpeakerMode._5Point1)
            {
                return selection != 5 && selection != 6;
            }

            return true;
        }

        Sound sound1, sound2;
        SpeakerMode SpeakerMode;
        int selection = 0;
        Channel channel;

        public MultipleSpeakerExample() : base("Fmod Multiple Speaker Example")
        {
            RegisterCommand(ConsoleKey.UpArrow, () =>
            {
                if (selection > 0)
                {
                    selection -= 1;
                }
            });
            RegisterCommand(ConsoleKey.DownArrow, () =>
            {
                if (selection < 9)
                {
                    selection += 1;
                }
            });
            RegisterCommand(ConsoleKey.Enter, () => 
            {
                if (!IsSelectionAvailable(SpeakerMode, selection))
                {
                    return;
                }

                if (channel != null && channel.IsPlaying)
                {
                    channel.Stop();
                }

                switch (selection)
                {
                    case 0:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(1f, 0, 0, 0, 0, 0, 0, 0);
                        channel.Paused = false;
                        break;
                    case 1:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 1, 0, 0, 0, 0, 0, 0);
                        channel.Paused = false;
                        break;
                    case 2:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 0, 1, 0, 0, 0, 0, 0);
                        channel.Paused = false;
                        break;
                    case 3:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 0, 0, 0, 1.0f, 0, 0, 0);
                        channel.Paused = false;
                        break;
                    case 4:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 0, 0, 0, 0, 1.0f, 0, 0);
                        channel.Paused = false;
                        break;
                    case 5:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 0, 0, 0, 0, 0, 1.0f, 0);
                        channel.Paused = false;
                        break;
                    case 6:
                        channel = System.PlaySound(sound1, paused: true);
                        channel.SetMixLevelsOutput(0, 0, 0, 0, 0, 0, 0, 1.0f);
                        channel.Paused = false;
                        break;
                    case 7:
                        channel = System.PlaySound(sound2);
                        break;
                    case 8:

                        channel = System.PlaySound(sound2, paused: true);
                        channel.SetMixMatrix(StereoReversedMatrix, 2, 2);
                        channel.Paused = false;
                        break;
                    case 9:
                        channel = System.PlaySound(sound2, paused: true);
                        channel.SetMixMatrix(RightOnlyFromStereoCenterMatrix, 3, 2);
                        channel.Paused = false;
                        break;
                }
            });

        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            System.GetSoftwareFormat(out _, out SpeakerMode, out _);

            sound1 = System.CreateSound(MediaPath("drumloop.wav"), Mode._2D | Mode.Loop_Off);

            sound2 = System.CreateSound(MediaPath("stereo.ogg"), Mode._2D | Mode.Loop_Off);
        }

        public override void Run()
        {
            DrawText("==================================================");
            DrawText("Multiple Speaker Example.");
            DrawText("Copyright (c) Firelight Technologies 2004-2019.");
            DrawText("==================================================");
            DrawText();
            DrawText("Speaker mode is set to " + SpeakerModeString[(int)SpeakerMode] + (SpeakerMode < SpeakerMode._7Point1 ? " causing some speaker options to be unavailable" : string.Empty));
            DrawText();
            DrawText("Press Up arrow or Down arrow to select");
            DrawText("Press Enter to play the sound.");
            DrawText("Press Esc to quit");
            DrawText();
            SetConsoleUpdateStart();

            BufferedStringBuilder builder = new BufferedStringBuilder(new char[50]);

            do
            {
                OnUpdate();

                System.Update();

                uint ms = 0, lenms = 0;
                bool playing = false, paused = false;

                if (channel != null)
                {
                    playing = channel.IsPlaying;

                    if (playing)
                    {
                        paused = channel.Paused;

                        ms = channel.GetPosition(TimeUnit.MS);

                        lenms = channel.CurrentSound.GetLength(TimeUnit.MS);
                    }
                }

                for (int i = 0; i < SelectionString.Length; ++i)
                {
                    bool disabled = !IsSelectionAvailable(SpeakerMode, i);

                    //Did not feel like writing this to be allocation heavy, so I did this
                    builder.WriteToBuffer('[');
                    builder.WriteToBuffer(selection == i ? (disabled ? '-' : 'X') : ' ');
                    builder.WriteToBuffer("] ");

                    if (disabled)
                    {
                        builder.WriteToBuffer("[N/A] ");
                    }

                    builder.WriteToBuffer(SelectionString[i]);

                    DrawText(builder.CurrentContent);

                    builder.Clear();
                }

                DrawText();

                builder.WriteToBuffer("Status: ");
                builder.WriteToBuffer(playing ? (paused ? "Paused" : "Playing") : "Stopped");

                DrawText(builder.CurrentContent);

                builder.Clear();

                DrawTime((int)ms, (int)lenms);

                Sleep(50);
            }
            while (!ShouldExit);
        }

        public override void Dispose()
        {
            sound1?.Dispose();
            sound2?.Dispose();

            base.Dispose();
        }

        private static float[] StereoReversedMatrix = new float[]
        {
            0f, 1f,
            1f, 0f
        };

        private static float[] RightOnlyFromStereoCenterMatrix = new float[]
        {
            0.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f
        };
    }
}
