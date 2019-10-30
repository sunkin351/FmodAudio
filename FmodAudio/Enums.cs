using System;

namespace FmodAudio
{
    public enum ChannelControlCallbackType : int
    {
        /// <summary>
        /// Called when a sound ends.
        /// </summary>
        End,

        /// <summary>
        /// Called when a voice is swapped out or swapped in.
        /// </summary>
        VirtualVoice,

        /// <summary>
        /// Called when a syncpoint is encountered.  Can be from wav file markers.
        /// </summary>
        SyncPoint,

        /// <summary>
        /// Called when the channel has its geometry occlusion value calculated.  Can be used to clamp or change the value.
        /// </summary>
        Occlusion,

        /// <summary>
        /// Maximum number of callback types supported.
        /// </summary>
        Max,
    }

    public enum ChannelControlType : int
    {
        Channel, ChannelGroup
    }

    [Flags]
    public enum ChannelMask : uint
    {
        Front_Left = 0x00000001,
        Front_Right = 0x00000002,
        Front_Center = 0x00000004,
        Low_Frequency = 0x00000008,
        Surround_Left = 0x00000010,
        Surround_Right = 0x00000020,
        Back_Left = 0x00000040,
        Back_Right = 0x00000080,
        Back_Center = 0x00000100,

        MONO = (Front_Left),
        STEREO = (Front_Left | Front_Right),
        LRC = (STEREO | Front_Center),
        QUAD = (STEREO | Surround_Left | Surround_Right),
        SURROUND = (Front_Left | Front_Right | Front_Center | Surround_Left | Surround_Right),
        _5POINT1 = (Front_Left | Front_Right | Front_Center | Low_Frequency | Surround_Left | Surround_Right),
        _5POINT1_REARS = (Front_Left | Front_Right | Front_Center | Low_Frequency | Back_Left | Back_Right),
        _7POINT0 = (Front_Left | Front_Right | Front_Center | Surround_Left | Surround_Right | Back_Left | Back_Right),
        _7POINT1 = (Front_Left | Front_Right | Front_Center | Low_Frequency | Surround_Left | Surround_Right | Back_Left | Back_Right)
    }

    public enum ChannelOrder : int
    {
        /// <summary>
        /// Left, Right, Center, LFE, Surround Left, Surround Right, Back Left, Back Right (see FMOD_SPEAKER enumeration)
        /// </summary>
        Default,

        /// <summary>
        /// Left, Right, Center, LFE, Back Left, Back Right, Surround Left, Surround Right (as per Microsoft .wav WAVEFORMAT structure master order)
        /// </summary>
        WaveFormat,

        /// <summary>
        /// Left, Center, Right, Surround Left, Surround Right, LFE
        /// </summary>
        Protools,

        /// <summary>
        /// Mono, Mono, Mono, Mono, Mono, Mono, ... (each channel all the way up to 32 channels are treated as if they were mono)
        /// </summary>
        AllMono,

        /// <summary>
        /// Left, Right, Left, Right, Left, Right, ... (each pair of channels is treated as stereo all the way up to 32 channels)
        /// </summary>
        AllStereo,

        /// <summary>
        /// Left, Right, Surround Left, Surround Right, Center, LFE (as per Linux ALSA channel order)
        /// </summary>
        ALSA,

        /// <summary>
        /// Maximum number of channel orderings supported.
        /// </summary>
        Max
    }

    [Flags]
    public enum DebugFlags : uint
    {
        /// <summary>
        /// Disable all messages
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Enable only error messages.
        /// </summary>
        Error = 0x00000001,

        /// <summary>
        /// Enable warning and error messages.
        /// </summary>
        Warning = 0x00000002,

        /// <summary>
        /// Enable informational, warning and error messages (default).
        /// </summary>
        Log = 0x00000004,

        /// <summary>
        /// Verbose logging for memory operations, only use this if you are debugging a memory related issue.
        /// </summary>
        Type_Memory = 0x00000100,

        /// <summary>
        /// Verbose logging for file access, only use this if you are debugging a file related issue.
        /// </summary>
        Type_File = 0x00000200,

        /// <summary>
        /// Verbose logging for codec initialization, only use this if you are debugging a codec related issue.
        /// </summary>
        Type_Codec = 0x00000400,

        /// <summary>
        /// Verbose logging for internal errors, use this for tracking the origin of error codes.
        /// </summary>
        Type_Trace = 0x00000800,

        /// <summary>
        /// Display the time stamp of the log message in milliseconds.
        /// </summary>
        Display_Timestamps = 0x00010000,

        /// <summary>
        /// Display the source code file and line number for where the message originated.
        /// </summary>
        Display_LineNumbers = 0x00020000,

        /// <summary>
        /// Display the thread ID of the calling function that generated the message.
        /// </summary>
        Display_Thread = 0x00040000,
    }

    public enum DebugMode : int
    {
        TTY, File, Callback
    }

    [Flags]
    public enum DriverState : uint
    {
        /// <summary>
        /// Device is currently plugged in.
        /// </summary>
        Connected = 0x00000001,

        /// <summary>
        /// Device is the users preferred choice.
        /// </summary>
        Default = 0x00000002,
    }

