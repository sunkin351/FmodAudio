using FmodAudio;
using System;

namespace Examples
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Base;
    using FmodAudio.Base;

    public class RecordExample : Example
    {
        const int Drift_MS = 1;
        const int Latency_MS = 50;

        int DriverIndex;
        uint LastRecordPos;
        uint MinRecordDelta = uint.MaxValue;
        uint LastPlayPos;
        int nativeRate;
        bool dspEnabled = false;

        Sound sound;

        uint driftThreshold, desiredLatency, adjustedLatency;
        int actualLatency;

        public RecordExample() : base("Fmod Record Example")
        {
            RegisterCommand(ConsoleKey.D1, () =>
            {
                dspEnabled = !dspEnabled;
                ReverbProperties prop = dspEnabled ? Preset.ConcertHall : Preset.Off;
                System.SetReverbProperties(0, in prop);
            });
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            if (!ChooseRecordDevice(out var recordDriverInfo, out DriverIndex))
            {
                ShouldEndExample = true;
                return;
            }

            nativeRate = recordDriverInfo.SystemRate;

            driftThreshold  = (uint)(nativeRate * Drift_MS / 1000);
            desiredLatency  = (uint)(nativeRate * Latency_MS / 1000);
            adjustedLatency = desiredLatency;
            actualLatency   = (int)desiredLatency;

            int nativeChannels = recordDriverInfo.SpeakerModeChannels;
            CreateSoundInfo info = new CreateSoundInfo()
            {
                ChannelCount = nativeChannels,
                Format = SoundFormat.PCM16,
                DefaultFrequency = nativeRate,
                Length = (uint)(nativeRate * sizeof(short) * nativeChannels)
            };

            sound = System.CreateSoundOpenUser(Mode.Loop_Normal, info);
        }

        public override void Run()
        {
            Channel channel = null;
            uint SamplesRecorded = 0;
            uint SamplesPlayed = 0;

            System.RecordStart(DriverIndex, sound, true);

            uint SoundLength = sound.GetLength(TimeUnit.PCM);
            
            do
            {
                OnUpdate();

                System.Update();

                var res = Fmod.Library.System_GetRecordPosition(System, DriverIndex, out uint recordPos);

                if (res != Result.Ok)
                {
                    if (res != Result.Err_Record_Disconnected)
                        res.CheckResult();

                    recordPos = 0; //Not garuenteed to be 0 on error
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
                    channel = System.PlaySound(sound);
                }

                if (channel != null)
                {
                    res = Fmod.Library.System_IsRecording(System, DriverIndex, out FmodBool isRecording);

                    if (res != Result.Ok)
                    {
                        if (res != Result.Err_Record_Disconnected)
                            res.CheckResult();

                        isRecording = false; //Not garuenteed to be false on error
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
                        playbackRate -= (nativeRate / 50); //Play position is catching up to the record position, slow playback down by 2%
                    }
                    else if (actualLatency > (int)(adjustedLatency + driftThreshold))
                    {
                        playbackRate += (nativeRate / 50); //Play position is falling behind the record position, speed playback up by 2%
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
            }
            while (!ShouldEndExample);
        }

        public override void Dispose()
        {
            if (System != default)
            {
                var res = Fmod.Library.System_IsRecording(System, DriverIndex, out FmodBool recording); //Using this to obtain access to the Result

                if (res == Result.Ok && recording)
                {
                    System.RecordStop(DriverIndex);
                }

                if (sound != default)
                    sound.Dispose();
            }

            base.Dispose();
        }

        private bool ChooseRecordDevice([NotNullWhen(true)] out RecordDriverInfo? driverInfo, out int driverIndex)
        {
            driverInfo = null;
            driverIndex = -1;

            System.GetRecordDriverCount(out int driverCount, out int connectedCount);

            if (driverCount == 0)
            {
                ConsoleHelpers.Draw("No recording devices found!");
                Sleep(5000);
                return false;
            }

            if (connectedCount == 0)
            {
                throw new Exception("No connected recording devices");
            }

            List<RecordDriverInfo> list = new List<RecordDriverInfo>(driverCount);

            for (int i = 0; i < driverCount; ++i)
            {
                list.Add(System.GetRecordDriverInfo(i));
            }

            ConsoleHelpers.Draw("Select a record device:");

            Span<char> buffer = stackalloc char[ConsoleHelpers.ColumnCount];

            buffer[1] = '[';
            buffer[3] = ']';

            int selection = 0;

            while (true)
            {
                ConsoleHelpers.SetCursorRow(1);

                for (int i = 0; i < driverCount; ++i)
                {
                    var info = list[i];

                    if (i == selection)
                    {
                        buffer[2] = info.State == DriverState.Connected ? '-' : 'x';
                    }
                    else
                    {
                        buffer[2] = char.MinValue;
                    }

                    ReadOnlySpan<char> name = info.DriverName;
                    var nameBuffer = buffer.Slice(5);

                    if (!name.TryCopyTo(nameBuffer))
                    {
                        name.Slice(0, nameBuffer.Length).CopyTo(nameBuffer);
                    }

                    ConsoleHelpers.Draw(buffer);
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selection = Math.Clamp(selection - 1, 0, driverCount - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selection = Math.Clamp(selection + 1, 0, driverCount - 1);
                        break;
                    case ConsoleKey.Enter:
                        driverInfo = list[selection];
                        driverIndex = selection;
                        return true;
                    case ConsoleKey.Escape:
                        return false;
                }
            }
        }
    }
}
