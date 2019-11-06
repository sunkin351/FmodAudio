#pragma warning disable IDE0052
#pragma warning disable IDE1006

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Text;

namespace FmodAudio
{
    using Interop;
    using Dsp;

    public unsafe sealed partial class FmodSystem : HandleBase
    {
        

        internal const int MaxInteropNameStringLength = 200;

        /// <summary>
        /// Affects string interop marshalling. If the buffer turns out to be too small, this will decide whether to throw an exception, or continue with the truncated string.
        /// </summary>
        public static bool AllowStringTruncation { get; set; }
        
        internal NativeLibrary library;

        private bool SystemOpen = false;
        
        public ChannelGroup MasterChannelGroup { get; private set; }
        public SoundGroup MasterSoundGroup { get; private set; }

        #region System Creation / Destruction

        internal FmodSystem(NativeLibrary lib, IntPtr handle) : base(handle)
        {
            library = lib;
            SetupEventCallbacks();
        }

        protected override void ReleaseImpl()
        {
            Close();
            
            lock (Fmod.CreationSyncObject)
            {
                library.System_Release(Handle).CheckResult();

                Fmod.SystemLookup.TryRemove(Handle, out _);
            }
        }

        public void Init(int MaxChannels, InitFlags flags = InitFlags.Normal, IntPtr ExtraDriverData = default) //default(IntPtr) == IntPtr.Zero
        {
            if (SystemOpen)
            {
                throw new InvalidOperationException("FmodSystem object already initialized"); //Semantic similarity with the native call
            }

            library.System_Init(Handle, MaxChannels, flags, ExtraDriverData).CheckResult();

            SystemOpen = true;

            SetupMasterGroups();
        }

        private void SetupMasterGroups()
        {
            IntPtr handle = default;
            library.System_GetMasterChannelGroup(Handle, &handle).CheckResult();

            {
                ChannelGroup group = GetChannelGroup(handle);
                group.IsMaster = true;
                MasterChannelGroup = group;
            }

            handle = default;
            library.System_GetMasterSoundGroup(Handle, &handle).CheckResult();

            {
                SoundGroup group = GetSoundGroup(handle);
                group.IsMaster = true;
                MasterSoundGroup = group;
            }
        }

