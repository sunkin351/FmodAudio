using FmodAudio;
using System;

namespace Examples
{
    using Base;

    public class ChannelGroupsExample : Example
    {
        private readonly Sound[] sounds = new Sound[6];
        private readonly Channel[] channels = new Channel[6];
        
        private ChannelGroup A, B, Master;

        public ChannelGroupsExample() : base("Fmod Channel Groups Example")
        {
            RegisterCommand(ConsoleKey.D1, () => A.Mute = !A.Mute);
            RegisterCommand(ConsoleKey.D2, () => B.Mute = !B.Mute);
            RegisterCommand(ConsoleKey.D3, () => Master.Mute = !Master.Mute);
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            sounds[0] = System.CreateSound(MediaPath("drumloop.wav"), Mode.Loop_Normal);
            sounds[1] = System.CreateSound(MediaPath("jaguar.wav"), Mode.Loop_Normal);
            sounds[2] = System.CreateSound(MediaPath("swish.wav"), Mode.Loop_Normal);
            sounds[3] = System.CreateSound(MediaPath("c.ogg"), Mode.Loop_Normal);
            sounds[4] = System.CreateSound(MediaPath("d.ogg"), Mode.Loop_Normal);
            sounds[5] = System.CreateSound(MediaPath("e.ogg"), Mode.Loop_Normal);

            A = System.CreateChannelGroup("Group A");
            B = System.CreateChannelGroup("Group B");
            Master = System.MasterChannelGroup;

            //Instead of being independent, set the group A and B to be children of the master group.
            Master.AddGroup(A, false);
            Master.AddGroup(B, false);

            for (int i = 0; i < 6; ++i)
            {
                channels[i] = System.PlaySound(sounds[i], i < 3 ? A : B);
            }

            //Change the volume of each group, just because we can! (reduce overall noise).
            A.Volume = 0.5f;
            B.Volume = 0.5f;
        }

        public override void Run()
        {
            do
            {
                OnUpdate();

                ProcessInput();
                
                System.Update();

                System.GetChannelsPlaying(out int channelsPlaying, out _);

                DrawText("==================================================");
                DrawText("Channel Groups Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Group A : drumloop.wav, jaguar.wav, swish.wav");
                DrawText("Group B : c.ogg, d.ogg, e.ogg");
                DrawText();
                DrawText("Press 1 to toggle mute group A");
                DrawText("Press 2 to toggle mute group B");
                DrawText("Press 3 to toggle mute master group");
                DrawText("Press Esc to quit");
                DrawText();
                DrawText($"Channels playing: {channelsPlaying}");

                Sleep(50);
            }
            while (!ShouldExit);

            //A little fade out over 2 seconds
            float pitch = 1.0f, vol = 1.0f;

            for (int i = 0; i < 200; ++i)
            {
                Master.Volume = vol;
                Master.Pitch = pitch;

                vol -= 1.0f / 200;
                pitch -= 0.5f / 200;

                System.Update();

                Sleep(10);
            }
        }

        public override void Dispose()
        {
            foreach (var sound in sounds)
            {
                sound?.Dispose();
            }

            A?.Dispose();
            B?.Dispose();

            base.Dispose();
        }
    }
}
