using System;
using System.Collections.Generic;
using System.Text;

using FmodAudio;
using FmodAudio.DigitalSignalProcessing;

namespace Examples
{
    using Base;

    public class DspEffectPerSpeakerExample : Example
    {
        Sound sound;
        Channel channel;
        Dsp lowPass, highPass;
        float pan = 0f;

        public DspEffectPerSpeakerExample() : base("Fmod DSP Effects per Speaker Example")
        {
            RegisterCommand(ConsoleKey.D1, () => lowPass.Bypass = !lowPass.Bypass);
            RegisterCommand(ConsoleKey.D2, () => highPass.Bypass = !highPass.Bypass);
            RegisterCommand(ConsoleKey.LeftArrow, () =>
            {
                var tmp = Math.Clamp(pan - 0.1f, -1, 1);
                pan = tmp;
                channel.SetPan(tmp);
            });
            RegisterCommand(ConsoleKey.RightArrow, () =>
            {
                var tmp = Math.Clamp(pan + 0.1f, -1, 1);
                pan = tmp;
                channel.SetPan(tmp);
            });
        }

        public override void Initialize()
        {
            base.Initialize();

            //In this special case, we want to use stereo output and not worry about
            //varying matrix sizes depending on user speaker mode.
            System.SetSoftwareFormat(48_000, SpeakerMode.Stereo, 0);

            //Initialize FMOD
            System.Init(32);

            sound = System.CreateSound(MediaPath("drumloop.wav"), Mode.Loop_Normal);

            channel = System.PlaySound(sound);

            //Create DSP Effects

            lowPass = System.CreateDSPByType(DSPType.LowPass);
            lowPass.SetParameterFloat(0, 1000f); //Lowpass Cutoff
            lowPass.SetParameterFloat(1, 4f); //Lowpass Resonance

            highPass = System.CreateDSPByType(DSPType.HighPass);
            highPass.SetParameterFloat(0, 4000f); //Highpass Cutoff
            highPass.SetParameterFloat(1, 4f); //Highpass Resonance

            //Build the DSP Network

            ///When a sound is played, a subnetwork is set up in the DSP Network which looks like this.
            ///Wavetable is the drumloop sound, and it feeds its data from the right to left

            //[DSPHEAD]<------------[DSPCHANNELMIXER]<------------[CHANNEL HEAD]<------------[WAVETABLE - DRUMLOOP.WAV]
            Dsp head, channelMixer;

            ChannelGroup masterGroup = System.MasterChannelGroup;
            head = masterGroup.GetDSP(ChannelControlDSPIndex.DspHead);
            channelMixer = head.GetInput(0).Dsp;

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
            DspConnection lowPassConnection, highPassConnection;

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

            float[] lowPassMatrix = new float[2 * 2] { 1, 0, 0, 0 };
            float[] highPassMatrix = new float[2 * 2] { 0, 0, 0, 1 };

            //Upgrade the signal coming from the channel mixer from Mono to Stereo. Otherwise the lowpass and highpass will get mono signals.
            channelMixer.SetChannelFormat(default, 0, SpeakerMode.Stereo);

            //Now set the above matrices.
            lowPassConnection.SetMixMatrix(lowPassMatrix, 2, 2);
            highPassConnection.SetMixMatrix(highPassMatrix, 2, 2);

            lowPass.Bypass = true;
            lowPass.Active = true;
            highPass.Bypass = true;
            highPass.Active = true;
        }

        public override void Run()
        {
            //Main Loop
            do
            {
                OnUpdate();

                System.Update();

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
                DrawText("Lowpass (left) is " + (lowPass.Bypass ? "inactive" : "active"));
                DrawText("Highpass (right) is " + (highPass.Bypass ? "inactive" : "active"));
                DrawText("Pan is " + pan.ToString("N1"));

                Sleep(50);
            }
            while (!ShouldEndExample);
        }

        public override void Dispose()
        {
            if (sound != default)
                sound.Dispose();

            if (lowPass != default)
                lowPass.Dispose();

            if (highPass != default)
                highPass.Dispose();

            base.Dispose();
        }
    }
}