    public enum DSPConnectionType : int
    {
        /// <summary>
        /// Default connection type.
        /// Audio is mixed from the input to the output DSP's audible buffer.
        /// </summary>
        Standard,

        /// <summary>
        /// Sidechain connection type.
        /// Audio is mixed from the input to the output DSP's sidechain buffer.
        /// </summary>
        Sidechain,

        /// <summary>
        /// Send connection type.
        /// Audio is mixed from the input to the output DSP's audible buffer, but the input is NOT executed, only copied from.
        /// A standard connection or sidechain needs to make an input execute to generate data.
        /// </summary>
        Send,

        /// <summary>
        /// Send sidechain connection type.
        /// Audio is mixed from the input to the output DSP's sidechain buffer, but the input is NOT executed, only copied from.
        /// A standard connection or sidechain needs to make an input execute to generate data.
        /// </summary>
        Send_Sidechain,

        /// <summary>
        /// Maximum number of DSP connection types supported.
        /// </summary>
        Max,
    }

    public enum DSPResampler : int
    {
        /// <summary>
        /// Default interpolation method.  Currently equal to FMOD_DSP_RESAMPLER_LINEAR.
        /// </summary>
        Default,

        /// <summary>
        /// No interpolation.  High frequency aliasing hiss will be audible depending on the sample rate of the sound.
        /// </summary>
        NoInterp,

        /// <summary>
        /// Linear interpolation (default method).  Fast and good quality, causes very slight lowpass effect on low frequency sounds.
        /// </summary>
        Linear,

        /// <summary>
        /// Cubic interpolation.  Slower than linear interpolation but better quality.
        /// </summary>
        Cubic,

        /// <summary>
        /// 5 point spline interpolation.  Slowest resampling method but best quality.
        /// </summary>
        Spline,

        /// <summary>
        /// Maximum number of resample methods supported.
        /// </summary>
        Max,
    }

    public enum ErrorCallbackInstanceType
    {
        None,
        System,
        Channel,
        ChannelGroup,
        ChannelControl,
        Sound,
        SoundGroup,
        DSP,
        DSPConnection,
        Geometry,
        Reverb3D,
        Studio_System,
        Studio_EventDescription,
        Studio_EventInstance,
        Studio_ParameterInstance,
        Studio_Bus,
        Studio_VCA,
        Studio_Bank,
        Studio_CommandReplay
    }

    [Flags]
    public enum InitFlags : uint
    {
        /// <summary>
        /// Initialize normally
        /// </summary>
        Normal = 0x00000000,

        /// <summary>
        /// No stream thread is created internally.
        /// Streams are driven from FmodSystem.Update().
        /// Mainly used with non-realtime outputs.
        /// </summary>
        Stream_From_Update = 0x00000001,

        /// <summary>
        /// Win/Wii/PS3/Xbox/Xbox 360 Only - FMOD Mixer thread is woken up to do a mix when FmodSystem.Update() is called rather than waking periodically on its own timer.
        /// </summary>
        Mix_From_Update = 0x00000002,

        /// <summary>
        /// FMOD will treat +X as right, +Y as up and +Z as backwards (towards you).
        /// </summary>
        _3D_RightHanded = 0x00000004,

        /// <summary>
        /// All FMOD_3D based voices will add a software lowpass filter effect into the DSP chain which is automatically used when Channel.Set3DOcclusion() is used or the geometry API.
        /// This also causes sounds to sound duller when the sound goes behind the listener, as a fake HRTF style effect.
        /// Use FmodSystem.SetAdvancedSettings() to disable or adjust cutoff frequency for this feature.
        /// </summary>
        Channel_Lowpass = 0x00000100,

        /// <summary>
        /// All FMOD_3D based voices will add a software lowpass and highpass filter effect into the DSP chain which will act as a distance-automated bandpass filter.
        /// Use FmodSystem.SetAdvancedSettings to adjust the center frequency.
        /// </summary>
        Channel_DistanceFilter = 0x00000200,

        /// <summary>
        /// Enable TCP/IP based host which allows FMOD Designer or FMOD Profiler to connect to it, and view memory, CPU and the DSP network graph in real-time.
        /// </summary>
        Profile_Enable = 0x00010000,

        /// <summary>
        /// Any sounds that are 0 volume will go virtual and not be processed except for having their positions updated virtually.
        /// Use FmodSystem.SetAdvancedSettings() to adjust what volume besides zero to switch to virtual at.
        /// </summary>
        Vol0_Becomes_Virtual = 0x00020000,

        /// <summary>
        /// With the geometry engine, only process the closest polygon rather than accumulating all polygons the sound to listener line intersects.
        /// </summary>
        Geomatry_UseClosest = 0x00040000,

        /// <summary>
        /// When using FMOD_SPEAKERMODE_5POINT1 with a stereo output device, use the Dolby Pro Logic II downmix algorithm instead of the SRS Circle Surround algorithm.
        /// </summary>
        Prefer_Dolby_Downmix = 0x00080000,

        /// <summary>
        /// Disables thread safety for API calls. Only use this if FMOD low level is being called from a single thread, and if Studio API is not being used!
        /// </summary>
        Thread_Unsafe = 0x00100000,

        /// <summary>
        /// Slower, but adds level metering for every single DSP unit in the graph.  Use DSP::setMeteringEnabled to turn meters off individually.
        /// </summary>
        Profile_Meter_All = 0x00200000,

    }

