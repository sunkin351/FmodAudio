using FmodAudio;
using FmodAudio.Dsp;
using System;
using System.Runtime.InteropServices;

namespace Examples
{
    using Base;

    public class ConvolutionReverbExample : Example
    {
        public override void Run()
        {
            FmodSystem system = Fmod.CreateSystem();

            TestVersion(system);

            system.Init(32);

            ChannelGroup reverbGroup, mainGroup;

            reverbGroup = system.CreateChannelGroup("reverb");
            mainGroup = system.CreateChannelGroup("main");

            DSP reverbUnit = system.CreateDSPByType(DSPType.ConvolutionReverb);

            reverbGroup.AddDSP(ChannelControlDSPIndex.DSPTail, reverbUnit);

            Sound sound = system.CreateSound(MediaPath("standrews.wav"), Mode.Default | Mode.OpenOnly);

            sound.GetFormat(out _, out SoundFormat sFormat, out int irSoundChannels, out _);

            if (sFormat != SoundFormat.PCM16)
            {
                Console.WriteLine("Sound file's format is not PCM16.");
                Environment.Exit(-1);
            }

            {
                int irSoundLength = (int)sound.GetLength(TimeUnit.PCM);

                int irDataLength = (irSoundLength * irSoundChannels + 1) * sizeof(short);

                var irData = Marshal.AllocHGlobal(irDataLength);

                try
                {
                    sound.ReadData(irData, irDataLength);

                    const int ReverbParamIR = 0;
                    const int ReverbParamDry = 2;

                    reverbUnit.SetParameterData(ReverbParamIR, irData, (uint)irDataLength);
                    reverbUnit.SetParameterFloat(ReverbParamDry, -80f);
                }
                finally
                {
                    Marshal.FreeHGlobal(irData);
                }
            }

            sound.Release();

            sound = system.CreateSound(MediaPath("singing.wav"), Mode._3D | Mode.Loop_Normal);

            Channel channel = system.PlaySound(sound, mainGroup, true);

            DSP channelHead = channel.GetDSP(ChannelControlDSPIndex.DspHead);

            DSPConnection reverbConnection = reverbUnit.AddInput(channelHead, DSPConnectionType.Send);

            channel.Paused = false;

            float wetVolume = 1f, dryVolume = 1f;

            do
            {
                OnUpdate();

                if (!Commands.IsEmpty)
                {
                    while (Commands.TryDequeue(out Button btn))
                    {
                        switch (btn)
                        {
                            case Button.Left:
                                wetVolume = Math.Clamp(0, 1, wetVolume - 0.05f);
                                break;
                            case Button.Right:
                                wetVolume = Math.Clamp(0, 1, wetVolume + 0.05f);
                                break;
                            case Button.Up:
                                dryVolume = Math.Clamp(0, 1, dryVolume + 0.05f);
                                break;
                            case Button.Down:
                                dryVolume = Math.Clamp(0, 1, dryVolume - 0.05f);
                                break;
                            case Button.Quit:
                                goto Exit;
                        }

                    }
                }

                system.Update();

                reverbConnection.Mix = wetVolume;
                mainGroup.Volume = dryVolume;

                DrawText("==================================================");
                DrawText("Convolution Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText("Press Up and Down arrows to change dry mix");
                DrawText("Press Left and Right Arrows to change wet mix");
                DrawText($"Wet mix: {wetVolume}, Dry mix: {dryVolume}");
                DrawText("Press Esc to Quit.");

                Sleep(50);

            } while (true);

            Exit:
            sound.Dispose();
            mainGroup.Dispose();
            reverbGroup.RemoveDSP(reverbUnit);
            //reverbUnit.DisconnectAll(true, true); Disposing does this already
            reverbUnit.Dispose();
            reverbGroup.Dispose();
        }
    }
}
