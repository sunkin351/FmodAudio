using System;
using System.Numerics;
using System.Runtime.InteropServices;

using FmodAudio.Codec;
using FmodAudio.Dsp;
using FmodAudio.Output;

namespace FmodAudio.Base
{
    [VTable]
    public unsafe partial class FmodLibrary
    {
        [InteropMethod]
        public partial Result Memory_Initialize(IntPtr poolmem, int poollen, delegate* stdcall<uint, MemoryType, IntPtr, IntPtr> useralloc, delegate* stdcall<IntPtr, uint, MemoryType, IntPtr, IntPtr> userrealloc, delegate* stdcall<IntPtr, MemoryType, IntPtr, void> userfree, MemoryType memtypeflags);

        private MemoryAllocCallback? memory_useralloc;
        private MemoryReallocCallback? memory_userrealloc;
        private MemoryFreeCallback? memory_userfree;

        public Result Memory_Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags)
        {
            delegate* stdcall<uint, MemoryType, IntPtr, IntPtr> allocPtr            = (delegate* stdcall<uint, MemoryType, IntPtr, IntPtr>)Marshal.GetFunctionPointerForDelegate(useralloc);
            delegate* stdcall<IntPtr, uint, MemoryType, IntPtr, IntPtr> reallocPtr  = (delegate* stdcall<IntPtr, uint, MemoryType, IntPtr, IntPtr>)Marshal.GetFunctionPointerForDelegate(userrealloc);
            delegate* stdcall<IntPtr, MemoryType, IntPtr, void> freePtr             = (delegate* stdcall<IntPtr, MemoryType, IntPtr, void>)Marshal.GetFunctionPointerForDelegate(userfree);

            var res = Memory_Initialize(poolmem, poollen, allocPtr, reallocPtr, freePtr, memtypeflags);

            if (res == Result.Ok)
            {
                memory_useralloc = useralloc;
                memory_userrealloc = userrealloc;
                memory_userfree = userfree;
            }

            return res;
        }

        [InteropMethod]
        private partial Result Memory_GetStats(int* currentalloced, int* maxalloced, int blocking);

        public Result Memory_GetStats(int* currentalloced, int* maxalloced, FmodBool blocking)
        {
            return Memory_GetStats(currentalloced, maxalloced, blocking.value);
        }

        public Result Memory_GetStats(out int currentAlloced, out int maxAlloced, FmodBool blocking)
        {
            fixed (int* pCurrent = &currentAlloced, pMax = &maxAlloced)
            {
                return Memory_GetStats(pCurrent, pMax, blocking.value);
            }
        }

        [InteropMethod]
        public partial Result Debug_Initialize(DebugFlags flags, DebugMode mode, delegate* stdcall<DebugFlags, byte*, int, byte*, byte*, Result> callback, byte* filename);

        public Result Debug_Initialize(DebugFlags flags, DebugMode mode, delegate* stdcall<DebugFlags, byte*, int, byte*, byte*, Result> callback, string filename)
        {
            fixed (byte* pFilename = FmodHelpers.ToUTF8NullTerminated(filename))
            {
                return Debug_Initialize(flags, mode, callback, pFilename);
            }
        }

        [InteropMethod]
        private partial Result File_SetDiskBusy(int busy);

        public Result File_SetDiskBusy(FmodBool busy)
        {
            return File_SetDiskBusy(busy.value);
        }

        [InteropMethod]
        public partial Result File_GetDiskBusy(FmodBool* busy);