    [Flags]
    public enum MemoryType : uint
    {
        /// <summary>
        /// Standard memory.
        /// </summary>
        Normal = 0x00000000,

        /// <summary>
        /// Stream file buffer, size controllable with System::setStreamBufferSize.
        /// </summary>
        Stream_File = 0x00000001,

        /// <summary>
        /// Stream decode buffer, size controllable with FMOD_CREATESOUNDEXINFO::decodebuffersize.
        /// </summary>
        Stream_Decode = 0x00000002,

        /// <summary>
        /// Sample data buffer.  Raw audio data, usually PCM/MPEG/ADPCM/XMA data.
        /// </summary>
        SampleData = 0x00000004,

        /// <summary>
        /// DSP memory block allocated when more than 1 output exists on a DSP node.
        /// </summary>
        DSP_Buffer = 0x00000008,

        /// <summary>
        /// Memory allocated by a third party plugin.
        /// </summary>
        Plugin = 0x00000010,

        /// <summary>
        /// Persistent memory. Memory will be freed when FmodSystem.Release() is called.
        /// </summary>
        Persistent = 0x00200000,

        All = 0xFFFFFFFF
    }

    [Flags]
    public enum Mode : uint
    {
        /// <summary>
        /// Default for all modes listed below. Loop_Off, _2D, _3D_WorldRelative, _3D_InverseRolloff
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// For non looping sounds. (default).  Overrides Loop_Normal / Loop_BiDi.
        /// </summary>
        Loop_Off = 0x00000001,

        /// <summary>
        /// For forward looping sounds.
        /// </summary>
        Loop_Normal = 0x00000002,

        /// <summary>
        /// For bidirectional looping sounds. (only works on software mixed static sounds).
        /// </summary>
        Loop_BiDi = 0x00000004,

        /// <summary>
        /// Ignores any 3d processing. (default).
        /// </summary>
        _2D = 0x00000008,

        /// <summary>
        /// Makes the sound positionable in 3D.  Overrides _2D.
        /// </summary>
        _3D = 0x00000010,

        /// <summary>
        /// Decompress at runtime, streaming from the source provided (standard stream).  Overrides CreateSample.
        /// </summary>
        CreateStream = 0x00000080,

        /// <summary>
        /// Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format. (standard sample).
        /// </summary>
        CreateSample = 0x00000100,

        /// <summary>
        /// Load MP2, MP3, IMAADPCM or XMA into memory and leave it compressed.
        /// During playback the FMOD software mixer will decode it in realtime as a 'compressed sample'.
        /// Can only be used in combination with FMOD_SOFTWARE.
        /// </summary>
        CreateCompressedSample = 0x00000200,

        /// <summary>
        /// Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.
        /// If a user created 'sample' is created with no read callback, the sample will be empty.
        /// Use FMOD_Sound_Lock and FMOD_Sound_Unlock to place sound data into the sound if this is the case. 
        /// </summary>
        OpenUser = 0x00000400,

        /// <summary>
        /// "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds.
        /// </summary>
        OpenMemory = 0x00000800,

        /// <summary>
        /// "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds.
        /// Use CreateSoundExInfo to specify length.  This differs to FMOD_OPENMEMORY in that it uses the memory as is, without duplicating the memory into its own buffers.
        /// Cannot be freed after open, only after Sound.Release().
        /// Will not work if the data is compressed and CreateCompressedSample is not used.
        /// </summary>
        OpenMemory_Point = 0x10000000,

        /// <summary>
        /// Will ignore file format and treat as raw pcm.
        /// User may need to declare if data is FMOD_SIGNED or FMOD_UNSIGNED
        /// </summary>
        OpenRaw = 0x00001000,

        /// <summary>
        /// Just open the file, dont prebuffer or read.
        /// Good for fast opens for info, or when Sound.ReadData() is to be used.
        /// </summary>
        OpenOnly = 0x00002000,

        /// <summary>
        /// For FmodSystem.CreateSound() - for accurate FMOD_Sound_GetLength / FMOD_Channel_SetPosition on VBR MP3, AAC and MOD/S3M/XM/IT/MIDI files.
        /// Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this.
        /// </summary>
        AccurateTime = 0x00004000,

        /// <summary>
        /// For corrupted / bad MP3 files.  This will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k.
        /// </summary>
        MpegSearch = 0x00008000,

        /// <summary>
        /// For opening sounds and getting streamed subsounds (seeking) asyncronously.
        /// Use Sound.GetOpenState() to poll the state of the sound as it opens or retrieves the subsound in the background.
        /// </summary>
        NonBlocking = 0x00010000,

        /// <summary>
        /// Unique sound, can only be played one at a time
        /// </summary>
        Unique = 0x00020000,

        /// <summary>
        /// Make the sound's position, velocity and orientation relative to the listener.
        /// </summary>
        _3D_HeadRelative = 0x00040000,

        /// <summary>
        /// Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT)
        /// </summary>
        _3D_WorldRelative = 0x00080000,

        /// <summary>
        /// This sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT)
        /// </summary>
        _3D_InverseRolloff = 0x00100000,

        /// <summary>
        /// This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence.
        /// </summary>
        _3D_LinearRolloff = 0x00200000,

