﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FmodAudio;

namespace Examples
{
    using Base;
    using FmodAudio.Base;

    public class LoadFromMemoryExample : Example
    {
        Sound Sound1, Sound2, Sound3;
        Channel Channel;

        public LoadFromMemoryExample() : base("Fmod Load from Memory Example")
        {
            RegisterCommand(ConsoleKey.D1, () => Channel = System.PlaySound(Sound1));
            RegisterCommand(ConsoleKey.D2, () => Channel = System.PlaySound(Sound2));
            RegisterCommand(ConsoleKey.D3, () => Channel = System.PlaySound(Sound3));
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            Sound1 = ReadSound(MediaPath("drumloop.wav"), Mode.Loop_Off);

            Sound2 = ReadSound(MediaPath("jaguar.wav"));

            Sound3 = ReadSound(MediaPath("swish.wav"));

            Sound ReadSound(string file, Mode mode = Mode.Default)
            {
                byte[] fileData = File.ReadAllBytes(file);

                var soundInfo = new CreateSoundInfo()
                {
                    Length = (uint)fileData.Length
                };

                return System.CreateSound(fileData, mode | Mode.OpenMemory, soundInfo);
            }
        }

        public override void Run()
        {
            do
            {
                OnUpdate();

                System.Update();

                uint ms = 0, lenms = 0;
                FmodBool isPlaying = false, paused = false;
                int channelsPlaying = 0;


                if (Channel != null)
                {
                    var library = Fmod.Library;

                    var res = library.Channel_IsPlaying(Channel, out isPlaying);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    res = library.Channel_GetPaused(Channel, out paused);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    res = library.Channel_GetPosition(Channel, out ms, TimeUnit.MS);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    library.Channel_GetCurrentSound(Channel, out SoundHandle shandle);

                    if (shandle != default)
                    {
                        res = library.Sound_GetLength(shandle, out lenms, TimeUnit.MS);
                        if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                        {
                            res.CheckResult();
                        }
                    }
                }

                System.GetChannelsPlaying(out channelsPlaying, out _);

                DrawText("==================================================");
                DrawText("Load From Memory Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2019.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to play a mono sound (drumloop)");
                DrawText("Press 2 to play a mono sound (jaguar)");
                DrawText("Press 3 to play a stereo sound (swish)");
                DrawText();
                DrawTime((int)ms, (int)lenms);
                DrawText($"Channels Playing: {channelsPlaying}");

            }
            while (!ShouldEndExample);
        }

        public override void Dispose()
        {
            if (Sound1 != default)
            {
                Sound1.Dispose();
            }

            if (Sound2 != default)
            {
                Sound2.Dispose();
            }

            if (Sound3 != default)
            {
                Sound3.Dispose();
            }

            base.Dispose();
        }
    }
}
