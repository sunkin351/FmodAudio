using System;

namespace FmodAudio;

public class DriverInfo
{
    internal DriverInfo(string name, Guid guid, int rate, SpeakerMode speakermode, int channels)
    {
        DriverName = name;
        DriverGuid = guid;
        SystemRate = rate;
        SpeakerMode = speakermode;
        SpeakerModeChannels = channels;
    }
    
    public string DriverName { get; }
    public Guid DriverGuid { get; }
    public int SystemRate { get; }
    public SpeakerMode SpeakerMode { get; }
    public int SpeakerModeChannels { get; }

    public override string ToString()
    {
        return DriverName;
    }
}

public class RecordDriverInfo : DriverInfo
{
    public DriverState State { get; }

    internal RecordDriverInfo(string name, Guid guid, int rate, SpeakerMode speakermode, int channels, DriverState state)
       : base (name, guid, rate, speakermode, channels)
    {
        State = state;
    }
}
