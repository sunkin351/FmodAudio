using FmodAudio;
using System;
using System.Numerics;

namespace Examples
{
    using Base;

    public class _3DExample : Example
    {
        const int InterfaceUpdateTime = 50;
        const float DistanceFactor = 1.0f;
        
        private readonly char[] ss = "|.............<1>......................<2>.......|".ToCharArray();

        float T = 0.0f;
        Vector3 LastPos = default;
        
        public override void Run()
        {
            //Create a System Object
            FmodSystem system = Fmod.CreateSystem();

            TestVersion(system);

            Sound s1, s2, s3;
            Channel c1, c2, c3 = null;
            bool autoMove = true;
            Vector3 listenerPos = new Vector3() { Z = -1.0f * DistanceFactor };

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
                Vector3 pos = default, vel = default;

                pos.X = -10.0f * DistanceFactor;

                c1 = system.PlaySound(s1, paused: true);
                c1.Set3DAttributes(in pos, in vel, default);
                c1.Paused = false;

                pos.X = 15.0f * DistanceFactor;

                c2 = system.PlaySound(s2, null, true);
                c2.Set3DAttributes(in pos, in vel, default);
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
                                autoMove = !autoMove;
                                break;
                            case Button.Quit:
                                goto BreakLoop;
                        }
                    }
                }

                // ==========================================================================================
                // UPDATE THE LISTENER
                // ==========================================================================================

                Vector3 forward = new Vector3() { Z = 1f }, up = new Vector3() { Y = 1f }, vel = default;

                if (autoMove)
                {
                    listenerPos.X = MathF.Sin(this.T * 0.05f) * 24 * DistanceFactor;
                }

                vel.X = (listenerPos.X - this.LastPos.X) * (1000 / InterfaceUpdateTime);
                vel.Y = (listenerPos.Y - this.LastPos.Y) * (1000 / InterfaceUpdateTime);
                vel.Z = (listenerPos.Z - this.LastPos.Z) * (1000 / InterfaceUpdateTime);

                this.LastPos = listenerPos;

                system.Set3DListenerAttributes(0, in listenerPos, in vel, in forward, in up);

                this.T += 30 * (1.0f / InterfaceUpdateTime);

                system.Update();

                ss.AsSpan().CopyTo(s);
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
            s1.Dispose();
            s2.Dispose();
            s3.Dispose();

            system.Dispose();
        }

        public override string Title => "Fmod 3D Example";
    }
}
