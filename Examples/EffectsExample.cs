using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;
using FmodAudio.Dsp;

namespace Examples
{
    using Base;
    public class EffectsExample : Example
    {
        readonly FmodSystem System;

        public override string Title => "Fmod Effects Example";

        public EffectsExample()
        {
            System = Fmod.CreateSystem();

            TestVersion(System);
        }

        public override void Run()
        {
            System.Init(32);

            Sound sound = System.CreateSound(MediaPath("drumloop.wav"));

            Channel channel = System.PlaySound(sound);

            var master = System.MasterChannelGroup;

            var dspLowpass = System.CreateDSPByType(DSPType.LowPass);
            var dspHighpass = System.CreateDSPByType(DSPType.HighPass);
            var dspEcho = System.CreateDSPByType(DSPType.Echo);
            var dspFlange = System.CreateDSPByType(DSPType.Flange);

            dspLowpass.Bypass = true;
            dspHighpass.Bypass = true;
            dspEcho.Bypass = true;
            dspFlange.Bypass = true;

            master.AddDSP(0, dspLowpass);
            master.AddDSP(0, dspHighpass);
            master.AddDSP(0, dspEcho);
            master.AddDSP(0, dspFlange);

            while (true)
            {
                OnUpdate();

                if (!Commands.IsEmpty)
                {
                    while (Commands.TryDequeue(out var command))
                    {
                        switch(command)
                        {
                            case Button.More:
                                if (channel != null)
                                {
                                    channel.Paused = !channel.Paused;
                                }
                                break;
                            case Button.Action1:
                                dspLowpass.Bypass = !dspLowpass.Bypass;
                                break;
                            case Button.Action2:
                                dspHighpass.Bypass = !dspHighpass.Bypass;
                                break;
                            case Button.Action3:
                                dspEcho.Bypass = !dspEcho.Bypass;
                                break;
                            case Button.Action4:
                                dspFlange.Bypass = !dspFlange.Bypass;
                                break;
                            case Button.Quit:
                                goto Exit;
                        }
                    }
                }

                System.Update();
                bool Paused = true;

                if (channel != null)
                {
                    try
                    {
                        Paused = channel.Paused;
                    }
                    catch(FmodException exception)
                    {
                        if (exception.Result != Result.Err_Invalid_Handle && exception.Result != Result.Err_Channel_Stolen)
                        {
                            throw;
                        }
                    }
                }

                char lp, hp, e, f;

                lp = dspLowpass.Bypass ? ' ' : 'x';
                hp = dspHighpass.Bypass ? ' ' : 'x';
                e = dspEcho.Bypass ? ' ' : 'x';
                f = dspFlange.Bypass ? ' ' : 'x';

                DrawText("==================================================");
                DrawText("Effects Example");
                DrawText();
                DrawText("==================================================");
                DrawText();
                DrawText("Press Space to toggle pause sound");
                DrawText("Press 1 to toggle dsp Lowpass effect");
                DrawText("Press 2 to toggle dsp Highpass effect");
                DrawText("Press 3 to toggle dsp echo effect");
                DrawText("Press 4 to toggle dsp flange effect");
                DrawText("Press Esc to quit");
                DrawText();
                DrawText($"{(Paused ? "Paused" : "Playing")} : lowpass[{lp}] highpass[{hp}] echo[{e}] flange[{f}]");

                Sleep(50);
            }

            Exit:

            master.RemoveDSP(dspLowpass);
            master.RemoveDSP(dspHighpass);
            master.RemoveDSP(dspEcho);
            master.RemoveDSP(dspFlange);

            dspLowpass.Dispose();
            dspHighpass.Dispose();
            dspEcho.Dispose();
            dspFlange.Dispose();

            sound.Dispose();

            System.Dispose();
        }
    }
}
