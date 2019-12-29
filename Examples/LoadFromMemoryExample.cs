using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FmodAudio;

namespace Examples
{
    using Base;

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

                ProcessInput();

                System.Update();

                uint ms = 0, lenms = 0;
                bool isPlaying = false, paused = false;
                int channelsPlaying = 0;


                if (Channel != null)
                {
                    var library = Fmod.Library;

                    var res = library.ChannelGroup_IsPlaying(Channel.Handle, out isPlaying);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    res = library.ChannelGroup_GetPaused(Channel.Handle, out paused);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    res = library.Channel_GetPosition(Channel.Handle, out ms, TimeUnit.MS);
                    if (res != Result.Ok && res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                    {
                        res.CheckResult();
                    }

                    IntPtr shandle = default;

                    library.Channel_GetCurrentSound(Channel.Handle, out shandle);

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
            while (!ShouldExit);
        }

        public override void Dispose()
        {
            Sound1?.Dispose();
            Sound2?.Dispose();
            Sound3?.Dispose();

            base.Dispose();
        }

        private static void DrawTime(int ms, int totalms)
        {
            const string PreText = "Time: ";

            Span<char> buffer = stackalloc char[50];

            PreText.AsSpan().CopyTo(buffer);

            int count = RenderTime(ms, buffer.Slice(PreText.Length));

            buffer[PreText.Length + count] = '/';
            count += 1;

            count += RenderTime(totalms, buffer.Slice(PreText.Length + count));

            DrawText(buffer.Slice(0, PreText.Length + count));
        }

        private static int RenderTime(int ms, Span<char> buffer)
        {
            if (ms < 0)
                throw new ArgumentOutOfRangeException(nameof(ms));

            int total = 0;

            var seconds = Math.DivRem(ms, 1000, out ms);

            var minutes = Math.DivRem(seconds, 60, out seconds);

            int count;
            if (minutes != 0)
            {
                minutes.TryFormat(buffer, out count);

                buffer[count] = ':';
                buffer = buffer.Slice(count + 1);

                total += count + 1;
            }

            seconds.TryFormat(buffer, out count);

            buffer[count] = '.';
            buffer = buffer.Slice(count + 1);

            total += count + 1;

            ms.TryFormat(buffer, out count);

            total += count;

            return total;
        }
    }
}
