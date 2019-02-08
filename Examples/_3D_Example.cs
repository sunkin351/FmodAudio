using FmodAudio;
using System;

namespace Examples
{
    using Base;

    public class _3DExample : Example
    {
        const int InterfaceUpdateTime = 50;
        const float DistanceFactor = 1.0f;
        
        private readonly char[] ss = "|.............<1>......................<2>.......|".ToCharArray();

        float T = 0.0f;
        Vector LastPos = default;
        
        public override void Run()
        {
            //Create a System Object
            FmodSystem system = new FmodSystem();

            TestVersion(system);

            Sound s1, s2, s3;
            Channel c1 = null, c2 = null, c3 = null;
            bool listenerFlag = true;
            Vector listenerPos = new Vector() { Z = -1.0f * DistanceFactor };

            //System object Initialization
            system.Init(32);

            //Set the distance Units (Meters/Feet etc)
            system.Set3DSettings(1.0f, DistanceFactor, 1.0f);

            {
                //Load some sounds
                float min = 0.5f * DistanceFactor, max = 5000.0f * DistanceFactor;

                s1 = system.CreateSound(MediaPath("drumloop.wav"), Mode._3D | Mode.Loop_Normal);
                s1.Set3DMinMaxDistance(min, max);

                s2 = system.CreateSound(MediaPath("jaguar.wav"), Mode._3D | Mode.Loop_Normal);
                s2.Set3DMinMaxDistance(min, max);
            }

            s3 = system.CreateSound(MediaPath("swish.wav"), Mode._2D);

            {
                //Play sounds at certain positions
                Vector pos = new Vector() { X = -10.0f * DistanceFactor }, vel = default;

                c1 = system.PlaySound(s1, null, true);
                c1.Set3DAttributes(ref pos, ref vel);
                c1.Paused = false;

                pos.X = 15.0f * DistanceFactor;

                c2 = system.PlaySound(s2, null, true);
                c2.Set3DAttributes(ref pos, ref vel);
                c2.Paused = false;
            }

            char[] s = new char[ss.Length];

            //Main Loop
            do
            {
                OnUpdate();

                if (!this.Commands.IsEmpty)
                {
                    while (this.Commands.TryDequeue(out Button button))
                    {
                        switch (button)
                        {
                            case Button.Action1:
                                c1.Paused = !c1.Paused;
                                break;
                            case Button.Action2:
                                c2.Paused = !c2.Paused;
                                break;
                            case Button.Action3:
                                if (c3 == null || !c3.IsPlaying)
                                    c3 = system.PlaySound(s3);
                                break;
                            case Button.More:
                                listenerFlag = !listenerFlag;
                                break;
                            case Button.Quit:
                                goto BreakLoop;
                        }
                    }
                }

                // ==========================================================================================
                // UPDATE THE LISTENER
                // ==========================================================================================

                Vector forward = new Vector() { Z = 1 }, up = new Vector() { Y = 1.0f }, vel = default;

                if (listenerFlag)
                {
                    listenerPos.X = (float)Math.Sin(this.T * 0.05f) * 24 * DistanceFactor;
                }

                vel.X = (listenerPos.X - this.LastPos.X) * (1000 / InterfaceUpdateTime);
                vel.Y = (listenerPos.Y - this.LastPos.Y) * (1000 / InterfaceUpdateTime);
                vel.Z = (listenerPos.Z - this.LastPos.Z) * (1000 / InterfaceUpdateTime);

                this.LastPos = listenerPos;

                system.Set3DListenerAttributes(0, ref listenerPos, ref vel, ref forward, ref up);

                this.T += 30 * (1.0f / InterfaceUpdateTime);

                system.Update();

                ss.CopyTo(s, 0);
                s[(int)(listenerPos.X / DistanceFactor) + 25] = 'L';

                DrawText("==================================================");
                DrawText("3D Example");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to toggle sound 1 (16bit Mono 3D)");
                DrawText("Press 2 to toggle sound 2 (8bit Mono 3D)");
                DrawText("Press 3 to play a sound (16bit Stereo 2D)");
                DrawText("Press Space to toggle listener auto movement");
                DrawText();
                DrawText(s);
                
                Sleep(InterfaceUpdateTime - 1);
            } while (true);

            BreakLoop:
            s1.Release();
            s2.Release();
            s3.Release();

            system.Release();
        }

        public override string Title => "Fmod 3D Example";
    }
}
