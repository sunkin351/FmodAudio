using FmodAudio;
using System;

namespace Examples
{
    using Base;

    public class ChannelGroupsExample : Example
    {
        public override void Run()
        {
            FmodSystem system = Fmod.CreateSystem();

            TestVersion(system);

            Sound[] sounds = new Sound[6];
            Channel[] channels = new Channel[6];

            ChannelGroup A, B, Master;
            
            system.Init(32);

            sounds[0] = system.CreateSound(MediaPath("drumloop.wav"), Mode.Loop_Normal);
            sounds[1] = system.CreateSound(MediaPath("jaguar.wav"), Mode.Loop_Normal);
            sounds[2] = system.CreateSound(MediaPath("swish.wav"), Mode.Loop_Normal);
            sounds[3] = system.CreateSound(MediaPath("c.ogg"), Mode.Loop_Normal);
            sounds[4] = system.CreateSound(MediaPath("d.ogg"), Mode.Loop_Normal);
            sounds[5] = system.CreateSound(MediaPath("e.ogg"), Mode.Loop_Normal);

            A = system.CreateChannelGroup("Group A");
            B = system.CreateChannelGroup("Group B");
            Master = system.MasterChannelGroup;

            //Instead of being independent, set the group A and B to be children of the master group.
            Master.AddGroup(A, false);
            Master.AddGroup(B, false);

            for(int i = 0; i < 6; ++i)
            {
                channels[i] = system.PlaySound(sounds[i], i < 3 ? A : B);
            }

            //Change the volume of each group, just because we can! (reduce overall noise).
            A.Volume = 0.5f;
            B.Volume = 0.5f;

            do
            {
                OnUpdate();

                if (!Commands.IsEmpty)
                {
                    int i = 0;
                    while (i++ < 5 && Commands.TryDequeue(out var btn))
                    {
                        switch(btn)
                        {
                            case Button.Action1:
                                A.Mute = !A.Mute;
                                break;
                            case Button.Action2:
                                B.Mute = !B.Mute;
                                break;
                            case Button.Action3:
                                Master.Mute = !Master.Mute;
                                break;
                            case Button.Quit:
                                goto Exit;
                        }
                    }
                }
            } while (true);

            Exit:
            //A little fade out over 2 seconds
            float pitch = 1.0f, vol = 1.0f;

            for (int i = 0; i < 200; ++i)
            {
                Master.Volume = vol;
                Master.Pitch = pitch;

                vol -= 1.0f / 200;
                pitch -= 0.5f / 200;
            }

            //Shutdown
            foreach (var sound in sounds)
            {
                sound.Dispose();
            }

            A.Dispose();
            B.Dispose();

            system.Dispose();
        }
    }
}
