using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;

namespace Examples
{
    using Base;
    public class GaplessPlaybackExample : Example
    {
        private static readonly byte[] Notes = new byte[] // Note C = 0, Note D = 1, Note E = 2
        {
            2,   /* Ma-    */
            1,   /* ry     */
            0,   /* had    */
            1,   /* a      */
            2,   /* lit-   */
            2,   /* tle    */
            2,   /* lamb,  */
            2,   /* .....  */
            1,   /* lit-   */
            1,   /* tle    */
            1,   /* lamb,  */
            1,   /* .....  */
            2,   /* lit-   */
            2,   /* tle    */
            2,   /* lamb,  */
            2,   /* .....  */

            2,   /* Ma-    */
            1,   /* ry     */
            0,   /* had    */
            1,   /* a      */
            2,   /* lit-   */
            2,   /* tle    */
            2,   /* lamb,  */
            2,   /* its    */
            1,   /* fleece */
            1,   /* was    */
            2,   /* white  */
            1,   /* as     */
            0,   /* snow.  */
            0,   /* .....  */
            0,   /* .....  */
            0,   /* .....  */
        };

        private ulong clockStart = 0;
        private readonly Sound[] sounds = new Sound[3];
        private ChannelGroup channelGroup;

        public GaplessPlaybackExample() : base("Fmod Gapless Playback Example")
        {
            RegisterCommand(ConsoleKey.D1, () => channelGroup.Paused = !channelGroup.Paused);
            RegisterCommand(ConsoleKey.D2, () =>
            {
                float pitch = channelGroup.Pitch;

                for (int i = 0; i < 50; ++i)
                {
                    pitch += 0.01f;
                    channelGroup.Pitch = pitch;
                    System.Update();
                    Sleep(10);
                }
            });

            RegisterCommand(ConsoleKey.D3, () =>
            {
                float pitch = channelGroup.Pitch;

                for (int i = 0; i < 50; ++i)
                {
                    if (pitch <= 0.01f)
                    {
                        break;
                    }

                    pitch -= 0.01f;
                    channelGroup.Pitch = pitch;
                    System.Update();
                    Sleep(10);
                }
            });
        }

        public override void Initialize()
        {
            base.Initialize(); //Creates FmodSystem object

            System.Init(100);

            /*
                Load 3 sounds - these are just sine wave tones at different frequencies.  C, D and E on the musical scale.
            */
            sounds[0] = System.CreateSound(MediaPath("c.ogg"));
            sounds[1] = System.CreateSound(MediaPath("d.ogg"));
            sounds[2] = System.CreateSound(MediaPath("e.ogg"));

            /* 
                Create a channelgroup that the channels will play on.  We can use this channelgroup as our clock reference. 
                It also means we can pause and pitch bend the channelgroup, without affecting the offsets of the delays, because the channelgroup clock
                which the channels feed off, will be pausing and speeding up/slowing down and still keeping the children in sync.
            */
            channelGroup = System.CreateChannelGroup("Parent");
        }

        public unsafe override void Run()
        {
            /*
                Get information needed later for scheduling.  The mixer block size, and the output rate of the mixer.
            */

            System.GetDSPBufferSize(out uint dspBlockLen, out _);
            System.GetSoftwareFormat(out int outputRate, out _, out _);

            /*
                Play all the sounds at once! Space them apart with set delay though so that they sound like they play in order.
            */
            for (int i = 0; i < Notes.Length; ++i)
            {
                Sound s = sounds[Notes[i]];

                var channel = System.PlaySound(s, channelGroup, true);

                if (clockStart == 0)
                {
                    channel.GetDSPClock(out _, out clockStart);

                    /*
                     Start the sound into the future, by 2 mixer blocks worth. 
                     Should be enough to avoid the mixer catching up and hitting the clock value before we've finished setting up everything. 
                     Alternatively the channelgroup we're basing the clock on could be paused to stop it ticking.
                    */
                    clockStart += dspBlockLen * 2;          
                }
                else
                {
                    uint slen = s.GetLength(TimeUnit.PCM);  /* Get the length of the sound in samples. */
                    s.GetDefaults(out float freq, out _);   /* Get the default frequency that the sound was recorded at. */
                    slen = (uint)(slen / freq * outputRate);/* Convert the length of the sound to 'output samples' for the output timeline. */
                    clockStart += slen;                     /* Place the sound clock start time to this value after the last one. */
                }

                channel.SetDelay(clockStart, 0, false);     /* Schedule the channel to start in the future at the newly calculated channelgroup clock value. */

                channel.Paused = false;                     /* Unpause the sound.  Note that you won't hear the sounds, they are scheduled into the future. */
            }

            /*
                Main loop.
            */
            do
            {
                OnUpdate();

                ProcessInput();

                System.Update();

                System.GetChannelsPlaying(out int playing, out _);

                DrawText("==================================================");
                DrawText("Gapless Playback example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to toggle pause");
                DrawText("Press 2 to increase pitch");
                DrawText("Press 3 to decrease pitch");
                DrawText("Press Esc to quit");
                DrawText();
                DrawText($"Channels Playing {playing} : {(channelGroup.Paused ? "Paused" : (channelGroup.IsPlaying ? "Playing" : "Stopped"))}");

                Sleep(50);
            }
            while (!ShouldExit);
        }

        public override void Dispose()
        {
            foreach (var sound in sounds)
            {
                sound?.Dispose();
            }

            channelGroup?.Dispose();

            base.Dispose();
        }
    }
}
