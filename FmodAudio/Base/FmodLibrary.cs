using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

using FmodAudio.Codec;
using FmodAudio.DigitalSignalProcessing;
using FmodAudio.Output;

namespace FmodAudio.Base
{
    [VTable]
    public unsafe sealed partial class FmodLibrary
    {
        [NotNull]
        private readonly Fmod.NativeLibraryHandle? Handle;

        internal FmodLibrary(Fmod.NativeLibraryHandle handle) : this(handle.DangerousGetHandle())
        {
            Handle = handle;
        }

        #region Global Functions
        /// <summary>
        /// Specifies a method for FMOD to allocate memory.
        /// </summary>
        /// <param name="poolmem">Block of memory of size poollen bytes for FMOD to manage, mutually exclusive with <paramref name="useralloc"/> / <paramref name="userrealloc"/> / <paramref name="userfree"/>.</param>
        /// <param name="poollen">Size of <paramref name="poolmem"/>, must be a multiple of 512.</param>
        /// <param name="useralloc">Memory allocation callback compatible with ANSI malloc, mutually exclusive with <paramref name="poolmem"/>.</param>
        /// <param name="userrealloc">Memory reallocation callback compatible with ANSI realloc, mutually exclusive with <paramref name="poolmem"/>.</param>
        /// <param name="userfree">Memory free callback compatible with ANSI free, mutually exclusive with <paramref name="poolmem"/>. </param>
        /// <param name="memtypeflags">Types of memory callbacks you wish to handle.</param>
        /// <remarks>
        /// This function must be called before any FMOD System object is created.
        /// 
        /// Valid usage of this function requires either <paramref name="poolmem"/> and <paramref name="poollen"/>, or <paramref name="useralloc"/>, <paramref name="userrealloc"/> and <paramref name="userfree"/> being set.
        /// If 'useralloc' and 'userfree' are provided without 'userrealloc' the reallocation is implemented via an allocation of the new size, copy from old address to new, then a free of the old address.
        /// 
        /// To find out the required fixed size call this method with an overly large pool size (or no pool) and find out the maximum RAM usage at any one time with <see cref="Memory_GetStats(int*, int*, FmodBool)"/>.
        /// 
        /// Callback implementations must be thread safe.
        /// 
        /// If you specify a fixed size pool that is too small, FMOD will return <see cref="Result.Err_Memory"/> when the limit of the fixed size pool is exceeded. At this point, it's possible that FMOD may become unstable. To maintain stability, do not allow FMOD to run out of memory.
        /// </remarks>
        [InteropMethod]
        public partial Result Memory_Initialize(IntPtr poolmem, int poollen, delegate* unmanaged<uint, MemoryType, IntPtr, IntPtr> useralloc, delegate* unmanaged<IntPtr, uint, MemoryType, IntPtr, IntPtr> userrealloc, delegate* unmanaged<IntPtr, MemoryType, IntPtr, void> userfree, MemoryType memtypeflags);

        /// <summary>
        /// Returns information on the memory usage of FMOD.
        /// </summary>
        /// <param name="currentalloced">Currently allocated memory at time of call.</param>
        /// <param name="maxalloced">Maximum allocated memory since System</param>
        /// <param name="blocking">
        /// Boolean indicating whether to favour speed or accuracy.
        /// Specifying true for this parameter will flush the DSP network to make sure all queued allocations happen immediately, which can be costly.
        /// </param>
        /// <remarks>
        /// This information is byte accurate and counts all allocs and frees internally.
        /// This is useful for determining a fixed memory size to make FMOD work within for fixed memory machines such as consoles.
        /// 
        /// Note that if using <see cref="Memory_Initialize(IntPtr, int, delegate*{uint, MemoryType, IntPtr, IntPtr}, delegate*{IntPtr, uint, MemoryType, IntPtr, IntPtr}, delegate*{IntPtr, MemoryType, IntPtr, void}, MemoryType)"/>, the memory usage will be slightly higher than without it, as FMOD has to have a small amount of memory overhead to manage the available memory.
        /// </remarks>
        [InteropMethod]
        public partial Result Memory_GetStats(int* currentAlloced, int* maxAlloced, FmodBool blocking);

        /// <inheritdoc cref="Memory_GetStats(int*, int*, FmodBool)"/>
        public Result Memory_GetStats(out int currentAlloced, out int maxAlloced, FmodBool blocking)
        {
            fixed (int* pCurrent = &currentAlloced, pMax = &maxAlloced)
            {
                return Memory_GetStats(pCurrent, pMax, blocking);
            }
        }

        /// <summary>
        /// Specify the level and delivery method of log messages when using the logging version of FMOD.
        /// </summary>
        /// <param name="flags">Bitfield representing the desired log information. Note: LOG implies WARN and WARN implies ERROR. </param>
        /// <param name="mode">Destination for log messages.</param>
        /// <param name="callback">Callback to use when mode is set to callback, only required when using that mode.</param>
        /// <param name="filename">Filename to use when mode is set to file, only required when using that mode.</param>
        /// <remarks>
        /// This function will return <see cref="Result.Err_Unsupported"/> when using the non-logging (release) versions of FMOD.
        /// The logging version of FMOD can be recognized by the 'L' suffix in the library name, `fmodL.dll` or `libfmodL.so` for instance.
        /// </remarks>
        [InteropMethod]
        public partial Result Debug_Initialize(DebugFlags flags, DebugMode mode, delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, Result> callback, byte* filename = null);

        ///<inheritdoc cref="Debug_Initialize(DebugFlags, DebugMode, delegate*{DebugFlags, byte*, int, byte*, byte*, Result}, byte*)"/>
        public Result Debug_Initialize(DebugFlags flags, DebugMode mode, delegate* unmanaged<DebugFlags, byte*, int, byte*, byte*, Result> callback, string? filename)
        {
            fixed (byte* pFilename = FmodHelpers.ToUTF8NullTerminated(filename))
            {
                return Debug_Initialize(flags, mode, callback, pFilename);
            }
        }

        /// <summary>
        /// Sets the busy state for disk access ensuring mutual exclusion of file operations.
        /// </summary>
        /// <param name="busy">Busy state where true represent the begining of disk access and false represents the end of disk access.</param>
        /// <remarks>
        /// If file IO is currently being performed by FMOD this function will block until it has completed.
        ///
        /// This function should be called in pairs once to set the state, then again to clear it once complete.
        /// </remarks>
        [InteropMethod]
        public partial Result File_SetDiskBusy(FmodBool busy);

        /// <summary>
        /// Information function to retrieve the state of FMOD disk access.
        /// </summary>
        /// <param name="busy">Busy state of the disk at the current time. true being busy, false being not busy</param>
        /// <remarks>
        /// Do not use this function to synchronize your own reads with, as due to timing,
        /// you might call this function and it says false = it is not busy, but the split second after call this function,
        /// internally FMOD might set it to busy.
        /// Use <see cref="File_SetDiskBusy(FmodBool)"/> for proper mutual exclusion as it uses semaphores.
        /// </remarks>
        [InteropMethod]
        public partial Result File_GetDiskBusy(FmodBool* busy);

        ///<inheritdoc cref="File_GetDiskBusy(FmodBool*)"/>
        public Result File_GetDiskBusy(out FmodBool busy)
        {
            fixed (FmodBool* pBusy = &busy)
            {
                return File_GetDiskBusy(pBusy);
            }
        }
        #endregion

        #region System Functions
        [InteropMethod]
        public partial Result System_Create(SystemHandle* System);

        public Result System_Create(out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return System_Create(pSystem);
            }
        }

        [InteropMethod]
        public partial Result System_Release(SystemHandle system);

        [InteropMethod]
        public partial Result System_SetOutput(SystemHandle system, OutputType output);

        [InteropMethod]
        public partial Result System_GetOutput(SystemHandle system, OutputType* output);

        public Result System_GetOutput(SystemHandle system, out OutputType output)
        {
            fixed (OutputType* pOutput = &output)
            {
                return System_GetOutput(system, pOutput);
            }
        }

        [InteropMethod]
        public partial Result System_GetNumDrivers(SystemHandle system, int* driverCount);

        public Result System_GetNumDrivers(SystemHandle system, out int driverCount)
        {
            fixed (int* pDriverCount = &driverCount)
            {
                return System_GetNumDrivers(system, pDriverCount);
            }
        }

        [InteropMethod]
        public partial Result System_GetDriverInfo(SystemHandle system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels);

        [InteropMethod]
        public partial Result System_SetDriver(SystemHandle system, int driver);

        [InteropMethod]
        public partial Result System_GetDriver(SystemHandle system, int* driver);

        public Result System_GetDriver(SystemHandle system, out int driver)
        {
            fixed (int* pDriver = &driver)
            {
                return System_GetDriver(system, pDriver);
            }
        }

        [InteropMethod]
        public partial Result System_SetSoftwareChannels(SystemHandle system, int softwareChannelCount);

        [InteropMethod]
        public partial Result System_GetSoftwareChannels(SystemHandle system, int* softwareChannelCount);
        
        public Result System_GetSoftwareChannels(SystemHandle system, out int softwareChannelCount)
        {
            fixed (int* pSCC = &softwareChannelCount)
            {
                return System_GetSoftwareChannels(system, pSCC);
            }
        }

        [InteropMethod]
        public partial Result System_SetSoftwareFormat(SystemHandle system, int sampleRate, SpeakerMode speakerMode, int rawSpeakerCount);

        [InteropMethod]
        public partial Result System_GetSoftwareFormat(SystemHandle system, int* sampleRate, SpeakerMode* speakerMode, int* rawSpeakerCount);

        public Result System_GetSoftwareFormat(SystemHandle system, out int sampleRate, out SpeakerMode speakerMode, out int rawSpeakerCount)
        {
            fixed (int* pSampleRate = &sampleRate, pRawSpeakerCount = &rawSpeakerCount)
            fixed (SpeakerMode* pSpeakerMode = &speakerMode)
            {
                return System_GetSoftwareFormat(system, pSampleRate, pSpeakerMode, pRawSpeakerCount);
            }
        }

        [InteropMethod]
        public partial Result System_SetDSPBufferSize(SystemHandle system, uint bufferLength, int bufferCount);

        [InteropMethod]
        public partial Result System_GetDSPBufferSize(SystemHandle system, uint* bufferLength, int* bufferCount);

        public Result System_GetDSPBufferSize(SystemHandle system, out uint bufferLength, out int bufferCount)
        {
            fixed (uint* pBufferLength = &bufferLength)
            fixed (int* pBufferCount = &bufferCount)
            {
                return System_GetDSPBufferSize(system, pBufferLength, pBufferCount);
            }
        }

        [InteropMethod]
        public partial Result System_SetFileSystem(SystemHandle system,
            delegate* unmanaged<byte*, uint*, IntPtr*, IntPtr, Result> useropen,
            delegate* unmanaged<IntPtr, IntPtr, Result> userclose,
            delegate* unmanaged<IntPtr, byte*, uint, uint*, IntPtr, Result> userread,
            delegate* unmanaged<IntPtr, uint, IntPtr, Result> userseek,
            delegate* unmanaged<AsyncReadInfo*, IntPtr, Result> userasyncread,
            delegate* unmanaged<AsyncReadInfo*, IntPtr, Result> userasynccancel,
            int blockalign);

        [InteropMethod]
        public partial Result System_AttachFileSystem(SystemHandle system,
            delegate* unmanaged<byte*, uint*, IntPtr*, IntPtr, Result> useropen,
            delegate* unmanaged<IntPtr, IntPtr, Result> userclose,
            delegate* unmanaged<IntPtr, byte*, uint, uint*, IntPtr, Result> userread,
            delegate* unmanaged<IntPtr, uint, IntPtr, Result> userseek);

        [InteropMethod]
        public partial Result System_SetAdvancedSettings(SystemHandle system, AdvancedSettings.Structure* settings);

        public Result System_SetAdvancedSettings(SystemHandle system, ref AdvancedSettings.Structure settings)
        {
            fixed (AdvancedSettings.Structure* pSettings = &settings)
            {
                return System_SetAdvancedSettings(system, pSettings);
            }
        }

        public Result System_SetAdvancedSettings(SystemHandle system, AdvancedSettings settings)
        {
            return System_SetAdvancedSettings(system, ref settings.Struct);
        }

        [InteropMethod]
        public partial Result System_GetAdvancedSettings(SystemHandle system, AdvancedSettings.Structure* settings);

        public Result System_GetAdvancedSettings(SystemHandle system, ref AdvancedSettings.Structure settings)
        {
            fixed (AdvancedSettings.Structure* pSettings = &settings)
            {
                return System_GetAdvancedSettings(system, pSettings);
            }
        }

        public Result System_GetAdvancedSettings(SystemHandle system, AdvancedSettings settings)
        {
            return System_GetAdvancedSettings(system, ref settings.Struct);
        }

        [InteropMethod]
        public partial Result System_SetCallback(SystemHandle system, delegate* unmanaged<IntPtr, SystemCallbackType, IntPtr, IntPtr, IntPtr, Result> callback, SystemCallbackType callbackmask);

        [InteropMethod]
        public partial Result System_SetPluginPath(SystemHandle system, byte* path);

        public Result System_SetPluginPath(SystemHandle system, ReadOnlySpan<char> path)
        {
            fixed (byte* pPath = FmodHelpers.ToUTF8NullTerminated(path))
            {
                return System_SetPluginPath(system, pPath);
            }
        }

        public Result System_SetPluginPath(SystemHandle system, string path)
        {
            return System_SetPluginPath(system, (ReadOnlySpan<char>)path);
        }