        public Result File_GetDiskBusy(out FmodBool busy)
        {
            fixed (FmodBool* pBusy = &busy)
            {
                return File_GetDiskBusy(pBusy);
            }
        }

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
            delegate* stdcall<byte*, uint*, IntPtr*, IntPtr, Result> useropen,
            delegate* stdcall<IntPtr, IntPtr, Result> userclose,
            delegate* stdcall<IntPtr, byte*, uint, uint*, IntPtr, Result> userread,
            delegate* stdcall<IntPtr, uint, IntPtr, Result> userseek,
            delegate* stdcall<AsyncReadInfo*, IntPtr, Result> userasyncread,
            delegate* stdcall<AsyncReadInfo*, IntPtr, Result> userasynccancel,
            int blockalign);

        [InteropMethod]
        public partial Result System_AttachFileSystem(SystemHandle system,
            delegate* stdcall<byte*, uint*, IntPtr*, IntPtr, Result> useropen,
            delegate* stdcall<IntPtr, IntPtr, Result> userclose,
            delegate* stdcall<IntPtr, byte*, uint, uint*, IntPtr, Result> userread,
            delegate* stdcall<IntPtr, uint, IntPtr, Result> userseek);

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
        public partial Result System_SetCallback(SystemHandle system, delegate* stdcall<IntPtr, SystemCallbackType, IntPtr, IntPtr, IntPtr, Result> callback, SystemCallbackType callbackmask);

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
        public partial Result System_CreateDSPByPlugin(SystemHandle system, PluginHandle handle, IntPtr* dsp);

        public Result System_CreateDSPByPlugin(SystemHandle system, PluginHandle handle, out IntPtr dsp)
        {
            fixed (IntPtr* pDsp = &dsp)
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
        public partial Result System_GetDSPInfoByPlugin(SystemHandle system, PluginHandle handle, IntPtr* description);

        public Result System_GetDSPInfoByPlugin(SystemHandle system, PluginHandle handle, out IntPtr description)
        {
            fixed (IntPtr* pDescription = &description)
            {
                return System_GetDSPInfoByPlugin(system, handle, pDescription);
            }
        }

        [InteropMethod]
        public partial Result System_RegisterCodec(SystemHandle system, CodecDescription.Structure* description, PluginHandle* handle, uint priority);

        public Result System_RegisterCodec(SystemHandle system, ref CodecDescription.Structure description, PluginHandle* handle, uint priority)
        {
            fixed (CodecDescription.Structure* pDescription = &description)
            {
                return System_RegisterCodec(system, pDescription, handle, priority);
            }
        }

        public Result System_RegisterCodec(SystemHandle system, ref CodecDescription.Structure description, out PluginHandle handle, uint priority)
        {
            fixed (CodecDescription.Structure* pDescription = &description)
            fixed (PluginHandle* pHandle = &handle)
            {
                return System_RegisterCodec(system, pDescription, pHandle, priority);
            }
        }

        public Result System_RegisterCodec(SystemHandle system, CodecDescription description, PluginHandle* handle, uint priority)
        {
            return System_RegisterCodec(system, ref description.Struct, handle, priority);
        }

        public Result System_RegisterCodec(SystemHandle system, CodecDescription description, out PluginHandle handle, uint priority)
        {
            return System_RegisterCodec(system, ref description.Struct, out handle, priority);
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
        private partial Result System_SetSpeakerPosition(SystemHandle system, Speaker speaker, float x, float y, int active);

        public Result System_SetSpeakerPosition(SystemHandle system, Speaker speaker, float x, float y, FmodBool active)
        {
            return System_SetSpeakerPosition(system, speaker, x, y, active.value);
        }

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
        public partial Result System_Set3DRolloffCallback(SystemHandle system, delegate* stdcall<IntPtr, float, float> callback);

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
        public partial Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, SoundHandle* sound);

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, out SoundHandle sound)
        { 
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, info, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, SoundHandle* sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            {
                return System_CreateSound(system, nameOrData, mode, pInfo, sound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, out SoundHandle sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, pInfo, pSound);
            }
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo info, SoundHandle* sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, sound);
        }

        public Result System_CreateSound(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo info, out SoundHandle sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, out sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, out sound);
        }

        [InteropMethod]
        public partial Result System_CreateStream(SystemHandle system, byte* name_or_data, Mode mode, CreateSoundInfo.Structure* exinfo, SoundHandle* sound);

        public Result System_CreateStream(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, out SoundHandle sound)
        {
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateStream(system, nameOrData, mode, info, pSound);
            }
        }

        public Result System_CreateStream(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, SoundHandle* sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            {
                return System_CreateStream(system, nameOrData, mode, pInfo, sound);
            }
        }

        public Result System_CreateStream(SystemHandle system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, out SoundHandle sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            fixed (SoundHandle* pSound = &sound)
            {
                return System_CreateStream(system, nameOrData, mode, pInfo, pSound);
            }
        }

        public Result System_CreateStream(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo info, SoundHandle* sound)
        {
            if (info is null)
            {
                return System_CreateStream(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, sound);
            }

            return System_CreateStream(system, nameOrData, mode, ref info.Struct, sound);
        }

        public Result System_CreateStream(SystemHandle system, byte* nameOrData, Mode mode, CreateSoundInfo info, out SoundHandle sound)
        {
            if (info is null)
            {
                return System_CreateStream(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, out sound);
            }

            return System_CreateStream(system, nameOrData, mode, ref info.Struct, out sound);
        }

        [InteropMethod]
        public partial Result System_CreateDSP(SystemHandle system, DspDescriptionStruct* description, IntPtr* dsp);

        public Result System_CreateDSP(SystemHandle system, ref DspDescriptionStruct description, IntPtr* dsp)
        {
            fixed (DspDescriptionStruct* pDescription = &description)
            {
                return System_CreateDSP(system, pDescription, dsp);
            }
        }

        public Result System_CreateDSP(SystemHandle system, ref DspDescriptionStruct description, out IntPtr dsp)
        {
            fixed (DspDescriptionStruct* pDescription = &description)
            fixed (IntPtr* pDsp = &dsp)
            {
                return System_CreateDSP(system, pDescription, pDsp);
            }
        }

        [InteropMethod]
        public partial Result System_CreateDSPByType(SystemHandle system, DSPType type, IntPtr* dsp);

        public Result System_CreateDSPByType(SystemHandle system, DSPType type, out IntPtr dsp)
        {
            fixed (IntPtr* pDsp = &dsp)
            {
                return System_CreateDSPByType(system, type, pDsp);
            }
        }

        [InteropMethod]
        public partial Result System_CreateChannelGroup(SystemHandle system, byte* name, IntPtr* channelGroup);

        public Result System_CreateChannelGroup(SystemHandle system, byte* name, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, name, pChannelGroup);
            }
        }

        public Result System_CreateChannelGroup(SystemHandle system, string name, IntPtr* channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateChannelGroup(system, pName, channelGroup);
            }
        }

        public Result System_CreateChannelGroup(SystemHandle system, string name, out IntPtr channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, pName, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result System_CreateSoundGroup(SystemHandle system, byte* name, SoundGroupHandle* soundGroup);

        public Result System_CreateSoundGroup(SystemHandle system, byte* name, out SoundGroupHandle channelGroup)
        {
            fixed (SoundGroupHandle* pChannelGroup = &channelGroup)
            {
                return System_CreateSoundGroup(system, name, pChannelGroup);
            }
        }

        public Result System_CreateSoundGroup(SystemHandle system, string name, SoundGroupHandle* channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateSoundGroup(system, pName, channelGroup);
            }
        }

        public Result System_CreateSoundGroup(SystemHandle system, string name, out SoundGroupHandle channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (SoundGroupHandle* pChannelGroup = &channelGroup)
            {
                return System_CreateSoundGroup(system, pName, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result System_CreateReverb3D(SystemHandle system, IntPtr* reverb);

        public Result System_CreateReverb3D(SystemHandle system, out IntPtr reverb)
        {
            fixed (IntPtr* pReverb = &reverb)
            {
                return System_CreateReverb3D(system, pReverb);
            }
        }

        [InteropMethod]
        public partial Result System_PlaySound(SystemHandle system, IntPtr sound, IntPtr channelGroup, bool paused, IntPtr* channel);

        public Result System_PlaySound(SystemHandle system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_PlaySound(system, sound, channelGroup, paused, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_PlayDSP(SystemHandle system, IntPtr dsp, IntPtr channelGroup, bool paused, IntPtr* channel);

        public Result System_PlayDSP(SystemHandle system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_PlayDSP(system, dsp, channelGroup, paused, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_GetChannel(SystemHandle system, int channelid, IntPtr* channel);

        public Result System_GetChannel(SystemHandle system, int channelid, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_GetChannel(system, channelid, pChannel);
            }
        }

        [InteropMethod]
        public partial Result System_GetMasterChannelGroup(SystemHandle system, IntPtr* channelGroup);

        public Result System_GetMasterChannelGroup(SystemHandle system, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_GetMasterChannelGroup(system, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result System_GetMasterSoundGroup(SystemHandle system, IntPtr* soundGroup);

        public Result System_GetMasterSoundGroup(SystemHandle system, out IntPtr soundGroup)
        {
            fixed (IntPtr* pSoundGroup = &soundGroup)
            {
                return System_GetMasterSoundGroup(system, pSoundGroup);
            }
        }

        [InteropMethod]
        public partial Result System_AttachChannelGroupToPort(SystemHandle system, uint portType, ulong portIndex, IntPtr channelGroup, bool passThru);

        [InteropMethod]
        public partial Result System_DetachChannelGroupFromPort(SystemHandle system, IntPtr channelGroup);

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
        public partial Result System_CreateGeometry(SystemHandle system, int maxpolygons, int maxvertices, IntPtr* geometry);

        public Result System_CreateGeometry(SystemHandle system, int maxpolygons, int maxvertices, out IntPtr geometry)
        {
            fixed (IntPtr* pGeometry = &geometry)
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

        [InteropMethod]
        public partial Result Sound_Release(IntPtr sound);

        [InteropMethod]
        public partial Result Sound_GetSystemObject(IntPtr sound, SystemHandle* system);

        public Result Sound_GetSystemObject(IntPtr sound, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return Sound_GetSystemObject(sound, pSystem);
            }
        }

        [InteropMethod]
        public partial Result Sound_Lock(IntPtr sound, uint offset, uint length, IntPtr* ptr1, IntPtr* ptr2, uint* len1, uint* len2);

        public Result Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            fixed (IntPtr* p1 = &ptr1, p2 = &ptr2)
            fixed (uint* l1 = &len1, l2 = &len2)
            {
                return Sound_Lock(sound, offset, length, p1, p2, l1, l2);
            }
        }

        [InteropMethod]
        public partial Result Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);

        [InteropMethod]
        public partial Result Sound_SetDefaults(IntPtr sound, float frequency, int priority);

        [InteropMethod]
        public partial Result Sound_GetDefaults(IntPtr sound, float* frequency, int* priority);

        public Result Sound_GetDefaults(IntPtr sound, out float frequency, out int priority)
        {
            fixed (float* pFrequency = &frequency)
            fixed (int* pPriority = &priority)
            {
                return Sound_GetDefaults(sound, pFrequency, pPriority);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);

        [InteropMethod]
        public partial Result Sound_Get3DMinMaxDistance(IntPtr sound, float* min, float* max);

        public Result Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max)
        {
            fixed (float* pMin = &min, pMax = &max)
            {
                return Sound_Get3DMinMaxDistance(sound, pMin, pMax);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);

        [InteropMethod]
        public partial Result Sound_Get3DConeSettings(IntPtr sound, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideConeAngle = &insideconeangle, pOutsideConeAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return Sound_Get3DConeSettings(sound, pInsideConeAngle, pOutsideConeAngle, pOutsideVolume);
            }
        }

        [InteropMethod]
        public partial Result Sound_Set3DCustomRolloff(IntPtr sound, Vector3* points, int pointCount);

        [InteropMethod]
        public partial Result Sound_Get3DCustomRolloff(IntPtr sound, Vector3** points, int* pointCount);

        public Result Sound_Get3DCustomRolloff(IntPtr sound, out Vector3* points, out int pointCount)
        {
            fixed (Vector3** pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return Sound_Get3DCustomRolloff(sound, pPoints, pPointCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSubSound(IntPtr sound, int index, IntPtr* subsound);

        public Result Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound)
        {
            fixed (IntPtr* pSubsound = &subsound)
            {
                return Sound_GetSubSound(sound, index, pSubsound);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSubSoundParent(IntPtr sound, IntPtr* parentsound);

        public Result Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound)
        {
            fixed (IntPtr* pParent = &parentsound)
            {
                return Sound_GetSubSoundParent(sound, pParent);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetName(IntPtr sound, byte* name, int namelen);

        [InteropMethod]
        public partial Result Sound_GetLength(IntPtr sound, uint* length, TimeUnit lengthtype);

        public Result Sound_GetLength(IntPtr sound, out uint length, TimeUnit lengthtype)
        {
            fixed (uint* pLength = &length)
            {
                return Sound_GetLength(sound, pLength, lengthtype);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetFormat(IntPtr sound, SoundType* type, SoundFormat* format, int* channels, int* bits);

        public Result Sound_GetFormat(IntPtr sound, out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            fixed (SoundType* pType = &type)
            fixed (SoundFormat* pFormat = &format)
            fixed (int* pChannels = &channels, pBits = &bits)
            {
                return Sound_GetFormat(sound, pType, pFormat, pChannels, pBits);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumSubSounds(IntPtr sound, int* numsubsounds);

        public Result Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds)
        {
            fixed (int* psubsoundCount = &numsubsounds)
            {
                return Sound_GetNumSubSounds(sound, psubsoundCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumTags(IntPtr sound, int* tagCount, int* updatedTagCount);

        public Result Sound_GetNumTags(IntPtr sound, out int tagCount, out int updatedTagCount)
        {
            fixed (int* pTagCount = &tagCount, pUpdatedTagCount = &updatedTagCount)
            {
                return Sound_GetNumTags(sound, pTagCount, pUpdatedTagCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetTag(IntPtr sound, byte* name, int index, Tag* tag);

        public Result Sound_GetTag(IntPtr sound, string name, int index, Tag* tag)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return Sound_GetTag(sound, pName, index, tag);
            }
        }

        public Result Sound_GetTag(IntPtr sound, string name, int index, out Tag tag)
        {
            fixed (Tag* pTag = &tag)
            {
                return Sound_GetTag(sound, name, index, pTag);
            }
        }


        [InteropMethod]
        public partial Result Sound_GetOpenState(IntPtr sound, OpenState* openstate, uint* percentbuffered, FmodBool* starving, FmodBool* diskbusy);

        public Result Sound_GetOpenState(IntPtr sound, out OpenState openstate, out uint percentBuffered, out FmodBool starving, out FmodBool diskbusy)
        {
            fixed (OpenState* pOpenState = &openstate)
            fixed (uint* pPercentBuffered = &percentBuffered)
            fixed (FmodBool* pStarving = &starving, pDiskBusy = &diskbusy)
            {
                return Sound_GetOpenState(sound, pOpenState, pPercentBuffered, pStarving, pDiskBusy);
            }
        }

        [InteropMethod]
        public partial Result Sound_ReadData(IntPtr sound, void* buffer, uint length, uint* read);

        public Result Sound_ReadData(IntPtr sound, void* buffer, uint length, out uint read)
        {
            fixed (uint* pRead = &read)
            {
                return Sound_ReadData(sound, buffer, length, pRead);
            }
        }

        public Result Sound_ReadData(IntPtr sound, Span<byte> buffer, uint* read)
        {
            fixed (byte* pBuffer = buffer)
            {
                return Sound_ReadData(sound, pBuffer, (uint)buffer.Length, read);
            }
        }

        public Result Sound_ReadData(IntPtr sound, Span<byte> buffer, out uint read)
        {
            fixed (byte* pBuffer = buffer)
            fixed (uint* pRead = &read)
            {
                return Sound_ReadData(sound, pBuffer, (uint)buffer.Length, pRead);
            }
        }

        [InteropMethod]
        public partial Result Sound_SeekData(IntPtr sound, uint pcm);

        [InteropMethod]
        public partial Result Sound_SetSoundGroup(IntPtr sound, IntPtr soundGroup);

        [InteropMethod]
        public partial Result Sound_GetSoundGroup(IntPtr sound, IntPtr* soundGroup);

        public Result Sound_GetSoundGroup(IntPtr sound, out IntPtr soundGroup)
        {
            fixed (IntPtr* pSoundGroup = &soundGroup)
            {
                return Sound_GetSoundGroup(sound, pSoundGroup);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetNumSyncPoints(IntPtr sound, int* syncPointCount);

        public Result Sound_GetNumSyncPoints(IntPtr sound, out int syncPointCount)
        {
            fixed (int* pSyncPointCount = &syncPointCount)
            {
                return Sound_GetNumSyncPoints(sound, pSyncPointCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSyncPoint(IntPtr sound, int index, IntPtr* point);

        public Result Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point)
        {
            fixed (IntPtr* pPoint = &point)
            {
                return Sound_GetSyncPoint(sound, index, pPoint);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, byte* name, int namelen, uint* offset, TimeUnit offsetType);

        public Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, byte* name, int namelen, out uint offset, TimeUnit offsetType)
        {
            fixed (uint* pOffset = &offset)
            {
                return Sound_GetSyncPointInfo(sound, point, name, namelen, pOffset, offsetType);
            }
        }

        public Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, Span<byte> nameBuffer, uint* offset, TimeUnit offsetType)
        {
            fixed (byte* pName = nameBuffer)
            {
                return Sound_GetSyncPointInfo(sound, point, pName, nameBuffer.Length, offset, offsetType);
            }
        }

        public Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, Span<byte> nameBuffer, out uint offset, TimeUnit offsetType)
        {
            fixed (byte* pName = nameBuffer)
            {
                return Sound_GetSyncPointInfo(sound, point, pName, nameBuffer.Length, out offset, offsetType);
            }
        }

        [InteropMethod]
        public partial Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsetType, byte* name, IntPtr* point);

        public Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsetType, string name, IntPtr* point)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return Sound_AddSyncPoint(sound, offset, offsetType, pName, point);
            }
        }

        public Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsetType, string name, out IntPtr point)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (IntPtr* pPoint = &point)
            {
                return Sound_AddSyncPoint(sound, offset, offsetType, pName, pPoint);
            }
        }

        [InteropMethod]
        public partial Result Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);

        [InteropMethod]
        public partial Result Sound_SetMode(IntPtr sound, Mode mode);

        [InteropMethod]
        public partial Result Sound_GetMode(IntPtr sound, Mode* mode);

        public Result Sound_GetMode(IntPtr sound, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Sound_GetMode(sound, pMode);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetLoopCount(IntPtr sound, int loopcount);

        [InteropMethod]
        public partial Result Sound_GetLoopCount(IntPtr sound, int* loopcount);

        public Result Sound_GetLoopCount(IntPtr sound, out int loopCount)
        {
            fixed (int* pLoopCount = &loopCount)
            {
                return Sound_GetLoopCount(sound, pLoopCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetLoopPoints(IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [InteropMethod]
        public partial Result Sound_GetLoopPoints(IntPtr sound, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Sound_GetLoopPoints(IntPtr sound, out uint loopStart, TimeUnit loopStartType, out uint loopEnd, TimeUnit loopEndType)
        {
            fixed (uint* pLoopStart = &loopStart, pLoopEnd = &loopEnd)
            {
                return Sound_GetLoopPoints(sound, pLoopStart, loopStartType, pLoopEnd, loopEndType);
            }
        }

        [InteropMethod]
        public partial Result Sound_GetMusicNumChannels(IntPtr sound, int* numchannels);

        public Result Sound_GetMusicNumChannels(IntPtr sound, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return Sound_GetMusicNumChannels(sound, pChannelCount);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);

        [InteropMethod]
        public partial Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, float* volume);

        public Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return Sound_GetMusicChannelVolume(sound, channel, pVolume);
            }
        }

        [InteropMethod]
        public partial Result Sound_SetMusicSpeed(IntPtr sound, float speed);

        [InteropMethod]
        public partial Result Sound_GetMusicSpeed(IntPtr sound, float* speed);

        public Result Sound_GetMusicSpeed(IntPtr sound, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return Sound_GetMusicSpeed(sound, pSpeed);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_Release(IntPtr soundGroup);

        [InteropMethod]
        public partial Result SoundGroup_GetSystemObject(IntPtr soundGroup, SystemHandle* system);

        public Result SoundGroup_GetSystemObject(IntPtr soundGroup, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return SoundGroup_GetSystemObject(soundGroup, pSystem);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMaxAudible(IntPtr soundGroup, int maxaudible);

        [InteropMethod]
        public partial Result SoundGroup_GetMaxAudible(IntPtr soundGroup, int* maxaudible);

        [InteropMethod]
        public Result SoundGroup_GetMaxAudible(IntPtr soundGroup, out int maxAudible)
        {
            fixed (int* pMaxAudible = &maxAudible)
            {
                return SoundGroup_GetMaxAudible(soundGroup, pMaxAudible);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMaxAudibleBehavior(IntPtr soundGroup, SoundGroupBehavior behavior);

        [InteropMethod]
        public partial Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundGroup, SoundGroupBehavior* behavior);

        public Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundGroup, out SoundGroupBehavior behavior)
        {
            fixed (SoundGroupBehavior* pBehavior = &behavior)
            {
                return SoundGroup_GetMaxAudibleBehavior(soundGroup, pBehavior);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetMuteFadeSpeed(IntPtr soundGroup, float speed);

        [InteropMethod]
        public partial Result SoundGroup_GetMuteFadeSpeed(IntPtr soundGroup, float* speed);

        public Result SoundGroup_GetMuteFadeSpeed(IntPtr soundGroup, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return SoundGroup_GetMuteFadeSpeed(soundGroup, pSpeed);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_SetVolume(IntPtr soundGroup, float volume);

        [InteropMethod]
        public partial Result SoundGroup_GetVolume(IntPtr soundGroup, float* volume);

        public Result SoundGroup_GetVolume(IntPtr soundGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return SoundGroup_GetVolume(soundGroup, pVolume);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_Stop(IntPtr soundGroup);

        [InteropMethod]
        public partial Result SoundGroup_GetName(IntPtr soundGroup, byte* name, int namelen);

        [InteropMethod]
        public partial Result SoundGroup_GetNumSounds(IntPtr soundGroup, int* numsounds);

        public Result SoundGroup_GetNumSounds(IntPtr soundGroup, out int soundCount)
        {
            fixed (int* pSoundCount = &soundCount)
            {
                return SoundGroup_GetNumSounds(soundGroup, pSoundCount);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_GetSound(IntPtr soundGroup, int index, IntPtr* sound);

        public Result SoundGroup_GetSound(IntPtr soundGroup, int index, out IntPtr sound)
        {
            fixed (IntPtr* pSound = &sound)
            {
                return SoundGroup_GetSound(soundGroup, index, pSound);
            }
        }

        [InteropMethod]
        public partial Result SoundGroup_GetNumPlaying(IntPtr soundGroup, int* soundsPlaying);

        public Result SoundGroup_GetNumPlaying(IntPtr soundGroup, out int soundsPlaying)
        {
            fixed (int* pSoundsPlaying = &soundsPlaying)
            {
                return SoundGroup_GetNumPlaying(soundGroup, pSoundsPlaying);
            }
        }

        [InteropMethod]
        internal partial Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

        [InteropMethod]
        internal partial Result SoundGroup_GetUserData(IntPtr soundgroup, IntPtr* userdata);

        [InteropMethod]
        public partial Result Channel_SetFrequency(IntPtr channel, float frequency);

        [InteropMethod]
        public partial Result Channel_GetFrequency(IntPtr channel, float* frequency);

        public Result Channel_GetFrequency(IntPtr channel, out float frequency)
        {
            fixed (float* pFrequency = &frequency)
            {
                return Channel_GetFrequency(channel, pFrequency);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetPriority(IntPtr channel, int priority);

        [InteropMethod]
        public partial Result Channel_GetPriority(IntPtr channel, int* priority);

        public Result Channel_GetPriority(IntPtr channel, out int priority)
        {
            fixed (int* pPriority = &priority)
            {
                return Channel_GetPriority(channel, pPriority);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetChannelGroup(IntPtr channel, IntPtr channelGroup);

        [InteropMethod]
        public partial Result Channel_GetChannelGroup(IntPtr channel, IntPtr* channelGroup);

        public Result Channel_GetChannelGroup(IntPtr channel, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return Channel_GetChannelGroup(channel, pChannelGroup);
            }
        }

        [InteropMethod]
        public partial Result Channel_IsVirtual(IntPtr channel, FmodBool* isvirtual);

        public Result Channel_IsVirtual(IntPtr channel, out FmodBool isvirtual)
        {
            fixed (FmodBool* pIsVirtual = &isvirtual)
                return Channel_IsVirtual(channel, pIsVirtual);
        }

        [InteropMethod]
        public partial Result Channel_GetCurrentSound(IntPtr channel, IntPtr* sound);

        public Result Channel_GetCurrentSound(IntPtr channel, out IntPtr sound)
        {
            fixed (IntPtr* pSound = &sound)
            {
                return Channel_GetCurrentSound(channel, pSound);
            }
        }

        [InteropMethod]
        public partial Result Channel_GetIndex(IntPtr channel, int* index);

        public Result Channel_GetIndex(IntPtr channel, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return Channel_GetIndex(channel, pIndex);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetPosition(IntPtr channel, uint position, TimeUnit posType);

        [InteropMethod]
        public partial Result Channel_GetPosition(IntPtr channel, uint* position, TimeUnit posType);

        public Result Channel_GetPosition(IntPtr channel, out uint position, TimeUnit posType)
        {
            fixed (uint* pPos = &position)
            {
                return Channel_GetPosition(channel, pPos, posType);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetMode(IntPtr channel, Mode mode);

        [InteropMethod]
        public partial Result Channel_GetMode(IntPtr channel, Mode* mode);

        public Result Channel_GetMode(IntPtr channel, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Channel_GetMode(channel, pMode);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetLoopCount(IntPtr channel, int loopcount);

        [InteropMethod]
        public partial Result Channel_GetLoopCount(IntPtr channel, int* loopcount);

        public Result Channel_GetLoopCount(IntPtr channel, out int loopcount)
        {
            fixed (int* pLoopCount = &loopcount)
            {
                return Channel_GetLoopCount(channel, pLoopCount);
            }
        }

        [InteropMethod]
        public partial Result Channel_SetLoopPoints(IntPtr channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [InteropMethod]
        public partial Result Channel_GetLoopPoints(IntPtr channel, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            fixed (uint* pLoopStart = &loopstart, pLoopEnd = &loopend)
            {
                return Channel_GetLoopPoints(channel, pLoopStart, loopstarttype, pLoopEnd, loopendtype);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Stop(IntPtr channelGroup);

        [InteropMethod]
        private partial Result ChannelGroup_SetPaused(IntPtr channelGroup, int paused);

        public Result ChannelGroup_SetPaused(IntPtr channelGroup, FmodBool paused)
        {
            return ChannelGroup_SetPaused(channelGroup, paused.value);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetPaused(IntPtr channelGroup, FmodBool* paused);

        public Result ChannelGroup_GetPaused(IntPtr channelGroup, out FmodBool paused)
        {
            fixed (FmodBool* pPaused = &paused)
                return ChannelGroup_GetPaused(channelGroup, pPaused);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetVolume(IntPtr channelGroup, float* volume);

        public Result ChannelGroup_GetVolume(IntPtr channelGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return ChannelGroup_GetVolume(channelGroup, pVolume);
            }
        }

        [InteropMethod]
        private partial Result ChannelGroup_SetVolumeRamp(IntPtr channelGroup, int ramp);

        public Result ChannelGroup_SetVolumeRamp(IntPtr channelGroup, FmodBool ramp)
        {
            return ChannelGroup_SetVolumeRamp(channelGroup, ramp.value);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetVolumeRamp(IntPtr channelGroup, FmodBool* ramp);

        public Result ChannelGroup_GetVolumeRamp(IntPtr channelGroup, out FmodBool ramp)
        {
            fixed (FmodBool* pRamp = &ramp)
                return ChannelGroup_GetVolumeRamp(channelGroup, pRamp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetAudibility(IntPtr channelGroup, float* audibility);

        public Result ChannelGroup_GetAudibility(IntPtr channelGroup, out float audibility)
        {
            fixed (float* pAudibility = &audibility)
            {
                return ChannelGroup_GetAudibility(channelGroup, pAudibility);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetPitch(IntPtr channelGroup, float pitch);

        [InteropMethod]
        public partial Result ChannelGroup_GetPitch(IntPtr channelGroup, float* pitch);

        public Result ChannelGroup_GetPitch(IntPtr channelGroup, out float pitch)
        {
            fixed (float* pPitch = &pitch)
            {
                return ChannelGroup_GetPitch(channelGroup, pPitch);
            }
        }

        [InteropMethod]
        private partial Result ChannelGroup_SetMute(IntPtr channelGroup, int mute);

        public Result ChannelGroup_SetMute(IntPtr channelGroup, FmodBool mute)
        {
            return ChannelGroup_SetMute(channelGroup, mute.value);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetMute(IntPtr channelGroup, FmodBool* mute);

        public Result ChannelGroup_GetMute(IntPtr channelGroup, out FmodBool mute)
        {
            fixed (FmodBool* pMute = &mute)
            {
                return ChannelGroup_GetMute(channelGroup, pMute);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetReverbProperties(IntPtr channelGroup, int instance, float wet);

        [InteropMethod]
        public partial Result ChannelGroup_GetReverbProperties(IntPtr channelGroup, int instance, float* wet);

        public Result ChannelGroup_GetReverbProperties(IntPtr channelGroup, int instance, out float wet)
        {
            fixed (float* pWet = &wet)
            {
                return ChannelGroup_GetReverbProperties(channelGroup, instance, pWet);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetLowPassGain(IntPtr channelGroup, float gain);

        [InteropMethod]
        public partial Result ChannelGroup_GetLowPassGain(IntPtr channelGroup, float* gain);

        public Result ChannelGroup_GetLowPassGain(IntPtr channelGroup, out float gain)
        {
            fixed (float* pGain = &gain)
            {
                return ChannelGroup_GetLowPassGain(channelGroup, pGain);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetMode(IntPtr channelGroup, Mode mode);

        [InteropMethod]
        public partial Result ChannelGroup_GetMode(IntPtr channelGroup, Mode* mode);

        public Result ChannelGroup_GetMode(IntPtr channelGroup, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return ChannelGroup_GetMode(channelGroup, pMode);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetCallback(IntPtr channelGroup, delegate* stdcall<IntPtr, ChannelControlType, ChannelControlCallbackType, IntPtr, IntPtr, void> callback);

        [InteropMethod]
        public partial Result ChannelGroup_IsPlaying(IntPtr channelGroup, FmodBool* isplaying);

        public Result ChannelGroup_IsPlaying(IntPtr channelGroup, out FmodBool isplaying)
        {
            fixed (FmodBool* pValue = &isplaying)
            {
                return ChannelGroup_IsPlaying(channelGroup, pValue);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetPan(IntPtr channelGroup, float pan);

        [InteropMethod]
        public partial Result ChannelGroup_SetMixLevelsOutput(IntPtr channelGroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

        [InteropMethod]
        public partial Result ChannelGroup_SetMixLevelsInput(IntPtr channelGroup, float* levels, int levelCount);

        public Result ChannelGroup_SetMixLevelsInput(IntPtr channelGroup, Span<float> levels)
        {
            fixed (float* pLevels = levels)
            {
                return ChannelGroup_SetMixLevelsInput(channelGroup, pLevels, levels.Length);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetMixMatrix(IntPtr channelGroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result ChannelGroup_GetMixMatrix(IntPtr channelGroup, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSPClock(IntPtr channelGroup, ulong* dspclock, ulong* parentclock);

        public Result ChannelGroup_GetDSPClock(IntPtr channelGroup, out ulong dspclock, out ulong parentclock)
        {
            fixed (ulong* pDspClock = &dspclock, pParentClock = &parentclock)
            {
                return ChannelGroup_GetDSPClock(channelGroup, pDspClock, pParentClock);
            }
        }

        [InteropMethod]
        private partial Result ChannelGroup_SetDelay(IntPtr channelGroup, ulong dspclock_start, ulong dspclock_end, int stopchannels);

        public Result ChannelGroup_SetDelay(IntPtr channelGroup, ulong dspclock_start, ulong dspclock_end, FmodBool stopchannels)
        {
            return ChannelGroup_SetDelay(channelGroup, dspclock_start, dspclock_end, stopchannels.value);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetDelay(IntPtr channelGroup, ulong* dspclock_start, ulong* dspclock_end, FmodBool* stopchannels);

        public Result ChannelGroup_GetDelay(IntPtr channelGroup, ulong* dspclock_start, ulong* dspclock_end, out FmodBool stopchannels)
        {
            fixed (FmodBool* pValue = &stopchannels)
            {
                return ChannelGroup_GetDelay(channelGroup, dspclock_start, dspclock_end, pValue);
            }
        }

        public Result ChannelGroup_GetDelay(IntPtr channelGroup, out ulong dspclock_start, out ulong dspclock_end, out FmodBool stopchannels)
        {
            fixed (ulong* pDspclock_start = &dspclock_start, pDspclock_end = &dspclock_end)
            fixed (FmodBool* pStopChannels = &stopchannels)
            {
                return ChannelGroup_GetDelay(channelGroup, pDspclock_start, pDspclock_end, pStopChannels);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_AddFadePoint(IntPtr channelGroup, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_SetFadePointRamp(IntPtr channelGroup, ulong dspclock, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_RemoveFadePoints(IntPtr channelGroup, ulong dspclock_start, ulong dspclock_end);

        [InteropMethod]
        public partial Result ChannelGroup_GetFadePoints(IntPtr channelGroup, uint* numpoints, ulong* point_dspclock, float* point_volume);

        public Result ChannelGroup_GetFadePoints(IntPtr channelGroup, out ulong[] pointDspClocks, out float[] pointVolumes)
        {
            uint count = 0;

            Result res = ChannelGroup_GetFadePoints(channelGroup, &count, null, null);

            if (res != Result.Ok || count == 0)
            {
                pointDspClocks = Array.Empty<ulong>();
                pointVolumes = Array.Empty<float>();
                return res;
            }

            var clocks = new ulong[count];
            var volumes = new float[count];

            fixed (ulong* pClocks = clocks)
            fixed (float* pVolumes = volumes)
            {
                res = ChannelGroup_GetFadePoints(channelGroup, &count, pClocks, pVolumes);
            }

            pointDspClocks = clocks;
            pointVolumes = volumes;

            return res;
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DAttributes(IntPtr channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Set3DAttributes(IntPtr channelGroup, in Vector3 pos, in Vector3 vel, in Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltpanpos = &alt_pan_pos)
            {
                return ChannelGroup_Set3DAttributes(channelGroup, pPos, pVel, pAltpanpos);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Get3DAttributes(IntPtr channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Get3DAttributes(IntPtr channelGroup, out Vector3 pos, out Vector3 vel, out Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltPanPos = &alt_pan_pos)
            {
                return ChannelGroup_Get3DAttributes(channelGroup, pPos, pVel, pAltPanPos);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DMinMaxDistance(IntPtr channelGroup, float mindistance, float maxdistance);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelGroup, float* mindistance, float* maxdistance);

        public Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelGroup, out float minDistance, out float maxDistance)
        {
            fixed (float* pMinDistance = &minDistance, pMaxdistance = &maxDistance)
            {
                return ChannelGroup_Get3DMinMaxDistance(channelGroup, pMinDistance, pMaxdistance);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DConeSettings(IntPtr channelGroup, float insideconeangle, float outsideconeangle, float outsidevolume);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DConeSettings(IntPtr channelGroup, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result ChannelGroup_Get3DConeSettings(IntPtr channelGroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideAngle = &insideconeangle, pOutsideAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return ChannelGroup_Get3DConeSettings(channelGroup, pInsideAngle, pOutsideAngle, pOutsideVolume);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DConeOrientation(IntPtr channelGroup, Vector3* orientation);

        public Result ChannelGroup_Set3DConeOrientation(IntPtr channelGroup, in Vector3 orientation)
        {
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Set3DConeOrientation(channelGroup, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Get3DConeOrientation(IntPtr channelGroup, Vector3* orientation);

        public Result ChannelGroup_Get3DConeOrientation(IntPtr channelGroup, out Vector3 orientation)
        {
            orientation = default;
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Get3DConeOrientation(channelGroup, pOrientation);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DCustomRolloff(IntPtr channelGroup, Vector3* points, int numpoints);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DCustomRolloff(IntPtr channelGroup, Vector3** points, int* numpoints);

        public Result ChannelGroup_Get3DCustomRolloff(IntPtr channelGroup, out Vector3* points, out int pointCount)
        {
            fixed (Vector3** pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return ChannelGroup_Get3DCustomRolloff(channelGroup, pPoints, pPointCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DOcclusion(IntPtr channelGroup, float directocclusion, float reverbocclusion);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DOcclusion(IntPtr channelGroup, float* directocclusion, float* reverbocclusion);

        public Result ChannelGroup_Get3DOcclusion(IntPtr channelGroup, out float directOcclusion, out float reverbOcclusion)
        {
            fixed (float* pDirectOcclusion = &directOcclusion, pReverbOcclusion = &reverbOcclusion)
            {
                return ChannelGroup_Get3DOcclusion(channelGroup, pDirectOcclusion, pReverbOcclusion);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DSpread(IntPtr channelGroup, float angle);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DSpread(IntPtr channelGroup, float* angle);

        public Result ChannelGroup_Get3DSpread(IntPtr channelGroup, out float angle)
        {
            fixed (float* pAngle = &angle)
            {
                return ChannelGroup_Get3DSpread(channelGroup, pAngle);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DLevel(IntPtr channelGroup, float level);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DLevel(IntPtr channelGroup, float* level);

        public Result ChannelGroup_Get3DLevel(IntPtr channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DLevel(channelGroup, pLevel);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Set3DDopplerLevel(IntPtr channelGroup, float level);

        [InteropMethod]
        public partial Result ChannelGroup_Get3DDopplerLevel(IntPtr channelGroup, float* level);

        public Result ChannelGroup_Get3DDopplerLevel(IntPtr channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DDopplerLevel(channelGroup, pLevel);
            }
        }

        [InteropMethod]
        private partial Result ChannelGroup_Set3DDistanceFilter(IntPtr channelGroup, int custom, float customLevel, float centerFreq);

        public Result ChannelGroup_Set3DDistanceFilter(IntPtr channelGroup, FmodBool custom, float customLevel, float centerFreq)
        {
            return ChannelGroup_Set3DDistanceFilter(channelGroup, custom.value, customLevel, centerFreq);
        }

        [InteropMethod]
        public partial Result ChannelGroup_Get3DDistanceFilter(IntPtr channelGroup, FmodBool* custom, float* customLevel, float* centerFreq);

        public Result ChannelGroup_Get3DDistanceFilter(IntPtr channelGroup, out FmodBool custom, out float customLevel, out float centerFreq)
        {
            fixed (FmodBool* pCustom = &custom)
            fixed (float* pCustomlevel = &customLevel, pCenterFreq = &centerFreq)
            {
                return ChannelGroup_Get3DDistanceFilter(channelGroup, pCustom, pCustomlevel, pCenterFreq);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetSystemObject(IntPtr channelGroup, SystemHandle* system);

        public Result ChannelGroup_GetSystemObject(IntPtr channelGroup, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return ChannelGroup_GetSystemObject(channelGroup, pSystem);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetVolume(IntPtr channelGroup, float volume);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSP(IntPtr channelGroup, int index, IntPtr* dsp);

        public Result ChannelGroup_GetDSP(IntPtr channelGroup, int index, out IntPtr dsp)
        {
            fixed (IntPtr* pDsp = &dsp)
            {
                return ChannelGroup_GetDSP(channelGroup, index, pDsp);
            }
        }

        public Result ChannelGroup_GetDSP(IntPtr channelGroup, ChannelControlDSPIndex index, IntPtr* dsp)
        {
            return ChannelGroup_GetDSP(channelGroup, (int)index, dsp);
        }

        public Result ChannelGroup_GetDSP(IntPtr channelGroup, ChannelControlDSPIndex index, out IntPtr dsp)
        {
            return ChannelGroup_GetDSP(channelGroup, (int)index, out dsp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_AddDSP(IntPtr channelGroup, int index, IntPtr dsp);

        public Result ChannelGroup_AddDSP(IntPtr channelGroup, ChannelControlDSPIndex index, IntPtr dsp)
        {
            return ChannelGroup_AddDSP(channelGroup, (int)index, dsp);
        }

        [InteropMethod]
        public partial Result ChannelGroup_RemoveDSP(IntPtr channelGroup, IntPtr dsp);

        [InteropMethod]
        public partial Result ChannelGroup_GetNumDSPs(IntPtr channelGroup, int* dspCount);

        public Result ChannelGroup_GetNumDSPs(IntPtr channelGroup, out int dspCount)
        {
            fixed (int* pDspCount = &dspCount)
            {
                return ChannelGroup_GetNumDSPs(channelGroup, pDspCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_SetDSPIndex(IntPtr channelGroup, IntPtr dsp, int index);

        [InteropMethod]
        public partial Result ChannelGroup_GetDSPIndex(IntPtr channelGroup, IntPtr dsp, int* index);

        public Result ChannelGroup_GetDSPIndex(IntPtr channelGroup, IntPtr dsp, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return ChannelGroup_GetDSPIndex(channelGroup, dsp, pIndex);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_Release(IntPtr Handle);

        [InteropMethod]
        public partial Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, int propagatedspclock, IntPtr* connection);

        public Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, bool propagatedspclock, IntPtr* connection)
        {
            return ChannelGroup_AddGroup(channelGroup, group, propagatedspclock ? 1 : 0, connection);
        }

        public Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, bool propagatedspclock, out IntPtr connection)
        {
            fixed (IntPtr* connectionRes = &connection)
                return ChannelGroup_AddGroup(channelGroup, group, propagatedspclock ? 1 : 0, connectionRes);
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetNumGroups(IntPtr channelGroup, int* groupCount);

        public Result ChannelGroup_GetNumGroups(IntPtr channelGroup, out int groupCount)
        {
            fixed (int* pGroupCount = &groupCount)
            {
                return ChannelGroup_GetNumGroups(channelGroup, pGroupCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetGroup(IntPtr channelGroup, int index, IntPtr* group);

        public Result ChannelGroup_GetGroup(IntPtr channelGroup, int index, out IntPtr group)
        {
            fixed (IntPtr* pGroup = &group)
            {
                return ChannelGroup_GetGroup(channelGroup, index, pGroup);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetParentGroup(IntPtr channelGroup, IntPtr* group);

        public Result ChannelGroup_GetParentGroup(IntPtr channelGroup, out IntPtr group)
        {
            fixed (IntPtr* pGroup = &group)
            {
                return ChannelGroup_GetParentGroup(channelGroup, pGroup);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetName(IntPtr channelGroup, byte* name, int namelen);

        [InteropMethod]
        public partial Result ChannelGroup_GetNumChannels(IntPtr channelGroup, int* channelCount);
        
        public Result ChannelGroup_GetNumChannels(IntPtr channelGroup, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return ChannelGroup_GetNumChannels(channelGroup, pChannelCount);
            }
        }

        [InteropMethod]
        public partial Result ChannelGroup_GetChannel(IntPtr channelGroup, int index, IntPtr* channel);

        public Result ChannelGroup_GetChannel(IntPtr channelGroup, int index, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return ChannelGroup_GetChannel(channelGroup, index, pChannel);
            }
        }

        [InteropMethod]
        internal partial Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

        [InteropMethod]
        internal partial Result ChannelGroup_GetUserData(IntPtr channelgroup, IntPtr* userdata);

        [InteropMethod]
        public partial Result DSP_Release(IntPtr dsp);

        [InteropMethod]
        public partial Result DSP_GetSystemObject(IntPtr dsp, SystemHandle* system);

        public Result DSP_GetSystemObject(IntPtr dsp, out SystemHandle system)
        {
            fixed (SystemHandle* pSystem = &system)
            {
                return DSP_GetSystemObject(dsp, pSystem);
            }
        }

        [InteropMethod]
        public partial Result DSP_AddInput(IntPtr dsp, IntPtr target, IntPtr* connection, DSPConnectionType type);

        public Result DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPConnectionType type)
        {
            fixed (IntPtr* pConnection = &connection)
            {
                return DSP_AddInput(dsp, target, pConnection, type);
            }
        }

        [InteropMethod]
        public partial Result DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);

        [InteropMethod]
        public partial Result DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);

        [InteropMethod]
        public partial Result DSP_GetNumInputs(IntPtr dsp, int* inputCount);

        public Result DSP_GetNumInputs(IntPtr dsp, out int inputCount)
        {
            fixed (int* pInputCount = &inputCount)
            {
                return DSP_GetNumInputs(dsp, pInputCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetNumOutputs(IntPtr dsp, int* outputCount);

        public Result DSP_GetNumOutputs(IntPtr dsp, out int outputCount)
        {
            fixed (int* pOutputCount = &outputCount)
            {
                return DSP_GetNumOutputs(dsp, pOutputCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetInput(IntPtr dsp, int index, IntPtr* input, IntPtr* inputconnection);

        public Result DSP_GetInput(IntPtr dsp, int index, out IntPtr input)
        {
            fixed (IntPtr* pInput = &input)
            {
                return DSP_GetInput(dsp, index, pInput, null);
            }
        }

        public Result DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection)
        {
            fixed (IntPtr* pInput = &input, pInputConnection = &inputconnection)
            {
                return DSP_GetInput(dsp, index, pInput, pInputConnection);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetOutput(IntPtr dsp, int index, IntPtr* output, IntPtr* outputconnection);

        public Result DSP_GetOutput(IntPtr dsp, int index, out IntPtr output)
        {
            fixed (IntPtr* pOutput = &output)
            {
                return DSP_GetOutput(dsp, index, pOutput, null);
            }
        }

        public Result DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection)
        {
            fixed (IntPtr* pOutput = &output, pOutputConnection = &outputconnection)
            {
                return DSP_GetOutput(dsp, index, pOutput, pOutputConnection);
            }
        }

        [InteropMethod]
        private partial Result DSP_SetActive(IntPtr dsp, FmodBool active);

        [InteropMethod]
        public partial Result DSP_GetActive(IntPtr dsp, FmodBool* active);

        public Result DSP_GetActive(IntPtr dsp, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
            {
                return DSP_GetActive(dsp, pActive);
            }
        }

        [InteropMethod]
        private partial Result DSP_SetBypass(IntPtr dsp, int bypass);

        public Result DSP_SetBypass(IntPtr dsp, FmodBool bypass)
        {
            return DSP_SetBypass(dsp, (int)bypass.value);
        }

        [InteropMethod]
        public partial Result DSP_GetBypass(IntPtr dsp, FmodBool* bypass);

        public Result DSP_GetBypass(IntPtr dsp, out FmodBool bypass)
        {
            fixed (FmodBool* pBypass = &bypass)
            {
                return DSP_GetBypass(dsp, pBypass);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

        [InteropMethod]
        public partial Result DSP_GetWetDryMix(IntPtr dsp, float* prewet, float* postwet, float* dry);

        public Result DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry)
        {
            fixed (float* pPrewet = &prewet, pPostwet = &postwet, pDry = &dry)
            {
                return DSP_GetWetDryMix(dsp, pPrewet, pPostwet, pDry);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetChannelFormat(IntPtr dsp, ChannelMask channelmask, int channelCount, SpeakerMode source_speakermode);

        [InteropMethod]
        public partial Result DSP_GetChannelFormat(IntPtr dsp, ChannelMask* channelmask, int* channelCount, SpeakerMode* source_speakermode);

        public Result DSP_GetChannelFormat(IntPtr dsp, out ChannelMask channelmask, out int channelCount, out SpeakerMode source_speakermode)
        {
            fixed (ChannelMask* pChannelMask = &channelmask)
            fixed (int* pChannelCount = &channelCount)
            fixed (SpeakerMode* pSource = &source_speakermode)
            {
                return DSP_GetChannelFormat(dsp, pChannelMask, pChannelCount, pSource);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, ChannelMask* outmask, int* outchannels, SpeakerMode* outspeakermode);
        
        public Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            fixed (ChannelMask* pOutMask = &outmask)
            fixed (int* pOutChannels = &outchannels)
            fixed (SpeakerMode* pOutSpeakerMode = &outspeakermode)
            {
                return DSP_GetOutputChannelFormat(dsp, inmask, inchannels, inspeakermode, pOutMask, pOutChannels, pOutSpeakerMode);
            }
        }

        [InteropMethod]
        public partial Result DSP_Reset(IntPtr dsp);

        [InteropMethod]
        public partial Result DSP_SetParameterFloat(IntPtr dsp, int index, float value);

        [InteropMethod]
        private partial Result DSP_SetParameterBool(IntPtr dsp, int index, int value);

        public Result DSP_SetParameterBool(IntPtr dsp, int index, bool value)
        {
            return DSP_SetParameterBool(dsp, index, value ? 1 : 0);
        }

        [InteropMethod]
        public partial Result DSP_SetParameterInt(IntPtr dsp, int index, int value);

        [InteropMethod]
        public partial Result DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);

        [InteropMethod]
        public partial Result DSP_GetParameterFloat(IntPtr dsp, int index, float* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterFloat(IntPtr dsp, int index, out float value, byte* valuestr, int valuestrlen)
        {
            fixed (float* pValue = &value)
            {
                return DSP_GetParameterFloat(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterFloat(IntPtr dsp, int index, float* value)
        {
            return DSP_GetParameterFloat(dsp, index, value, null, 0);
        }

        public Result DSP_GetParameterFloat(IntPtr dsp, int index, out float value)
        {
            fixed (float* pValue = &value)
            {
                return DSP_GetParameterFloat(dsp, index, pValue, null, 0);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetParameterBool(IntPtr dsp, int index, FmodBool* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterBool(IntPtr dsp, int index, out FmodBool value, byte* valuestr, int valuestrlen)
        {
            fixed (FmodBool* pValue = &value)
            {
                return DSP_GetParameterBool(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterBool(IntPtr dsp, int index, out FmodBool value)
        {
            return DSP_GetParameterBool(dsp, index, out value, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetParameterint(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterint(IntPtr dsp, int index, out int value, byte* valuestr, int valuestrlen)
        {
            fixed (int* pValue = &value)
            {
                return DSP_GetParameterint(dsp, index, pValue, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterint(IntPtr dsp, int index, int* value)
        {
            return DSP_GetParameterint(dsp, index, value, null, 0);
        }

        public Result DSP_GetParameterint(IntPtr dsp, int index, out int value)
        {
            return DSP_GetParameterint(dsp, index, out value, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetParameterData(IntPtr dsp, int index, IntPtr* data, uint* length, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, byte* valuestr, int valuestrlen)
        {
            fixed (IntPtr* pData = &data)
            fixed (uint* pLength = &length)
            {
                return DSP_GetParameterData(dsp, index, pData, pLength, valuestr, valuestrlen);
            }
        }

        public Result DSP_GetParameterData(IntPtr dsp, int index, IntPtr* data, uint* length)
        {
            return DSP_GetParameterData(dsp, index, data, length, null, 0);
        }

        public Result DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length)
        {
            return DSP_GetParameterData(dsp, index, out data, out length, null, 0);
        }

        [InteropMethod]
        public partial Result DSP_GetNumParameters(IntPtr dsp, int* paramCount);

        public Result DSP_GetNumParameters(IntPtr dsp, out int paramCount)
        {
            fixed (int* pParamCount = &paramCount)
            {
                return DSP_GetNumParameters(dsp, pParamCount);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetParameterInfo(IntPtr dsp, int index, Dsp.Base.ParameterDescriptionStruct** desc);

        [InteropMethod]
        public partial Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, int* index);

        public Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return DSP_GetDataParameterIndex(dsp, datatype, pIndex);
            }
        }

        public Result DSP_GetDataParameterIndex(IntPtr dsp, ParameterDataType datatype, int* index)
        {
            return DSP_GetDataParameterIndex(dsp, (int)datatype, index);
        }

        public Result DSP_GetDataParameterIndex(IntPtr dsp, ParameterDataType datatype, out int index)
        {
            return DSP_GetDataParameterIndex(dsp, (int)datatype, out index);
        }

        [InteropMethod]
        public partial Result DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);

        [InteropMethod]
        public partial Result DSP_GetInfo(IntPtr dsp, byte* name, FmodVersion* version, int* channels, int* configwidth, int* configheight);

        public Result DSP_GetInfo(IntPtr dsp, out string name, FmodVersion* version, int* channels, int* configwidth, int* configheight)
        {
            byte* buffer = stackalloc byte[32];

            var res = DSP_GetInfo(dsp, buffer, version, channels, configwidth, configheight);

            name = FmodHelpers.PtrToString(buffer, 32);

            return res;
        }

        public Result DSP_GetInfo(IntPtr dsp, out string name, out FmodVersion version, out int channels, out int configwidth, out int configheight)
        {
            fixed (FmodVersion* pVersion = &version)
            fixed (int* pChannels = &channels, pConfigWidth = &configwidth, pConfigHeight = &configheight)
            {
                return DSP_GetInfo(dsp, out name, pVersion, pChannels, pConfigWidth, pConfigHeight);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetType(IntPtr dsp, DSPType* type);

        public Result DSP_GetType(IntPtr dsp, out DSPType type)
        {
            fixed (DSPType* pType = &type)
            {
                return DSP_GetType(dsp, pType);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetIdle(IntPtr dsp, FmodBool* idle);

        public Result DSP_GetIdle(IntPtr dsp, out FmodBool idle)
        {
            fixed (FmodBool* pIdle = &idle)
            {
                return DSP_GetIdle(dsp, pIdle);
            }
        }

        [InteropMethod]
        public partial Result DSP_SetMeteringEnabled(IntPtr dsp, FmodBool inputEnabled, FmodBool outputEnabled);

        [InteropMethod]
        public partial Result DSP_GetMeteringEnabled(IntPtr dsp, FmodBool* inputEnabled, FmodBool* outputEnabled);

        public Result DSP_GetMeteringEnabled(IntPtr dsp, out FmodBool inputEnabled, out FmodBool outputEnabled)
        {
            fixed (FmodBool* pInputEnabled = &inputEnabled, pOutputEnabled = &outputEnabled)
            {
                return DSP_GetMeteringEnabled(dsp, pInputEnabled, pOutputEnabled);
            }
        }

        [InteropMethod]
        public partial Result DSP_GetMeteringInfo(IntPtr dsp, DSPMeteringInfo* inputInfo, DSPMeteringInfo* outputInfo);

        public Result DSP_GetMeteringInfo(IntPtr dsp, out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo)
        {
            fixed (DSPMeteringInfo* pInputInfo = &inputInfo, pOutputInfo = &outputInfo)
            {
                return DSP_GetMeteringInfo(dsp, pInputInfo, pOutputInfo);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_GetInput(IntPtr dspconnection, IntPtr* input);

        public Result DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input)
        {
            fixed (IntPtr* pInput = &input)
            {
                return DSPConnection_GetInput(dspconnection, pInput);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_GetOutput(IntPtr dspconnection, IntPtr* output);

        public Result DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output)
        {
            fixed (IntPtr* pOutput = &output)
            {
                return DSPConnection_GetOutput(dspconnection, pOutput);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_SetMix(IntPtr dspconnection, float volume);

        [InteropMethod]
        public partial Result DSPConnection_GetMix(IntPtr dspconnection, float* volume);

        public Result DSPConnection_GetMix(IntPtr dspconnection, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return DSPConnection_GetMix(dspconnection, pVolume);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_SetMixMatrix(IntPtr dspconnection, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [InteropMethod]
        public partial Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        public Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            fixed (int* pOutChannels = &outchannels, pInChannels = &inchannels)
            {
                return DSPConnection_GetMixMatrix(dspconnection, matrix, pOutChannels, pInChannels, inchannel_hop);
            }
        }

        [InteropMethod]
        public partial Result DSPConnection_GetType(IntPtr dspconnection, DSPConnectionType* type);

        public Result DSPConnection_GetType(IntPtr dspconnection, out DSPConnectionType type)
        {
            fixed (DSPConnectionType* pType = &type)
            {
                return DSPConnection_GetType(dspconnection, pType);
            }
        }

        [InteropMethod]
        internal partial Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

        [InteropMethod]
        internal partial Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);

        [InteropMethod]
        internal partial Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);

        [InteropMethod]
        internal partial Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);

        [InteropMethod]
        public partial Result Geometry_Release(IntPtr geometry);

        [InteropMethod]
        public partial Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector3* vertices, int* polygonindex);

        public Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, ReadOnlySpan<Vector3> vertices, int* polygonindex)
        {
            fixed (Vector3* pVertices = vertices)
            {
                return Geometry_AddPolygon(geometry, directocclusion, reverbocclusion, doublesided, vertices.Length, pVertices, polygonindex);
            }
        }

        public Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, ReadOnlySpan<Vector3> vertices, out int polygonindex)
        {
            fixed (int* pPolygonIndex = &polygonindex)
            {
                return Geometry_AddPolygon(geometry, directocclusion, reverbocclusion, doublesided, vertices, pPolygonIndex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetNumPolygons(IntPtr geometry, int* polygonCount);

        public Result Geometry_GetNumPolygons(IntPtr geometry, out int polygonCount)
        {
            fixed (int* pPolygonCount = &polygonCount)
            {
                return Geometry_GetNumPolygons(geometry, pPolygonCount);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetMaxPolygons(IntPtr geometry, int* maxpolygons, int* maxvertices);

        public Result Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices)
        {
            fixed (int* pMaxPolygons = &maxpolygons, pMaxVertices = &maxvertices)
            {
                return Geometry_GetMaxPolygons(geometry, pMaxPolygons, pMaxVertices);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, int* vertexCount);

        public Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int vertexCount)
        {
            fixed (int* pVertexCount = &vertexCount)
            {
                return Geometry_GetPolygonNumVertices(geometry, index, pVertexCount);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, in Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_SetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_GetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

        [InteropMethod]
        public partial Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, FmodBool* doublesided);

        public Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, out FmodBool doublesided)
        {
            fixed (FmodBool* pDoubleSided = &doublesided)
            {
                return Geometry_GetPolygonAttributes(geometry, index, directocclusion, reverbocclusion, pDoubleSided);
            }
        }

        public Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out FmodBool doublesided)
        {
            fixed (float* pDirectOcclusion = &directocclusion, pReverbOcclusion = &reverbocclusion)
            {
                return Geometry_GetPolygonAttributes(geometry, index, pDirectOcclusion, pReverbOcclusion, out doublesided);
            }
        }

        [InteropMethod]
        private partial Result Geometry_SetActive(IntPtr geometry, int active);

        public Result Geometry_SetActive(IntPtr geometry, FmodBool active)
        {
            return Geometry_SetActive(geometry, active.value);
        }

        [InteropMethod]
        public partial Result Geometry_GetActive(IntPtr geometry, FmodBool* active);

        public Result Geometry_GetActive(IntPtr geometry, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
            {
                return Geometry_GetActive(geometry, pActive);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetRotation(IntPtr geometry, Vector3* forward, Vector3* up);

        public Result Geometry_SetRotation(IntPtr geometry, in Vector3 forward, in Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_SetRotation(geometry, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetRotation(IntPtr geometry, Vector3* forward, Vector3* up);

        public Result Geometry_GetRotation(IntPtr geometry, out Vector3 forward, out Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_GetRotation(geometry, pForward, pUp);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetPosition(IntPtr geometry, Vector3* position);

        public Result Geometry_SetPosition(IntPtr geometry, in Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_SetPosition(geometry, pPosition);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetPosition(IntPtr geometry, Vector3* position);

        public Result Geometry_GetPosition(IntPtr geometry, out Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_GetPosition(geometry, pPosition);
            }
        }

        [InteropMethod]
        public partial Result Geometry_SetScale(IntPtr geometry, Vector3* scale);

        public Result Geometry_SetScale(IntPtr geometry, in Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_SetScale(geometry, pScale);
            }
        }

        [InteropMethod]
        public partial Result Geometry_GetScale(IntPtr geometry, Vector3* scale);

        public Result Geometry_GetScale(IntPtr geometry, out Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_GetScale(geometry, pScale);
            }
        }

        [InteropMethod]
        public partial Result Geometry_Save(IntPtr geometry, void* data, int* datasize);

        [InteropMethod]
        internal partial Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

        [InteropMethod]
        internal partial Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);

        [InteropMethod]
        public partial Result Reverb3D_Release(IntPtr reverb3D);

        [InteropMethod]
        public partial Result Reverb3D_Set3DAttributes(IntPtr reverb3D, Vector3* position, float minDistance, float maxDistance);

        public Result Reverb3D_Set3DAttributes(IntPtr reverb3D, in Vector3 position, float minDistance, float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Reverb3D_Set3DAttributes(reverb3D, pPosition, minDistance, maxDistance);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_Get3DAttributes(IntPtr reverb3D, Vector3* position, float* minDistance, float* maxDistance);

        public Result Reverb3D_Get3DAttributes(IntPtr reverb3D, out Vector3 position, out float minDistance, out float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            fixed (float* pMin = &minDistance, pMax = &maxDistance)
            {
                return Reverb3D_Get3DAttributes(reverb3D, pPosition, pMin, pMax);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_SetProperties(IntPtr reverb3D, ReverbProperties* properties);

        public Result Reverb3D_SetProperties(IntPtr reverb3D, in ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_SetProperties(reverb3D, pProps);
            }
        }

        [InteropMethod]
        public partial Result Reverb3D_GetProperties(IntPtr reverb3D, ReverbProperties* properties);

        public Result Reverb3D_GetProperties(IntPtr reverb3D, out ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_GetProperties(reverb3D, pProps);
            }
        }

        [InteropMethod]
        private partial Result Reverb3D_SetActive(IntPtr reverb3D, int active);

        public Result Reverb3D_SetActive(IntPtr reverb3D, FmodBool active)
        {
            return Reverb3D_SetActive(reverb3D, active.value);
        }

        [InteropMethod]
        public partial Result Reverb3D_GetActive(IntPtr reverb3D, FmodBool* active);

        public Result Reverb3D_GetActive(IntPtr reverb3D, out FmodBool active)
        {
            fixed (FmodBool* pActive = &active)
                return Reverb3D_GetActive(reverb3D, pActive);
        }

        [InteropMethod]
        internal partial Result Reverb3D_SetUserData(IntPtr reverb3D, IntPtr userdata);

        [InteropMethod]
        internal partial Result Reverb3D_GetUserData(IntPtr reverb3D, IntPtr* userdata);
    }
}