        /// <summary>
        /// This sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence.  Rolloffscale is ignored.
        /// </summary>
        _3D_LinearSquareRolloff = 0x00400000,

        /// <summary>
        /// This sound will follow the inverse rolloff model at distances close to mindistance and a linear-square rolloff close to maxdistance.
        /// </summary>
        _3D_InverseTaperedRolloff = 0x00800000,

        /// <summary>
        /// This sound will follow a rolloff model defined by Sound.Set3DCustomRolloff() / Channel.Set3DCustomRolloff().
        /// </summary>
        _3D_CustomRolloff = 0x04000000,

        /// <summary>
        /// Is not affect by geometry occlusion.  If not specified when setting Sound.Mode, or Channel.Mode, the flag is cleared and it is affected by geometry again.
        /// </summary>
        _3D_IgnoreGeometry = 0x40000000,

        /// <summary>
        /// Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance).
        /// </summary>
        IgnoreTags = 0x02000000,

        /// <summary>
        /// Removes some features from samples to give a lower memory overhead, like Sound.Name.
        /// </summary>
        LowMem = 0x08000000,

        /// <summary>
        /// Load sound into the secondary RAM of supported platform.  On PS3, sounds will be loaded into RSX/VRAM.
        /// </summary>
        LoadSecondaryRAM = 0x20000000,

        /// <summary>
        /// For sounds that start virtual (due to being quiet or low importance), instead of swapping back to audible, and playing at the correct offset according to time, this flag makes the sound play from the start.
        /// </summary>
        Virtual_PlayFromStart = 0x80000000
    }

    public enum OpenState : int
    {
        Ready = 0,       /* Opened and ready to play */
        Loading,         /* Initial load in progress */
        Error,           /* Failed to open - file not found, out of memory etc.  See return value of Sound::getOpenState for what happened. */
        Connecting,      /* Connecting to remote host (internet sounds only) */
        Buffering,       /* Buffering data */
        Seeking,         /* Seeking to subsound and re-flushing stream buffer. */
        Playing,         /* Ready and playing, but not possible to release at this time without stalling the main thread. */
        SetPosition,     /* Seeking within a stream to a different position. */

        Max,             /* Maximum number of open state types. */
    }

    /// <summary>
    /// These output types are used with System::setOutput / System::getOutput, to choose which output method to use.
    /// </summary>
    public enum OutputType : int
    {
        Autodetect,      /* Picks the best output mode for the platform. This is the default. */

        Unknown,         /* All - 3rd party plugin, unknown. This is for use with System::getOutput only. */
        NoSound,         /* All - Perform all mixing but discard the final output. */
        WavWriter,       /* All - Writes output to a .wav file. */
        NoSound_NRT,     /* All - Non-realtime version of FMOD_OUTPUTTYPE_NOSOUND. User can drive mixer with System::update at whatever rate they want. */
        WavWriter_NRT,   /* All - Non-realtime version of FMOD_OUTPUTTYPE_WAVWRITER. User can drive mixer with System::update at whatever rate they want. */

        WASAPI,          /* Win/WinStore/XboxOne - Windows Audio Session API.           (Default on Windows Vista and above, Xbox One and Windows Store Applications) */
        ASIO,            /* Win                  - Low latency ASIO 2.0. */
        PulseAudio,      /* Linux                - Pulse Audio.                         (Default on Linux if available) */
        ALSA,            /* Linux                - Advanced Linux Sound Architecture.   (Default on Linux if PulseAudio isn't available) */
        CoreAudio,       /* Mac/iOS              - Core Audio.                          (Default on Mac and iOS) */
        XAudio,          /* Xbox 360             - XAudio.                              (Default on Xbox 360) */
        PS3,             /* PS3                  - Audio Out.                           (Default on PS3) */
        AudioTrack,      /* Android              - Java Audio Track.                    (Default on Android 2.2 and below) */
        OpenSL,          /* Android              - OpenSL ES.                           (Default on Android 2.3 and above) */
        AudioOut,        /* PS4/PSVita           - Audio Out.                           (Default on PS4 and PS Vita) */
        Audio3D,         /* PS4                  - Audio3D. */
        WebAudio,        /* Web Browser          - JavaScript webaudio output.          (Default on JavaScript) */
        NNAudio,         /* NX                   - NX nn::audio.                        (Default on NX) */
        WINSonic,        /* XboxOne              - Windows Sonic. */
        AAudio,

        Max,             /* Maximum number of output types supported. */
    }

    public enum PluginType : int
    {
        Output,          /* The plugin type is an output module.  FMOD mixed audio will play through one of these devices */
        Codec,           /* The plugin type is a file format codec.  FMOD will use these codecs to load file formats for playback. */
        DSP,             /* The plugin type is a DSP unit.  FMOD will use these plugins as part of its DSP network to apply effects to output or generate sound in realtime. */

        Max,             /* Maximum number of plugin types supported. */
    }

    /// <summary>
    /// Codes returned from every function
    /// </summary>
    public enum Result : int
    {
        /// <summary>
        /// No Errors.
        /// </summary>
        Ok,

        /// <summary>
        /// Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound.lock() on a streaming sound).
        /// </summary>
        Err_BadCommand,

