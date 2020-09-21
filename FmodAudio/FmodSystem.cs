#pragma warning disable IDE0052, IDE1006, CA1031

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable

namespace FmodAudio
{
    using Base;
    using DigitalSignalProcessing;

    public unsafe partial struct FmodSystem : IDisposable
    {
        public static implicit operator FmodSystem(SystemHandle handle)
        {
            return new FmodSystem(handle);
        }

        public static implicit operator SystemHandle(FmodSystem system)
        {
            return system.Handle;
        }

        public static bool operator ==(FmodSystem l, FmodSystem r)
        {
            return l.Handle == r.Handle;
        }

        public static bool operator !=(FmodSystem l, FmodSystem r)
        {
            return l.Handle != r.Handle;
        }

        /// <summary>
        /// Internal VTable for Fmod
        /// </summary>
        private static readonly FmodLibrary library = Fmod.Library;
        private readonly SystemHandle Handle;

        internal FmodSystem(SystemHandle handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            Release();
        }
        
        /// <summary>
        /// Master Channel Group for this System Object. Null if System is not initialized
        /// </summary>
        public ChannelGroup? MasterChannelGroup
        {
            get
            {
                ChannelGroupHandle handle;
                library.System_GetMasterChannelGroup(Handle, &handle).CheckResult();

                return handle;
            }
        }

        /// <summary>
        /// Master Sound Group for this System Object. Null if System is not initialized
        /// </summary>
        public SoundGroup? MasterSoundGroup
        {
            get
            {
                SoundGroupHandle handle;
                library.System_GetMasterSoundGroup(Handle, &handle).CheckResult();

                return handle.IsNull() ? (SoundGroup?)null : (SoundGroup)handle;
            }
        }

        #region System Creation / Destruction

        public void Release()
        {
            lock (Fmod.CreationSyncObject)
            {
                library.System_Release(Handle).CheckResult();
            }
        }

        public void Init(int MaxChannels, InitFlags flags = InitFlags.Normal, IntPtr ExtraDriverData = default) //default(IntPtr) == IntPtr.Zero
        {
            library.System_Init(Handle, MaxChannels, flags, ExtraDriverData).CheckResult();
        }

        public void Close()
        {
            library.System_Close(Handle).CheckResult();
        }

        #endregion

        #region UtilityFunctions
        
        internal static void ReportErrorAndCrash(Result res)
        {
            if (res == Result.Ok)
                return;

            Console.WriteLine($"Unexpected Error Crash: {res} caught while releasing FmodAudio Resource in destructor.");
            Console.WriteLine($"Details: {FmodHelpers.GetErrorMessage(res)}");

            Environment.Exit(-255);
        }

        #endregion

        #region Plugin-Support

        public unsafe void SetPluginPath(string path)
        {
            library.System_SetPluginPath(Handle, path).CheckResult();
        }
        
        /// <summary>
        /// Loads an FMod Plugin and returns a handle to it.
        /// </summary>
        /// <param name="Filename">File to load</param>
        /// <param name="Priority">Priority of plugin</param>
        /// <returns>The plugin Handle</returns>
        public unsafe PluginHandle LoadPlugin(string Filename, uint Priority = 0)
        {
            PluginHandle handle;
            library.System_LoadPlugin(Handle, Filename, &handle, Priority).CheckResult();
            return handle;
        }

        public void UnloadPlugin(PluginHandle plugin)
        {
            library.System_UnloadPlugin(Handle, plugin).CheckResult();
        }

        public int GetPluginCount(PluginType type)
        {
            int count = 0;
            library.System_GetNumPlugins(Handle, type, &count).CheckResult();
            return count;
        }
         
        public int GetNestedPluginCount(PluginHandle plugin)
        {
            int count = 0;
            library.System_GetNumNestedPlugins(Handle, plugin, &count).CheckResult();
            return count;
        }

        public PluginHandle GetPluginHandle(PluginType type, int index)
        {
            PluginHandle handle;
            library.System_GetPluginHandle(Handle, type, index, &handle).CheckResult();
            return handle;
        }

