using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;
using FmodAudio.Dsp;

namespace Examples
{
    using Base;

    public class GranularSynthExample : Example
    {
        string[] soundNames = new string[6] 
        {
            MediaPath("granular/truck_idle_off_01.wav"),
            MediaPath("granular/truck_idle_off_02.wav"),
            MediaPath("granular/truck_idle_off_03.wav"),
            MediaPath("granular/truck_idle_off_04.wav"),
            MediaPath("granular/truck_idle_off_05.wav"),
            MediaPath("granular/truck_idle_off_06.wav"),
        };

        Random RNG = new Random();

        Sound[] Sounds = new Sound[6];

        Channel[] Channels = new Channel[2];

        bool Paused;

        int Slot, OutputRate;

        public GranularSynthExample() : base("Fmod Granular Synth Example")
        {
            RegisterCommand(ConsoleKey.D1, () =>
            {
                Paused = !Paused;

                System.MasterChannelGroup.Paused = Paused;
            });
        }

        public override void Initialize()
        {
            Paused = false;

            Slot = 0;

            base.Initialize();

            System.Init(100);

            System.GetSoftwareFormat(out OutputRate, out _, out _);

            for (int i = 0; i < Sounds.Length; ++i)
            {
                Sounds[i] = System.CreateSound(soundNames[i], Mode.IgnoreTags);
            }

            QueueNextSound();
            QueueNextSound();
        }

        public override void Run()
        {
            DrawText("==================================================");
            DrawText("Granular Synthesis SetDelay Example.");
            DrawText("Copyright (c) Firelight Technologies 2004-2018.");
            DrawText("==================================================");
            DrawText();
            DrawText("Press 1 to Toggle Pause");
            DrawText("Press Esc to Quit");
            DrawText();

            SetConsoleUpdateStart();

            do
            {
                OnUpdate();

                System.Update();

                var res = Fmod.Library.ChannelGroup_IsPlaying(Channels[Slot].Handle, out bool isPlaying);

                if (res != Result.Ok && res != Result.Err_Invalid_Handle)
                {
                    res.CheckResult();
                }

                if (!isPlaying && !Paused)
                {
                    QueueNextSound();
                }

                DrawText($"Channels are {(Paused ? "Paused" : "Playing")}");

                Sleep(10);
            }
            while (!ShouldExit);
        }

        public override void Dispose()
        {
            foreach(var sound in Sounds)
            {
                sound?.Dispose();
            }

            base.Dispose();
        }

        private void QueueNextSound()
        {
            Channel playingChannel = Channels[Slot ^ 1];

            int newIndex = RNG.Next(0, Sounds.Length);

            Sound newSound = Sounds[newIndex];

            var newChannel = System.PlaySound(newSound, paused: true);

            if (playingChannel != null)
            {
                ulong startDelay;
                uint soundLength;
                float soundFreq;
                Sound PlayingSound;

                playingChannel.GetDelay(out startDelay, out _, out _);

                PlayingSound = playingChannel.CurrentSound;

                soundLength = PlayingSound.GetLength(TimeUnit.PCM);

                soundFreq = playingChannel.Frequency;

                soundLength *= (uint)OutputRate;

                soundLength /= (uint)soundFreq;

                startDelay += soundLength;

                newChannel.SetDelay(startDelay, 0);
            }
            else
            {
                uint bufferLen;
                ulong startDelay;

                System.GetDSPBufferSize(out bufferLen, out _);

                newChannel.GetDSPClock(out _, out startDelay);

                startDelay += 2 * bufferLen;

                newChannel.SetDelay(startDelay, 0);
            }

            float val = newChannel.Frequency;
            float variation = (float)RNG.NextDouble() * 2f - 1f;

            val *= 1f - (variation * 0.2f);

            newChannel.Frequency = val;

            val = newChannel.Volume;
            variation = (float)RNG.NextDouble();

            val *= 1f - (variation * 0.2f);
            newChannel.Volume = val;

            newChannel.Paused = false;

            Channels[Slot] = newChannel;

            Slot ^= 1;
        }
    }
}