        /// <summary>
        /// Error trying to allocate a channel.
        /// </summary>
        Err_Channel_Alloc,

        /// <summary>
        /// The specified channel has been reused to play another sound.
        /// </summary>
        Err_Channel_Stolen,

        /// <summary>
        /// DMA Failure.
        /// See debug output for more information.
        /// </summary>
        Err_DMA,

        /// <summary>
        /// DSP connection error.
        /// Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts.
        /// </summary>
        Err_DSP_Connection,

        /// <summary>
        /// DSP return code from a DSP process query callback.
        /// Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph.
        /// </summary>
        Err_DSP_DontProcess,

        /// <summary>
        /// DSP Format error.
        /// A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map.
        /// </summary>
        Err_DSP_Format,

        /// <summary>
        /// DSP is already in the mixer's DSP network.
        /// It must be removed before being reinserted or released.
        /// </summary>
        Err_DSP_InUse,

        /// <summary>
        /// DSP connection error.
        /// Couldn't find the DSP unit specified.
        /// </summary>
        Err_DSP_NotFound,

        /// <summary>
        /// DSP operation error.
        /// Cannot perform operation on this DSP as it is reserved by the system.
        /// </summary>
        Err_DSP_Reserved,

        /// <summary>
        /// DSP return code from a DSP process query callback.
        /// Tells mixer silence would be produced from read, so go idle and not consume CPU.
        /// Use this to optimize the DSP graph.
        /// </summary>
        Err_DSP_Silence,

        /// <summary>
        /// DSP operation cannot be performed on a DSP of this type.
        /// </summary>
        Err_DSP_Type,

        /// <summary>
        /// Error loading file.
        /// </summary>
        Err_File_Bad,

        /// <summary>
        /// Couldn't perform seek operation.
        /// This is a limitation of the medium (ie netstreams) or the file format.
        /// </summary>
        Err_File_CouldNotSeek,

        /// <summary>
        /// Media was ejected while reading.
        /// </summary>
        Err_File_DiskEjected,

        /// <summary>
        /// End of file unexpectedly reached while trying to read essential data (truncated?).
        /// </summary>
        Err_File_EOF,

        /// <summary>
        /// End of current chunk reached while trying to read data.
        /// </summary>
        Err_File_EndOfData,

        /// <summary>
        /// File not found.
        /// </summary>
        Err_File_NotFound,

        /// <summary>
        /// Unsupported file or audio format.
        /// </summary>
        Err_Format,

        /// <summary>
        /// There is a version mismatch between the FMOD header and either the FMOD Studio library or the FMOD Low Level library.
        /// </summary>
        Err_Header_Mismatch,

        /// <summary>
        /// A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere.
        /// </summary>
        Err_HTTP,

        /// <summary>
        /// The specified resource requires authentication or is forbidden.
        /// </summary>
        Err_HTTP_Access,

        /// <summary>
        /// Proxy authentication is required to access the specified resource.
        /// </summary>
        Err_HTTP_Proxy_Auth,

        /// <summary>
        /// A HTTP server error occurred.
        /// </summary>
        Err_HTTP_Server_Error,

        /// <summary>
        /// The HTTP request timed out.
        /// </summary>
        Err_HTTP_Timeout,

        /// <summary>
        /// FMOD was not initialized correctly to support this function.
        /// </summary>
        Err_Initialization,

        /// <summary>
        /// Cannot call this command after System.Init().
        /// </summary>
        Err_Initialized,

        /// <summary>
        /// An error occurred that wasn't supposed to.  Contact support.
        /// </summary>
        Err_Internal,

        /// <summary>
        /// Value passed in was a NaN, Inf or denormalized float.
        /// </summary>
        Err_Invalid_Float,

        /// <summary>
        /// An invalid object handle was used.
        /// </summary>
        Err_Invalid_Handle,

        /// <summary>
        /// An invalid parameter was passed to this function.
        /// </summary>
        Err_Invalid_Param,

        /// <summary>
        /// An invalid seek position was passed to this function.
        /// </summary>
        Err_Invalid_Position,

        /// <summary>
        /// An invalid speaker was passed to this function based on the current speaker mode.
        /// </summary>
        Err_Invalid_Speaker,

        /// <summary>
        /// The syncpoint did not come from this sound handle.
        /// </summary>
        Err_Invalid_SyncPoint,

        /// <summary>
        /// Tried to call a function on a thread that is not supported.
        /// </summary>
        Err_Invalid_Thread,

        /// <summary>
        /// The vectors passed in are not unit length, or perpendicular.
        /// </summary>
        Err_Invalid_Vector,

        /// <summary>
        /// Reached maximum audible playback count for this sound's soundgroup.
        /// </summary>
        Err_MaxAudible,

        /// <summary>
        /// Not enough memory or resources.
        /// </summary>
        Err_Memory,

        /// <summary>
        /// Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used.
        /// </summary>
        Err_Memory_CantPoint,

        /// <summary>
        /// Tried to call a command on a 2d sound when the command was meant for 3d sound.
        /// </summary>
        Err_Needs3D,

        /// <summary>
        /// Tried to use a feature that requires hardware support.
        /// </summary>
        Err_NeedsHardware,

        /// <summary>
        /// Couldn't connect to the specified host.
        /// </summary>
        Err_Net_Connect,