        public PluginHandle GetNestedPlugin(PluginHandle plugin, int index)
        {
            PluginHandle handle;
            library.System_GetNestedPlugin(Handle, plugin, index, &handle).CheckResult();
            return handle;
        }

        public unsafe PluginInfo GetPluginInfo(PluginHandle plugin)
        {
            const int buflen = Fmod.MaxInteropNameStringLength;
            byte* buffer = stackalloc byte[buflen];
            PluginType type;
            FmodVersion version;

            library.System_GetPluginInfo(Handle, plugin, &type, buffer, buflen, &version).CheckResult();

            string name = FmodHelpers.BufferToString(buffer, buflen);
            return new PluginInfo(name, type, version);
        }

        public PluginHandle OutputPlugin
        {
            get
            {
                PluginHandle handle;
                library.System_GetOutputByPlugin(Handle, &handle).CheckResult();
                return handle;
            }

            set
            {
                library.System_SetOutputByPlugin(Handle, value).CheckResult();
            }
        }

        public DspHandle CreateDSPByPlugin(PluginHandle plugin)
        {
            DspHandle handle = default;
            library.System_CreateDSPByPlugin(Handle, plugin, &handle).CheckResult();

            return handle;
        }

        public DspDescriptionStruct* GetDSPInfoByPlugin(PluginHandle plugin)
        {
            DspDescriptionStruct* ptr;

            library.System_GetDSPInfoByPlugin(Handle, plugin, &ptr).CheckResult();

            return ptr;
        }

        public PluginHandle RegisterCodec(ref Codec.CodecDescriptionStruct description, uint priority = 0)
        {
            PluginHandle handle = default;
            library.System_RegisterCodec(Handle, ref description, &handle, priority).CheckResult();

            return handle;
        }

        public PluginHandle RegisterDSP(ref DspDescriptionStruct description)
        {
            PluginHandle handle;

            fixed (DspDescriptionStruct* pDescription = &description)
            {
                library.System_RegisterDSP(Handle, pDescription, &handle).CheckResult();
            }

            return handle;
        }

        #endregion

        #region Pre-Init Functions

        public OutputType Output
        {
            get
            {
                OutputType type;
                library.System_GetOutput(Handle, &type).CheckResult();

                return type;
            }
            set
            {
                library.System_SetOutput(Handle, value).CheckResult();
            }
        }

        public int DriverCount
        {
            get
            {
                int count;
                library.System_GetNumDrivers(Handle, &count).CheckResult();
                return count;
            }
        }

        public int CurrentDriver
        {
            get
            {
                int driver;
                library.System_GetDriver(Handle, &driver).CheckResult();
                return driver;
            }
            set
            {
                library.System_SetDriver(Handle, value).CheckResult();
            }
        }

        public int SoftwareChannels
        {
            get
            {
                int value;
                library.System_GetSoftwareChannels(Handle, &value).CheckResult();
                return value;
            }
            set
            {
                library.System_SetSoftwareChannels(Handle, value).CheckResult();
            }
        }

        public unsafe DriverInfo GetDriverInfo(int DriverId)
        {
            const int AllocSize = Fmod.MaxInteropNameStringLength;
            Guid guid;
            int rate, channels;
            SpeakerMode mode;

            var NamePtr = stackalloc byte[AllocSize];

            library.System_GetDriverInfo(Handle, DriverId, NamePtr, AllocSize, &guid, &rate, &mode, &channels).CheckResult();

            return new DriverInfo(FmodHelpers.BufferToString(NamePtr, AllocSize), guid, rate, mode, channels);
        }

        public void SetSoftwareFormat(int SampleRate, SpeakerMode speakerMode, int RawSpeakerCount)
        {
            library.System_SetSoftwareFormat(Handle, SampleRate, speakerMode, RawSpeakerCount).CheckResult();
        }

        public void GetSoftwareFormat(out int SampleRate, out SpeakerMode speakerMode, out int RawSpeakerCount)
        {
            library.System_GetSoftwareFormat(Handle, out SampleRate, out speakerMode, out RawSpeakerCount).CheckResult();
        }

