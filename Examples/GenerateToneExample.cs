using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;
using FmodAudio.DigitalSignalProcessing;

namespace Examples
{
    using Base;
    public class GenerateToneExample : Example
    {
        Dsp dsp;
        Channel channel = null;

        public GenerateToneExample() : base("Fmod Generate Tone Example")
        {
            RegisterCommand(ConsoleKey.D1, () =>
            {
                if (channel != null)
                {
                    channel.Stop();
                }

                dsp.SetParameterInt(0, 0);

                channel = System.PlayDsp(dsp, paused: true);

                channel.Volume = 0.5f;

                channel.Paused = false;
            });

            RegisterCommand(ConsoleKey.D2, () =>
            {
                if (channel != null)
                {
                    channel.Stop();
                }

                dsp.SetParameterInt(0, 1);

                channel = System.PlayDsp(dsp, paused: true);

                channel.Volume = 0.125f;

                channel.Paused = false;
            });

            RegisterCommand(ConsoleKey.D3, () =>
            {
                if (channel != null)
                {
                    channel.Stop();
                }

                dsp.SetParameterInt(0, 2);

                channel = System.PlayDsp(dsp, paused: true);

                channel.Volume = 0.125f;

                channel.Paused = false;
            });

            RegisterCommand(ConsoleKey.D4, () =>
            {
                if (channel != null)
                {
                    channel.Stop();
                }

                dsp.SetParameterInt(0, 3);

                channel = System.PlayDsp(dsp, paused: true);

                channel.Volume = 0.5f;

                channel.Paused = false;
            });

            RegisterCommand(ConsoleKey.Spacebar, () =>
            {
                if (channel != null)
                {
                    channel.Stop();
                    channel = null;
                }
            });

            RegisterCommand(ConsoleKey.UpArrow, () =>
            {
                if (channel != null)
                {
                    float vol = channel.Volume + 0.1f;
                    if (vol > 1.0f)
                    {
                        vol = 1.0f;
                    }
                    channel.Volume = vol;
                }
            });

            RegisterCommand(ConsoleKey.DownArrow, () =>
            {
                if (channel != null)
                {
                    float vol = channel.Volume - 0.1f;
                    if (vol < 0f)
                    {
                        vol = 0f;
                    }
                    channel.Volume = vol;
                }
            });

            RegisterCommand(ConsoleKey.LeftArrow, () =>
            {
                if (channel != null)
                {
                    channel.Frequency -= 500f;
                }
            });

            RegisterCommand(ConsoleKey.RightArrow, () =>
            {
                if (channel != null)
                {
                    channel.Frequency += 500f;
                }
            });
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            dsp = System.CreateDSPByType(DSPType.Oscillator);

            dsp.SetParameterFloat(1, 440.0f);
        }

        public override void Run()
        {
            do
            {
                OnUpdate();

                System.Update();

                float freq = 0, volume = 0;
                bool playing = false;

                if (channel != null)
                {
                    freq = channel.Frequency;
                    volume = channel.Volume;
                    playing = channel.IsPlaying;
                }

                DrawText("==================================================");
                DrawText("Generate Tone Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText("");
                DrawText("Press 1 to play a sine wave");
                DrawText("Press 2 to play a square wave");
                DrawText("Press 3 to play a saw wave");
                DrawText("Press 4 to play a triangle wave");
                DrawText("Press Space to stop the channel");
                DrawText("Press Up and Down Arrows to change volume");
                DrawText("Press Left and Right Arrows to change frequency");
                DrawText("Press Esc to quit");
                DrawText("");
                DrawText($"Channel is {(playing ? "playing" : "stopped")}");
                DrawText("Volume: " + volume.ToString("N2"));
                DrawText("Frequency: " + freq.ToString("N2"));

                Sleep(50);
            }
            while (!ShouldEndExample);
        }

        public override void Dispose()
        {
            if (dsp != default)
                dsp.Dispose();

            base.Dispose();
        }
    }
}