        /// <summary>
        /// A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere.
        /// </summary>
        Err_Net_Socket_Error,

        /// <summary>
        /// The specified URL couldn't be resolved.
        /// </summary>
        Err_Net_Url,

        /// <summary>
        /// Operation on a non-blocking socket could not complete immediately.
        /// </summary>
        Err_Net_Would_Block,

        /// <summary>
        /// Operation could not be performed because specified sound/DSP connection is not ready.
        /// </summary>
        Err_NotReady,

        /// <summary>
        /// Error initializing output device, but more specifically, the output device is already in use and cannot be reused.
        /// </summary>
        Err_Output_Allocated,

        /// <summary>
        /// Error creating hardware sound buffer.
        /// </summary>
        Err_Output_CreateBuffer,

        /// <summary>
        /// A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted.
        /// </summary>
        Err_Output_DriverCall,

        /// <summary>
        /// Soundcard does not support the specified format.
        /// </summary>
        Err_Output_Format,

        /// <summary>
        /// Error initializing output device.
        /// </summary>
        Err_Output_Init,

        /// <summary>
        /// The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails.
        /// </summary>
        Err_Output_NoDrivers,

        /// <summary>
        /// An unspecified error has been returned from a plugin.
        /// </summary>
        Err_Plugin,

        /// <summary>
        /// A requested output, dsp unit type or codec was not available.
        /// </summary>
        Err_Plugin_Missing,

        /// <summary>
        /// A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback)
        /// </summary>
        Err_Plugin_Resource,

        /// <summary>
        /// A plugin was built with an unsupported SDK version.
        /// </summary>
        Err_Plugin_Version,

        /// <summary>
        /// An error occurred trying to initialize the recording device.
        /// </summary>
        Err_Record,

        /// <summary>
        /// Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection.
        /// </summary>
        Err_Reverb_ChannelGroup,

        /// <summary>
        /// Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist.
        /// </summary>
        Err_Reverb_Instance,

        /// <summary>
        /// The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.
        /// The operation may also not be able to be performed on a parent sound.
        /// </summary>
        Err_SubSounds,

        /// <summary>
        /// This subsound is already being used by another sound, you cannot have more than one parent to a sound.
        /// Null out the other parent's entry first.
        /// </summary>
        Err_SubSound_Allocated,

        /// <summary>
        /// Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file.
        /// </summary>
        Err_SubSound_CantMove,

        /// <summary>
        /// The specified tag could not be found or there are no tags.
        /// </summary>
        Err_TagNotFound,

        /// <summary>
        /// The sound created exceeds the allowable input channel count.
        /// This can be increased using the 'maxinputchannels' parameter in System.SetSoftwareFormat.
        /// </summary>
        Err_TooManyChannels,

        /// <summary>
        /// The retrieved string is too long to fit in the supplied buffer and has been truncated.
        /// </summary>
        Err_Truncated,

        /// <summary>
        /// Something in FMOD hasn't been implemented when it should be! contact support!
        /// </summary>
        Err_Unimplemented,

        /// <summary>
        /// This command failed because System::init or System::setDriver was not called.
        /// </summary>
        Err_Uninitialized,

        /// <summary>
        /// A command issued was not supported by this object.
        /// Possibly a plugin without certain callbacks specified.
        /// </summary>
        Err_Unsupported,

        /// <summary>
        /// The version number of this file format is not supported.
        /// </summary>
        Err_Version,

        /// <summary>
        /// The specified bank has already been loaded.
        /// </summary>
        Err_Event_Already_Loaded,

        /// <summary>
        /// The live update connection failed due to the game already being connected.
        /// </summary>
        Err_Event_LiveUpdate_Busy,

        /// <summary>
        /// The live update connection failed due to the game data being out of sync with the tool.
        /// </summary>
        Err_Event_LiveUpdate_Mismatch,

        /// <summary>
        /// The live update connection timed out.
        /// </summary>
        Err_Event_LiveUpdate_Timeout,

        /// <summary>
        /// The requested event, bus or vca could not be found.
        /// </summary>
        Err_Event_NotFound,

        /// <summary>
        /// The Studio::System object is not yet initialized.
        /// </summary>
        Err_Studio_Uninitialized,

        /// <summary>
        /// The specified resource is not loaded, so it can't be unloaded.
        /// </summary>
        Err_Studio_Not_Loaded,

        /// <summary>
        /// An invalid string was passed to this function.
        /// </summary>
        Err_Invalid_String,

        /// <summary>
        /// The specified resource is already locked.
        /// </summary>
        Err_Already_Locked,

        /// <summary>
        /// The specified resource is not locked, so it can't be unlocked.
        /// </summary>
        Err_Not_Locked,

        /// <summary>
        /// The specified recording driver has been disconnected.
        /// </summary>
        Err_Record_Disconnected,

        /// <summary>
        /// The length provided exceeds the allowable limit.
        /// </summary>
        Err_TooManySamples,
    }

    public enum SoundFormat
    {
        /// <summary>
        /// Unitialized / unknown
        /// </summary>
        None,

        /// <summary>
        /// 8bit integer PCM data
        /// </summary>
        PCM8,

        /// <summary>
        /// 16bit integer PCM data
        /// </summary>
        PCM16,