        [InteropMethod]
        public partial Result System_LoadPlugin(SystemHandle system, byte* filename, PluginHandle* handle, uint priority);

        public Result System_LoadPlugin(SystemHandle system, string filename, PluginHandle* handle, uint priority)
        {
            fixed (byte* pFilename = FmodHelpers.ToUTF8NullTerminated(filename))
            {
                return System_LoadPlugin(system, pFilename, handle, priority);
            }
        }

        public Result System_LoadPlugin(SystemHandle system, string filename, out PluginHandle handle, uint priority)
        {
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_LoadPlugin(system, filename, pHandle, priority);
            }
        }

        [InteropMethod]
        public partial Result System_UnloadPlugin(SystemHandle system, PluginHandle handle);

        [InteropMethod]
        public partial Result System_GetNumNestedPlugins(SystemHandle system, PluginHandle handle, int* count);

        public Result System_GetNumNestedPlugins(SystemHandle system, PluginHandle handle, out int count)
        {
            fixed (int* pCount = &count)
            {
                return System_GetNumNestedPlugins(system, handle, pCount);
            }
        }

        [InteropMethod]
        public partial Result System_GetNestedPlugin(SystemHandle system, PluginHandle handle, int index, PluginHandle* nestedhandle);

        public Result System_GetNestedPlugin(SystemHandle system, PluginHandle handle, int index, out PluginHandle nestedhandle)
        {
            fixed (PluginHandle* pHandle = &nestedhandle)
            {
                return System_GetNestedPlugin(system, handle, index, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_GetNumPlugins(SystemHandle system, PluginType plugintype, int* pluginCount);

        public Result System_GetNumPlugins(SystemHandle system, PluginType plugintype, out int pluginCount)
        {
            fixed (int* pCount = &pluginCount)
            {
                return System_GetNumPlugins(system, plugintype, pCount);
            }
        }

        [InteropMethod]
        public partial Result System_GetPluginHandle(SystemHandle system, PluginType plugintype, int index, PluginHandle* handle);

        public Result System_GetPluginHandle(SystemHandle system, PluginType plugintype, int index, out PluginHandle handle)
        {
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_GetPluginHandle(system, plugintype, index, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_GetPluginInfo(SystemHandle system, PluginHandle handle, PluginType* plugintype, byte* name, int namelen, FmodVersion* version);

        [InteropMethod]
        public partial Result System_CreateDSPByPlugin(SystemHandle system, PluginHandle handle, DspHandle* dsp);

        public Result System_CreateDSPByPlugin(SystemHandle system, PluginHandle handle, out DspHandle dsp)
        {
            fixed (DspHandle* pDsp = &dsp)
            {
                return System_CreateDSPByPlugin(system, handle, pDsp);
            }
        }

        [InteropMethod]
        public partial Result System_SetOutputByPlugin(SystemHandle system, PluginHandle handle);

        [InteropMethod]
        public partial Result System_GetOutputByPlugin(SystemHandle system, PluginHandle* handle);

        public Result System_GetOutputByPlugin(SystemHandle system, out PluginHandle handle)
        {
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_GetOutputByPlugin(system, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_GetDSPInfoByPlugin(SystemHandle system, PluginHandle handle, DspDescriptionStruct** description);

        public Result System_GetDSPInfoByPlugin(SystemHandle system, PluginHandle handle, out DspDescriptionStruct* description)
        {
            fixed (DspDescriptionStruct** pDescription = &description)
            {
                return System_GetDSPInfoByPlugin(system, handle, pDescription);
            }
        }

        [InteropMethod]
        public partial Result System_RegisterCodec(SystemHandle system, CodecDescriptionStruct* description, PluginHandle* handle, uint priority);

        public Result System_RegisterCodec(SystemHandle system, ref CodecDescriptionStruct description, PluginHandle* handle, uint priority)
        {
            fixed (CodecDescriptionStruct* pDescription = &description)
            {
                return System_RegisterCodec(system, pDescription, handle, priority);
            }
        }

        public Result System_RegisterCodec(SystemHandle system, ref CodecDescriptionStruct description, out PluginHandle handle, uint priority)
        {
            fixed (CodecDescriptionStruct* pDescription = &description)
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_RegisterCodec(system, pDescription, pHandle, priority);
            }
        }

        [InteropMethod]
        public partial Result System_RegisterDSP(SystemHandle system, DspDescriptionStruct* description, PluginHandle* handle);

        public Result System_RegisterDSP(SystemHandle system, ref DspDescriptionStruct description, PluginHandle* handle)
        {
            fixed (DspDescriptionStruct* pDescription = &description)
            {
                return System_RegisterDSP(system, pDescription, handle);
            }
        }

        public Result System_RegisterDSP(SystemHandle system, ref DspDescriptionStruct description, out PluginHandle handle)
        {
            fixed (DspDescriptionStruct* pDescription = &description)
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_RegisterDSP(system, pDescription, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_RegisterOutput(SystemHandle system, OutputDescriptionStruct* description, PluginHandle* handle);

        public Result System_RegisterOutput(SystemHandle system, ref OutputDescriptionStruct description, PluginHandle* handle)
        {
            fixed (OutputDescriptionStruct* pDescription = &description)
            {
                return System_RegisterOutput(system, pDescription, handle);
            }
        }

        public Result System_RegisterOutput(SystemHandle system, ref OutputDescriptionStruct description, out PluginHandle handle)
        {
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_RegisterOutput(system, ref description, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_Init(SystemHandle system, int maxchannels, InitFlags flags, IntPtr extradriverdata);

        [InteropMethod]
        public partial Result System_Close(SystemHandle system);

        [InteropMethod]
        public partial Result System_Update(SystemHandle system);

        [InteropMethod]
        public partial Result System_SetSpeakerPosition(SystemHandle system, Speaker speaker, float x, float y, FmodBool active);

        [InteropMethod]
        public partial Result System_GetSpeakerPosition(SystemHandle system, Speaker speaker, float* x, float* y, FmodBool* active);

        public Result System_GetSpeakerPosition(SystemHandle system, Speaker speaker, out float x, out float y, out FmodBool active)
        {
            fixed (float* pX = &x, pY = &y)
            fixed (FmodBool* pActive = &active)
            {
                return System_GetSpeakerPosition(system, speaker, pX, pY, pActive);
            }
        }

        [InteropMethod]
        public partial Result System_SetStreamBufferSize(SystemHandle system, uint filebuffersize, TimeUnit filebuffersizetype);

        [InteropMethod]
        public partial Result System_GetStreamBufferSize(SystemHandle system, uint* filebuffersize, TimeUnit* filebuffersizetype);

        public Result System_GetStreamBufferSize(SystemHandle system, out uint fileBufferSize, out TimeUnit fileBufferSizeType)
        {
            fixed (uint* pFBS = &fileBufferSize)
            fixed (TimeUnit* pFBST = &fileBufferSizeType)
            {
                return System_GetStreamBufferSize(system, pFBS, pFBST);
            }
        }

        [InteropMethod]
        public partial Result System_Set3DSettings(SystemHandle system, float dopplerscale, float distancefactor, float rolloffscale);

        [InteropMethod]
        public partial Result System_Get3DSettings(SystemHandle system, float* dopplerscale, float* distancefactor, float* rolloffscale);

        public Result System_Get3DSettings(SystemHandle system, out float dopplerscale, out float distancefactor, out float rolloffscale)
        {
            fixed (float* pDopplerScale = &dopplerscale, pDistanceFactor = &distancefactor, pRolloffScale = &rolloffscale)
            {
                return System_Get3DSettings(system, pDopplerScale, pDistanceFactor, pRolloffScale);
            }
        }

        [InteropMethod]
        public partial Result System_Set3DNumListeners(SystemHandle system, int listenerCount);

        [InteropMethod]
        public partial Result System_Get3DNumListeners(SystemHandle system, int* listenerCount);

        public Result System_Get3DNumListeners(SystemHandle system, out int listenerCount)
        {
            fixed (int* pListeners = &listenerCount)
            {
                return System_Get3DNumListeners(system, pListeners);
            }
        }

        [InteropMethod]
        public partial Result System_Set3DListenerAttributes(SystemHandle system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);

        public Result System_Set3DListenerAttributes(SystemHandle system, int listener, in Vector3 pos, in Vector3 vel, in Vector3 forward, in Vector3 up)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pForward = &forward, pUp = &up)
            {
                return System_Set3DListenerAttributes(system, listener, pPos, pVel, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result System_Get3DListenerAttributes(SystemHandle system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);

        public Result System_Get3DListenerAttributes(SystemHandle system, int listener, out Vector3 pos, out Vector3 vel, out Vector3 forward, out Vector3 up)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pForward = &forward, pUp = &up)
            {
                return System_Get3DListenerAttributes(system, listener, pPos, pVel, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result System_Set3DRolloffCallback(SystemHandle system, delegate* unmanaged<IntPtr, float, float> callback);

        [InteropMethod]
        public partial Result System_MixerSuspend(SystemHandle system);

        [InteropMethod]
        public partial Result System_MixerResume(SystemHandle system);

        [InteropMethod]
        public partial Result System_GetDefaultMixMatrix(SystemHandle system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float* matrix, int matrixhop = 0);

        [InteropMethod]
        public partial Result System_GetSpeakerModeChannels(SystemHandle system, SpeakerMode mode, int* channels);

        public Result System_GetSpeakerModeChannels(SystemHandle system, SpeakerMode mode, out int channels)
        {
            fixed (int* pChannels = &channels)
            {
                return System_GetSpeakerModeChannels(system, mode, pChannels);
            }
        }

        [InteropMethod]
        public partial Result System_GetVersion(SystemHandle system, FmodVersion* version);

        public Result System_GetVersion(SystemHandle system, out FmodVersion version)
        {
            fixed (FmodVersion* pVersion = &version)
            {
                return System_GetVersion(system, pVersion);
            }
        }

        [InteropMethod]
        public partial Result System_GetOutputHandle(SystemHandle system, IntPtr* handle);

        public Result System_GetOutputHandle(SystemHandle system, out IntPtr handle)
        {
            fixed (IntPtr* pHandle = &handle)
            {
                return System_GetOutputHandle(system, pHandle);
            }
        }

        [InteropMethod]
        public partial Result System_GetChannelsPlaying(SystemHandle system, int* channels, int* realchannels);

        public Result System_GetChannelsPlaying(SystemHandle system, out int channels, out int realchannels)
        {
            fixed (int* pChannels = &channels, pRealChannels = &realchannels)
            {
                return System_GetChannelsPlaying(system, pChannels, pRealChannels);
            }
        }

        [InteropMethod]
        public partial Result System_GetCPUUsage(SystemHandle system, float* dsp, float* stream, float* geometry, float* update, float* total);

        public Result System_GetCPUUsage(SystemHandle system, out float dsp, out float stream, out float geometry, out float update, out float total)
        {
            fixed (float* pDsp = &dsp, pStream = &stream, pGeometry = &geometry, pUpdate = &update, ptotal = &total)
            {
                return System_GetCPUUsage(system, pDsp, pStream, pGeometry, pUpdate, ptotal);
            }
        }

        [InteropMethod]
        public partial Result System_GetFileUsage(SystemHandle system, long* sampleBytesRead, long* streamBytesRead, long* otherBytesRead);

        public Result System_GetFileUsage(SystemHandle system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
        {
            fixed (long* pSample = &sampleBytesRead, pStream = &streamBytesRead, pOther = &otherBytesRead)
            {
                return System_GetFileUsage(system, pSample, pStream, pOther);
            }
        }

        [InteropMethod]
        public partial Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfoStruct* info, SoundHandle* sound);

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfoStruct* info, out SoundHandle sound)
        { 
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, info, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, CreateSoundInfoStruct* info, SoundHandle* sound)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateSound(system, pName, mode, info, sound);
            }
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, CreateSoundInfoStruct* info, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, name, mode, info, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfoStruct info, SoundHandle* sound)
        {
            fixed (CreateSoundInfoStruct* pInfo = &info)
            {
                return System_CreateSound(system, nameOrData, mode, pInfo, sound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfoStruct info, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, ref info, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, ref CreateSoundInfoStruct info, SoundHandle* sound)
        {
            fixed (CreateSoundInfoStruct* pInfo = &info)
            {
                return System_CreateSound(system, name, mode, pInfo, sound);
            }
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, ref CreateSoundInfoStruct info, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, name, mode, ref info, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo? info, SoundHandle* sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfoStruct*)null, sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, sound);
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo? info, out SoundHandle sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfoStruct*)null, out sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, out sound);
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, CreateSoundInfo? info, SoundHandle* sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, name, mode, (CreateSoundInfoStruct*)null, sound);
            }

            return System_CreateSound(system, name, mode, ref info.Struct, sound);
        }

        public Result System_CreateSound(SystemHandle system, string name, Mode mode, CreateSoundInfo? info, out SoundHandle sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, name, mode, (CreateSoundInfoStruct*)null, out sound);
            }

            return System_CreateSound(system, name, mode, ref info.Struct, out sound);
        }

        [InteropMethod]
        public partial Result System_CreateDSP(SystemHandle system, DspDescriptionStruct* description, DspHandle* dsp);

        public Result System_CreateDSP(SystemHandle system, in DspDescriptionStruct description, DspHandle* dsp)
        {
            fixed (DspDescriptionStruct* pDescription = &description)
            {
                return System_CreateDSP(system, pDescription, dsp);
            }
        }

        public Result System_CreateDSP(SystemHandle system, in DspDescriptionStruct description, out DspHandle dsp)
        {
            fixed (DspHandle* pDsp = &dsp)
            {
                return System_CreateDSP(system, in description, pDsp);
            }
        }

        [InteropMethod]
        public partial Result System_CreateDSPByType(SystemHandle system, DSPType type, DspHandle* dsp);

        public Result System_CreateDSPByType(SystemHandle system, DSPType type, out DspHandle dsp)
        {
            fixed (DspHandle* pDsp = &dsp)
            {
                return System_CreateDSPByType(system, type, pDsp);
            }
        }

        [InteropMethod]
        public partial Result System_CreateChannelGroup(SystemHandle system, byte* name, ChannelGroupHandle* channelGroup);

        public Result System_CreateChannelGroup(SystemHandle system, byte* name, out ChannelGroupHandle channelGroup)
        {
            fixed (ChannelGroupHandle* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, name, pChannelGroup);
            }
        }

        public Result System_CreateChannelGroup(SystemHandle system, string name, ChannelGroupHandle* channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateChannelGroup(system, pName, channelGroup);
            }
        }

        public Result System_CreateChannelGroup(SystemHandle system, string name, out ChannelGroupHandle channelGroup)
        {
            fixed (ChannelGroupHandle* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, name, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result System_CreateSoundGroup(SystemHandle system, byte* name, SoundGroupHandle* soundGroup);

        public Result System_CreateSoundGroup(SystemHandle system, byte* name, out SoundGroupHandle soundGroup)
        {
            fixed (SoundGroupHandle* pSoundGroup = &soundGroup)
            {
                return System_CreateSoundGroup(system, name, pSoundGroup);
            }
        }

        public Result System_CreateSoundGroup(SystemHandle system, string name, SoundGroupHandle* soundGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateSoundGroup(system, pName, soundGroup);
            }
        }

        public Result System_CreateSoundGroup(SystemHandle system, string name, out SoundGroupHandle soundGroup)
        {
            fixed (SoundGroupHandle* pSoundGroup = &soundGroup)
            {
                return System_CreateSoundGroup(system, name, pSoundGroup);
            }
        }

        [InteropMethod]
        public partial Result System_CreateReverb3D(SystemHandle system, Reverb3DHandle* reverb);

        public Result System_CreateReverb3D(SystemHandle system, out Reverb3DHandle reverb)
        {
            fixed (Reverb3DHandle* pReverb = &reverb)
            {
                return System_CreateReverb3D(system, pReverb);
            }
        }

        [InteropMethod]
        public partial Result System_PlaySound(SystemHandle system, SoundHandle sound, ChannelGroupHandle channelGroup, FmodBool paused, ChannelHandle* channel);

        public Result System_PlaySound(SystemHandle system, SoundHandle sound, ChannelGroupHandle channelGroup, FmodBool paused, out ChannelHandle channel)
        {
            fixed (ChannelHandle* pChannel = &channel)
            {
                return System_PlaySound(system, sound, channelGroup, paused, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_PlayDSP(SystemHandle system, DspHandle dsp, ChannelGroupHandle channelGroup, FmodBool paused, ChannelHandle* channel);

        public Result System_PlayDSP(SystemHandle system, DspHandle dsp, ChannelGroupHandle channelGroup, FmodBool paused, out ChannelHandle channel)
        {
            fixed (ChannelHandle* pChannel = &channel)
            {
                return System_PlayDSP(system, dsp, channelGroup, paused, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_GetChannel(SystemHandle system, int channelid, ChannelHandle* channel);

        public Result System_GetChannel(SystemHandle system, int channelid, out ChannelHandle channel)
        {
            fixed (ChannelHandle* pChannel = &channel)
            {
                return System_GetChannel(system, channelid, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_GetMasterChannelGroup(SystemHandle system, ChannelGroupHandle* channelGroup);

        public Result System_GetMasterChannelGroup(SystemHandle system, out ChannelGroupHandle channelGroup)
        {
            fixed (ChannelGroupHandle* pChannelGroup = &channelGroup)
            {
                return System_GetMasterChannelGroup(system, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result System_GetMasterSoundGroup(SystemHandle system, SoundGroupHandle* soundGroup);

        public Result System_GetMasterSoundGroup(SystemHandle system, out SoundGroupHandle soundGroup)
        {
            fixed (SoundGroupHandle* pSoundGroup = &soundGroup)
            {
                return System_GetMasterSoundGroup(system, pSoundGroup);
            }
        }

        [InteropMethod]
        public partial Result System_AttachChannelGroupToPort(SystemHandle system, uint portType, ulong portIndex, ChannelGroupHandle channelGroup, bool passThru);

        [InteropMethod]
        public partial Result System_DetachChannelGroupFromPort(SystemHandle system, ChannelGroupHandle channelGroup);

        [InteropMethod]
        public partial Result System_SetReverbProperties(SystemHandle system, int instance, ReverbProperties* prop);

        public Result System_SetReverbProperties(SystemHandle system, int instance, in ReverbProperties prop)
        {
            fixed (ReverbProperties* pProp = &prop)
            {
                return System_SetReverbProperties(system, instance, pProp);
            }
        }

        [InteropMethod]
        public partial Result System_GetReverbProperties(SystemHandle system, int instance, ReverbProperties* prop);

        public Result System_GetReverbProperties(SystemHandle system, int instance, out ReverbProperties prop)
        {
            fixed (ReverbProperties* pProp = &prop)
            {
                return System_GetReverbProperties(system, instance, pProp);
            }
        }

        [InteropMethod]
        public partial Result System_LockDSP(SystemHandle system);

        [InteropMethod]
        public partial Result System_UnlockDSP(SystemHandle system);

        [InteropMethod]
        public partial Result System_GetRecordNumDrivers(SystemHandle system, int* numdrivers, int* numconnected);

        public Result System_GetRecordNumDrivers(SystemHandle system, out int driverCount, out int connectedCount)
        {
            fixed (int* pdrivers = &driverCount, pConnectedCount = &connectedCount)
            {
                return System_GetRecordNumDrivers(system, pdrivers, pConnectedCount);
            }
        }

        [InteropMethod]
        public partial Result System_GetRecordDriverInfo(SystemHandle system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels, DriverState* state);

        [InteropMethod]
        public partial Result System_GetRecordPosition(SystemHandle system, int id, uint* position);

        public Result System_GetRecordPosition(SystemHandle system, int id, out uint position)
        {
            fixed (uint* pPosition = &position)
            {
                return System_GetRecordPosition(system, id, pPosition);
            }
        }

        [InteropMethod]
        public partial Result System_RecordStart(SystemHandle system, int id, SoundHandle sound, bool loop);

        [InteropMethod]
        public partial Result System_RecordStop(SystemHandle system, int id);

        [InteropMethod]
        public partial Result System_IsRecording(SystemHandle system, int id, FmodBool* recording);

        public Result System_IsRecording(SystemHandle system, int id, out FmodBool recording)
        {
            fixed(FmodBool* pRecording = &recording)
            {
                return System_IsRecording(system, id, pRecording);
            }
        }

        [InteropMethod]
        public partial Result System_CreateGeometry(SystemHandle system, int maxpolygons, int maxvertices, GeometryHandle* geometry);

        public Result System_CreateGeometry(SystemHandle system, int maxpolygons, int maxvertices, out GeometryHandle geometry)
        {
            fixed (GeometryHandle* pGeometry = &geometry)
            {
                return System_CreateGeometry(system, maxpolygons, maxvertices, pGeometry);
            }
        }

        [InteropMethod]
        public partial Result System_SetGeometrySettings(SystemHandle system, float maxworldsize);

        [InteropMethod]
        public partial Result System_GetGeometrySettings(SystemHandle system, float* maxworldsize);

        public Result System_GetGeometrySettings(SystemHandle system, out float maxworldsize)
        {
            fixed (float* pMaxWorldsize = &maxworldsize)
            {
                return System_GetGeometrySettings(system, pMaxWorldsize);
            }
        }

        [InteropMethod]
        public partial Result System_LoadGeometry(SystemHandle system, byte* data, int datasize, GeometryHandle* geometry);

        public Result System_LoadGeometry(SystemHandle system, byte* data, int datasize, out GeometryHandle geometry)
        {
            fixed (GeometryHandle* pGeometry = &geometry)
            {
                return System_LoadGeometry(system, data, datasize, pGeometry);
            }
        }

        public Result System_LoadGeometry(SystemHandle system, ReadOnlySpan<byte> data, GeometryHandle* geometry)
        {
            fixed (byte* pData = data)
            {
                return System_LoadGeometry(system, pData, data.Length, geometry);
            }
        }

        public Result System_LoadGeometry(SystemHandle system, ReadOnlySpan<byte> data, out GeometryHandle geometry)
        {
            fixed (byte* pData = data)
            fixed (GeometryHandle* pGeometry = &geometry)
            {
                return System_LoadGeometry(system, pData, data.Length, pGeometry);
            }
        }

        [InteropMethod]
        public partial Result System_GetGeometryOcclusion(SystemHandle system, Vector3* listener, Vector3* source, float* direct, float* reverb);

        public Result System_GetGeometryOcclusion(SystemHandle system, out Vector3 listener, out Vector3 source, out float direct, out float reverb)
        {
            fixed (Vector3* pListener = &listener, pSource = &source)
            fixed (float* pDirect = &direct, pReverb = &reverb)
            {
                return System_GetGeometryOcclusion(system, pListener, pSource, pDirect, pReverb);
            }
        }

        [InteropMethod]
        public partial Result System_SetNetworkProxy(SystemHandle system, byte* proxy);

        public Result System_SetNetworkProxy(SystemHandle system, string proxy)
        {
            fixed (byte* pProxy = FmodHelpers.ToUTF8NullTerminated(proxy))
            {
                return System_SetNetworkProxy(system, pProxy);
            }
        }

        [InteropMethod]
        public partial Result System_GetNetworkProxy(SystemHandle system, byte* proxy, int proxylen);

        [InteropMethod]
        public partial Result System_SetNetworkTimeout(SystemHandle system, int timeout);

        [InteropMethod]
        public partial Result System_GetNetworkTimeout(SystemHandle system, int* timeout);

        public Result System_GetNetworkTimeout(SystemHandle system, out int timeout)
        {
            fixed (int* pTimeout = &timeout)
            {
                return System_GetNetworkTimeout(system, pTimeout);
            }
        }

        [InteropMethod]
        internal partial Result System_SetUserData(SystemHandle system, IntPtr userdata);

        [InteropMethod]
        internal partial Result System_GetUserData(SystemHandle system, IntPtr* userdata);
        #endregion

        #region Sound Functions
        [InteropMethod]
        public partial Result Sound_Release(SoundHandle sound);

        [InteropMethod]
        public partial Result Sound_GetSystemObject(SoundHandle sound, SystemHandle* system);

        public Result Sound_GetSystemObject(SoundHandle sound, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return Sound_GetSystemObject(sound, pSystem);
            }
        }

        [InteropMethod]
        public partial Result Sound_Lock(SoundHandle sound, uint offset, uint length, IntPtr* ptr1, IntPtr* ptr2, uint* len1, uint* len2);

        public Result Sound_Lock(SoundHandle sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            fixed (IntPtr* p1 = &ptr1, p2 = &ptr2)
            fixed (uint* l1 = &len1, l2 = &len2)
            {
                return Sound_Lock(sound, offset, length, p1, p2, l1, l2);
            }
        }

        [InteropMethod]
        public partial Result Sound_Unlock(SoundHandle sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);

        [InteropMethod]
        public partial Result Sound_SetDefaults(SoundHandle sound, float frequency, int priority);

        [InteropMethod]
        public partial Result Sound_GetDefaults(SoundHandle sound, float* frequency, int* priority);

        public Result Sound_GetDefaults(SoundHandle sound, out float frequency, out int priority)
        {
            fixed (float* pFrequency = &frequency)
            fixed (int* pPriority = &priority)
            {
                return Sound_GetDefaults(sound, pFrequency, pPriority);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DMinMaxDistance(SoundHandle sound, float min, float max);

        [InteropMethod]
        public partial Result Sound_Get3DMinMaxDistance(SoundHandle sound, float* min, float* max);

        public Result Sound_Get3DMinMaxDistance(SoundHandle sound, out float min, out float max)
        {
            fixed (float* pMin = &min, pMax = &max)
            {
                return Sound_Get3DMinMaxDistance(sound, pMin, pMax);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DConeSettings(SoundHandle sound, float insideconeangle, float outsideconeangle, float outsidevolume);

        [InteropMethod]
        public partial Result Sound_Get3DConeSettings(SoundHandle sound, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result Sound_Get3DConeSettings(SoundHandle sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideConeAngle = &insideconeangle, pOutsideConeAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return Sound_Get3DConeSettings(sound, pInsideConeAngle, pOutsideConeAngle, pOutsideVolume);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DCustomRolloff(SoundHandle sound, RolloffPoint* points, int pointCount);

        [InteropMethod]
        public partial Result Sound_Get3DCustomRolloff(SoundHandle sound, RolloffPoint** points, int* pointCount);

        public Result Sound_Get3DCustomRolloff(SoundHandle sound, out RolloffPoint* points, out int pointCount)
        {
            fixed (RolloffPoint** pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return Sound_Get3DCustomRolloff(sound, pPoints, pPointCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSubSound(SoundHandle sound, int index, SoundHandle* subsound);

        public Result Sound_GetSubSound(SoundHandle sound, int index, out SoundHandle subsound)
        {
            fixed (SoundHandle* pSubsound = &subsound)
            {
                return Sound_GetSubSound(sound, index, pSubsound);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSubSoundParent(SoundHandle sound, SoundHandle* parentsound);

        public Result Sound_GetSubSoundParent(SoundHandle sound, out SoundHandle parentsound)
        {
            fixed (SoundHandle* pParent = &parentsound)
            {
                return Sound_GetSubSoundParent(sound, pParent);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetName(SoundHandle sound, byte* name, int namelen);

        [InteropMethod]
        public partial Result Sound_GetLength(SoundHandle sound, uint* length, TimeUnit lengthtype);

        public Result Sound_GetLength(SoundHandle sound, out uint length, TimeUnit lengthtype)
        {
            fixed (uint* pLength = &length)
            {
                return Sound_GetLength(sound, pLength, lengthtype);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetFormat(SoundHandle sound, SoundType* type, SoundFormat* format, int* channels, int* bits);

        public Result Sound_GetFormat(SoundHandle sound, out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            fixed (SoundType* pType = &type)
            fixed (SoundFormat* pFormat = &format)
            fixed (int* pChannels = &channels, pBits = &bits)
            {
                return Sound_GetFormat(sound, pType, pFormat, pChannels, pBits);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumSubSounds(SoundHandle sound, int* numsubsounds);

        public Result Sound_GetNumSubSounds(SoundHandle sound, out int numsubsounds)
        {
            fixed (int* psubsoundCount = &numsubsounds)
            {
                return Sound_GetNumSubSounds(sound, psubsoundCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumTags(SoundHandle sound, int* tagCount, int* updatedTagCount);

        public Result Sound_GetNumTags(SoundHandle sound, out int tagCount, out int updatedTagCount)
        {
            fixed (int* pTagCount = &tagCount, pUpdatedTagCount = &updatedTagCount)
            {
                return Sound_GetNumTags(sound, pTagCount, pUpdatedTagCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetTag(SoundHandle sound, byte* name, int index, Tag* tag);

        public Result Sound_GetTag(SoundHandle sound, string name, int index, Tag* tag)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return Sound_GetTag(sound, pName, index, tag);
            }
        }

        public Result Sound_GetTag(SoundHandle sound, string name, int index, out Tag tag)
        {
            fixed (Tag* pTag = &tag)
            {
                return Sound_GetTag(sound, name, index, pTag);
            }
        }


        [InteropMethod]
        public partial Result Sound_GetOpenState(SoundHandle sound, OpenState* openstate, uint* percentbuffered, FmodBool* starving, FmodBool* diskbusy);

        public Result Sound_GetOpenState(SoundHandle sound, out OpenState openstate, out uint percentBuffered, out FmodBool starving, out FmodBool diskbusy)
        {
            fixed (OpenState* pOpenState = &openstate)
            fixed (uint* pPercentBuffered = &percentBuffered)
            fixed (FmodBool* pStarving = &starving, pDiskBusy = &diskbusy)
            {
                return Sound_GetOpenState(sound, pOpenState, pPercentBuffered, pStarving, pDiskBusy);
            }
        }

        [InteropMethod]
        public partial Result Sound_ReadData(SoundHandle sound, void* buffer, uint length, uint* read);

        public Result Sound_ReadData(SoundHandle sound, void* buffer, uint length, out uint read)
        {
            fixed (uint* pRead = &read)
            {
                return Sound_ReadData(sound, buffer, length, pRead);
            }
        }

        public Result Sound_ReadData(SoundHandle sound, Span<byte> buffer, uint* read)
        {
            fixed (byte* pBuffer = buffer)
            {
                return Sound_ReadData(sound, pBuffer, (uint)buffer.Length, read);
            }
        }

        public Result Sound_ReadData(SoundHandle sound, Span<byte> buffer, out uint read)
        {
            fixed (byte* pBuffer = buffer)
            fixed (uint* pRead = &read)
            {
                return Sound_ReadData(sound, pBuffer, (uint)buffer.Length, pRead);
            }
        }

        [InteropMethod]
        public partial Result Sound_SeekData(SoundHandle sound, uint pcm);

        [InteropMethod]
        public partial Result Sound_SetSoundGroup(SoundHandle sound, SoundGroupHandle soundGroup);

        [InteropMethod]
        public partial Result Sound_GetSoundGroup(SoundHandle sound, SoundGroupHandle* soundGroup);

        public Result Sound_GetSoundGroup(SoundHandle sound, out SoundGroupHandle soundGroup)
        {
            fixed (SoundGroupHandle* pSoundGroup = &soundGroup)
            {
                return Sound_GetSoundGroup(sound, pSoundGroup);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumSyncPoints(SoundHandle sound, int* syncPointCount);

        public Result Sound_GetNumSyncPoints(SoundHandle sound, out int syncPointCount)
        {
            fixed (int* pSyncPointCount = &syncPointCount)
            {
                return Sound_GetNumSyncPoints(sound, pSyncPointCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSyncPoint(SoundHandle sound, int index, SyncPointHandle* point);

        public Result Sound_GetSyncPoint(SoundHandle sound, int index, out SyncPointHandle point)
        {
            fixed (SyncPointHandle* pPoint = &point)
            {
                return Sound_GetSyncPoint(sound, index, pPoint);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSyncPointInfo(SoundHandle sound, SyncPointHandle point, byte* name, int namelen, uint* offset, TimeUnit offsetType);

        public Result Sound_GetSyncPointInfo(SoundHandle sound, SyncPointHandle point, byte* name, int namelen, out uint offset, TimeUnit offsetType)
        {
            fixed (uint* pOffset = &offset)
            {
                return Sound_GetSyncPointInfo(sound, point, name, namelen, pOffset, offsetType);
            }
        }

        public Result Sound_GetSyncPointInfo(SoundHandle sound, SyncPointHandle point, Span<byte> nameBuffer, uint* offset, TimeUnit offsetType)
        {
            fixed (byte* pName = nameBuffer)
            {
                return Sound_GetSyncPointInfo(sound, point, pName, nameBuffer.Length, offset, offsetType);
            }
        }

        public Result Sound_GetSyncPointInfo(SoundHandle sound, SyncPointHandle point, Span<byte> nameBuffer, out uint offset, TimeUnit offsetType)
        {
            fixed (byte* pName = nameBuffer)
            {
                return Sound_GetSyncPointInfo(sound, point, pName, nameBuffer.Length, out offset, offsetType);
            }
        }

        [InteropMethod]
        public partial Result Sound_AddSyncPoint(SoundHandle sound, uint offset, TimeUnit offsetType, byte* name, SyncPointHandle* point);

        public Result Sound_AddSyncPoint(SoundHandle sound, uint offset, TimeUnit offsetType, string name, SyncPointHandle* point)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return Sound_AddSyncPoint(sound, offset, offsetType, pName, point);
            }
        }

        public Result Sound_AddSyncPoint(SoundHandle sound, uint offset, TimeUnit offsetType, string name, out SyncPointHandle point)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (SyncPointHandle* pPoint = &point)
            {
                return Sound_AddSyncPoint(sound, offset, offsetType, pName, pPoint);
            }
        }

        [InteropMethod]
        public partial Result Sound_DeleteSyncPoint(SoundHandle sound, SyncPointHandle point);

        [InteropMethod]
        public partial Result Sound_SetMode(SoundHandle sound, Mode mode);

        [InteropMethod]
        public partial Result Sound_GetMode(SoundHandle sound, Mode* mode);

        public Result Sound_GetMode(SoundHandle sound, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Sound_GetMode(sound, pMode);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetLoopCount(SoundHandle sound, int loopcount);

        [InteropMethod]
        public partial Result Sound_GetLoopCount(SoundHandle sound, int* loopcount);

        public Result Sound_GetLoopCount(SoundHandle sound, out int loopCount)
        {
            fixed (int* pLoopCount = &loopCount)
            {
                return Sound_GetLoopCount(sound, pLoopCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetLoopPoints(SoundHandle sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [InteropMethod]
        public partial Result Sound_GetLoopPoints(SoundHandle sound, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Sound_GetLoopPoints(SoundHandle sound, out uint loopStart, TimeUnit loopStartType, out uint loopEnd, TimeUnit loopEndType)
        {
            fixed (uint* pLoopStart = &loopStart, pLoopEnd = &loopEnd)
            {
                return Sound_GetLoopPoints(sound, pLoopStart, loopStartType, pLoopEnd, loopEndType);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetMusicNumChannels(SoundHandle sound, int* numchannels);

        public Result Sound_GetMusicNumChannels(SoundHandle sound, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return Sound_GetMusicNumChannels(sound, pChannelCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetMusicChannelVolume(SoundHandle sound, int channel, float volume);

        [InteropMethod]
        public partial Result Sound_GetMusicChannelVolume(SoundHandle sound, int channel, float* volume);

        public Result Sound_GetMusicChannelVolume(SoundHandle sound, int channel, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return Sound_GetMusicChannelVolume(sound, channel, pVolume);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetMusicSpeed(SoundHandle sound, float speed);

        [InteropMethod]
        public partial Result Sound_GetMusicSpeed(SoundHandle sound, float* speed);

        public Result Sound_GetMusicSpeed(SoundHandle sound, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return Sound_GetMusicSpeed(sound, pSpeed);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetUserData(SoundHandle sound, IntPtr* userdata);

        [InteropMethod]
        public partial Result Sound_SetUserData(SoundHandle sound, IntPtr userdata);

        #endregion

        #region Sound Group Functions
        [InteropMethod]
        public partial Result SoundGroup_Release(SoundGroupHandle soundGroup);

        [InteropMethod]
        public partial Result SoundGroup_GetSystemObject(SoundGroupHandle soundGroup, SystemHandle* system);

        public Result SoundGroup_GetSystemObject(SoundGroupHandle soundGroup, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return SoundGroup_GetSystemObject(soundGroup, pSystem);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMaxAudible(SoundGroupHandle soundGroup, int maxaudible);

        [InteropMethod]
        public partial Result SoundGroup_GetMaxAudible(SoundGroupHandle soundGroup, int* maxaudible);

        [InteropMethod]
        public Result SoundGroup_GetMaxAudible(SoundGroupHandle soundGroup, out int maxAudible)
        {
            fixed (int* pMaxAudible = &maxAudible)
            {
                return SoundGroup_GetMaxAudible(soundGroup, pMaxAudible);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMaxAudibleBehavior(SoundGroupHandle soundGroup, SoundGroupBehavior behavior);

        [InteropMethod]
        public partial Result SoundGroup_GetMaxAudibleBehavior(SoundGroupHandle soundGroup, SoundGroupBehavior* behavior);

        public Result SoundGroup_GetMaxAudibleBehavior(SoundGroupHandle soundGroup, out SoundGroupBehavior behavior)
        {
            fixed (SoundGroupBehavior* pBehavior = &behavior)
            {
                return SoundGroup_GetMaxAudibleBehavior(soundGroup, pBehavior);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMuteFadeSpeed(SoundGroupHandle soundGroup, float speed);

        [InteropMethod]
        public partial Result SoundGroup_GetMuteFadeSpeed(SoundGroupHandle soundGroup, float* speed);

        public Result SoundGroup_GetMuteFadeSpeed(SoundGroupHandle soundGroup, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return SoundGroup_GetMuteFadeSpeed(soundGroup, pSpeed);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetVolume(SoundGroupHandle soundGroup, float volume);

        [InteropMethod]
        public partial Result SoundGroup_GetVolume(SoundGroupHandle soundGroup, float* volume);

        public Result SoundGroup_GetVolume(SoundGroupHandle soundGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return SoundGroup_GetVolume(soundGroup, pVolume);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_Stop(SoundGroupHandle soundGroup);

        [InteropMethod]
        public partial Result SoundGroup_GetName(SoundGroupHandle soundGroup, byte* name, int namelen);

        public Result SoundGroup_GetName(SoundGroupHandle soundGroup, Span<byte> nameBuffer)
        {
            fixed (byte* pName = nameBuffer)
            {
                return SoundGroup_GetName(soundGroup, pName, nameBuffer.Length);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_GetNumSounds(SoundGroupHandle soundGroup, int* numsounds);

        public Result SoundGroup_GetNumSounds(SoundGroupHandle soundGroup, out int soundCount)
        {
            fixed (int* pSoundCount = &soundCount)
            {
                return SoundGroup_GetNumSounds(soundGroup, pSoundCount);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_GetSound(SoundGroupHandle soundGroup, int index, SoundHandle* sound);

        public Result SoundGroup_GetSound(SoundGroupHandle soundGroup, int index, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return SoundGroup_GetSound(soundGroup, index, pSound);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_GetNumPlaying(SoundGroupHandle soundGroup, int* soundsPlaying);

        public Result SoundGroup_GetNumPlaying(SoundGroupHandle soundGroup, out int soundsPlaying)
        {
            fixed (int* pSoundsPlaying = &soundsPlaying)
            {
                return SoundGroup_GetNumPlaying(soundGroup, pSoundsPlaying);
            }
        }

        [InteropMethod]
        internal partial Result SoundGroup_SetUserData(SoundGroupHandle soundgroup, IntPtr userdata);

        [InteropMethod]
        internal partial Result SoundGroup_GetUserData(SoundGroupHandle soundgroup, IntPtr* userdata);
        #endregion

        #region Channel Control Functions

        [InteropMethod]
        public partial Result ChannelGroup_IsPlaying(ChannelGroupHandle channelGroup, FmodBool* isplaying);

        public Result ChannelGroup_IsPlaying(ChannelGroupHandle channelGroup, out FmodBool isplaying)
        {
            fixed (FmodBool* pValue = &isplaying)
            {
                return ChannelGroup_IsPlaying(channelGroup, pValue);
            }
        }

        [InteropMethod]
        public partial Result Channel_IsPlaying(ChannelHandle channel, FmodBool* isplaying);

        public Result Channel_IsPlaying(ChannelHandle channel, out FmodBool isplaying)
        {
            fixed (FmodBool* pValue = &isplaying)
            {
                return Channel_IsPlaying(channel, pValue);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Stop(ChannelGroupHandle channelGroup);

        [InteropMethod]
        public partial Result Channel_Stop(ChannelHandle channel);

        [InteropMethod]
        public partial Result ChannelGroup_SetPaused(ChannelGroupHandle channelGroup, FmodBool paused);

        [InteropMethod]
        public partial Result Channel_SetPaused(ChannelHandle channel, FmodBool paused);

        [InteropMethod]
        public partial Result ChannelGroup_GetPaused(ChannelGroupHandle channelGroup, FmodBool* paused);

        public Result ChannelGroup_GetPaused(ChannelGroupHandle channelGroup, out FmodBool paused)
        {
            fixed (FmodBool* pPaused = &paused)
                return ChannelGroup_GetPaused(channelGroup, pPaused);
        }

        [InteropMethod]
        public partial Result Channel_GetPaused(ChannelHandle channel, FmodBool* paused);

        public Result Channel_GetPaused(ChannelHandle channel, out FmodBool paused)
        {
            fixed (FmodBool* pPaused = &paused)
                return Channel_GetPaused(channel, pPaused);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetVolume(ChannelGroupHandle channelGroup, float* volume);

        public Result ChannelGroup_GetVolume(ChannelGroupHandle channelGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return ChannelGroup_GetVolume(channelGroup, pVolume);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetVolume(ChannelHandle channel, float* volume);

        public Result Channel_GetVolume(ChannelHandle channel, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return Channel_GetVolume(channel, pVolume);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetVolume(ChannelGroupHandle channelGroup, float volume);

        [InteropMethod]
        public partial Result Channel_SetVolume(ChannelHandle channelGroup, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_SetVolumeRamp(ChannelGroupHandle channelGroup, FmodBool ramp);

        [InteropMethod]
        public partial Result Channel_SetVolumeRamp(ChannelHandle channelGroup, FmodBool ramp);

        [InteropMethod]
        public partial Result ChannelGroup_GetVolumeRamp(ChannelGroupHandle channelGroup, FmodBool* ramp);

        public Result ChannelGroup_GetVolumeRamp(ChannelGroupHandle channelGroup, out FmodBool ramp)
        {
            fixed (FmodBool* pRamp = &ramp)
                return ChannelGroup_GetVolumeRamp(channelGroup, pRamp);
        }

        [InteropMethod]
        public partial Result Channel_GetVolumeRamp(ChannelHandle channelGroup, FmodBool* ramp);

        public Result Channel_GetVolumeRamp(ChannelHandle channelGroup, out FmodBool ramp)
        {
            fixed (FmodBool* pRamp = &ramp)
                return Channel_GetVolumeRamp(channelGroup, pRamp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetAudibility(ChannelGroupHandle channelGroup, float* audibility);

        public Result ChannelGroup_GetAudibility(ChannelGroupHandle channelGroup, out float audibility)
        {
            fixed (float* pAudibility = &audibility)
            {
                return ChannelGroup_GetAudibility(channelGroup, pAudibility);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetAudibility(ChannelHandle channelGroup, float* audibility);

        public Result Channel_GetAudibility(ChannelHandle channelGroup, out float audibility)
        {
            fixed (float* pAudibility = &audibility)
            {
                return Channel_GetAudibility(channelGroup, pAudibility);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetPitch(ChannelGroupHandle channelGroup, float pitch);

        [InteropMethod]
        public partial Result Channel_SetPitch(ChannelHandle channelGroup, float pitch);

        [InteropMethod]
        public partial Result ChannelGroup_GetPitch(ChannelGroupHandle channelGroup, float* pitch);

        public Result ChannelGroup_GetPitch(ChannelGroupHandle channelGroup, out float pitch)
        {
            fixed (float* pPitch = &pitch)
            {
                return ChannelGroup_GetPitch(channelGroup, pPitch);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetPitch(ChannelHandle channelGroup, float* pitch);

        public Result Channel_GetPitch(ChannelHandle channel, out float pitch)
        {
            fixed (float* pPitch = &pitch)
            {
                return Channel_GetPitch(channel, pPitch);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetMute(ChannelGroupHandle channelGroup, FmodBool mute);

        [InteropMethod]
        public partial Result Channel_SetMute(ChannelHandle channel, FmodBool mute);

        [InteropMethod]
        public partial Result ChannelGroup_GetMute(ChannelGroupHandle channelGroup, FmodBool* mute);

        public Result ChannelGroup_GetMute(ChannelGroupHandle channelGroup, out FmodBool mute)
        {
            fixed (FmodBool* pMute = &mute)
            {
                return ChannelGroup_GetMute(channelGroup, pMute);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetMute(ChannelHandle channelGroup, FmodBool* mute);

        public Result Channel_GetMute(ChannelHandle channelGroup, out FmodBool mute)
        {
            fixed (FmodBool* pMute = &mute)
            {
                return Channel_GetMute(channelGroup, pMute);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetReverbProperties(ChannelGroupHandle channelGroup, int instance, float wet);

        [InteropMethod]
        public partial Result Channel_SetReverbProperties(ChannelHandle channel, int instance, float wet);

        /// <summary>
        /// Retrieves the wet level (or send level) for a particular reverb instance.
        /// </summary>
        /// <param name="instance">Reverb instance index.</param>
        /// <param name="wet">Send level for the signal to the reverb.</param>
        [InteropMethod]
        public partial Result ChannelGroup_GetReverbProperties(ChannelGroupHandle channelGroup, int instance, float* wet);

        /// <inheritdoc cref="ChannelGroup_GetReverbProperties(ChannelGroupHandle, int, float*)"/>
        public Result ChannelGroup_GetReverbProperties(ChannelGroupHandle channelGroup, int instance, out float wet)
        {
            fixed (float* pWet = &wet)
            {
                return ChannelGroup_GetReverbProperties(channelGroup, instance, pWet);
            }
        }


        [InteropMethod]
        public partial Result Channel_GetReverbProperties(ChannelHandle channel, int instance, float* wet);

        public Result Channel_GetReverbProperties(ChannelHandle channel, int instance, out float wet)
        {
            fixed (float* pWet = &wet)
            {
                return Channel_GetReverbProperties(channel, instance, pWet);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetLowPassGain(ChannelGroupHandle channelGroup, float gain);

        [InteropMethod]
        public partial Result Channel_SetLowPassGain(ChannelHandle channel, float gain);

        [InteropMethod]
        public partial Result ChannelGroup_GetLowPassGain(ChannelGroupHandle channelGroup, float* gain);

        public Result ChannelGroup_GetLowPassGain(ChannelGroupHandle channelGroup, out float gain)
        {
            fixed (float* pGain = &gain)
            {
                return ChannelGroup_GetLowPassGain(channelGroup, pGain);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetLowPassGain(ChannelHandle channel, float* gain);

        public Result Channel_GetLowPassGain(ChannelHandle channel, out float gain)
        {
            fixed (float* pGain = &gain)
            {
                return Channel_GetLowPassGain(channel, pGain);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetMode(ChannelGroupHandle channelGroup, Mode mode);

        [InteropMethod]
        public partial Result Channel_SetMode(ChannelHandle channel, Mode mode);

        [InteropMethod]
        public partial Result ChannelGroup_GetMode(ChannelGroupHandle channelGroup, Mode* mode);

        public Result ChannelGroup_GetMode(ChannelGroupHandle channelGroup, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return ChannelGroup_GetMode(channelGroup, pMode);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetMode(ChannelHandle channel, Mode* mode);

        public Result Channel_GetMode(ChannelHandle channel, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Channel_GetMode(channel, pMode);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetCallback(ChannelGroupHandle channelGroup, delegate* unmanaged<IntPtr, ChannelControlType, ChannelControlCallbackType, void*, void*, void> callback);

        [InteropMethod]
        public partial Result Channel_SetCallback(ChannelHandle channel, delegate* unmanaged<IntPtr, ChannelControlType, ChannelControlCallbackType, void*, void*, void> callback);

        [InteropMethod]
        public partial Result ChannelGroup_SetPan(ChannelGroupHandle channelGroup, float pan);

        [InteropMethod]
        public partial Result Channel_SetPan(ChannelHandle channel, float pan);

        [InteropMethod]
        public partial Result ChannelGroup_SetMixLevelsOutput(ChannelGroupHandle channelGroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

        [InteropMethod]
        public partial Result Channel_SetMixLevelsOutput(ChannelHandle channelGroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

        [InteropMethod]
        public partial Result ChannelGroup_SetMixLevelsInput(ChannelGroupHandle channelGroup, float* levels, int levelCount);

        public Result ChannelGroup_SetMixLevelsInput(ChannelGroupHandle channelGroup, ReadOnlySpan<float> levels)
        {
            fixed (float* pLevels = levels)
            {
                return ChannelGroup_SetMixLevelsInput(channelGroup, pLevels, levels.Length);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetMixLevelsInput(ChannelHandle channelGroup, float* levels, int levelCount);

        public Result Channel_SetMixLevelsInput(ChannelHandle channelGroup, ReadOnlySpan<float> levels)
        {
            fixed (float* pLevels = levels)
            {
                return Channel_SetMixLevelsInput(channelGroup, pLevels, levels.Length);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetMixMatrix(ChannelGroupHandle channelGroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result Channel_SetMixMatrix(ChannelHandle channel, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result ChannelGroup_GetMixMatrix(ChannelGroupHandle channelGroup, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result Channel_GetMixMatrix(ChannelHandle channel, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSPClock(ChannelGroupHandle channelGroup, ulong* dspclock, ulong* parentclock);

        public Result ChannelGroup_GetDSPClock(ChannelGroupHandle channelGroup, out ulong dspclock, out ulong parentclock)
        {
            fixed (ulong* pDspClock = &dspclock, pParentClock = &parentclock)
            {
                return ChannelGroup_GetDSPClock(channelGroup, pDspClock, pParentClock);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetDSPClock(ChannelHandle channelGroup, ulong* dspclock, ulong* parentclock);

        public Result Channel_GetDSPClock(ChannelHandle channelGroup, out ulong dspclock, out ulong parentclock)
        {
            fixed (ulong* pDspClock = &dspclock, pParentClock = &parentclock)
            {
                return Channel_GetDSPClock(channelGroup, pDspClock, pParentClock);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetDelay(ChannelGroupHandle channelGroup, ulong dspclock_start, ulong dspclock_end, FmodBool stopchannels);

        [InteropMethod]
        public partial Result Channel_SetDelay(ChannelHandle channel, ulong dspclock_start, ulong dspclock_end, FmodBool stopchannels);

        [InteropMethod]
        public partial Result ChannelGroup_GetDelay(ChannelGroupHandle channel, ulong* dspclock_start, ulong* dspclock_end, FmodBool* stopchannels);

        public Result ChannelGroup_GetDelay(ChannelGroupHandle channelGroup, out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopchannels)
        {
            fixed (ulong* pDspclock_start = &dspclock_start, pDspclock_end = &dspclock_end)
            fixed (FmodBool* pStopChannels = &stopchannels)
            {
                return ChannelGroup_GetDelay(channelGroup, pDspclock_start, pDspclock_end, pStopChannels);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetDelay(ChannelHandle channel, ulong* dspclock_start, ulong* dspclock_end, FmodBool* stopchannels);

        public Result Channel_GetDelay(ChannelHandle channel, out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopchannels)
        {
            fixed (ulong* pDspclock_start = &dspclock_start, pDspclock_end = &dspclock_end)
            fixed (FmodBool* pStopChannels = &stopchannels)
            {
                return Channel_GetDelay(channel, pDspclock_start, pDspclock_end, pStopChannels);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_AddFadePoint(ChannelGroupHandle channelGroup, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result Channel_AddFadePoint(ChannelHandle channel, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_SetFadePointRamp(ChannelGroupHandle channelGroup, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result Channel_SetFadePointRamp(ChannelHandle channel, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_RemoveFadePoints(ChannelGroupHandle channelGroup, ulong dspclock_start, ulong dspclock_end);

        [InteropMethod]
        public partial Result Channel_RemoveFadePoints(ChannelHandle channel, ulong dspclock_start, ulong dspclock_end);

        [InteropMethod]
        public partial Result ChannelGroup_GetFadePoints(ChannelGroupHandle channelGroup, uint* numpoints, ulong* point_dspclock, float* point_volume);

        [InteropMethod]
        public partial Result Channel_GetFadePoints(ChannelHandle channel, uint* numpoints, ulong* point_dspclock, float* point_volume);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSP(ChannelGroupHandle channelGroup, int index, DspHandle* dsp);

        public Result ChannelGroup_GetDSP(ChannelGroupHandle channelGroup, int index, out DspHandle dsp)
        {
            fixed (DspHandle* pDsp = &dsp)
            {
                return ChannelGroup_GetDSP(channelGroup, index, pDsp);
            }
        }

        public Result ChannelGroup_GetDSP(ChannelGroupHandle channelGroup, ChannelControlDSPIndex index, DspHandle* dsp)
        {
            return ChannelGroup_GetDSP(channelGroup, (int)index, dsp);
        }

        public Result ChannelGroup_GetDSP(ChannelGroupHandle channelGroup, ChannelControlDSPIndex index, out DspHandle dsp)
        {
            return ChannelGroup_GetDSP(channelGroup, (int)index, out dsp);
        }

        [InteropMethod]
        public partial Result Channel_GetDSP(ChannelHandle channel, int index, DspHandle* dsp);

        public Result Channel_GetDSP(ChannelHandle channel, int index, out DspHandle dsp)
        {
            fixed (DspHandle* pDsp = &dsp)
            {
                return Channel_GetDSP(channel, index, pDsp);
            }
        }

        public Result Channel_GetDSP(ChannelHandle channelGroup, ChannelControlDSPIndex index, DspHandle* dsp)
        {
            return Channel_GetDSP(channelGroup, (int)index, dsp);
        }

        public Result Channel_GetDSP(ChannelHandle channelGroup, ChannelControlDSPIndex index, out DspHandle dsp)
        {
            return Channel_GetDSP(channelGroup, (int)index, out dsp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_AddDSP(ChannelGroupHandle channelGroup, int index, DspHandle dsp);

        public Result ChannelGroup_AddDSP(ChannelGroupHandle channelGroup, ChannelControlDSPIndex index, DspHandle dsp)
        {
            return ChannelGroup_AddDSP(channelGroup, (int)index, dsp);
        }

        [InteropMethod]
        public partial Result Channel_AddDSP(ChannelHandle channel, int index, DspHandle dsp);

        public Result Channel_AddDSP(ChannelHandle channel, ChannelControlDSPIndex index, DspHandle dsp)
        {
            return Channel_AddDSP(channel, (int)index, dsp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_RemoveDSP(ChannelGroupHandle channelGroup, DspHandle dsp);

        [InteropMethod]
        public partial Result Channel_RemoveDSP(ChannelHandle channel, DspHandle dsp);

        [InteropMethod]
        public partial Result ChannelGroup_GetNumDSPs(ChannelGroupHandle channelGroup, int* count);

        public Result ChannelGroup_GetNumDSPs(ChannelGroupHandle channel, out int count)
        {
            fixed (int* pCount = &count)
            {
                return ChannelGroup_GetNumDSPs(channel, pCount);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetNumDSPs(ChannelHandle channel, int* count);

        public Result Channel_GetNumDSPs(ChannelHandle channel, out int count)
        {
            fixed (int* pCount = &count)
            {
                return Channel_GetNumDSPs(channel, pCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetDSPIndex(ChannelGroupHandle channelGroup, DspHandle dsp, int index);

        [InteropMethod]
        public partial Result Channel_SetDSPIndex(ChannelHandle channelGroup, DspHandle dsp, int index);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSPIndex(ChannelGroupHandle channelGroup, DspHandle dsp, int* index);

        public Result ChannelGroup_GetDSPIndex(ChannelGroupHandle channelGroup, DspHandle dsp, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return ChannelGroup_GetDSPIndex(channelGroup, dsp, pIndex);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetDSPIndex(ChannelHandle channelGroup, DspHandle dsp, int* index);

        public Result Channel_GetDSPIndex(ChannelHandle channel, DspHandle dsp, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return Channel_GetDSPIndex(channel, dsp, pIndex);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DAttributes(ChannelGroupHandle channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Set3DAttributes(ChannelGroupHandle channelGroup, in Vector3 pos, in Vector3 vel, in Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltpanpos = &alt_pan_pos)
            {
                return ChannelGroup_Set3DAttributes(channelGroup, pPos, pVel, pAltpanpos);
            }
        }

        [InteropMethod]
        public partial Result Channel_Set3DAttributes(ChannelHandle channel, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result Channel_Set3DAttributes(ChannelHandle channel, in Vector3 pos, in Vector3 vel, in Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltpanpos = &alt_pan_pos)
            {
                return Channel_Set3DAttributes(channel, pPos, pVel, pAltpanpos);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Get3DAttributes(ChannelGroupHandle channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Get3DAttributes(ChannelGroupHandle channelGroup, out Vector3 pos, out Vector3 vel, out Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltPanPos = &alt_pan_pos)
            {
                return ChannelGroup_Get3DAttributes(channelGroup, pPos, pVel, pAltPanPos);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DAttributes(ChannelHandle channel, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result Channel_Get3DAttributes(ChannelHandle channel, out Vector3 pos, out Vector3 vel, out Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltPanPos = &alt_pan_pos)
            {
                return Channel_Get3DAttributes(channel, pPos, pVel, pAltPanPos);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DMinMaxDistance(ChannelGroupHandle channelGroup, float mindistance, float maxdistance);

        [InteropMethod]
        public partial Result Channel_Set3DMinMaxDistance(ChannelHandle channel, float mindistance, float maxdistance);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DMinMaxDistance(ChannelGroupHandle channelGroup, float* mindistance, float* maxdistance);

        public Result ChannelGroup_Get3DMinMaxDistance(ChannelGroupHandle channelGroup, out float minDistance, out float maxDistance)
        {
            fixed (float* pMinDistance = &minDistance, pMaxdistance = &maxDistance)
            {
                return ChannelGroup_Get3DMinMaxDistance(channelGroup, pMinDistance, pMaxdistance);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DMinMaxDistance(ChannelHandle channel, float* mindistance, float* maxdistance);

        public Result Channel_Get3DMinMaxDistance(ChannelHandle channel, out float minDistance, out float maxDistance)
        {
            fixed (float* pMinDistance = &minDistance, pMaxdistance = &maxDistance)
            {
                return Channel_Get3DMinMaxDistance(channel, pMinDistance, pMaxdistance);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DConeSettings(ChannelGroupHandle channelGroup, float insideconeangle, float outsideconeangle, float outsidevolume);

        [InteropMethod]
        public partial Result Channel_Set3DConeSettings(ChannelHandle channel, float insideconeangle, float outsideconeangle, float outsidevolume);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DConeSettings(ChannelGroupHandle channelGroup, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result ChannelGroup_Get3DConeSettings(ChannelGroupHandle channelGroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideAngle = &insideconeangle, pOutsideAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return ChannelGroup_Get3DConeSettings(channelGroup, pInsideAngle, pOutsideAngle, pOutsideVolume);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DConeSettings(ChannelHandle channel, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result Channel_Get3DConeSettings(ChannelHandle channel, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideAngle = &insideconeangle, pOutsideAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return Channel_Get3DConeSettings(channel, pInsideAngle, pOutsideAngle, pOutsideVolume);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DConeOrientation(ChannelGroupHandle channelGroup, Vector3* orientation);

        public Result ChannelGroup_Set3DConeOrientation(ChannelGroupHandle channelGroup, in Vector3 orientation)
        {
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Set3DConeOrientation(channelGroup, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result Channel_Set3DConeOrientation(ChannelHandle channel, Vector3* orientation);

        public Result Channel_Set3DConeOrientation(ChannelHandle channel, in Vector3 orientation)
        {
            fixed (Vector3* pOrientation = &orientation)
            {
                return Channel_Set3DConeOrientation(channel, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Get3DConeOrientation(ChannelGroupHandle channelGroup, Vector3* orientation);

        public Result ChannelGroup_Get3DConeOrientation(ChannelGroupHandle channelGroup, out Vector3 orientation)
        {
            orientation = default;
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Get3DConeOrientation(channelGroup, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DConeOrientation(ChannelHandle channel, Vector3* orientation);

        public Result Channel_Get3DConeOrientation(ChannelHandle channel, out Vector3 orientation)
        {
            orientation = default;
            fixed (Vector3* pOrientation = &orientation)
            {
                return Channel_Get3DConeOrientation(channel, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DCustomRolloff(ChannelGroupHandle channelGroup, RolloffPoint* points, int numpoints);

        [InteropMethod]
        public partial Result Channel_Set3DCustomRolloff(ChannelHandle channel, RolloffPoint* points, int numpoints);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DCustomRolloff(ChannelGroupHandle channelGroup, RolloffPoint** points, int* numpoints);

        public Result ChannelGroup_Get3DCustomRolloff(ChannelGroupHandle channelGroup, out RolloffPoint* points, out int pointCount)
        {
            fixed (RolloffPoint** pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return ChannelGroup_Get3DCustomRolloff(channelGroup, pPoints, pPointCount);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DCustomRolloff(ChannelHandle channel, RolloffPoint** points, int* numpoints);

        public Result Channel_Get3DCustomRolloff(ChannelHandle channel, out RolloffPoint* points, out int pointCount)
        {
            fixed (RolloffPoint** pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return Channel_Get3DCustomRolloff(channel, pPoints, pPointCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DOcclusion(ChannelGroupHandle channelGroup, float directocclusion, float reverbocclusion);

        [InteropMethod]
        public partial Result Channel_Set3DOcclusion(ChannelHandle channel, float directocclusion, float reverbocclusion);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DOcclusion(ChannelGroupHandle channelGroup, float* directocclusion, float* reverbocclusion);

        public Result ChannelGroup_Get3DOcclusion(ChannelGroupHandle channelGroup, out float directOcclusion, out float reverbOcclusion)
        {
            fixed (float* pDirectOcclusion = &directOcclusion, pReverbOcclusion = &reverbOcclusion)
            {
                return ChannelGroup_Get3DOcclusion(channelGroup, pDirectOcclusion, pReverbOcclusion);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DOcclusion(ChannelHandle channel, float* directocclusion, float* reverbocclusion);

        public Result Channel_Get3DOcclusion(ChannelHandle channel, out float directOcclusion, out float reverbOcclusion)
        {
            fixed (float* pDirectOcclusion = &directOcclusion, pReverbOcclusion = &reverbOcclusion)
            {
                return Channel_Get3DOcclusion(channel, pDirectOcclusion, pReverbOcclusion);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DSpread(ChannelGroupHandle channelGroup, float angle);

        [InteropMethod]
        public partial Result Channel_Set3DSpread(ChannelHandle channel, float angle);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DSpread(ChannelGroupHandle channelGroup, float* angle);

        public Result ChannelGroup_Get3DSpread(ChannelGroupHandle channelGroup, out float angle)
        {
            fixed (float* pAngle = &angle)
            {
                return ChannelGroup_Get3DSpread(channelGroup, pAngle);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DSpread(ChannelHandle channel, float* angle);

        public Result Channel_Get3DSpread(ChannelHandle channel, out float angle)
        {
            fixed (float* pAngle = &angle)
            {
                return Channel_Get3DSpread(channel, pAngle);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DLevel(ChannelGroupHandle channelGroup, float level);

        [InteropMethod]
        public partial Result Channel_Set3DLevel(ChannelHandle channel, float level);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DLevel(ChannelGroupHandle channelGroup, float* level);

        public Result ChannelGroup_Get3DLevel(ChannelGroupHandle channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DLevel(channelGroup, pLevel);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DLevel(ChannelHandle channel, float* level);

        public Result Channel_Get3DLevel(ChannelHandle channel, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return Channel_Get3DLevel(channel, pLevel);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DDopplerLevel(ChannelGroupHandle channelGroup, float level);

        [InteropMethod]
        public partial Result Channel_Set3DDopplerLevel(ChannelHandle channel, float level);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DDopplerLevel(ChannelGroupHandle channelGroup, float* level);

        public Result ChannelGroup_Get3DDopplerLevel(ChannelGroupHandle channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DDopplerLevel(channelGroup, pLevel);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DDopplerLevel(ChannelHandle channel, float* level);

        public Result Channel_Get3DDopplerLevel(ChannelHandle channel, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return Channel_Get3DDopplerLevel(channel, pLevel);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DDistanceFilter(ChannelGroupHandle channelGroup, FmodBool custom, float customLevel, float centerFreq);

        [InteropMethod]
        public partial Result Channel_Set3DDistanceFilter(ChannelHandle channel, FmodBool custom, float customLevel, float centerFreq);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DDistanceFilter(ChannelGroupHandle channelGroup, FmodBool* custom, float* customLevel, float* centerFreq);

        public Result ChannelGroup_Get3DDistanceFilter(ChannelGroupHandle channelGroup, out FmodBool custom, out float customLevel, out float centerFreq)
        {
            fixed (FmodBool* pCustom = &custom)
            fixed (float* pCustomlevel = &customLevel, pCenterFreq = &centerFreq)
            {
                return ChannelGroup_Get3DDistanceFilter(channelGroup, pCustom, pCustomlevel, pCenterFreq);
            }
        }

        [InteropMethod]
        public partial Result Channel_Get3DDistanceFilter(ChannelHandle channel, FmodBool* custom, float* customLevel, float* centerFreq);

        public Result Channel_Get3DDistanceFilter(ChannelHandle channel, out FmodBool custom, out float customLevel, out float centerFreq)
        {
            fixed (FmodBool* pCustom = &custom)
            fixed (float* pCustomlevel = &customLevel, pCenterFreq = &centerFreq)
            {
                return Channel_Get3DDistanceFilter(channel, pCustom, pCustomlevel, pCenterFreq);
            }
        }

        #endregion

        #region Channel Functions

        [InteropMethod]
        public partial Result Channel_SetFrequency(ChannelHandle channel, float frequency);

        [InteropMethod]
        public partial Result Channel_GetFrequency(ChannelHandle channel, float* frequency);

        public Result Channel_GetFrequency(ChannelHandle channel, out float frequency)
        {
            fixed (float* pFrequency = &frequency)
            {
                return Channel_GetFrequency(channel, pFrequency);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetPriority(ChannelHandle channel, int priority);

        [InteropMethod]
        public partial Result Channel_GetPriority(ChannelHandle channel, int* priority);

        public Result Channel_GetPriority(ChannelHandle channel, out int priority)
        {
            fixed (int* pPriority = &priority)
            {
                return Channel_GetPriority(channel, pPriority);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetChannelGroup(ChannelHandle channel, ChannelGroupHandle channelGroup);

        [InteropMethod]
        public partial Result Channel_GetChannelGroup(ChannelHandle channel, ChannelGroupHandle* channelGroup);

        public Result Channel_GetChannelGroup(ChannelHandle channel, out ChannelGroupHandle channelGroup)
        {
            fixed (ChannelGroupHandle* pChannelGroup = &channelGroup)
            {
                return Channel_GetChannelGroup(channel, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result Channel_IsVirtual(ChannelHandle channel, FmodBool* isvirtual);

        public Result Channel_IsVirtual(ChannelHandle channel, out FmodBool isvirtual)
        {
            fixed (FmodBool* pIsVirtual = &isvirtual)
                return Channel_IsVirtual(channel, pIsVirtual);
        }

        [InteropMethod]
        public partial Result Channel_GetCurrentSound(ChannelHandle channel, SoundHandle* sound);

        public Result Channel_GetCurrentSound(ChannelHandle channel, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return Channel_GetCurrentSound(channel, pSound);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetIndex(ChannelHandle channel, int* index);

        public Result Channel_GetIndex(ChannelHandle channel, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return Channel_GetIndex(channel, pIndex);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetPosition(ChannelHandle channel, uint position, TimeUnit posType);

        [InteropMethod]
        public partial Result Channel_GetPosition(ChannelHandle channel, uint* position, TimeUnit posType);

        public Result Channel_GetPosition(ChannelHandle channel, out uint position, TimeUnit posType)
        {
            fixed (uint* pPos = &position)
            {
                return Channel_GetPosition(channel, pPos, posType);
            }
        }


        [InteropMethod]
        public partial Result Channel_SetLoopCount(ChannelHandle channel, int loopcount);

        [InteropMethod]
        public partial Result Channel_GetLoopCount(ChannelHandle channel, int* loopcount);

        public Result Channel_GetLoopCount(ChannelHandle channel, out int loopcount)
        {
            fixed (int* pLoopCount = &loopcount)
            {
                return Channel_GetLoopCount(channel, pLoopCount);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetLoopPoints(ChannelHandle channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [InteropMethod]
        public partial Result Channel_GetLoopPoints(ChannelHandle channel, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Channel_GetLoopPoints(ChannelHandle channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            fixed (uint* pLoopStart = &loopstart, pLoopEnd = &loopend)
            {
                return Channel_GetLoopPoints(channel, pLoopStart, loopstarttype, pLoopEnd, loopendtype);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetUserData(ChannelHandle channel, IntPtr* userData);

        [InteropMethod]
        public partial Result Channel_SetUserData(ChannelHandle channel, IntPtr userData);

        #endregion

        #region Channel Group Specific Functions

        [InteropMethod]
        public partial Result ChannelGroup_GetSystemObject(ChannelGroupHandle channelGroup, SystemHandle* system);

        public Result ChannelGroup_GetSystemObject(ChannelGroupHandle channelGroup, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return ChannelGroup_GetSystemObject(channelGroup, pSystem);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Release(ChannelGroupHandle Handle);

        [InteropMethod]
        public partial Result ChannelGroup_AddGroup(ChannelGroupHandle channelGroup, ChannelGroupHandle group, FmodBool propagatedspclock, DspConnectionHandle* connection);

        public Result ChannelGroup_AddGroup(ChannelGroupHandle channelGroup, ChannelGroupHandle group, FmodBool propagatedspclock, out DspConnectionHandle connection)
        {
            fixed (DspConnectionHandle* connectionRes = &connection)
            {
                return ChannelGroup_AddGroup(channelGroup, group, propagatedspclock, connectionRes);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetNumGroups(ChannelGroupHandle channelGroup, int* groupCount);

        public Result ChannelGroup_GetNumGroups(ChannelGroupHandle channelGroup, out int groupCount)
        {
            fixed (int* pGroupCount = &groupCount)
            {
                return ChannelGroup_GetNumGroups(channelGroup, pGroupCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetGroup(ChannelGroupHandle channelGroup, int index, ChannelGroupHandle* group);

        public Result ChannelGroup_GetGroup(ChannelGroupHandle channelGroup, int index, out ChannelGroupHandle group)
        {
            fixed (ChannelGroupHandle* pGroup = &group)
            {
                return ChannelGroup_GetGroup(channelGroup, index, pGroup);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetParentGroup(ChannelGroupHandle channelGroup, ChannelGroupHandle* group);

        public Result ChannelGroup_GetParentGroup(ChannelGroupHandle channelGroup, out ChannelGroupHandle group)
        {
            fixed (ChannelGroupHandle* pGroup = &group)
            {
                return ChannelGroup_GetParentGroup(channelGroup, pGroup);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetName(ChannelGroupHandle channelGroup, byte* name, int namelen);

        [InteropMethod]
        public partial Result ChannelGroup_GetNumChannels(ChannelGroupHandle channelGroup, int* channelCount);
        
        public Result ChannelGroup_GetNumChannels(ChannelGroupHandle channelGroup, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return ChannelGroup_GetNumChannels(channelGroup, pChannelCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetChannel(ChannelGroupHandle channelGroup, int index, ChannelHandle* channel);

        public Result ChannelGroup_GetChannel(ChannelGroupHandle channelGroup, int index, out ChannelHandle channel)
        {
            fixed (ChannelHandle* pChannel = &channel)
            {
                return ChannelGroup_GetChannel(channelGroup, index, pChannel);
            }
        }

        [InteropMethod]
        internal partial Result ChannelGroup_SetUserData(ChannelGroupHandle channelgroup, IntPtr userdata);

        [InteropMethod]
        internal partial Result ChannelGroup_GetUserData(ChannelGroupHandle channelgroup, IntPtr* userdata);

        #endregion

        #region DSP Functions

        [InteropMethod]
        public partial Result DSP_Release(DspHandle dsp);

        [InteropMethod]
        public partial Result DSP_GetSystemObject(DspHandle dsp, SystemHandle* system);

        public Result DSP_GetSystemObject(DspHandle dsp, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return DSP_GetSystemObject(dsp, pSystem);
            }
        }

        [InteropMethod]
        public partial Result DSP_AddInput(DspHandle dsp, DspHandle target, DspConnectionHandle* connection, DSPConnectionType type);

        public Result DSP_AddInput(DspHandle dsp, DspHandle target, out DspConnectionHandle connection, DSPConnectionType type)
        {
            fixed (DspConnectionHandle* pConnection = &connection)
            {
                return DSP_AddInput(dsp, target, pConnection, type);
            }
        }

        [InteropMethod]
        public partial Result DSP_DisconnectFrom(DspHandle dsp, DspHandle target, DspConnectionHandle connection);

        [InteropMethod]
        public partial Result DSP_DisconnectAll(DspHandle dsp, bool inputs, bool outputs);

        [InteropMethod]
        public partial Result DSP_GetNumInputs(DspHandle dsp, int* inputCount);

        public Result DSP_GetNumInputs(DspHandle dsp, out int inputCount)
        {
            fixed (int* pInputCount = &inputCount)
            {
                return DSP_GetNumInputs(dsp, pInputCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetNumOutputs(DspHandle dsp, int* outputCount);

        public Result DSP_GetNumOutputs(DspHandle dsp, out int outputCount)
        {
            fixed (int* pOutputCount = &outputCount)
            {
                return DSP_GetNumOutputs(dsp, pOutputCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetInput(DspHandle dsp, int index, DspHandle* input, DspConnectionHandle* inputconnection);

        public Result DSP_GetInput(DspHandle dsp, int index, out DspHandle input)
        {
            fixed (DspHandle* pInput = &input)
            {
                return DSP_GetInput(dsp, index, pInput, null);
            }
        }

        public Result DSP_GetInput(DspHandle dsp, int index, out DspHandle input, out DspConnectionHandle inputconnection)
        {
            fixed (DspHandle* pInput = &input)
            fixed (DspConnectionHandle* pInputConnection = &inputconnection)
            {
                return DSP_GetInput(dsp, index, pInput, pInputConnection);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetOutput(DspHandle dsp, int index, DspHandle* output, DspConnectionHandle* outputconnection);

        public Result DSP_GetOutput(DspHandle dsp, int index, out DspHandle output)
        {
            fixed (DspHandle* pOutput = &output)
            {
                return DSP_GetOutput(dsp, index, pOutput, null);
            }
        }

        public Result DSP_GetOutput(DspHandle dsp, int index, out DspHandle output, out DspConnectionHandle outputconnection)
        {
            fixed (DspHandle* pOutput = &output)
            fixed (DspConnectionHandle* pOutputConnection = &outputconnection)
            {
                return DSP_GetOutput(dsp, index, pOutput, pOutputConnection);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetActive(DspHandle dsp, FmodBool active);

        [InteropMethod]
        public partial Result DSP_GetActive(DspHandle dsp, FmodBool* active);

        public Result DSP_GetActive(DspHandle dsp, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
            {
                return DSP_GetActive(dsp, pActive);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetBypass(DspHandle dsp, FmodBool bypass);

        [InteropMethod]
        public partial Result DSP_GetBypass(DspHandle dsp, FmodBool* bypass);

        public Result DSP_GetBypass(DspHandle dsp, out FmodBool bypass)
        {
            fixed (FmodBool* pBypass = &bypass)
            {
                return DSP_GetBypass(dsp, pBypass);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetWetDryMix(DspHandle dsp, float prewet, float postwet, float dry);

        [InteropMethod]
        public partial Result DSP_GetWetDryMix(DspHandle dsp, float* prewet, float* postwet, float* dry);

        public Result DSP_GetWetDryMix(DspHandle dsp, out float prewet, out float postwet, out float dry)
        {
            fixed (float* pPrewet = &prewet, pPostwet = &postwet, pDry = &dry)
            {
                return DSP_GetWetDryMix(dsp, pPrewet, pPostwet, pDry);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetChannelFormat(DspHandle dsp, ChannelMask channelmask, int channelCount, SpeakerMode source_speakermode);

        [InteropMethod]
        public partial Result DSP_GetChannelFormat(DspHandle dsp, ChannelMask* channelmask, int* channelCount, SpeakerMode* source_speakermode);

        public Result DSP_GetChannelFormat(DspHandle dsp, out ChannelMask channelmask, out int channelCount, out SpeakerMode source_speakermode)
        {
            fixed (ChannelMask* pChannelMask = &channelmask)
            fixed (int* pChannelCount = &channelCount)
            fixed (SpeakerMode* pSource = &source_speakermode)
            {
                return DSP_GetChannelFormat(dsp, pChannelMask, pChannelCount, pSource);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetOutputChannelFormat(DspHandle dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, ChannelMask* outmask, int* outchannels, SpeakerMode* outspeakermode);
        
        public Result DSP_GetOutputChannelFormat(DspHandle dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            fixed (ChannelMask* pOutMask = &outmask)
            fixed (int* pOutChannels = &outchannels)
            fixed (SpeakerMode* pOutSpeakerMode = &outspeakermode)
            {
                return DSP_GetOutputChannelFormat(dsp, inmask, inchannels, inspeakermode, pOutMask, pOutChannels, pOutSpeakerMode);
            }
        }

        [InteropMethod]
        public partial Result DSP_Reset(DspHandle dsp);

        [InteropMethod]
        public partial Result DSP_SetParameterFloat(DspHandle dsp, int index, float value);

        [InteropMethod]
        public partial Result DSP_SetParameterBool(DspHandle dsp, int index, FmodBool value);

        [InteropMethod]
        public partial Result DSP_SetParameterInt(DspHandle dsp, int index, int value);

        [InteropMethod]
        public partial Result DSP_SetParameterData(DspHandle dsp, int index, void* data, uint length);

        [InteropMethod]
        public partial Result DSP_GetParameterFloat(DspHandle dsp, int index, float* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterFloat(DspHandle dsp, int index, out float value, byte* valuestr, int valuestrlen)
        {
            fixed (float* pValue = &value)
            {
                return DSP_GetParameterFloat(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterFloat(DspHandle dsp, int index, float* value)
        {
            return DSP_GetParameterFloat(dsp, index, value, null, 0);
        }

        public Result DSP_GetParameterFloat(DspHandle dsp, int index, out float value)
        {
            fixed (float* pValue = &value)
            {
                return DSP_GetParameterFloat(dsp, index, pValue, null, 0);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetParameterBool(DspHandle dsp, int index, FmodBool* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterBool(DspHandle dsp, int index, out FmodBool value, byte* valuestr, int valuestrlen)
        {
            fixed (FmodBool* pValue = &value)
            {
                return DSP_GetParameterBool(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterBool(DspHandle dsp, int index, FmodBool* value)
        {
            return DSP_GetParameterBool(dsp, index, value, null, 0);
        }

        public Result DSP_GetParameterBool(DspHandle dsp, int index, out FmodBool value)
        {
            return DSP_GetParameterBool(dsp, index, out value, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetParameterInt(DspHandle dsp, int index, int* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterInt(DspHandle dsp, int index, out int value, byte* valuestr, int valuestrlen)
        {
            fixed (int* pValue = &value)
            {
                return DSP_GetParameterInt(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterInt(DspHandle dsp, int index, int* value)
        {
            return DSP_GetParameterInt(dsp, index, value, null, 0);
        }

        public Result DSP_GetParameterInt(DspHandle dsp, int index, out int value)
        {
            return DSP_GetParameterInt(dsp, index, out value, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetParameterData(DspHandle dsp, int index, void** data, uint* length, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterData(DspHandle dsp, int index, out void* data, out uint length, byte* valuestr, int valuestrlen)
        {
            fixed (void** pData = &data)
            fixed (uint* pLength = &length)
            {
                return DSP_GetParameterData(dsp, index, pData, pLength, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterData(DspHandle dsp, int index, void** data, uint* length)
        {
            return DSP_GetParameterData(dsp, index, data, length, null, 0);
        }

        public Result DSP_GetParameterData(DspHandle dsp, int index, out void* data, out uint length)
        {
            return DSP_GetParameterData(dsp, index, out data, out length, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetNumParameters(DspHandle dsp, int* paramCount);

        public Result DSP_GetNumParameters(DspHandle dsp, out int paramCount)
        {
            fixed (int* pParamCount = &paramCount)
            {
                return DSP_GetNumParameters(dsp, pParamCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetParameterInfo(DspHandle dsp, int index, DigitalSignalProcessing.ParameterDescriptionStruct** desc);

        [InteropMethod]
        public partial Result DSP_GetDataParameterIndex(DspHandle dsp, int datatype, int* index);

        public Result DSP_GetDataParameterIndex(DspHandle dsp, int datatype, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return DSP_GetDataParameterIndex(dsp, datatype, pIndex);
            }
        }

        public Result DSP_GetDataParameterIndex(DspHandle dsp, ParameterDataType datatype, int* index)
        {
            return DSP_GetDataParameterIndex(dsp, (int)datatype, index);
        }

        public Result DSP_GetDataParameterIndex(DspHandle dsp, ParameterDataType datatype, out int index)
        {
            return DSP_GetDataParameterIndex(dsp, (int)datatype, out index);
        }

        [InteropMethod]
        public partial Result DSP_ShowConfigDialog(DspHandle dsp, IntPtr hwnd, bool show);

        [InteropMethod]
        public partial Result DSP_GetInfo(DspHandle dsp, byte* name, FmodVersion* version, int* channels, int* configwidth, int* configheight);

        public Result DSP_GetInfo(DspHandle dsp, out string name, FmodVersion* version, int* channels, int* configwidth, int* configheight)
        {
            byte* buffer = stackalloc byte[32];

            var res = DSP_GetInfo(dsp, buffer, version, channels, configwidth, configheight);

            name = FmodHelpers.BufferToString(buffer, 32);

            return res;
        }

        public Result DSP_GetInfo(DspHandle dsp, out string name, out FmodVersion version, out int channels, out int configwidth, out int configheight)
        {
            fixed (FmodVersion* pVersion = &version)
            fixed (int* pChannels = &channels, pConfigWidth = &configwidth, pConfigHeight = &configheight)
            {
                return DSP_GetInfo(dsp, out name, pVersion, pChannels, pConfigWidth, pConfigHeight);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetType(DspHandle dsp, DSPType* type);

        public Result DSP_GetType(DspHandle dsp, out DSPType type)
        {
            fixed (DSPType* pType = &type)
            {
                return DSP_GetType(dsp, pType);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetIdle(DspHandle dsp, FmodBool* idle);

        public Result DSP_GetIdle(DspHandle dsp, out FmodBool idle)
        {
            fixed (FmodBool* pIdle = &idle)
            {
                return DSP_GetIdle(dsp, pIdle);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetMeteringEnabled(DspHandle dsp, FmodBool inputEnabled, FmodBool outputEnabled);

        [InteropMethod]
        public partial Result DSP_GetMeteringEnabled(DspHandle dsp, FmodBool* inputEnabled, FmodBool* outputEnabled);

        public Result DSP_GetMeteringEnabled(DspHandle dsp, out FmodBool inputEnabled, out FmodBool outputEnabled)
        {
            fixed (FmodBool* pInputEnabled = &inputEnabled, pOutputEnabled = &outputEnabled)
            {
                return DSP_GetMeteringEnabled(dsp, pInputEnabled, pOutputEnabled);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetMeteringInfo(DspHandle dsp, DSPMeteringInfo* inputInfo, DSPMeteringInfo* outputInfo);

        public Result DSP_GetMeteringInfo(DspHandle dsp, out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo)
        {
            fixed (DSPMeteringInfo* pInputInfo = &inputInfo, pOutputInfo = &outputInfo)
            {
                return DSP_GetMeteringInfo(dsp, pInputInfo, pOutputInfo);
            }
        }

        [InteropMethod]
        internal partial Result DSP_SetUserData(DspHandle dsp, IntPtr userdata);

        [InteropMethod]
        internal partial Result DSP_GetUserData(DspHandle dsp, IntPtr* userdata);

        #endregion

        #region Dsp Connection Functions

        [InteropMethod]
        public partial Result DSPConnection_GetInput(DspConnectionHandle dspconnection, DspHandle* input);

        public Result DSPConnection_GetInput(DspConnectionHandle dspconnection, out DspHandle input)
        {
            fixed (DspHandle* pInput = &input)
            {
                return DSPConnection_GetInput(dspconnection, pInput);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_GetOutput(DspConnectionHandle dspconnection, DspHandle* output);

        public Result DSPConnection_GetOutput(DspConnectionHandle dspconnection, out DspHandle output)
        {
            fixed (DspHandle* pOutput = &output)
            {
                return DSPConnection_GetOutput(dspconnection, pOutput);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_SetMix(DspConnectionHandle dspconnection, float volume);

        [InteropMethod]
        public partial Result DSPConnection_GetMix(DspConnectionHandle dspconnection, float* volume);

        public Result DSPConnection_GetMix(DspConnectionHandle dspconnection, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return DSPConnection_GetMix(dspconnection, pVolume);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_SetMixMatrix(DspConnectionHandle dspconnection, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result DSPConnection_GetMixMatrix(DspConnectionHandle dspconnection, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        public Result DSPConnection_GetMixMatrix(DspConnectionHandle dspconnection, float* matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            fixed (int* pOutChannels = &outchannels, pInChannels = &inchannels)
            {
                return DSPConnection_GetMixMatrix(dspconnection, matrix, pOutChannels, pInChannels, inchannel_hop);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_GetType(DspConnectionHandle dspconnection, DSPConnectionType* type);

        public Result DSPConnection_GetType(DspConnectionHandle dspconnection, out DSPConnectionType type)
        {
            fixed (DSPConnectionType* pType = &type)
            {
                return DSPConnection_GetType(dspconnection, pType);
            }
        }

        [InteropMethod]
        internal partial Result DSPConnection_SetUserData(DspConnectionHandle dspconnection, IntPtr userdata);

        [InteropMethod]
        internal partial Result DSPConnection_GetUserData(DspConnectionHandle dspconnection, IntPtr* userdata);

        #endregion

        #region Geometry Functions

        [InteropMethod]
        public partial Result Geometry_Release(GeometryHandle geometry);

        [InteropMethod]
        public partial Result Geometry_AddPolygon(GeometryHandle geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector3* vertices, int* polygonindex);

        public Result Geometry_AddPolygon(GeometryHandle geometry, float directocclusion, float reverbocclusion, bool doublesided, ReadOnlySpan<Vector3> vertices, int* polygonindex)
        {
            fixed (Vector3* pVertices = vertices)
            {
                return Geometry_AddPolygon(geometry, directocclusion, reverbocclusion, doublesided, vertices.Length, pVertices, polygonindex);
            }
        }

        public Result Geometry_AddPolygon(GeometryHandle geometry, float directocclusion, float reverbocclusion, bool doublesided, ReadOnlySpan<Vector3> vertices, out int polygonindex)
        {
            fixed (int* pPolygonIndex = &polygonindex)
            {
                return Geometry_AddPolygon(geometry, directocclusion, reverbocclusion, doublesided, vertices, pPolygonIndex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetNumPolygons(GeometryHandle geometry, int* polygonCount);

        public Result Geometry_GetNumPolygons(GeometryHandle geometry, out int polygonCount)
        {
            fixed (int* pPolygonCount = &polygonCount)
            {
                return Geometry_GetNumPolygons(geometry, pPolygonCount);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetMaxPolygons(GeometryHandle geometry, int* maxpolygons, int* maxvertices);

        public Result Geometry_GetMaxPolygons(GeometryHandle geometry, out int maxpolygons, out int maxvertices)
        {
            fixed (int* pMaxPolygons = &maxpolygons, pMaxVertices = &maxvertices)
            {
                return Geometry_GetMaxPolygons(geometry, pMaxPolygons, pMaxVertices);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPolygonNumVertices(GeometryHandle geometry, int index, int* vertexCount);

        public Result Geometry_GetPolygonNumVertices(GeometryHandle geometry, int index, out int vertexCount)
        {
            fixed (int* pVertexCount = &vertexCount)
            {
                return Geometry_GetPolygonNumVertices(geometry, index, pVertexCount);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPolygonVertex(GeometryHandle geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_SetPolygonVertex(GeometryHandle geometry, int index, int vertexindex, in Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_SetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPolygonVertex(GeometryHandle geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_GetPolygonVertex(GeometryHandle geometry, int index, int vertexindex, out Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_GetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPolygonAttributes(GeometryHandle geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

        [InteropMethod]
        public partial Result Geometry_GetPolygonAttributes(GeometryHandle geometry, int index, float* directocclusion, float* reverbocclusion, FmodBool* doublesided);

        public Result Geometry_GetPolygonAttributes(GeometryHandle geometry, int index, float* directocclusion, float* reverbocclusion, out FmodBool doublesided)
        {
            fixed (FmodBool* pDoubleSided = &doublesided)
            {
                return Geometry_GetPolygonAttributes(geometry, index, directocclusion, reverbocclusion, pDoubleSided);
            }
        }

        public Result Geometry_GetPolygonAttributes(GeometryHandle geometry, int index, out float directocclusion, out float reverbocclusion, out FmodBool doublesided)
        {
            fixed (float* pDirectOcclusion = &directocclusion, pReverbOcclusion = &reverbocclusion)
            {
                return Geometry_GetPolygonAttributes(geometry, index, pDirectOcclusion, pReverbOcclusion, out doublesided);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetActive(GeometryHandle geometry, FmodBool active);

        [InteropMethod]
        public partial Result Geometry_GetActive(GeometryHandle geometry, FmodBool* active);

        public Result Geometry_GetActive(GeometryHandle geometry, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
            {
                return Geometry_GetActive(geometry, pActive);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetRotation(GeometryHandle geometry, Vector3* forward, Vector3* up);

        public Result Geometry_SetRotation(GeometryHandle geometry, in Vector3 forward, in Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_SetRotation(geometry, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetRotation(GeometryHandle geometry, Vector3* forward, Vector3* up);

        public Result Geometry_GetRotation(GeometryHandle geometry, out Vector3 forward, out Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_GetRotation(geometry, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPosition(GeometryHandle geometry, Vector3* position);

        public Result Geometry_SetPosition(GeometryHandle geometry, in Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_SetPosition(geometry, pPosition);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPosition(GeometryHandle geometry, Vector3* position);

        public Result Geometry_GetPosition(GeometryHandle geometry, out Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_GetPosition(geometry, pPosition);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetScale(GeometryHandle geometry, Vector3* scale);

        public Result Geometry_SetScale(GeometryHandle geometry, in Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_SetScale(geometry, pScale);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetScale(GeometryHandle geometry, Vector3* scale);

        public Result Geometry_GetScale(GeometryHandle geometry, out Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_GetScale(geometry, pScale);
            }
        }

        [InteropMethod]
        public partial Result Geometry_Save(GeometryHandle geometry, void* data, int* datasize);

        [InteropMethod]
        internal partial Result Geometry_SetUserData(GeometryHandle geometry, IntPtr userdata);

        [InteropMethod]
        internal partial Result Geometry_GetUserData(GeometryHandle geometry, IntPtr* userdata);

        #endregion

        #region Reverb 3D Functions

        [InteropMethod]
        public partial Result Reverb3D_Release(Reverb3DHandle reverb3D);

        [InteropMethod]
        public partial Result Reverb3D_Set3DAttributes(Reverb3DHandle reverb3D, Vector3* position, float minDistance, float maxDistance);

        public Result Reverb3D_Set3DAttributes(Reverb3DHandle reverb3D, in Vector3 position, float minDistance, float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Reverb3D_Set3DAttributes(reverb3D, pPosition, minDistance, maxDistance);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_Get3DAttributes(Reverb3DHandle reverb3D, Vector3* position, float* minDistance, float* maxDistance);

        public Result Reverb3D_Get3DAttributes(Reverb3DHandle reverb3D, out Vector3 position, out float minDistance, out float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            fixed (float* pMin = &minDistance, pMax = &maxDistance)
            {
                return Reverb3D_Get3DAttributes(reverb3D, pPosition, pMin, pMax);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_SetProperties(Reverb3DHandle reverb3D, ReverbProperties* properties);

        public Result Reverb3D_SetProperties(Reverb3DHandle reverb3D, in ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_SetProperties(reverb3D, pProps);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_GetProperties(Reverb3DHandle reverb3D, ReverbProperties* properties);

        public Result Reverb3D_GetProperties(Reverb3DHandle reverb3D, out ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_GetProperties(reverb3D, pProps);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_SetActive(Reverb3DHandle reverb3D, FmodBool active);

        [InteropMethod]
        public partial Result Reverb3D_GetActive(Reverb3DHandle reverb3D, FmodBool* active);

        public Result Reverb3D_GetActive(Reverb3DHandle reverb3D, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
                return Reverb3D_GetActive(reverb3D, pActive);
        }

        [InteropMethod]
        internal partial Result Reverb3D_SetUserData(Reverb3DHandle reverb3D, IntPtr userdata);

        [InteropMethod]
        internal partial Result Reverb3D_GetUserData(Reverb3DHandle reverb3D, IntPtr* userdata);

        #endregion
    }
}