        public void GetDSPBufferSize(out uint bufferLength, out int bufferCount)
        {
            library.System_GetDSPBufferSize(Handle, out bufferLength, out bufferCount).CheckResult();
        }

        public void SetDSPBufferSize(uint bufferLength, int bufferCount)
        {
            library.System_SetDSPBufferSize(Handle, bufferLength, bufferCount).CheckResult();
        }

        public void SetStreamBufferSize(uint FileBufferSize, TimeUnit SizeType)
        {
            library.System_SetStreamBufferSize(Handle, FileBufferSize, SizeType).CheckResult();
        }

        public void GetStreamBufferSize(out uint FileBufferSize, out TimeUnit SizeType)
        {
            library.System_GetStreamBufferSize(Handle, out FileBufferSize, out SizeType).CheckResult();
        }

        public void SetFileSystem(
            delegate* unmanaged<byte*, uint*, IntPtr*, IntPtr, Result> userOpen,
            delegate* unmanaged<IntPtr, IntPtr, Result> userClose,
            delegate* unmanaged<IntPtr, byte*, uint, uint*, IntPtr, Result> userRead,
            delegate* unmanaged<IntPtr, uint, IntPtr, Result> userSeek,
            delegate* unmanaged<AsyncReadInfo*, IntPtr, Result> userAsyncRead,
            delegate* unmanaged<AsyncReadInfo*, IntPtr, Result> userAsyncCancel,
            int blockAlignment = -1)
        {
            library.System_SetFileSystem(Handle, userOpen, userClose, userRead, userSeek, userAsyncRead, userAsyncCancel, blockAlignment).CheckResult();
        }

        public void AttachFileSystem(
            delegate* unmanaged<byte*, uint*, IntPtr*, IntPtr, Result> userOpen,
            delegate* unmanaged<IntPtr, IntPtr, Result> userClose,
            delegate* unmanaged<IntPtr, byte*, uint, uint*, IntPtr, Result> userRead,
            delegate* unmanaged<IntPtr, uint, IntPtr, Result> userSeek)
        {
            library.System_AttachFileSystem(Handle, userOpen, userClose, userRead, userSeek).CheckResult();
        }

        public void GetAdvancedSettings(AdvancedSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            library.System_GetAdvancedSettings(Handle, settings).CheckResult();
        }

        public AdvancedSettings GetAdvancedSettings()
        {
            var settings = new AdvancedSettings();
            library.System_GetAdvancedSettings(Handle, settings).CheckResult();
            return settings;
        }

        public void SetAdvancedSettings(AdvancedSettings settings)
        {
            library.System_SetAdvancedSettings(Handle, ref settings.Struct).CheckResult();
        }

        public unsafe void SetCallback(delegate* unmanaged<IntPtr, SystemCallbackType, IntPtr, IntPtr, IntPtr, Result> callback, SystemCallbackType mask)
        {
            library.System_SetCallback(Handle, callback, mask).CheckResult();
        }

        #endregion

        #region General Post-Init System Functions

        public void Update()
        {
            library.System_Update(Handle).CheckResult();
        }

        public void SetSpeakerPosition(Speaker speaker, float x, float y, bool active)
        {
            library.System_SetSpeakerPosition(Handle, speaker, x, y, active).CheckResult();
        }

        public void GetSpeakerPosition(Speaker speaker, out float x, out float y, out FmodBool active)
        {
            library.System_GetSpeakerPosition(Handle, speaker, out x, out y, out active).CheckResult();
        }

        public void Set3DSettings(float DopplerScale, float DistanceFactor, float RolloffScale)
        {
            library.System_Set3DSettings(Handle, DopplerScale, DistanceFactor, RolloffScale).CheckResult();
        }

        public void Get3DSettings(out float DopplerScale, out float DistanceFactor, out float RolloffScale)
        {
            library.System_Get3DSettings(Handle, out DopplerScale, out DistanceFactor, out RolloffScale).CheckResult();
        }

        public int ListenerCount3D
        {
            get
            {
                int value;
                library.System_Get3DNumListeners(Handle, &value).CheckResult();

                return value;
            }

            set
            {
                library.System_Set3DNumListeners(Handle, value).CheckResult();
            }
        }