        /// <summary>
        /// 24bit integer PCM data
        /// </summary>
        PCM24,

        /// <summary>
        /// 32bit integer PCM data
        /// </summary>
        PCM32,

        /// <summary>
        /// 32bit floating point PCM data
        /// </summary>
        PCMFloat,

        /// <summary>
        /// Sound data is in its native compressed format.
        /// </summary>
        BitStream,

        /// <summary>
        /// Maximum number of sound formats supported.
        /// </summary>
        Max
    }

    public enum SoundType
    {
        /// <summary>
        /// 3rd party / unknown plugin format.
        /// </summary>
        Unknown,

        /// <summary>
        /// AIFF.
        /// </summary>
        AIFF,

        /// <summary>
        /// Microsoft Advanced Systems Format (ie WMA/ASF/WMV).
        /// </summary>
        ASF,

        /// <summary>
        /// Sound font / downloadable sound bank.
        /// </summary>
        DLS,

        /// <summary>
        /// FLAC lossless codec.
        /// </summary>
        FLAC,

        /// <summary>
        /// FMOD Sample Bank.
        /// </summary>
        FSB,

        /// <summary>
        /// Impulse Tracker.
        /// </summary>
        IT,

        /// <summary>
        /// MIDI. extracodecdata is a pointer to an FMOD_MIDI_EXTRACODECDATA structure.
        /// </summary>
        MIDI,

        /// <summary>
        /// Protracker / Fasttracker MOD.
        /// </summary>
        MOD,

        /// <summary>
        /// MP2/MP3 MPEG.
        /// </summary>
        Mpeg,

        /// <summary>
        /// Ogg vorbis.
        /// </summary>
        OggVorbis,

        /// <summary>
        /// Information only from ASX/PLS/M3U/WAX playlists
        /// </summary>
        Playlist,

        /// <summary>
        /// Raw PCM data.
        /// </summary>
        Raw,

        /// <summary>
        /// ScreamTracker 3.
        /// </summary>
        S3M,

        /// <summary>
        /// User created sound.
        /// </summary>
        User,

        /// <summary>
        /// Microsoft WAV.
        /// </summary>
        Wav,

        /// <summary>
        /// FastTracker 2 XM.
        /// </summary>
        XM,

        /// <summary>
        /// Xbox360 XMA
        /// </summary>
        XMA,

        /// <summary>
        /// iPhone hardware decoder, supports AAC, ALAC and MP3. extracodecdata is a pointer to an FMOD_AUDIOQUEUE_EXTRACODECDATA structure.
        /// </summary>
        AudioQueue,

        /// <summary>
        /// PS4 / PSVita ATRAC 9 format
        /// </summary>
        AT9,

        /// <summary>
        /// Vorbis
        /// </summary>
        Vorbis,

        /// <summary>
        /// Windows Store Application built in system codecs
        /// </summary>
        MediaFoundation,

        /// <summary>
        /// Android MediaCodec
        /// </summary>
        MediaCodec,

        /// <summary>
        /// FMOD Adaptive Differential Pulse Code Modulation
        /// </summary>
        FADPCM,

        /// <summary>
        /// Maximum number of sound types supported.
        /// </summary>
        Max
    }

    public enum Speaker : int
    {
        None = -1,
        /// <summary>
        /// The front left speaker
        /// </summary>
        Front_Left = 0,

        /// <summary>
        /// The front right speaker
        /// </summary>
        Front_Right,

        /// <summary>
        /// The front center speaker
        /// </summary>
        Front_Center,

        /// <summary>
        /// The LFE or 'subwoofer' speaker
        /// </summary>
        Low_Frequency,

        /// <summary>
        /// The surround left (usually to the side) speaker
        /// </summary>
        Surround_Left,

        /// <summary>
        /// The surround right (usually to the side) speaker
        /// </summary>
        Surround_Right,

        /// <summary>
        /// The back left speaker
        /// </summary>
        Back_Left,

        /// <summary>
        /// The back right speaker
        /// </summary>
        Back_Right,

        /// <summary>
        /// The top front left speaker
        /// </summary>
        Top_Front_Left,

        /// <summary>
        /// The top front right speaker
        /// </summary>
        Top_Front_Right,

        /// <summary>
        /// The top back left speaker
        /// </summary>
        Top_Back_Left,

        /// <summary>
        /// The top back right speaker
        /// </summary>
        Top_Back_Right,

        /// <summary>
        /// Maximum number of speaker types supported.
        /// </summary>
        Max
    }

    public enum SpeakerMode : int
    {
        /// <summary>
        /// Default speaker mode for the chosen output mode which will resolve after System.Init().
        /// </summary>
        Default,

        /// <summary>
        /// Assume there is no special mapping from a given channel to a speaker, channels map 1:1 in order. Use System.SetSoftwareFormat() to specify the speaker count.
        /// </summary>
        Raw,

        /// <summary>
        /// 1 speaker setup (monaural).
        /// </summary>
        Mono,

        /// <summary>
        /// 2 speaker setup (stereo) front left, front right.
        /// </summary>
        Stereo,

        /// <summary>
        /// 4 speaker setup (4.0)    front left, front right, surround left, surround right.
        /// </summary>
        Quad,

        /// <summary>
        /// 5 speaker setup (5.0)    front left, front right, center, surround left, surround right.
        /// </summary>
        Surround,

