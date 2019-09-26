using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FmodAudio
{
    /// <summary>
    /// TODO Continue adapting this class for interop
    /// </summary>
    public class AdvancedSettings
    {
        internal _interopStruct Struct;

        public AdvancedSettings()
        {
            Struct.cbSize = Unsafe.SizeOf<_interopStruct>();
            Struct.HRTFMaxAngle = 360.0f;
            Struct.HRTFMinAngle = 180.0f;
        }

        public int MaxMPEGCodecs { get => Struct.maxMPEGCodecs; set => Struct.maxMPEGCodecs = value; }

        public int MaxADPCMCodecs { get => Struct.maxADPCMCodecs; set => Struct.maxADPCMCodecs = value; }

        public int MaxXMACodecs { get => Struct.maxXMACodecs; set => Struct.maxXMACodecs = value; }

        public int MaxVorbisCodecs { get => Struct.maxVorbisCodecs; set => Struct.maxVorbisCodecs = value; }

        public int MaxAT9Codecs { get => Struct.maxAT9Codecs; set => Struct.maxAT9Codecs = value; }

        public int MaxFADPCMCodecs { get => Struct.maxFADPCMCodecs; set => Struct.maxFADPCMCodecs = value; }
        
        public int MaxPCMCodecs { get => Struct.maxPCMCodecs; set => Struct.maxPCMCodecs = value; }

        public int ASIOChannelCount
        {
            get => Struct.ASIONumChannels; set => Struct.ASIONumChannels = value;
        }

        /// <summary>
        /// Unmanaged type char**, an array of strings. It is recommended to use the UTF8 encoding. Length must be equal to ASIOChannelCount
        /// </summary>
        public IntPtr ASIOChannelList
        {
            get => Struct.ASIOChannelList; set => Struct.ASIOChannelList = value;
        }

        /// <summary>
        /// Unmanaged type Speaker*, an array of the FmodAudio.Speaker Enumeration
        /// </summary>
        public IntPtr ASIOSpeakerList
        {
            get => Struct.ASIOSpeakerList; set => Struct.ASIOSpeakerList = value;
        }

        public float HRTFMinAngle { get => Struct.HRTFMinAngle; set => Struct.HRTFMinAngle = value; }

        public float HRTFMaxAngle { get => Struct.HRTFMaxAngle; set => Struct.HRTFMaxAngle = value; }

        public float HRTFFreq { get => Struct.HRTFFreq; set => Struct.HRTFFreq = value; }

        public float Vol0VirtualVol { get => Struct.vol0virtualvol; set => Struct.vol0virtualvol = value; }

        public uint DefaultDecodeBufferSize { get => Struct.defaultDecodeBufferSize; set => Struct.defaultDecodeBufferSize = value; }

        public ushort ProfilePort { get => Struct.profilePort; set => Struct.profilePort = value; }

        public uint GeometryMaxFadeTime { get => Struct.geometryMaxFadeTime; set => Struct.geometryMaxFadeTime = value; }

        public float DistanceFilterCenterFreq { get => Struct.distanceFilterCenterFreq; set => Struct.distanceFilterCenterFreq = value; }

        public int Reverb3DInstance { get => Struct.reverb3Dinstance; set => Struct.reverb3Dinstance = value; }

        public int DSPBufferPoolSize { get => Struct.DSPBufferPoolSize; set => Struct.DSPBufferPoolSize = value; }

        public uint StackSizeStream { get => Struct.stackSizeStream; set => Struct.stackSizeStream = value; }

        public uint StackSizeNonBlocking { get => Struct.stackSizeNonBlocking; set => Struct.stackSizeNonBlocking = value; }

        public uint StackSizeMixer { get => Struct.stackSizeMixer; set => Struct.stackSizeMixer = value; }

        public DSPResampler ResamplerMethod { get => Struct.resamplerMethod; set => Struct.resamplerMethod = value; }

        public uint CommandQueueSize { get => Struct.commandQueueSize; set => Struct.commandQueueSize = value; }

        public uint RandomSeed { get => Struct.randomSeed; set => Struct.randomSeed = value; }

        [StructLayout(LayoutKind.Sequential)]
        public struct _interopStruct
        {
            public int cbSize;                     /* [w]   Size of this structure.  Use sizeof(FMOD_ADVANCEDSETTINGS) */
            public int maxMPEGCodecs;              /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  MPEG   codecs consume 30,528 bytes per instance and this number will determine how many MPEG   channels can be played simultaneously. Default = 32. */
            public int maxADPCMCodecs;             /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  ADPCM  codecs consume  3,128 bytes per instance and this number will determine how many ADPCM  channels can be played simultaneously. Default = 32. */
            public int maxXMACodecs;               /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  XMA    codecs consume 14,836 bytes per instance and this number will determine how many XMA    channels can be played simultaneously. Default = 32. */
            public int maxVorbisCodecs;            /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  Vorbis codecs consume 23,256 bytes per instance and this number will determine how many Vorbis channels can be played simultaneously. Default = 32. */
            public int maxAT9Codecs;               /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  AT9    codecs consume  8,720 bytes per instance and this number will determine how many AT9    channels can be played simultaneously. Default = 32. */
            public int maxFADPCMCodecs;            /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  This number will determine how many FADPCM channels can be played simultaneously. Default = 32. */
            public int maxPCMCodecs;               /* [r/w] Optional. Specify 0 to ignore. For use with PS3 only.                          PCM    codecs consume 12,672 bytes per instance and this number will determine how many streams and PCM voices can be played simultaneously. Default = 16. */
            public int ASIONumChannels;            /* [r/w] Optional. Specify 0 to ignore. Number of channels available on the ASIO device. */
            public IntPtr ASIOChannelList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to an array of strings (number of entries defined by ASIONumChannels) with ASIO channel names. */
            public IntPtr ASIOSpeakerList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to a list of speakers that the ASIO channels map to.  This can be called after System::init to remap ASIO output. */
            public float HRTFMinAngle;               /* [r/w] Optional.                      For use with FMOD_INIT_HRTF_LOWPASS.  The angle range (0-360) of a 3D sound in relation to the listener, at which the HRTF function begins to have an effect. 0 = in front of the listener. 180 = from 90 degrees to the left of the listener to 90 degrees to the right. 360 = behind the listener. Default = 180.0. */
            public float HRTFMaxAngle;               /* [r/w] Optional.                      For use with FMOD_INIT_HRTF_LOWPASS.  The angle range (0-360) of a 3D sound in relation to the listener, at which the HRTF function has maximum effect. 0 = front of the listener. 180 = from 90 degrees to the left of the listener to 90 degrees to the right. 360 = behind the listener. Default = 360.0. */
            public float HRTFFreq;                   /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_HRTF_LOWPASS.  The cutoff frequency of the HRTF's lowpass filter function when at maximum effect. (i.e. at HRTFMaxAngle).  Default = 4000.0. */
            public float vol0virtualvol;             /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_VOL0_BECOMES_VIRTUAL.  If this flag is used, and the volume is below this, then the sound will become virtual.  Use this value to raise the threshold to a different point where a sound goes virtual. */
            public uint defaultDecodeBufferSize;    /* [r/w] Optional. Specify 0 to ignore. For streams. This determines the default size of the double buffer (in milliseconds) that a stream uses.  Default = 400ms */
            public ushort profilePort;                /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_PROFILE_ENABLE.  Specify the port to listen on for connections by the profiler application. */
            public uint geometryMaxFadeTime;        /* [r/w] Optional. Specify 0 to ignore. The maximum time in miliseconds it takes for a channel to fade to the new level when its occlusion changes. */
            public float distanceFilterCenterFreq;   /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_DISTANCE_FILTERING.  The default center frequency in Hz for the distance filtering effect. Default = 1500.0. */
            public int reverb3Dinstance;           /* [r/w] Optional. Specify 0 to ignore. Out of 0 to 3, 3d reverb spheres will create a phyical reverb unit on this instance slot.  See FMOD_REVERB_PROPERTIES. */
            public int DSPBufferPoolSize;          /* [r/w] Optional. Specify 0 to ignore. Number of buffers in DSP buffer pool.  Each buffer will be DSPBlockSize * sizeof(float) * SpeakerModeChannelCount.  ie 7.1 @ 1024 DSP block size = 8 * 1024 * 4 = 32kb.  Default = 8. */
            public uint stackSizeStream;            /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the FMOD Stream thread in bytes.  Useful for custom codecs that use excess stack.  Default 49,152 (48kb) */
            public uint stackSizeNonBlocking;       /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the FMOD_NONBLOCKING loading thread.  Useful for custom codecs that use excess stack.  Default 65,536 (64kb) */
            public uint stackSizeMixer;             /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the FMOD mixer thread.  Useful for custom dsps that use excess stack.  Default 49,152 (48kb) */
            public DSPResampler resamplerMethod;            /* [r/w] Optional. Specify 0 to ignore. Resampling method used with fmod's software mixer.  See FMOD_DSP_RESAMPLER for details on methods. */
            public uint commandQueueSize;           /* [r/w] Optional. Specify 0 to ignore. Specify the command queue size for thread safe processing.  Default 2048 (2kb) */
            public uint randomSeed;                 /* [r/w] Optional. Specify 0 to ignore. Seed value that FMOD will use to initialize its internal random number generators. */
        }
    }
}