        public void Set3DListenerAttributes(int listener, in Vector3 pos, in Vector3 vel, in Vector3 forward, in Vector3 up)
        {
            library.System_Set3DListenerAttributes(Handle, listener, in pos, in vel, in forward, in up).CheckResult();
        }

        public void Get3DListenerAttributes(int listener, out Vector3 pos, out Vector3 vel, out Vector3 forward, out Vector3 up)
        {
            library.System_Get3DListenerAttributes(Handle, listener, out pos, out vel, out forward, out up).CheckResult();
        }

        public void Set3DRolloffCallback(delegate* unmanaged<IntPtr, float, float> callback)
        {
            library.System_Set3DRolloffCallback(Handle, callback).CheckResult();
        }

        public void MixerSuspend()
        {
            library.System_MixerSuspend(Handle).CheckResult();
        }

        public void MixerResume()
        {
            library.System_MixerResume(Handle).CheckResult();
        }

        /// <summary>
        /// Warning, don't call this method without reading the FMOD Documentation on FMOD_System_GetDefaultMixMatrix()
        /// </summary>
        /// <param name="sourcespeakermode"></param>
        /// <param name="targetspeakermode"></param>
        /// <param name="matrix"></param>
        /// <param name="matrixhop"></param>
        public void GetDefaultMixMatrix(SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, Span<float> matrix, int matrixhop)
        {
            if (matrix.IsEmpty)
            {
                throw new ArgumentException("Empty span passed for Matrix", nameof(matrix));
            }

            int required = GetSpeakerModeChannels(sourcespeakermode) * GetSpeakerModeChannels(targetspeakermode);

            if (matrix.Length < required)
            {
                throw new ArgumentException("Matrix span is smaller than required.", nameof(matrix));
            }

            fixed (float* pMatrix = matrix)
            {
                library.System_GetDefaultMixMatrix(Handle, sourcespeakermode, targetspeakermode, pMatrix, matrixhop).CheckResult();
            }
        }

        public int GetSpeakerModeChannels(SpeakerMode mode)
        {
            int channels;
            library.System_GetSpeakerModeChannels(Handle, mode, &channels).CheckResult();

            return channels;
        }

        #endregion

        #region System Information Functions

        public FmodVersion Version
        {
            get
            {
                library.System_GetVersion(Handle, out FmodVersion version).CheckResult();

                return version;
            }
        }

        public IntPtr OutputHandle
        {
            get
            {
                IntPtr value;
                library.System_GetOutputHandle(Handle, &value).CheckResult();

                return value;
            }
        }
        
        public void GetChannelsPlaying(out int channels, out int realchannels)
        {
            library.System_GetChannelsPlaying(Handle, out channels, out realchannels).CheckResult();
        }
        
        public void GetCPUUsage(out float dsp, out float stream, out float geometry, out float update, out float total)
        {
            library.System_GetCPUUsage(Handle, out dsp, out stream, out geometry, out update, out total).CheckResult();
        }