        public void Close()
        {
            if (!SystemOpen)
                return;

            library.System_Close(Handle).CheckResult();

            SystemOpen = false;

            MasterChannelGroup = null;
            MasterSoundGroup = null;
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
        public unsafe Plugin LoadPlugin(string Filename, uint Priority = 0)
        {
            uint handle = 0;
            library.System_LoadPlugin(Handle, Filename, &handle, Priority).CheckResult();
            return new Plugin(handle);
        }

        public void UnloadPlugin(Plugin plugin)
        {
            library.System_UnloadPlugin(Handle, plugin.Handle).CheckResult();
        }

        public int GetPluginCount (PluginType type)
        {
            int count = 0;
            library.System_GetNumPlugins(Handle, type, &count).CheckResult();
            return count;
        }
         
        public int GetNestedPluginCount(Plugin plugin)
        {
            int count = 0;
            library.System_GetNumNestedPlugins(Handle, plugin.Handle, &count).CheckResult();
            return count;
        }

        public Plugin GetPluginHandle(PluginType type, int index)
        {
            uint handle = 0;
            library.System_GetPluginHandle(Handle, type, index, &handle).CheckResult();
            return new Plugin(handle);
        }

        public Plugin GetNestedPlugin(Plugin plugin, int index)
        {
            uint handle = 0;
            library.System_GetNestedPlugin(Handle, plugin.Handle, index, &handle).CheckResult();
            return new Plugin(handle);
        }

        public unsafe PluginInfo GetPluginInfo(Plugin plugin)
        {
            const int buflen = MaxInteropNameStringLength;
            byte* buffer = stackalloc byte[buflen];
            PluginType type;
            FmodVersion version;

            library.System_GetPluginInfo(Handle, plugin.Handle, &type, buffer, buflen, &version).CheckResult();

            string name = FmodHelpers.PtrToString(buffer, buflen);
            return new PluginInfo(name, type, version);
        }

        public Plugin OutputPlugin
        {
            get
            {
                uint handle = 0;
                library.System_GetOutputByPlugin(Handle, &handle).CheckResult();
                return new Plugin(handle);
            }

            set
            {
                library.System_SetOutputByPlugin(Handle, value.Handle).CheckResult();
            }
        }

        public Dsp.DSP CreateDSPByPlugin(Plugin plugin)
        {
            IntPtr handle = default;
            library.System_CreateDSPByPlugin(Handle, plugin.Handle, &handle).CheckResult();
            
            var dsp = GetDSP(handle);

            if (UserRegisteredDSPs.TryGetValue(plugin, out var value))
            {
                dsp.Description = value;
            }

            return dsp;
        }

        public DspDescription.Structure* GetDSPInfoByPlugin(Plugin plugin)
        {
            library.System_GetDSPInfoByPlugin(Handle, plugin.Handle, out IntPtr ptr).CheckResult();

            return (DspDescription.Structure*)ptr;
        }

        public Plugin RegisterDSP(DspDescription description)
        {
            uint handle;
            library.System_RegisterDSP(Handle, description, &handle).CheckResult();
            var plug = new Plugin(handle);

            if (!UserRegisteredDSPs.TryAdd(plug, description))
            {
                throw new InvalidOperationException();
            }

            return plug;
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
            const int AllocSize = MaxInteropNameStringLength;
            Guid guid;
            int rate, channels;
            SpeakerMode mode;

            var NamePtr = stackalloc byte[AllocSize];

            library.System_GetDriverInfo(Handle, DriverId, NamePtr, AllocSize, &guid, &rate, &mode, &channels).CheckResult();

            return new DriverInfo(FmodHelpers.PtrToString(NamePtr, AllocSize), guid, rate, mode, channels);
        }

        public void SetSoftwareFormat(int SampleRate, SpeakerMode speakerMode, int RawSpeakerCount)
        {
            library.System_SetSoftwareFormat(Handle, SampleRate, speakerMode, RawSpeakerCount).CheckResult();
        }

        public void GetSoftwareFormat(out int SampleRate, out SpeakerMode speakerMode, out int RawSpeakerCount)
        {
            library.System_GetSoftwareFormat(Handle, out SampleRate, out speakerMode, out RawSpeakerCount).CheckResult();
        }

        //Keep References to all delegates passed as filesystem functions
        private FileOpenCallback UserOpen;
        private FileCloseCallback UserClose;
        private FileReadCallback UserRead;
        private FileSeekCallback UserSeek;
        private FileAsyncReadCallback UserAsyncRead;
        private FileAsyncCancelCallback UserAsyncCancel;

        public void SetFileSystem(
            FileOpenCallback userOpen,
            FileCloseCallback userClose,
            FileReadCallback userRead,
            FileSeekCallback userSeek,
            FileAsyncReadCallback userAsyncRead,
            FileAsyncCancelCallback userAsyncCancel,
            int blockAlignment = -1)
        {
            library.System_SetFileSystem(Handle, userOpen, userClose, userRead, userSeek, userAsyncRead, userAsyncCancel, blockAlignment).CheckResult();

            UserOpen = userOpen;
            UserClose = userClose;
            UserRead = userRead;
            UserSeek = userSeek;
            UserAsyncRead = userAsyncRead;
            UserAsyncCancel = userAsyncCancel;
        }

        public void AttachFileSystem(
            FileOpenCallback userOpen,
            FileCloseCallback userClose,
            FileReadCallback userRead,
            FileSeekCallback userSeek)
        {
            library.System_AttachFileSystem(Handle, userOpen, userClose, userRead, userSeek).CheckResult();

            UserOpen = userOpen;
            UserClose = userClose;
            UserRead = userRead;
            UserSeek = userSeek;
        }

        public void GetAdvancedSettings(AdvancedSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            library.System_GetAdvancedSettings(Handle, ref settings.Struct).CheckResult();
        }

        public AdvancedSettings GetAdvancedSettings()
        {
            var settings = new AdvancedSettings();
            library.System_GetAdvancedSettings(Handle, ref settings.Struct).CheckResult();
            return settings;
        }

        public void SetAdvancedSettings(AdvancedSettings settings)
        {
            library.System_SetAdvancedSettings(Handle, ref settings.Struct).CheckResult();
        }

        public event Action DeviceListChanged;

        public event Action DeviceLost;

        public event Action<string> MemoryAllocationFailed;

        public event Action<DSP, DSP> BadDSPConnection;

        public event Action<ErrorCallbackInfo> Error;

        private SystemCallback syscallback;

        private unsafe Result SystemCallbackRoutine(IntPtr sysPtr, SystemCallbackType type, IntPtr ptr1, IntPtr ptr2, IntPtr userdata)
        {
            if (sysPtr != this.Handle)
            {
                return Result.Err_Invalid_Param;
            }

            try
            {
                switch (type)
                {
                    case SystemCallbackType.DeviceListChanged:
                        DeviceListChanged?.Invoke();
                        break;
                    case SystemCallbackType.DeviceLost:
                        DeviceLost?.Invoke();
                        break;
                    case SystemCallbackType.MemoryAllocationFailed:
                        if (MemoryAllocationFailed != null)
                        {
                            string debugStr = Encoding.ASCII.GetString(new Span<byte>(ptr1.ToPointer(), ptr2.ToInt32()));
                            MemoryAllocationFailed(debugStr);
                        }
                        break;
                    case SystemCallbackType.BadDSPConnection:
                        if (BadDSPConnection != null)
                        {
                            var target = new DSP(this, ptr1, false);
                            var source = new DSP(this, ptr2, false);
                            BadDSPConnection(target, source);
                        }
                        break;
                    case SystemCallbackType.Error:
                        Error?.Invoke(new ErrorCallbackInfo(ref *(ErrorCallbackInfoNative*)ptr1));
                        break;

                }
            }
            catch (FmodException e)
            {
                return e.Result ?? Result.Err_Internal;
            }
            catch
            {
                return Result.Err_Internal;
            }

            return Result.Ok;
        }

        private unsafe void SetupEventCallbacks()
        {
            const SystemCallbackType mask =
                SystemCallbackType.DeviceListChanged |
                SystemCallbackType.DeviceLost |
                SystemCallbackType.BadDSPConnection |
                SystemCallbackType.MemoryAllocationFailed |
                SystemCallbackType.Error;

            syscallback = this.SystemCallbackRoutine;

            library.System_SetCallback(Handle, syscallback, mask);
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

        public void GetSpeakerPosition(Speaker speaker, out float x, out float y, out bool active)
        {
            library.System_GetSpeakerPosition(Handle, speaker, out x, out y, out active).CheckResult();
        }

        public void SetStreamBufferSize(uint FileBufferSize, TimeUnit SizeType)
        {
            library.System_SetStreamBufferSize(Handle, FileBufferSize, SizeType).CheckResult();
        }

        public void GetStreamBufferSize(out uint FileBufferSize, out TimeUnit SizeType)
        {
            library.System_GetStreamBufferSize(Handle, out FileBufferSize, out SizeType).CheckResult();
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

        private _3DRolloffCallback rolloffCallback;

        public void Set3DRolloffCallback(_3DRolloffCallback callback)
        {
            library.System_Set3DRolloffCallback(Handle, callback).CheckResult();
            rolloffCallback = callback;
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

        public unsafe Sound CreateSound(string Filename, Mode mode = Mode.Default, CreateSoundInfo info = null)
        {
            if (Filename is null)
            {
                throw new ArgumentNullException(nameof(Filename));
            }

            IntPtr handle;

            fixed(byte* dataPtr = FmodHelpers.ToUTF8NullTerminated(Filename))
            {
                library.System_CreateSound(Handle, dataPtr, MemoryBits(mode, true), info, &handle).CheckResult();
            }

            var sound = GetSound(handle);

            sound.soundGroup = info?.InitialSoundGroup;

            return sound;
        }

        public unsafe Sound CreateSound(ReadOnlySpan<byte> data, Mode mode, CreateSoundInfo info)
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

            IntPtr handle;

            fixed(byte* dataPtr = data)
            {
                library.System_CreateSound(Handle, dataPtr, MemoryBits(mode, false), info, &handle).CheckResult();
            }

            var sound = GetSound(handle);

            sound.soundGroup = info.InitialSoundGroup;

            return sound;
        }

        /// <summary>
        /// Creates a User Created Sound
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public unsafe Sound CreateSoundOpenUser(Mode mode, CreateSoundInfo info)
        {
            mode = (mode & ~(Mode.OpenMemory_Point | Mode.OpenMemory)) | Mode.OpenUser;

            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            IntPtr handle;

            library.System_CreateSound(Handle, null, mode, info, &handle).CheckResult();

            var sound = GetSound(handle);

            sound.soundGroup = info.InitialSoundGroup;

            return sound;
        }

        public Sound CreateStream(string Filename, Mode mode = Mode.Default, CreateSoundInfo info = null)
        {
            if (Filename is null)
            {
                throw new ArgumentNullException(nameof(Filename));
            }

            IntPtr handle;
            fixed (byte* pFilename = FmodHelpers.ToUTF8NullTerminated(Filename))
            {
                library.System_CreateStream(Handle, pFilename, mode, info, &handle);
            }
            return GetSound(handle);
        }

        public unsafe DSP CreateDSP(DspDescription description)
        {
            IntPtr handle;
            library.System_CreateDSP(Handle, description, &handle).CheckResult();

            var dsp = GetDSP(handle);

            dsp.Description = description;

            return dsp;
        }

        public DSP CreateDSPByType(DSPType type)
        {
            IntPtr handle = default;
            library.System_CreateDSPByType(Handle, type, &handle).CheckResult();
            return GetDSP(handle);
        }
        
        public unsafe ChannelGroup CreateChannelGroup(string name)
        {
            IntPtr handle = default;
            library.System_CreateChannelGroup(Handle, name, &handle).CheckResult();
            return GetChannelGroup(handle);
        }

        public unsafe SoundGroup CreateSoundGroup(string name)
        {
            IntPtr handle;
            library.System_CreateSoundGroup(Handle, name, &handle).CheckResult();
            return GetSoundGroup(handle);
        }
        
        public Channel PlaySound(Sound sound, ChannelGroup group = null, bool paused = false)
        {
            library.System_PlaySound(Handle, sound.Handle, group?.Handle ?? IntPtr.Zero, paused, out IntPtr handle).CheckResult();

            return new Channel(this, handle);
        }

        public Channel PlayDsp(DSP dsp, ChannelGroup group = null, bool paused = false)
        {
            IntPtr handle;
            library.System_PlayDSP(Handle, dsp.Handle, group?.Handle ?? IntPtr.Zero, paused, &handle).CheckResult();
            return new Channel(this, handle);
        }

        public Channel GetChannel(int channelID)
        {
            IntPtr handle;
            library.System_GetChannel(Handle, channelID, &handle).CheckResult();

            return new Channel(this, handle);
        }

        public void AttachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroup channelGroup, bool passThrough = false)
        {
            if (channelGroup is null)
                throw new ArgumentNullException(nameof(channelGroup));

            library.System_AttachChannelGroupToPort(Handle, portType, portIndex, channelGroup.Handle, passThrough).CheckResult();
        }

        public void DetachChannelGroupFromPort(ChannelGroup channelGroup)
        {
            if (channelGroup is null)
                throw new ArgumentNullException(nameof(channelGroup));

            library.System_DetachChannelGroupFromPort(Handle, channelGroup.Handle).CheckResult();
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
            const int AllocSize = MaxInteropNameStringLength;
            Guid guid;
            int rate, channels;
            SpeakerMode mode;
            DriverState state;

            var NamePtr = stackalloc byte[AllocSize];

            library.System_GetRecordDriverInfo(Handle, ID, NamePtr, AllocSize, &guid, &rate, &mode, &channels, &state).CheckResult();

            return new RecordDriverInfo(FmodHelpers.PtrToString(NamePtr, AllocSize), guid, rate, mode, channels, state);
        }

        public unsafe uint GetRecordPosition(int ID)
        {
            uint position = 0;
            library.System_GetRecordPosition(Handle, ID, &position).CheckResult();

            return position;
        }

        public void RecordStart(int ID, Sound sound, bool loop)
        {
            if (sound is null)
                throw new ArgumentNullException(nameof(sound));

            library.System_RecordStart(Handle, ID, sound.Handle, loop).CheckResult();
        }

        public void RecordStop(int ID)
        {
            library.System_RecordStop(Handle, ID).CheckResult();
        }

        public bool IsRecording(int ID)
        {
            var res = library.System_IsRecording(Handle, ID, out bool value);

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

        public unsafe Geometry CreateGeomatry(int maxPolygons, int maxVertices)
        {
            IntPtr handle = default;
            library.System_CreateGeometry(Handle, maxPolygons, maxVertices, &handle).CheckResult();

            return GetGeometry(handle);
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

        public unsafe Geometry LoadGeometry(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
                return null;

            IntPtr handle;
            library.System_LoadGeometry(Handle, data, &handle).CheckResult();
            return GetGeometry(handle);
        }

        public void GetGeometryOcclusion(out Vector3 listener, out Vector3 source, out float direct, out float reverb)
        {
            library.System_GetGeometryOcclusion(Handle, out listener, out source, out direct, out reverb).CheckResult();
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

                return FmodHelpers.MemoryToString(buffer);
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

        public IntPtr UserData
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
