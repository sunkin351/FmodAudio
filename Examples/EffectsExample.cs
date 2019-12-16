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
        Sound sound;
        Channel channel;

        DSP dspLowpass, dspHighpass, dspEcho, dspFlange;

        public EffectsExample() : base("Fmod Effects Example")
        {
            RegisterCommand(ConsoleKey.Spacebar, () =>
            {
                if (channel != null)
                {
                    channel.Paused = !channel.Paused;
                }
            });

            RegisterCommand(ConsoleKey.D1, () => dspLowpass.Bypass = !dspLowpass.Bypass);
            RegisterCommand(ConsoleKey.D2, () => dspHighpass.Bypass = !dspHighpass.Bypass);
            RegisterCommand(ConsoleKey.D3, () => dspEcho.Bypass = !dspEcho.Bypass);
            RegisterCommand(ConsoleKey.D4, () => dspFlange.Bypass = !dspFlange.Bypass);
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            sound = System.CreateSound(MediaPath("drumloop.wav"));

            channel = System.PlaySound(sound);

            dspLowpass = System.CreateDSPByType(DSPType.LowPass);
            dspHighpass = System.CreateDSPByType(DSPType.HighPass);
            dspEcho = System.CreateDSPByType(DSPType.Echo);
            dspFlange = System.CreateDSPByType(DSPType.Flange);

            dspLowpass.Bypass = true;
            dspHighpass.Bypass = true;
            dspEcho.Bypass = true;
            dspFlange.Bypass = true;
        }

        public override void Run()
        {
            var master = System.MasterChannelGroup;

            master.AddDSP(0, dspLowpass);
            master.AddDSP(0, dspHighpass);
            master.AddDSP(0, dspEcho);
            master.AddDSP(0, dspFlange);

            do
            {
                OnUpdate();

                ProcessInput();

                System.Update();
                bool Paused = true;

                if (channel != null)
                {
                    var res = Fmod.Library.ChannelGroup_GetPaused(channel.Handle, out Paused);

                    if (res != Result.Ok)
                    {
                        Paused = true;

                        if (res != Result.Err_Invalid_Handle && res != Result.Err_Channel_Stolen)
                        {
                            res.CheckResult();
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
            while (!ShouldExit);
            
            master.RemoveDSP(dspLowpass);
            master.RemoveDSP(dspHighpass);
            master.RemoveDSP(dspEcho);
            master.RemoveDSP(dspFlange);
        }

        public override void Dispose()
        {
            dspLowpass?.Dispose();
            dspHighpass?.Dispose();
            dspEcho?.Dispose();
            dspFlange?.Dispose();

            sound?.Dispose();

            base.Dispose();
        }
    }
}