        public void GetFileUsage(out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
        {
            library.System_GetFileUsage(Handle, out sampleBytesRead, out streamBytesRead, out otherBytesRead).CheckResult();
        }
        #endregion

        #region Sound/DSP/Channel/FX Creation and Retrieval

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Mode MemoryBits(Mode mode, bool clear)
        {
            const uint cmask = ~(uint)(Mode.OpenMemory | Mode.OpenMemory_Point);
            const uint OMPBit = (uint)Mode.OpenMemory_Point;

            uint m = (uint)mode;

            if (clear)
                m &= cmask;
            else if ((m & OMPBit) > 0)
            {
                m &= ~OMPBit;
                m |= (uint)Mode.OpenMemory;
            }

            return (Mode)m;
        }

        public unsafe Sound CreateSound(string Filename, Mode mode = Mode.Default, CreateSoundInfo? info = null)
        {
            if (Filename is null)
            {
                throw new ArgumentNullException(nameof(Filename));
            }

            SoundHandle handle;

            fixed(byte* dataPtr = FmodHelpers.ToUTF8NullTerminated(Filename))
            {
                library.System_CreateSound(Handle, dataPtr, MemoryBits(mode, true), info, &handle).CheckResult();
            }

            return handle;
        }

        public unsafe SoundHandle CreateSound(ReadOnlySpan<byte> data, Mode mode, CreateSoundInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (info.Length == 0)
            {
                throw new ArgumentException("Data length not specified in CreateSoundInfo.Length");
            }

            if (data.Length < info.Length)
            {
                throw new ArgumentException("data length less than length specified in CreateSoundInfo structure.");
            }

            SoundHandle handle;

            fixed(byte* dataPtr = data)
            {
                library.System_CreateSound(Handle, dataPtr, MemoryBits(mode, false), info, &handle).CheckResult();
            }

            return handle;
        }

        /// <summary>
        /// Creates a User Created Sound
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public unsafe SoundHandle CreateSoundOpenUser(Mode mode, CreateSoundInfo info)
        {
            mode = (mode & ~(Mode.OpenMemory_Point | Mode.OpenMemory)) | Mode.OpenUser;

            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            SoundHandle handle;

            library.System_CreateSound(Handle, null, mode, info, &handle).CheckResult();

            return handle;
        }

        public SoundHandle CreateStream(string Filename, Mode mode = Mode.Default, CreateSoundInfo? info = null)
        {
            return CreateSound(Filename, mode | Mode.CreateStream, info);
        }

        public unsafe DspHandle CreateDSP(in DspDescriptionStruct description)
        {
            DspHandle handle;
            fixed (DspDescriptionStruct* pDescription = &description)
                library.System_CreateDSP(Handle, pDescription, &handle).CheckResult();

            return handle;
        }

        public DspHandle CreateDSPByType(DSPType type)
        {
            DspHandle handle;
            library.System_CreateDSPByType(Handle, type, &handle).CheckResult();

            return handle;
        }
        
        public unsafe ChannelGroupHandle CreateChannelGroup(string name)
        {
            ChannelGroupHandle handle;
            library.System_CreateChannelGroup(Handle, name, &handle).CheckResult();

            return handle;
        }

        public unsafe SoundGroupHandle CreateSoundGroup(string name)
        {
            SoundGroupHandle handle;
            library.System_CreateSoundGroup(Handle, name, &handle).CheckResult();

            return handle;
        }
        
        public ChannelHandle PlaySound(SoundHandle sound, ChannelGroupHandle group = default, bool paused = false)
        {
            ChannelHandle handle;
            library.System_PlaySound(Handle, sound, group, paused, &handle).CheckResult();

            return handle;
        }

        public ChannelHandle PlayDsp(DspHandle dsp, ChannelGroupHandle group = default, bool paused = false)
        {
            ChannelHandle handle;
            library.System_PlayDSP(Handle, dsp, group, paused, &handle).CheckResult();
            return handle;
        }

        public ChannelHandle GetChannel(int channelID)
        {
            ChannelHandle handle;
            library.System_GetChannel(Handle, channelID, &handle).CheckResult();

            return handle;
        }

        public void AttachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroupHandle channelGroup, bool passThrough = false)
        {
            if (channelGroup.IsNull())
                throw new ArgumentNullException(nameof(channelGroup));

            library.System_AttachChannelGroupToPort(Handle, portType, portIndex, channelGroup, passThrough).CheckResult();
        }

        public void DetachChannelGroupFromPort(ChannelGroupHandle channelGroup)
        {
            if (channelGroup.IsNull())
                throw new ArgumentNullException(nameof(channelGroup));

            library.System_DetachChannelGroupFromPort(Handle, channelGroup).CheckResult();
        }

        public void SetReverbProperties(int instance, in ReverbProperties prop)
        {
            library.System_SetReverbProperties(Handle, instance, in prop).CheckResult();
        }

        public void GetReverbProperties(int instance, out ReverbProperties prop)
        {
            library.System_GetReverbProperties(Handle, instance, out prop).CheckResult();
        }

        public void LockDSP()
        {
            library.System_LockDSP(Handle).CheckResult();
        }

        public void UnlockDSP()
        {
            library.System_UnlockDSP(Handle).CheckResult();
        }