        /// <summary>
        /// 6 speaker setup (5.1)    front left, front right, center, low frequency, surround left, surround right.
        /// </summary>
        _5Point1,

        /// <summary>
        /// 8 speaker setup (7.1)    front left, front right, center, low frequency, surround left, surround right, back left, back right.
        /// </summary>
        _7Point1,

        /// <summary>
        /// 12 speaker setup (7.1.4)  front left, front right, center, low frequency, surround left, surround right, back left, back right, top front left, top front right, top back left, top back right.
        /// </summary>
        _7Point1Point4,

        /// <summary>
        /// Maximum number of speaker modes supported.
        /// </summary>
        Max
    }

    [Flags]
    public enum SystemCallbackType : uint
    {
        /// <summary>
        /// Called from <see cref="FmodSystem.Update"/> when the enumerated list of devices has changed.
        /// </summary>
        DeviceListChanged = 0x00000001,

        /// <summary>
        /// Called from <see cref="FmodSystem.Update"/> when an output device has been lost due to control panel parameter changes and FMOD cannot automatically recover.
        /// </summary>
        DeviceLost = 0x00000002,

        /// <summary>
        /// Called directly when a memory allocation fails somewhere in FMOD.  (NOTE - 'system' will be NULL in this callback type.)
        /// </summary>
        MemoryAllocationFailed = 0x00000004,

        /// <summary>
        /// Called directly when a thread is created. (NOTE - 'system' will be NULL in this callback type.)
        /// </summary>
        ThreadCreated = 0x00000008,

        /// <summary>
        /// Called when a bad connection was made with DSP::addInput. Usually called from mixer thread because that is where the connections are made.
        /// </summary>
        BadDSPConnection = 0x00000010,

        /// <summary>
        /// Called each tick before a mix update happens.
        /// </summary>
        PreMix = 0x00000020,

        /// <summary>
        /// Called each tick after a mix update happens.
        /// </summary>
        PostMix = 0x00000040,

        /// <summary>
        /// Called when each API function returns an error code, including delayed async functions.
        /// </summary>
        Error = 0x00000080,

        /// <summary>
        /// Called each tick in mix update after clocks have been updated before the main mix occurs.
        /// </summary>
        MidMix = 0x00000100,

        /// <summary>
        /// Called directly when a thread is destroyed.
        /// </summary>
        ThreadDestroyed = 0x00000200,

        /// <summary>
        /// Called at start of System.Update() function.
        /// </summary>
        PreUpdate = 0x00000400,

        /// <summary>
        /// Called at end of System::update function.
        /// </summary>
        PostUpdate = 0x00000800,

        /// <summary>
        /// Called from System::update when the enumerated list of recording devices has changed.
        /// </summary>
        RecordListChanged = 0x00001000,

        /// <summary>
        /// Pass this mask to System.SetCallback() to receive all callback types.
        /// </summary>
        All = 0xFFFFFFFF
    }

    public enum TagDataType : int
    {
        Binary = 0,
        Int,
        Float,
        String,
        String_UTF16,
        String_UTF16BE,
        String_UTF8,
        CDTOC,

        Max                /* Maximum number of tag datatypes supported. */
    }

    public enum TagType : int
    {
        Unknown = 0,
        ID3V1,
        ID3V2,
        VorbisComment,
        ShoutCast,
        IceCast,
        ASF,
        MIDI,
        Playlist,
        FMod,
        User,

        Max                /* Maximum number of tag types supported. */
    }

    public enum ChannelControlDSPIndex
    {
        DspHead = -1,
        DSPFader = -2,
        DSPTail = -3
    }

    public enum SoundGroupBehavior : int
    {
        Fail,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will simply fail during System::playSound. */
        Mute,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will be silent, then if another sound in the group stops the sound that was silent before becomes audible again. */
        StealLowest,       /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will steal the quietest / least important sound playing in the group. */

        MAX,               /* Maximum number of sound group behaviors. */
    }

    [Flags]
    public enum TimeUnit : uint
    {
        MS = 0x00000001,  /* Milliseconds. */
        PCM = 0x00000002,  /* PCM Samples, related to milliseconds * samplerate / 1000. */
        PCMBytes = 0x00000004,  /* Bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        RAWBytes = 0x00000008,  /* Raw file bytes of (compressed) sound data (does not include headers).  Only used by Sound::getLength and Channel::getPosition. */
        PCMFraction = 0x00000010,  /* Fractions of 1 PCM sample.  Unsigned int range 0 to 0xFFFFFFFF.  Used for sub-sample granularity for DSP purposes. */
        ModOrder = 0x00000100,  /* MOD/S3M/XM/IT.  Order in a sequenced module format.  Use Sound::getFormat to determine the format. */
        ModRow = 0x00000200,  /* MOD/S3M/XM/IT.  Current row in a sequenced module format.  Cannot use with Channel::setPosition.  Sound::getLength will return the number if rows in the currently playing or seeked to pattern. */
        ModPattern = 0x00000400,  /* MOD/S3M/XM/IT.  Current pattern in a sequenced module format.  Cannot use with Channel::setPosition.  Sound::getLength will return the number of patterns in the song and Channel::getPosition will return the currently playing pattern. */
    }
}

