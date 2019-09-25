using FmodAudio;
using System;

namespace Examples
{
    using Base;

    public class RecordExample : Example
    {
        const int DriverIndex = 0;
        const int Drift_MS = 1;
        const int Latency_MS = 50;

        uint LastRecordPos = 0;
        uint MinRecordDelta = uint.MaxValue;
        uint LastPlayPos = 0;
        
        public override void Run()
        {
            FmodSystem system = Fmod.CreateSystem();

            TestVersion(system);
            Channel channel = null;
            Sound sound;
            uint SamplesRecorded = 0;
            uint SamplesPlayed = 0;
            bool dspEnabled = false;

            system.Init(32);

            system.GetRecordDriverCount(out _, out int numDrivers);

            if (numDrivers == 0)
            {
                Console.WriteLine("No recording devices found/plugged in!  Aborting.");
                return;
            }
            
            RecordDriverInfo recordDriverInfo = system.GetRecordDriverInfo(DriverIndex);

            int nativeRate = recordDriverInfo.SystemRate;
            int nativeChannels = recordDriverInfo.SpeakerModeChannels;

            uint driftThreshold = (uint)(nativeRate * Drift_MS / 1000);
            uint desiredLatency = (uint)(nativeRate * Latency_MS / 1000);
            uint adjustedLatency = desiredLatency;
            int actualLatency = (int)desiredLatency;

            CreateSoundInfo info = new CreateSoundInfo()
            {
                ChannelCount = nativeChannels,
                Format = SoundFormat.PCM16,
                DefaultFrequency = nativeRate,
                Length = (uint)(nativeRate * sizeof(short) * nativeChannels)
            };

            sound = system.CreateSoundOpenUser(Mode.Loop_Normal, info);

            system.RecordStart(DriverIndex, sound, true);

            uint SoundLength = sound.GetLength(TimeUnit.PCM);
            
            do
            {
                OnUpdate();

                if (!Commands.IsEmpty)
                {
                    while(Commands.TryDequeue(out Button button))
                    {
                        switch (button)
                        {
                            case Button.Quit:
                                goto Exit;
                            case Button.Action1:
                                dspEnabled = !dspEnabled;
                                ReverbProperties prop = dspEnabled ? Preset.ConcertHall : Preset.Off;
                                system.SetReverbProperties(0, ref prop);
                                break;
                        }

                    }
                }

                system.Update();
                uint recordPos = 0;
                try
                {
                    recordPos = system.GetRecordPosition(DriverIndex);
                }
                catch(FmodException e)
                {
                    var res = e.Result;
                    if (!res.HasValue || res.Value != Result.Err_Record_Disconnected)
                    {
                        throw;
                    }
                }

                uint recordDelta = (recordPos >= LastRecordPos) ? recordPos - LastRecordPos : recordPos + SoundLength - LastRecordPos;
                LastRecordPos = recordPos;
                SamplesRecorded += recordDelta;

                if (recordDelta > 0 && recordDelta < MinRecordDelta)
                {
                    MinRecordDelta = recordDelta;
                    adjustedLatency = (recordDelta <= desiredLatency) ? desiredLatency : recordDelta;
                }

                if (channel == null && SamplesRecorded >= adjustedLatency)
                {
                    channel = system.PlaySound(sound);
                }

                if (channel != null)
                {
                    bool isRecording = false;
                    try
                    {
                        isRecording = system.IsRecording(DriverIndex);
                    }
                    catch(FmodException e)
                    {
                        var res = e.Result;
                        if (!res.HasValue || res.Value != Result.Err_Record_Disconnected)
                        {
                            throw;
                        }
                    }

                    if (!isRecording)
                    {
                        channel.Paused = true;
                    }

                    uint playPos = channel.GetPosition(TimeUnit.PCM);

                    uint playDelta = (playPos >= LastPlayPos) ? playPos - LastPlayPos : playPos + SoundLength - LastPlayPos;
                    LastPlayPos = playPos;
                    SamplesPlayed += playDelta;

                    /*
                        Compensate for any drift.
                    */
                    int latency = (int)(SamplesRecorded - SamplesPlayed);
                    actualLatency = (int)((0.97f * actualLatency) + (0.03f * latency));

                    int playbackRate = nativeRate;
                    if (actualLatency < (int)(adjustedLatency - driftThreshold))
                    {
                        playbackRate = nativeRate - (nativeRate / 50); //Play position is catching up to the record position, slow playback down by 2%
                    }
                    else if (actualLatency > (int)(adjustedLatency + driftThreshold))
                    {
                        playbackRate = nativeRate + (nativeRate / 50); //Play position is falling behind the record position, speed playback up by 2%
                    }

                    channel.Frequency = playbackRate;
                }

                DrawText("==================================================");
                DrawText("Record Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText("");
                DrawText("Adjust LATENCY define to compensate for stuttering");
                DrawText($"Current value is {Latency_MS}");
                DrawText("");
                DrawText($"Press 1 to {(dspEnabled ? "Disable" : "Enable")} DSP effect");
                DrawText("Press Esc to quit.");
                DrawText("");
                DrawText($"Adjusted latency: {adjustedLatency} ({adjustedLatency * 1000 / nativeRate}ms)");
                DrawText($"Actual Latency: {actualLatency} ({actualLatency * 1000 / nativeRate}ms)");
                DrawText("");
                DrawText($"Recorded: {SamplesRecorded} ({SamplesRecorded / nativeRate}s)");
                DrawText($"Played: {SamplesPlayed} ({SamplesPlayed / nativeRate}s)");

                Sleep(10);

            } while (true);

            Exit:
            try
            {
                if (system.IsRecording(DriverIndex))
                {
                    system.RecordStop(DriverIndex);
                }
            }
            finally
            {
                sound.Dispose();
                system.Dispose();
            }
        }
    }
}
