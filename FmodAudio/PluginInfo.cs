using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio
{
    public class PluginInfo
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
}
