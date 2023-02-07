namespace FmodAudio;

public readonly record struct PluginInfo
{
    public string Name { get; }
    public PluginType Type { get; }
    public FmodVersion Version { get; }

    internal PluginInfo(string name, PluginType type, FmodVersion version)
    {
        Name = name;
        Type = type;
        Version = version;
    }
}
