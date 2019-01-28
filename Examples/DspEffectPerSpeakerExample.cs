using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;
using FmodAudio.Dsp;

namespace Examples
{
    public class DspEffectPerSpeakerExample : Base.Example
    {
        private FmodSystem System;

        public override void Run()
        {
            FmodSystem system = new FmodSystem();
            System = system;

            TestVersion(system);

            //In this special case, we want to use stereo output and not worry about
            //varying matrix sizes depending on user speaker mode.
            system.SetSoftwareFormat(48_000, SpeakerMode.Stereo, 0);

            //Initialize FMOD
            system.Init(32);

            Sound sound = system.CreateSound(MediaPath("drumloop.wav"), Mode.Loop_Normal);

            Channel channel = system.PlaySound(sound);

            //Create DSP Effects
            DSP lowPass, highPass;

            lowPass = system.CreateDSPByType(DSPType.LowPass);
            lowPass.SetParameterFloat(0, 1000f); //Lowpass Cutoff
            lowPass.SetParameterFloat(1, 4f); //Lowpass Resonance

            highPass = system.CreateDSPByType(DSPType.HighPass);
            highPass.SetParameterFloat(0, 4000f); //Highpass Cutoff
            highPass.SetParameterFloat(1, 4f); //Highpass Resonance

            //Build the DSP Network

            ///When a sound is played, a subnetwork is set up in the DSP Network which looks like this.
            ///Wavetable is the drumloop sound, and it feeds its data from the right to left

            //[DSPHEAD]<------------[DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]
            DSP head, channelMixer;

            ChannelGroup masterGroup = system.MasterChannelGroup;
            head = masterGroup.GetDSP(ChannelControlDSPIndex.DspHead);
            channelMixer = head.GetInput(0).Item1;

            ///Now disconnect channel dsp head from the wavetable to look like this.
            //[DSPHEAD]             [DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]

            head.DisconnectFrom(channelMixer);

            ///Now connect the 2 effects to the channel DSP head.
            ///Store the 2 connections this makes to we can set their matrix later
            /*
                      [DSPLOWPASS]
                     /x           
            [DSPHEAD]             [DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]
                     \y           
                      [DSPHIGHPASS]
            */
            DSPConnection lowPassConnection, highPassConnection;

            lowPassConnection = head.AddInput(lowPass);
            highPassConnection = head.AddInput(highPass);

            ///Now connect the channelMixer to the 2 effects
            /*
                      [DSPLOWPASS]
                     /x          \
            [DSPHEAD]             [DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]
                     \y          /
                      [DSPHIGHPASS]
            */

            //Ignore the connections, we don't care about them
            lowPass.AddInput(channelMixer);
            highPass.AddInput(channelMixer);

            //Now the drumloop will be twice as loud, because it is being split into 2, then recombined at the end.
            //What we really want is th only feed the head<-lowpass through the left speaker for that effect, and
            //head<-highpass to the right speaker for that effect.
            //We can do that simply by setting the pan, or speaker matrix of the connections.
            /*
                      [DSPLOWPASS]
                     /x=1,0      \
            [DSPHEAD]             [DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]
                     \y=0,1      /
                      [DSPHIGHPASS]
            */

            Span<float> lowPassMatrix = stackalloc float[2 * 2] { 1, 0, 0, 0 };
            Span<float> highPassMatrix = stackalloc float[2 * 2] { 0, 0, 0, 1 };

            //Upgrade the signal coming from the channel mixer from Mono to Stereo. Otherwise the lowpass and highpass will get mono signals.
            channelMixer.SetChannelFormat(default, 0, SpeakerMode.Stereo);

            //Now set the above matrices.
            lowPassConnection.SetMixMatrix(lowPassMatrix, 2, 2);
            highPassConnection.SetMixMatrix(highPassMatrix, 2, 2);

            lowPass.Bypass = true;
            lowPass.Active = true;
            highPass.Bypass = true;
            highPass.Active = true;

            float pan = 0f;

            //Main Loop
            do
            {
                OnUpdate();

                bool lowpassBypass = lowPass.Bypass, highpassBypass = highPass.Bypass;

                if (!Commands.IsEmpty)
                {
                    while(Commands.TryDequeue(out Button btn))
                    {
                        switch (btn)
                        {
                            case Button.Action1:
                                lowpassBypass = !lowpassBypass;
                                lowPass.Bypass = lowpassBypass;
                                break;

                            case Button.Action2:
                                highpassBypass = !highpassBypass;
                                highPass.Bypass = highpassBypass;
                                break;

                            case Button.Left:
                                pan = Clamp(-1, 1, pan - 0.1f);
                                channel.SetPan(pan);
                                break;

                            case Button.Right:
                                pan = Clamp(-1, 1, pan + 0.1f);
                                channel.SetPan(pan);
                                break;

                            case Button.Quit:
                                goto Exit;
                        }

                    }
                }

                system.Update();

                DrawText("==================================================");
                DrawText("DSP Effect Per Speaker Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to toggle lowpass (left speaker)");
                DrawText("Press 2 to toggle highpass (right speaker)");
                DrawText("Press Left or Right Arrow to pan sound.");
                DrawText("Press Esc to quit.");
                DrawText();
                DrawText("Lowpass (left) is " + (lowpassBypass ? "inactive" : "active"));
                DrawText("Highpass (right) is " + (highpassBypass ? "inactive" : "active"));
                DrawText("Pan is " + pan.ToString("N1"));

                Sleep(50);

            } while (true);

            Exit:
            sound.Release();

            lowPass.Release();
            highPass.Release();

            system.Close();
        }

        public override void Dispose()
        {
            System.Release();
            base.Dispose();
        }
    }
}