        public void GetRecordDriverCount(out int numDrivers, out int numConnected)
        {
            library.System_GetRecordNumDrivers(Handle, out numDrivers, out numConnected).CheckResult();
        }

        public unsafe RecordDriverInfo GetRecordDriverInfo(int ID)
        {
            const int AllocSize = Fmod.MaxInteropNameStringLength;
            Guid guid;
            int rate, channels;
            SpeakerMode mode;
            DriverState state;

            var NamePtr = stackalloc byte[AllocSize];

            library.System_GetRecordDriverInfo(Handle, ID, NamePtr, AllocSize, &guid, &rate, &mode, &channels, &state).CheckResult();

            return new RecordDriverInfo(FmodHelpers.BufferToString(NamePtr, AllocSize), guid, rate, mode, channels, state);
        }

        public unsafe uint GetRecordPosition(int ID)
        {
            uint position = 0;
            library.System_GetRecordPosition(Handle, ID, &position).CheckResult();

            return position;
        }

        public void RecordStart(int ID, SoundHandle sound, bool loop)
        {
            if (sound.IsNull())
                throw new ArgumentNullException(nameof(sound));

            library.System_RecordStart(Handle, ID, sound, loop).CheckResult();
        }

        public void RecordStop(int ID)
        {
            library.System_RecordStop(Handle, ID).CheckResult();
        }

        public bool IsRecording(int ID)
        {
            FmodBool value;
            var res = library.System_IsRecording(Handle, ID, &value);

            if (res != Result.Ok)
            {
                if (res == Result.Err_Record_Disconnected)
                {
                    return false;
                }

                res.CheckResult();
            }

            return value;
        }

        public unsafe GeometryHandle CreateGeomatry(int maxPolygons, int maxVertices)
        {
            GeometryHandle handle = default;
            library.System_CreateGeometry(Handle, maxPolygons, maxVertices, &handle).CheckResult();

            return handle;
        }

        public float GeomatryWorldSize
        {
            get
            {
                float worldSize;
                library.System_GetGeometrySettings(Handle, &worldSize).CheckResult();
                return worldSize;
            }

            set
            {
                library.System_SetGeometrySettings(Handle, value).CheckResult();
            }
        }

        public unsafe GeometryHandle LoadGeometry(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
                throw new ArgumentException("data.Length == 0", nameof(data));

            GeometryHandle handle;
            library.System_LoadGeometry(Handle, data, &handle).CheckResult();

            return handle;
        }

        public void GetGeometryOcclusion(out Vector3 listener, out Vector3 source, out float direct, out float reverb)
        {
            library.System_GetGeometryOcclusion(Handle, out listener, out source, out direct, out reverb).CheckResult();
        }

        public Reverb3DHandle CreateReverb3D()
        {
            Reverb3DHandle handle;
            library.System_CreateReverb3D(Handle, &handle).CheckResult();
            return handle;
        }

        public unsafe string NetworkProxy
        {
            get
            {
                Result res;
                int size = 200;
                int RetryCount = 2;

                Retry:
                byte[] buffer = new byte[size];

                fixed (byte* ptr = buffer)
                {
                    res = library.System_GetNetworkProxy(Handle, ptr, size);
                }

                if (res != Result.Ok)
                {
                    if (res == Result.Err_Truncated && RetryCount > 0)
                    {
                        size += 100;
                        RetryCount -= 1;
                        goto Retry;
                    }

                    res.CheckResult();
                }

                return FmodHelpers.BufferToString(buffer);
            }
            set
            {
                library.System_SetNetworkProxy(Handle, value).CheckResult();
            }
        }

        public int NetworkTimeout
        {
            get
            {
                int value;
                library.System_GetNetworkTimeout(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.System_SetNetworkTimeout(Handle, value).CheckResult();
            }
        }

        internal IntPtr UserData
        {
            get
            {
                IntPtr value;
                library.System_GetUserData(Handle, &value).CheckResult();
                return value;
            }

            set
            {
                library.System_SetUserData(Handle, value).CheckResult();
            }
        }

        #endregion
    }
}
