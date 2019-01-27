using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmodAudio
{
    public class DriverInfo
    {
        internal string dname;
        internal Guid dguid;
        internal int systemRate;
        internal SpeakerMode speakerMode;
        internal int speakerModeChannels;

        internal DriverInfo()
        {
        }
        
        public string DriverName => dname;
        public Guid DriverGuid => dguid;
        public int SystemRate => systemRate;
        public SpeakerMode SpeakerMode => speakerMode;
        public int SpeakerModeChannels => speakerModeChannels;

        public override string ToString()
        {
            return dname;
        }
    }

    public class RecordDriverInfo : DriverInfo
    {
        internal DriverState state;
        public DriverState State => state;
    }
}
