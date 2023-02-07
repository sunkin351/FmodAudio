using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FmodAudio;

/// <summary>
/// TODO Continue adapting this class for interop
/// </summary>
public unsafe class AdvancedSettings
{
    internal Structure Struct;

    public AdvancedSettings()
    {
        Struct.cbSize = Unsafe.SizeOf<Structure>();
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
    public byte** ASIOChannelList
    {
        get => Struct.ASIOChannelList; set => Struct.ASIOChannelList = value;
    }

    /// <summary>
    /// Unmanaged type Speaker*, an array of the FmodAudio.Speaker Enumeration
    /// </summary>
    public Speaker* ASIOSpeakerList
    {
        get => Struct.ASIOSpeakerList; set => Struct.ASIOSpeakerList = value;
    }

    public float Vol0VirtualVol { get => Struct.vol0virtualvol; set => Struct.vol0virtualvol = value; }

    public uint DefaultDecodeBufferSize { get => Struct.defaultDecodeBufferSize; set => Struct.defaultDecodeBufferSize = value; }

    public ushort ProfilePort { get => Struct.profilePort; set => Struct.profilePort = value; }

    public uint GeometryMaxFadeTime { get => Struct.geometryMaxFadeTime; set => Struct.geometryMaxFadeTime = value; }

    public float DistanceFilterCenterFreq { get => Struct.distanceFilterCenterFreq; set => Struct.distanceFilterCenterFreq = value; }

    public int Reverb3DInstance { get => Struct.reverb3Dinstance; set => Struct.reverb3Dinstance = value; }

    public int DSPBufferPoolSize { get => Struct.DSPBufferPoolSize; set => Struct.DSPBufferPoolSize = value; }

    public DSPResampler ResamplerMethod { get => Struct.resamplerMethod; set => Struct.resamplerMethod = value; }

    public uint RandomSeed { get => Struct.randomSeed; set => Struct.randomSeed = value; }

    public int MaxConvolutionThreads
    {
        get => Struct.maxConvolutionThreads;
        set => Struct.maxConvolutionThreads = Math.Clamp(value, 0, 3);
    }

    public int MaxOpusCodecs
    {
        get => Struct.maxOpusCodecs;
        set => Struct.maxOpusCodecs = Math.Clamp(value, 0, 256);
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Structure
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
        public byte** ASIOChannelList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to an array of strings (number of entries defined by ASIONumChannels) with ASIO channel names. */
        public Speaker* ASIOSpeakerList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to a list of speakers that the ASIO channels map to.  This can be called after System::init to remap ASIO output. */
        public float vol0virtualvol;             /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_VOL0_BECOMES_VIRTUAL.  If this flag is used, and the volume is below this, then the sound will become virtual.  Use this value to raise the threshold to a different point where a sound goes virtual. */
        public uint defaultDecodeBufferSize;    /* [r/w] Optional. Specify 0 to ignore. For streams. This determines the default size of the double buffer (in milliseconds) that a stream uses.  Default = 400ms */
        public ushort profilePort;                /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_PROFILE_ENABLE.  Specify the port to listen on for connections by the profiler application. */
        public uint geometryMaxFadeTime;        /* [r/w] Optional. Specify 0 to ignore. The maximum time in miliseconds it takes for a channel to fade to the new level when its occlusion changes. */
        public float distanceFilterCenterFreq;   /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_DISTANCE_FILTERING.  The default center frequency in Hz for the distance filtering effect. Default = 1500.0. */
        public int reverb3Dinstance;           /* [r/w] Optional. Specify 0 to ignore. Out of 0 to 3, 3d reverb spheres will create a phyical reverb unit on this instance slot.  See FMOD_REVERB_PROPERTIES. */
        public int DSPBufferPoolSize;          /* [r/w] Optional. Specify 0 to ignore. Number of buffers in DSP buffer pool.  Each buffer will be DSPBlockSize * sizeof(float) * SpeakerModeChannelCount.  ie 7.1 @ 1024 DSP block size = 8 * 1024 * 4 = 32kb.  Default = 8. */
        public DSPResampler resamplerMethod;            /* [r/w] Optional. Specify 0 to ignore. Resampling method used with fmod's software mixer.  See FMOD_DSP_RESAMPLER for details on methods. */
        public uint randomSeed;                 /* [r/w] Optional. Specify 0 to ignore. Seed value that FMOD will use to initialize its internal random number generators. */
        public int maxConvolutionThreads;
        public int maxOpusCodecs;
    }
}
