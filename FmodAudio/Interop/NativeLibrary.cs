using System;
using System.Numerics;

using AdvancedDLSupport;

using FmodAudio.Codec;
using FmodAudio.Dsp;
using FmodAudio.Output;

namespace FmodAudio.Interop
{
    public unsafe abstract class NativeLibrary : NativeLibraryBase, INativeLibrary
    {
        protected NativeLibrary(string path, ImplementationOptions options) : base(path, options)
        {
        }

        public abstract Result Memory_Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags);

        public abstract Result Memory_GetStats(int* currentalloced, int* maxalloced, bool blocking);

        public Result Memory_GetStats(out int currentAlloced, out int maxAlloced, bool blocking)
        {
            fixed (int* pCurrent = &currentAlloced, pMax = &maxAlloced)
            {
                return Memory_GetStats(pCurrent, pMax, blocking);
            }
        }
        
        public abstract Result Debug_Initialize(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename);
        
        public abstract Result File_SetDiskBusy(int busy);

        public Result File_SetDiskBusy(bool busy)
        {
            return File_SetDiskBusy(busy ? 1 : 0);
        }
        
        public abstract Result File_GetDiskBusy(int* busy);

        public Result File_GetDiskBusy(bool* busy)
        {
            int tmp = 0;
            var res = File_GetDiskBusy(&tmp);
            *busy = tmp != 0;
            return res;
        }

        public Result File_GetDiskBusy(out bool busy)
        {
            int tmp = 0;
            var res = File_GetDiskBusy(&tmp);
            busy = tmp != 0;
            return res;
        }
        
        public abstract Result System_Create(IntPtr* System);

        public Result System_Create(out IntPtr system)
        {
            fixed (IntPtr* pSystem = &system)
            {
                return System_Create(pSystem);
            }
        }
        
        public abstract Result System_Release(IntPtr system);
        
        public abstract Result System_SetOutput(IntPtr system, OutputType output);
        
        public abstract Result System_GetOutput(IntPtr system, OutputType* output);

        public Result System_GetOutput(IntPtr system, out OutputType output)
        {
            fixed (OutputType* pOutput = &output)
            {
                return System_GetOutput(system, pOutput);
            }
        }
        
        public abstract Result System_GetNumDrivers(IntPtr system, int* driverCount);

        public Result System_GetNumDrivers(IntPtr system, out int driverCount)
        {
            fixed (int* pDriverCount = &driverCount)
            {
                return System_GetNumDrivers(system, pDriverCount);
            }
        }

        public abstract Result System_GetDriverInfo(IntPtr system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels);

        public abstract Result System_SetDriver(IntPtr system, int driver);
        
        public abstract Result System_GetDriver(IntPtr system, int* driver);

        public Result System_GetDriver(IntPtr system, out int driver)
        {
            fixed (int* pDriver = &driver)
            {
                return System_GetDriver(system, pDriver);
            }
        }

        public abstract Result System_SetSoftwareChannels(IntPtr system, int softwareChannelCount);
        
        public abstract Result System_GetSoftwareChannels(IntPtr system, int* softwareChannelCount);
        
        public Result System_GetSoftwareChannels(IntPtr system, out int softwareChannelCount)
        {
            fixed (int* pSCC = &softwareChannelCount)
            {
                return System_GetSoftwareChannels(system, pSCC);
            }
        }

        public abstract Result System_SetSoftwareFormat(IntPtr system, int sampleRate, SpeakerMode speakerMode, int rawSpeakerCount);
        
        public abstract Result System_GetSoftwareFormat(IntPtr system, int* sampleRate, SpeakerMode* speakerMode, int* rawSpeakerCount);

        public Result System_GetSoftwareFormat(IntPtr system, out int sampleRate, out SpeakerMode speakerMode, out int rawSpeakerCount)
        {
            fixed (int* pSampleRate = &sampleRate, pRawSpeakerCount = &rawSpeakerCount)
            fixed (SpeakerMode* pSpeakerMode = &speakerMode)
            {
                return System_GetSoftwareFormat(system, pSampleRate, pSpeakerMode, pRawSpeakerCount);
            }
        }
        
        public abstract Result System_SetDSPBufferSize(IntPtr system, uint bufferLength, int bufferCount);
        
        public abstract Result System_GetDSPBufferSize(IntPtr system, uint* bufferLength, int* bufferCount);

        public Result System_GetDSPBufferSize(IntPtr system, out uint bufferLength, out int bufferCount)
        {
            fixed (uint* pBufferLength = &bufferLength)
            fixed (int* pBufferCount = &bufferCount)
            {
                return System_GetDSPBufferSize(system, pBufferLength, pBufferCount);
            }
        }
        
        public abstract Result System_SetFileSystem(IntPtr system, FileOpenCallback useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek, FileAsyncReadCallback userasyncread, FileAsyncCancelCallback userasynccancel, int blockalign);
        
        public abstract Result System_AttachFileSystem(IntPtr system, FileOpenCallback useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek);
        
        public abstract Result System_SetAdvancedSettings(IntPtr system, AdvancedSettings.Structure* settings);

        public Result System_SetAdvancedSettings(IntPtr system, ref AdvancedSettings.Structure settings)
        {
            fixed (AdvancedSettings.Structure* pSettings = &settings)
            {
                return System_SetAdvancedSettings(system, pSettings);
            }
        }

        public Result System_SetAdvancedSettings(IntPtr system, AdvancedSettings settings)
        {
            return System_SetAdvancedSettings(system, ref settings.Struct);
        }

        public abstract Result System_GetAdvancedSettings(IntPtr system, AdvancedSettings.Structure* settings);

        public Result System_GetAdvancedSettings(IntPtr system, ref AdvancedSettings.Structure settings)
        {
            fixed (AdvancedSettings.Structure* pSettings = &settings)
            {
                return System_GetAdvancedSettings(system, pSettings);
            }
        }

        public Result System_GetAdvancedSettings(IntPtr system, AdvancedSettings settings)
        {
            return System_GetAdvancedSettings(system, ref settings.Struct);
        }

        public abstract Result System_SetCallback(IntPtr system, SystemCallback callback, SystemCallbackType callbackmask);

        public abstract Result System_SetPluginPath(IntPtr system, byte* path);

        public Result System_SetPluginPath(IntPtr system, string path)
        {
            fixed (byte* pPath = FmodHelpers.ToUTF8NullTerminated(path))
            {
                return System_SetPluginPath(system, pPath);
            }
        }
        
        public abstract Result System_LoadPlugin(IntPtr system, byte* filename, uint* handle, uint priority);

        public Result System_LoadPlugin(IntPtr system, string filename, uint* handle, uint priority)
        {
            fixed (byte* pFilename = FmodHelpers.ToUTF8NullTerminated(filename))
            {
                return System_LoadPlugin(system, pFilename, handle, priority);
            }
        }

        public Result System_LoadPlugin(IntPtr system, string filename, out uint handle, uint priority)
        {
            fixed (uint* pHandle = &handle)
            {
                return System_LoadPlugin(system, filename, pHandle, priority);
            }
        }

        public abstract Result System_UnloadPlugin(IntPtr system, uint handle);
        
        public abstract Result System_GetNumNestedPlugins(IntPtr system, uint handle, int* count);

        public Result System_GetNumNestedPlugins(IntPtr system, uint handle, out int count)
        {
            fixed (int* pCount = &count)
            {
                return System_GetNumNestedPlugins(system, handle, pCount);
            }
        }

        public abstract Result System_GetNestedPlugin(IntPtr system, uint handle, int index, uint* nestedhandle);

        public Result System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle)
        {
            fixed (uint* pHandle = &nestedhandle)
            {
                return System_GetNestedPlugin(system, handle, index, pHandle);
            }
        }

        public abstract Result System_GetNumPlugins(IntPtr system, PluginType plugintype, int* pluginCount);

        public Result System_GetNumPlugins(IntPtr system, PluginType plugintype, out int pluginCount)
        {
            fixed (int* pCount = &pluginCount)
            {
                return System_GetNumPlugins(system, plugintype, pCount);
            }
        }

        public abstract Result System_GetPluginHandle(IntPtr system, PluginType plugintype, int index, uint* handle);

        public Result System_GetPluginHandle(IntPtr system, PluginType plugintype, int index, out uint handle)
        {
            fixed (uint* pHandle = &handle)
            {
                return System_GetPluginHandle(system, plugintype, index, pHandle);
            }
        }

        public abstract Result System_GetPluginInfo(IntPtr system, uint handle, PluginType* plugintype, byte* name, int namelen, FmodVersion* version);
        
        public abstract Result System_CreateDSPByPlugin(IntPtr system, uint handle, IntPtr* dsp);

        public Result System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp)
        {
            fixed (IntPtr* pDsp = &dsp)
            {
                return System_CreateDSPByPlugin(system, handle, pDsp);
            }
        }

        public abstract Result System_SetOutputByPlugin(IntPtr system, uint handle);
        
        public abstract Result System_GetOutputByPlugin(IntPtr system, uint* handle);

        public Result System_GetOutputByPlugin(IntPtr system, out uint handle)
        {
            fixed (uint* pHandle = &handle)
            {
                return System_GetOutputByPlugin(system, pHandle);
            }
        }

        public abstract Result System_GetDSPInfoByPlugin(IntPtr system, uint handle, IntPtr* description);

        public Result System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description)
        {
            fixed (IntPtr* pDescription = &description)
            {
                return System_GetDSPInfoByPlugin(system, handle, pDescription);
            }
        }

        public abstract Result System_RegisterCodec(IntPtr system, CodecDescription.Structure* description, uint* handle, uint priority);

        public Result System_RegisterCodec(IntPtr system, ref CodecDescription.Structure description, uint* handle, uint priority)
        {
            fixed (CodecDescription.Structure* pDescription = &description)
            {
                return System_RegisterCodec(system, pDescription, handle, priority);
            }
        }

        public Result System_RegisterCodec(IntPtr system, ref CodecDescription.Structure description, out uint handle, uint priority)
        {
            fixed (CodecDescription.Structure* pDescription = &description)
            fixed (uint* pHandle = &handle)
            {
                return System_RegisterCodec(system, pDescription, pHandle, priority);
            }
        }

        public Result System_RegisterCodec(IntPtr system, CodecDescription description, uint* handle, uint priority)
        {
            return System_RegisterCodec(system, ref description.Struct, handle, priority);
        }

        public Result System_RegisterCodec(IntPtr system, CodecDescription description, out uint handle, uint priority)
        {
            return System_RegisterCodec(system, ref description.Struct, out handle, priority);
        }

        public abstract Result System_RegisterDSP(IntPtr system, DspDescription.Structure* description, uint* handle);

        public Result System_RegisterDSP(IntPtr system, ref DspDescription.Structure description, uint* handle)
        {
            fixed (DspDescription.Structure* pDescription = &description)
            {
                return System_RegisterDSP(system, pDescription, handle);
            }
        }

        public Result System_RegisterDSP(IntPtr system, ref DspDescription.Structure description, out uint handle)
        {
            fixed (DspDescription.Structure* pDescription = &description)
            fixed (uint* pHandle = &handle)
            {
                return System_RegisterDSP(system, pDescription, pHandle);
            }
        }

        public Result System_RegisterDSP(IntPtr system, DspDescription description, uint* handle)
        {
            return System_RegisterDSP(system, ref description.Struct, handle);
        }

        public Result System_RegisterDSP(IntPtr system, DspDescription description, out uint handle)
        {
            return System_RegisterDSP(system, ref description.Struct, out handle);
        }

        public abstract Result System_RegisterOutput(IntPtr system, OutputDescriptionStruct* description, uint* handle);

        public Result System_RegisterOutput(IntPtr system, ref OutputDescriptionStruct description, uint* handle)
        {
            fixed (OutputDescriptionStruct* pDescription = &description)
            {
                return System_RegisterOutput(system, pDescription, handle);
            }
        }

        public Result System_RegisterOutput(IntPtr system, ref OutputDescriptionStruct description, out uint handle)
        {
            fixed (uint* pHandle = &handle)
            {
                return System_RegisterOutput(system, ref description, pHandle);
            }
        }

        public abstract Result System_Init(IntPtr system, int maxchannels, InitFlags flags, IntPtr extradriverdata);

        public abstract Result System_Close(IntPtr system);

        public abstract Result System_Update(IntPtr system);

        public abstract Result System_SetSpeakerPosition(IntPtr system, Speaker speaker, float x, float y, bool active);

        public abstract Result System_GetSpeakerPosition(IntPtr system, Speaker speaker, float* x, float* y, out bool active);

        public Result System_GetSpeakerPosition(IntPtr system, Speaker speaker, out float x, out float y, out bool active)
        {
            fixed (float* pX = &x, pY = &y)
            {
                return System_GetSpeakerPosition(system, speaker, pX, pY, out active);
            }
        }

        public abstract Result System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TimeUnit filebuffersizetype);

        public abstract Result System_GetStreamBufferSize(IntPtr system, uint* filebuffersize, TimeUnit* filebuffersizetype);

        public Result System_GetStreamBufferSize(IntPtr system, out uint fileBufferSize, out TimeUnit fileBufferSizeType)
        {
            fixed (uint* pFBS = &fileBufferSize)
            fixed (TimeUnit* pFBST = &fileBufferSizeType)
            {
                return System_GetStreamBufferSize(system, pFBS, pFBST);
            }
        }

        public abstract Result System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);

        public abstract Result System_Get3DSettings(IntPtr system, float* dopplerscale, float* distancefactor, float* rolloffscale);

        public Result System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale)
        {
            fixed (float* pDopplerScale = &dopplerscale, pDistanceFactor = &distancefactor, pRolloffScale = &rolloffscale)
            {
                return System_Get3DSettings(system, pDopplerScale, pDistanceFactor, pRolloffScale);
            }
        }

        public abstract Result System_Set3DNumListeners(IntPtr system, int listenerCount);

        public abstract Result System_Get3DNumListeners(IntPtr system, int* listenerCount);

        public Result System_Get3DNumListeners(IntPtr system, out int listenerCount)
        {
            fixed (int* pListeners = &listenerCount)
            {
                return System_Get3DNumListeners(system, pListeners);
            }
        }

        public abstract Result System_Set3DListenerAttributes(IntPtr system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);

        public Result System_Set3DListenerAttributes(IntPtr system, int listener, in Vector3 pos, in Vector3 vel, in Vector3 forward, in Vector3 up)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pForward = &forward, pUp = &up)
            {
                return System_Set3DListenerAttributes(system, listener, pPos, pVel, pForward, pUp);
            }
        }

        public abstract Result System_Get3DListenerAttributes(IntPtr system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);

        public Result System_Get3DListenerAttributes(IntPtr system, int listener, out Vector3 pos, out Vector3 vel, out Vector3 forward, out Vector3 up)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pForward = &forward, pUp = &up)
            {
                return System_Get3DListenerAttributes(system, listener, pPos, pVel, pForward, pUp);
            }
        }

        public abstract Result System_Set3DRolloffCallback(IntPtr system, _3DRolloffCallback callback);
        
        public abstract Result System_MixerSuspend(IntPtr system);
        
        public abstract Result System_MixerResume(IntPtr system);
        
        public abstract Result System_GetDefaultMixMatrix(IntPtr system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float* matrix, int matrixhop = 0);

        public abstract Result System_GetSpeakerModeChannels(IntPtr system, SpeakerMode mode, int* channels);

        public Result System_GetSpeakerModeChannels(IntPtr system, SpeakerMode mode, out int channels)
        {
            fixed (int* pChannels = &channels)
            {
                return System_GetSpeakerModeChannels(system, mode, pChannels);
            }
        }

        public abstract Result System_GetVersion(IntPtr system, FmodVersion* version);

        public Result System_GetVersion(IntPtr system, out FmodVersion version)
        {
            fixed (FmodVersion* pVersion = &version)
            {
                return System_GetVersion(system, pVersion);
            }
        }

        public abstract Result System_GetOutputHandle(IntPtr system, IntPtr* handle);

        public Result System_GetOutputHandle(IntPtr system, out IntPtr handle)
        {
            fixed (IntPtr* pHandle = &handle)
            {
                return System_GetOutputHandle(system, pHandle);
            }
        }

        public abstract Result System_GetChannelsPlaying(IntPtr system, int* channels, int* realchannels);

        public Result System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels)
        {
            fixed (int* pChannels = &channels, pRealChannels = &realchannels)
            {
                return System_GetChannelsPlaying(system, pChannels, pRealChannels);
            }
        }

        public abstract Result System_GetCPUUsage(IntPtr system, float* dsp, float* stream, float* geometry, float* update, float* total);

        public Result System_GetCPUUsage(IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total)
        {
            fixed (float* pDsp = &dsp, pStream = &stream, pGeometry = &geometry, pUpdate = &update, ptotal = &total)
            {
                return System_GetCPUUsage(system, pDsp, pStream, pGeometry, pUpdate, ptotal);
            }
        }

        public abstract Result System_GetFileUsage(IntPtr system, long* sampleBytesRead, long* streamBytesRead, long* otherBytesRead);

        public Result System_GetFileUsage(IntPtr system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
        {
            fixed (long* pSample = &sampleBytesRead, pStream = &streamBytesRead, pOther = &otherBytesRead)
            {
                return System_GetFileUsage(system, pSample, pStream, pOther);
            }
        }

        public abstract Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, IntPtr* sound);

        public Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, out IntPtr sound)
        { 
            fixed (IntPtr* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, info, pSound);
            }
        }

        public Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, IntPtr* sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            {
                return System_CreateSound(system, nameOrData, mode, pInfo, sound);
            }
        }

        public Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, out IntPtr sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            fixed (IntPtr* pSound = &sound)
            {
                return System_CreateSound(system, nameOrData, mode, pInfo, pSound);
            }
        }

        public Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo info, IntPtr* sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, sound);
        }

        public Result System_CreateSound(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo info, out IntPtr sound)
        {
            if (info is null)
            {
                return System_CreateSound(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, out sound);
            }

            return System_CreateSound(system, nameOrData, mode, ref info.Struct, out sound);
        }

        public abstract Result System_CreateStream(IntPtr system, byte* name_or_data, Mode mode, CreateSoundInfo.Structure* exinfo, IntPtr* sound);

        public Result System_CreateStream(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo.Structure* info, out IntPtr sound)
        {
            fixed (IntPtr* pSound = &sound)
            {
                return System_CreateStream(system, nameOrData, mode, info, pSound);
            }
        }

        public Result System_CreateStream(IntPtr system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, IntPtr* sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            {
                return System_CreateStream(system, nameOrData, mode, pInfo, sound);
            }
        }

        public Result System_CreateStream(IntPtr system, byte* nameOrData, Mode mode, ref CreateSoundInfo.Structure info, out IntPtr sound)
        {
            fixed (CreateSoundInfo.Structure* pInfo = &info)
            fixed (IntPtr* pSound = &sound)
            {
                return System_CreateStream(system, nameOrData, mode, pInfo, pSound);
            }
        }

        public Result System_CreateStream(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo info, IntPtr* sound)
        {
            if (info is null)
            {
                return System_CreateStream(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, sound);
            }

            return System_CreateStream(system, nameOrData, mode, ref info.Struct, sound);
        }

        public Result System_CreateStream(IntPtr system, byte* nameOrData, Mode mode, CreateSoundInfo info, out IntPtr sound)
        {
            if (info is null)
            {
                return System_CreateStream(system, nameOrData, mode, (CreateSoundInfo.Structure*)null, out sound);
            }

            return System_CreateStream(system, nameOrData, mode, ref info.Struct, out sound);
        }

        public abstract Result System_CreateDSP(IntPtr system, DspDescription.Structure* description, IntPtr* dsp);

        public Result System_CreateDSP(IntPtr system, ref DspDescription.Structure description, IntPtr* dsp)
        {
            fixed (DspDescription.Structure* pDescription = &description)
            {
                return System_CreateDSP(system, pDescription, dsp);
            }
        }

        public Result System_CreateDSP(IntPtr system, ref DspDescription.Structure description, out IntPtr dsp)
        {
            fixed (DspDescription.Structure* pDescription = &description)
            fixed (IntPtr* pDsp = &dsp)
            {
                return System_CreateDSP(system, pDescription, pDsp);
            }
        }

        public Result System_CreateDSP(IntPtr system, DspDescription description, IntPtr* dsp)
        {
            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            return System_CreateDSP(system, ref description.Struct, dsp);
        }

        public Result System_CreateDSP(IntPtr system, DspDescription description, out IntPtr dsp)
        {
            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            return System_CreateDSP(system, ref description.Struct, out dsp);
        }


        public abstract Result System_CreateDSPByType(IntPtr system, DSPType type, IntPtr* dsp);

        public Result System_CreateDSPByType(IntPtr system, DSPType type, out IntPtr dsp)
        {
            fixed (IntPtr* pDsp = &dsp)
            {
                return System_CreateDSPByType(system, type, pDsp);
            }
        }
        
        public abstract Result System_CreateChannelGroup(IntPtr system, byte* name, IntPtr* channelGroup);

        public Result System_CreateChannelGroup(IntPtr system, byte* name, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, name, pChannelGroup);
            }
        }

        public Result System_CreateChannelGroup(IntPtr system, string name, IntPtr* channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateChannelGroup(system, pName, channelGroup);
            }
        }

        public Result System_CreateChannelGroup(IntPtr system, string name, out IntPtr channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateChannelGroup(system, pName, pChannelGroup);
            }
        }

        public abstract Result System_CreateSoundGroup(IntPtr system, byte* name, IntPtr* soundGroup);

        public Result System_CreateSoundGroup(IntPtr system, byte* name, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateSoundGroup(system, name, pChannelGroup);
            }
        }

        public Result System_CreateSoundGroup(IntPtr system, string name, IntPtr* channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            {
                return System_CreateSoundGroup(system, pName, channelGroup);
            }
        }

        public Result System_CreateSoundGroup(IntPtr system, string name, out IntPtr channelGroup)
        {
            fixed (byte* pName = FmodHelpers.ToUTF8NullTerminated(name))
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_CreateSoundGroup(system, pName, pChannelGroup);
            }
        }

        public abstract Result System_CreateReverb3D(IntPtr system, IntPtr* reverb);

        public Result System_CreateReverb3D(IntPtr system, out IntPtr reverb)
        {
            fixed (IntPtr* pReverb = &reverb)
            {
                return System_CreateReverb3D(system, pReverb);
            }
        }
        
        public abstract Result System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, IntPtr* channel);

        public Result System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_PlaySound(system, sound, channelGroup, paused, pChannel);
            }
        }

        public abstract Result System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, IntPtr* channel);

        public Result System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_PlayDSP(system, dsp, channelGroup, paused, pChannel);
            }
        }

        public abstract Result System_GetChannel(IntPtr system, int channelid, IntPtr* channel);

        public Result System_GetChannel(IntPtr system, int channelid, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return System_GetChannel(system, channelid, pChannel);
            }
        }
        
        public abstract Result System_GetMasterChannelGroup(IntPtr system, IntPtr* channelGroup);

        public Result System_GetMasterChannelGroup(IntPtr system, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return System_GetMasterChannelGroup(system, pChannelGroup);
            }
        }

        public abstract Result System_GetMasterSoundGroup(IntPtr system, IntPtr* soundGroup);

        public Result System_GetMasterSoundGroup(IntPtr system, out IntPtr soundGroup)
        {
            fixed (IntPtr* pSoundGroup = &soundGroup)
            {
                return System_GetMasterSoundGroup(system, pSoundGroup);
            }
        }

        public abstract Result System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelGroup, bool passThru);
        
        public abstract Result System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelGroup);
        
        public abstract Result System_SetReverbProperties(IntPtr system, int instance, ReverbProperties* prop);

        public Result System_SetReverbProperties(IntPtr system, int instance, in ReverbProperties prop)
        {
            fixed (ReverbProperties* pProp = &prop)
            {
                return System_SetReverbProperties(system, instance, pProp);
            }
        }

        public abstract Result System_GetReverbProperties(IntPtr system, int instance, ReverbProperties* prop);

        public Result System_GetReverbProperties(IntPtr system, int instance, out ReverbProperties prop)
        {
            fixed (ReverbProperties* pProp = &prop)
            {
                return System_GetReverbProperties(system, instance, pProp);
            }
        }

        public abstract Result System_LockDSP(IntPtr system);
        
        public abstract Result System_UnlockDSP(IntPtr system);
        
        public abstract Result System_GetRecordNumDrivers(IntPtr system, int* numdrivers, int* numconnected);

        public Result System_GetRecordNumDrivers(IntPtr system, out int driverCount, out int connectedCount)
        {
            fixed (int* pdrivers = &driverCount, pConnectedCount = &connectedCount)
            {
                return System_GetRecordNumDrivers(system, pdrivers, pConnectedCount);
            }
        }

        public abstract Result System_GetRecordDriverInfo(IntPtr system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels, DriverState* state);
        
        public abstract Result System_GetRecordPosition(IntPtr system, int id, uint* position);

        public Result System_GetRecordPosition(IntPtr system, int id, out uint position)
        {
            fixed (uint* pPosition = &position)
            {
                return System_GetRecordPosition(system, id, pPosition);
            }
        }
        
        public abstract Result System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);
        
        public abstract Result System_RecordStop(IntPtr system, int id);
        
        public abstract Result System_IsRecording(IntPtr system, int id, out bool recording);
        
        public abstract Result System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, IntPtr* geometry);

        public Result System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry)
        {
            fixed (IntPtr* pGeometry = &geometry)
            {
                return System_CreateGeometry(system, maxpolygons, maxvertices, pGeometry);
            }
        }

        public abstract Result System_SetGeometrySettings(IntPtr system, float maxworldsize);
        
        public abstract Result System_GetGeometrySettings(IntPtr system, float* maxworldsize);

        public Result System_GetGeometrySettings(IntPtr system, out float maxworldsize)
        {
            fixed (float* pMaxWorldsize = &maxworldsize)
            {
                return System_GetGeometrySettings(system, pMaxWorldsize);
            }
        }

        public abstract Result System_LoadGeometry(IntPtr system, byte* data, int datasize, IntPtr* geometry);

        public Result System_LoadGeometry(IntPtr system, byte* data, int datasize, out IntPtr geometry)
        {
            fixed (IntPtr* pGeometry = &geometry)
            {
                return System_LoadGeometry(system, data, datasize, pGeometry);
            }
        }

        public Result System_LoadGeometry(IntPtr system, ReadOnlySpan<byte> data, IntPtr* geometry)
        {
            fixed (byte* pData = data)
            {
                return System_LoadGeometry(system, pData, data.Length, geometry);
            }
        }

        public Result System_LoadGeometry(IntPtr system, ReadOnlySpan<byte> data, out IntPtr geometry)
        {
            fixed (byte* pData = data)
            fixed (IntPtr* pGeometry = &geometry)
            {
                return System_LoadGeometry(system, pData, data.Length, pGeometry);
            }
        }
        
        public abstract Result System_GetGeometryOcclusion(IntPtr system, Vector3* listener, Vector3* source, float* direct, float* reverb);

        public Result System_GetGeometryOcclusion(IntPtr system, out Vector3 listener, out Vector3 source, out float direct, out float reverb)
        {
            fixed (Vector3* pListener = &listener, pSource = &source)
            fixed (float* pDirect = &direct, pReverb = &reverb)
            {
                return System_GetGeometryOcclusion(system, pListener, pSource, pDirect, pReverb);
            }
        }

        public abstract Result System_SetNetworkProxy(IntPtr system, byte* proxy);

        public Result System_SetNetworkProxy(IntPtr system, string proxy)
        {
            fixed (byte* pProxy = FmodHelpers.ToUTF8NullTerminated(proxy))
            {
                return System_SetNetworkProxy(system, pProxy);
            }
        }

        public abstract Result System_GetNetworkProxy(IntPtr system, byte* proxy, int proxylen);
        
        public abstract Result System_SetNetworkTimeout(IntPtr system, int timeout);
        
        public abstract Result System_GetNetworkTimeout(IntPtr system, int* timeout);

        public Result System_GetNetworkTimeout(IntPtr system, out int timeout)
        {
            fixed (int* pTimeout = &timeout)
            {
                return System_GetNetworkTimeout(system, pTimeout);
            }
        }

        [Obsolete]
        public abstract Result System_SetUserData(IntPtr system, IntPtr userdata);
        
        [Obsolete]
        public abstract Result System_GetUserData(IntPtr system, IntPtr* userdata);

        [Obsolete]
        public Result System_GetUserData(IntPtr system, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return System_GetUserData(system, pUserdata);
            }
        }

        public abstract Result Sound_Release(IntPtr sound);

        public abstract Result Sound_GetSystemObject(IntPtr sound, IntPtr* system);

        public Result Sound_GetSystemObject(IntPtr sound, out IntPtr system)
        {
            fixed (IntPtr* pSystem = &system)
            {
                return Sound_GetSystemObject(sound, pSystem);
            }
        }
        
        public abstract Result Sound_Lock(IntPtr sound, uint offset, uint length, IntPtr* ptr1, IntPtr* ptr2, uint* len1, uint* len2);

        public Result Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            fixed (IntPtr* p1 = &ptr1, p2 = &ptr2)
            fixed (uint* l1 = &len1, l2 = &len2)
            {
                return Sound_Lock(sound, offset, length, p1, p2, l1, l2);
            }
        }

        public abstract Result Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);
        
        public abstract Result Sound_SetDefaults(IntPtr sound, float frequency, int priority);
        
        public abstract Result Sound_GetDefaults(IntPtr sound, float* frequency, int* priority);

        public Result Sound_GetDefaults(IntPtr sound, out float frequency, out int priority)
        {
            fixed (float* pFrequency = &frequency)
            fixed (int* pPriority = &priority)
            {
                return Sound_GetDefaults(sound, pFrequency, pPriority);
            }
        }

        public abstract Result Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);
        
        public abstract Result Sound_Get3DMinMaxDistance(IntPtr sound, float* min, float* max);

        public Result Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max)
        {
            fixed (float* pMin = &min, pMax = &max)
            {
                return Sound_Get3DMinMaxDistance(sound, pMin, pMax);
            }
        }

        public abstract Result Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        
        public abstract Result Sound_Get3DConeSettings(IntPtr sound, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideConeAngle = &insideconeangle, pOutsideConeAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return Sound_Get3DConeSettings(sound, pInsideConeAngle, pOutsideConeAngle, pOutsideVolume);
            }
        }

        public abstract Result Sound_Set3DCustomRolloff(IntPtr sound, IntPtr points, int pointCount);
        
        public abstract Result Sound_Get3DCustomRolloff(IntPtr sound, IntPtr* points, int* pointCount);

        public Result Sound_Get3DCustomRolloff(IntPtr sound, out IntPtr points, out int pointCount)
        {
            fixed (IntPtr* pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return Sound_Get3DCustomRolloff(sound, pPoints, pPointCount);
            }
        }

        public abstract Result Sound_GetSubSound(IntPtr sound, int index, IntPtr* subsound);

        public Result Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound)
        {
            fixed (IntPtr* pSubsound = &subsound)
            {
                return Sound_GetSubSound(sound, index, pSubsound);
            }
        }

        public abstract Result Sound_GetSubSoundParent(IntPtr sound, IntPtr* parentsound);

        public Result Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound)
        {
            fixed (IntPtr* pParent = &parentsound)
            {
                return Sound_GetSubSoundParent(sound, pParent);
            }
        }

        public abstract Result Sound_GetName(IntPtr sound, byte* name, int namelen);
        
        public abstract Result Sound_GetLength(IntPtr sound, uint* length, TimeUnit lengthtype);

        public Result Sound_GetLength(IntPtr sound, out uint length, TimeUnit lengthtype)
        {
            fixed (uint* pLength = &length)
            {
                return Sound_GetLength(sound, pLength, lengthtype);
            }
        }

        public abstract Result Sound_GetFormat(IntPtr sound, SoundType* type, SoundFormat* format, int* channels, int* bits);

        public Result Sound_GetFormat(IntPtr sound, out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            fixed (SoundType* pType = &type)
            fixed (SoundFormat* pFormat = &format)
            fixed (int* pChannels = &channels, pBits = &bits)
            {
                return Sound_GetFormat(sound, pType, pFormat, pChannels, pBits);
            }
        }

        public abstract Result Sound_GetNumSubSounds(IntPtr sound, int* numsubsounds);

        public Result Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds)
        {
            fixed (int* psubsoundCount = &numsubsounds)
            {
                return Sound_GetNumSubSounds(sound, psubsoundCount);
            }
        }

        public abstract Result Sound_GetNumTags(IntPtr sound, int* tagCount, int* updatedTagCount);

        public Result Sound_GetNumTags(IntPtr sound, out int tagCount, out int updatedTagCount)
        {
            fixed (int* pTagCount = &tagCount, pUpdatedTagCount = &updatedTagCount)
            {
                return Sound_GetNumTags(sound, pTagCount, pUpdatedTagCount);
            }
        }

        public abstract Result Sound_GetTag(IntPtr sound, string name, int index, Tag* tag);

        public Result Sound_GetTag(IntPtr sound, string name, int index, out Tag tag)
        {
            fixed (Tag* pTag = &tag)
            {
                return Sound_GetTag(sound, name, index, pTag);
            }
        }


        public abstract Result Sound_GetOpenState(IntPtr sound, OpenState* openstate, uint* percentbuffered, int* starving, int* diskbusy);

        public Result Sound_GetOpenState(IntPtr sound, out OpenState openstate, out uint percentBuffered, out bool starving, out bool diskbusy)
        {
            int _starving = 0, _diskbusy = 0;
            Result res;

            fixed (OpenState* pOpenState = &openstate)
            fixed (uint* pPercentBuffered = &percentBuffered)
            {
                res = Sound_GetOpenState(sound, pOpenState, pPercentBuffered, &_starving, &_diskbusy);
            }

            starving = _starving != 0;
            diskbusy = _diskbusy != 0;
            return res;
        }
        
        public abstract Result Sound_ReadData(IntPtr sound, void* buffer, uint length, uint* read);

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

        public abstract Result Sound_SeekData(IntPtr sound, uint pcm);
        
        public abstract Result Sound_SetSoundGroup(IntPtr sound, IntPtr soundGroup);
        
        public abstract Result Sound_GetSoundGroup(IntPtr sound, IntPtr* soundGroup);

        public Result Sound_GetSoundGroup(IntPtr sound, out IntPtr soundGroup)
        {
            fixed (IntPtr* pSoundGroup = &soundGroup)
            {
                return Sound_GetSoundGroup(sound, pSoundGroup);
            }
        }

        public abstract Result Sound_GetNumSyncPoints(IntPtr sound, int* syncPointCount);

        public Result Sound_GetNumSyncPoints(IntPtr sound, out int syncPointCount)
        {
            fixed (int* pSyncPointCount = &syncPointCount)
            {
                return Sound_GetNumSyncPoints(sound, pSyncPointCount);
            }
        }

        public abstract Result Sound_GetSyncPoint(IntPtr sound, int index, IntPtr* point);

        public Result Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point)
        {
            fixed (IntPtr* pPoint = &point)
            {
                return Sound_GetSyncPoint(sound, index, pPoint);
            }
        }

        public abstract Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, byte* name, int namelen, uint* offset, TimeUnit offsetType);

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

        public abstract Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsetType, byte* name, IntPtr* point);

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

        public abstract Result Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);
        
        public abstract Result Sound_SetMode(IntPtr sound, Mode mode);
        
        public abstract Result Sound_GetMode(IntPtr sound, Mode* mode);

        public Result Sound_GetMode(IntPtr sound, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Sound_GetMode(sound, pMode);
            }
        }
        
        public abstract Result Sound_SetLoopCount(IntPtr sound, int loopcount);
        
        public abstract Result Sound_GetLoopCount(IntPtr sound, int* loopcount);

        public Result Sound_GetLoopCount(IntPtr sound, out int loopCount)
        {
            fixed (int* pLoopCount = &loopCount)
            {
                return Sound_GetLoopCount(sound, pLoopCount);
            }
        }
        
        public abstract Result Sound_SetLoopPoints(IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        
        public abstract Result Sound_GetLoopPoints(IntPtr sound, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Sound_GetLoopPoints(IntPtr sound, out uint loopStart, TimeUnit loopStartType, out uint loopEnd, TimeUnit loopEndType)
        {
            fixed (uint* pLoopStart = &loopStart, pLoopEnd = &loopEnd)
            {
                return Sound_GetLoopPoints(sound, pLoopStart, loopStartType, pLoopEnd, loopEndType);
            }
        }

        public abstract Result Sound_GetMusicNumChannels(IntPtr sound, int* numchannels);

        public Result Sound_GetMusicNumChannels(IntPtr sound, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return Sound_GetMusicNumChannels(sound, pChannelCount);
            }
        }

        public abstract Result Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);
        
        public abstract Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, float* volume);

        public Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return Sound_GetMusicChannelVolume(sound, channel, pVolume);
            }
        }

        public abstract Result Sound_SetMusicSpeed(IntPtr sound, float speed);
        
        public abstract Result Sound_GetMusicSpeed(IntPtr sound, float* speed);

        public Result Sound_GetMusicSpeed(IntPtr sound, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return Sound_GetMusicSpeed(sound, pSpeed);
            }
        }
        
        [Obsolete]
        public abstract Result Sound_SetUserData(IntPtr sound, IntPtr userData);

        [Obsolete]
        public abstract Result Sound_GetUserData(IntPtr sound, IntPtr* userData);

        [Obsolete]
        public Result Sound_GetUserData(IntPtr sound, out IntPtr userData)
        {
            fixed (IntPtr* pUserData = &userData)
            {
                return Sound_GetUserData(sound, pUserData);
            }
        }

        public abstract Result SoundGroup_Release(IntPtr soundGroup);
       
        public abstract Result SoundGroup_GetSystemObject(IntPtr soundGroup, IntPtr* system);

        public Result SoundGroup_GetSystemObject(IntPtr soundGroup, out IntPtr system)
        {
            fixed (IntPtr* pSystem = &system)
            {
                return SoundGroup_GetSystemObject(soundGroup, pSystem);
            }
        }
        
        public abstract Result SoundGroup_SetMaxAudible(IntPtr soundGroup, int maxaudible);
        
        public abstract Result SoundGroup_GetMaxAudible(IntPtr soundGroup, int* maxaudible);

        public Result SoundGroup_GetMaxAudible(IntPtr soundGroup, out int maxAudible)
        {
            fixed (int* pMaxAudible = &maxAudible)
            {
                return SoundGroup_GetMaxAudible(soundGroup, pMaxAudible);
            }
        }
        
        public abstract Result SoundGroup_SetMaxAudibleBehavior(IntPtr soundGroup, SoundGroupBehavior behavior);
        
        public abstract Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundGroup, SoundGroupBehavior* behavior);

        public Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundGroup, out SoundGroupBehavior behavior)
        {
            fixed (SoundGroupBehavior* pBehavior = &behavior)
            {
                return SoundGroup_GetMaxAudibleBehavior(soundGroup, pBehavior);
            }
        }
        
        public abstract Result SoundGroup_SetMuteFadeSpeed(IntPtr soundGroup, float speed);
        
        public abstract Result SoundGroup_GetMuteFadeSpeed(IntPtr soundGroup, float* speed);

        public Result SoundGroup_GetMuteFadeSpeed(IntPtr soundGroup, out float speed)
        {
            fixed (float* pSpeed = &speed)
            {
                return SoundGroup_GetMuteFadeSpeed(soundGroup, pSpeed);
            }
        }
        
        public abstract Result SoundGroup_SetVolume(IntPtr soundGroup, float volume);
        
        public abstract Result SoundGroup_GetVolume(IntPtr soundGroup, float* volume);

        public Result SoundGroup_GetVolume(IntPtr soundGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return SoundGroup_GetVolume(soundGroup, pVolume);
            }
        }

        public abstract Result SoundGroup_Stop(IntPtr soundGroup);
        
        public abstract Result SoundGroup_GetName(IntPtr soundGroup, byte* name, int namelen);
        
        public abstract Result SoundGroup_GetNumSounds(IntPtr soundGroup, int* numsounds);

        public Result SoundGroup_GetNumSounds(IntPtr soundGroup, out int soundCount)
        {
            fixed (int* pSoundCount = &soundCount)
            {
                return SoundGroup_GetNumSounds(soundGroup, pSoundCount);
            }
        }

        public abstract Result SoundGroup_GetSound(IntPtr soundGroup, int index, IntPtr* sound);

        public Result SoundGroup_GetSound(IntPtr soundGroup, int index, out IntPtr sound)
        {
            fixed (IntPtr* pSound = &sound)
            {
                return SoundGroup_GetSound(soundGroup, index, pSound);
            }
        }

        public abstract Result SoundGroup_GetNumPlaying(IntPtr soundGroup, int* soundsPlaying);

        public Result SoundGroup_GetNumPlaying(IntPtr soundGroup, out int soundsPlaying)
        {
            fixed (int* pSoundsPlaying = &soundsPlaying)
            {
                return SoundGroup_GetNumPlaying(soundGroup, pSoundsPlaying);
            }
        }

        [Obsolete]
        public abstract Result SoundGroup_SetUserData(IntPtr soundGroup, IntPtr userdata);

        [Obsolete]
        public abstract Result SoundGroup_GetUserData(IntPtr soundGroup, IntPtr* userdata);

        [Obsolete]
        public Result SoundGroup_GetUserData(IntPtr soundGroup, out IntPtr userdata)
        {
            fixed (IntPtr* pUserData = &userdata)
            {
                return SoundGroup_GetUserData(soundGroup, pUserData);
            }
        }

        public abstract Result Channel_SetFrequency(IntPtr channel, float frequency);
        
        public abstract Result Channel_GetFrequency(IntPtr channel, float* frequency);

        public Result Channel_GetFrequency(IntPtr channel, out float frequency)
        {
            fixed (float* pFrequency = &frequency)
            {
                return Channel_GetFrequency(channel, pFrequency);
            }
        }

        public abstract Result Channel_SetPriority(IntPtr channel, int priority);
        
        public abstract Result Channel_GetPriority(IntPtr channel, int* priority);

        public Result Channel_GetPriority(IntPtr channel, out int priority)
        {
            fixed (int* pPriority = &priority)
            {
                return Channel_GetPriority(channel, pPriority);
            }
        }
        
        public abstract Result Channel_SetChannelGroup(IntPtr channel, IntPtr channelGroup);
        
        public abstract Result Channel_GetChannelGroup(IntPtr channel, IntPtr* channelGroup);

        public Result Channel_GetChannelGroup(IntPtr channel, out IntPtr channelGroup)
        {
            fixed (IntPtr* pChannelGroup = &channelGroup)
            {
                return Channel_GetChannelGroup(channel, pChannelGroup);
            }
        }
        
        public abstract Result Channel_IsVirtual(IntPtr channel, out bool isvirtual);

        public abstract Result Channel_GetCurrentSound(IntPtr channel, IntPtr* sound);

        public Result Channel_GetCurrentSound(IntPtr channel, out IntPtr sound)
        {
            fixed (IntPtr* pSound = &sound)
            {
                return Channel_GetCurrentSound(channel, pSound);
            }
        }

        public abstract Result Channel_GetIndex(IntPtr channel, int* index);

        public Result Channel_GetIndex(IntPtr channel, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return Channel_GetIndex(channel, pIndex);
            }
        }

        public abstract Result Channel_SetPosition(IntPtr channel, uint position, TimeUnit posType);
        
        public abstract Result Channel_GetPosition(IntPtr channel, uint* position, TimeUnit posType);

        public Result Channel_GetPosition(IntPtr channel, out uint position, TimeUnit posType)
        {
            fixed (uint* pPos = &position)
            {
                return Channel_GetPosition(channel, pPos, posType);
            }
        }
        
        public abstract Result Channel_SetMode(IntPtr channel, Mode mode);
        
        public abstract Result Channel_GetMode(IntPtr channel, Mode* mode);

        public Result Channel_GetMode(IntPtr channel, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return Channel_GetMode(channel, pMode);
            }
        }
        
        public abstract Result Channel_SetLoopCount(IntPtr channel, int loopcount);
        
        public abstract Result Channel_GetLoopCount(IntPtr channel, int* loopcount);

        public Result Channel_GetLoopCount(IntPtr channel, out int loopcount)
        {
            fixed (int* pLoopCount = &loopcount)
            {
                return Channel_GetLoopCount(channel, pLoopCount);
            }
        }

        public abstract Result Channel_SetLoopPoints(IntPtr channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        
        public abstract Result Channel_GetLoopPoints(IntPtr channel, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);

        public Result Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            fixed (uint* pLoopStart = &loopstart, pLoopEnd = &loopend)
            {
                return Channel_GetLoopPoints(channel, pLoopStart, loopstarttype, pLoopEnd, loopendtype);
            }
        }

        [Obsolete]
        public abstract Result Channel_SetUserData(IntPtr channel, IntPtr userdata);

        [Obsolete]
        public abstract Result Channel_GetUserData(IntPtr channel, IntPtr* userdata);

        [Obsolete]
        public Result Channel_GetUserData(IntPtr channel, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return Channel_GetUserData(channel, pUserdata);
            }
        }

        public abstract Result ChannelGroup_Stop(IntPtr channelGroup);

        public abstract Result ChannelGroup_SetPaused(IntPtr channelGroup, bool paused);
        
        public abstract Result ChannelGroup_GetPaused(IntPtr channelGroup, out bool paused);

        public abstract Result ChannelGroup_GetVolume(IntPtr channelGroup, float* volume);

        public Result ChannelGroup_GetVolume(IntPtr channelGroup, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return ChannelGroup_GetVolume(channelGroup, pVolume);
            }
        }
        
        public abstract Result ChannelGroup_SetVolumeRamp(IntPtr channelGroup, bool ramp);
        
        public abstract Result ChannelGroup_GetVolumeRamp(IntPtr channelGroup, out bool ramp);

        public abstract Result ChannelGroup_GetAudibility(IntPtr channelGroup, float* audibility);

        public Result ChannelGroup_GetAudibility(IntPtr channelGroup, out float audibility)
        {
            fixed (float* pAudibility = &audibility)
            {
                return ChannelGroup_GetAudibility(channelGroup, pAudibility);
            }
        }

        public abstract Result ChannelGroup_SetPitch(IntPtr channelGroup, float pitch);

        public abstract Result ChannelGroup_GetPitch(IntPtr channelGroup, float* pitch);

        public Result ChannelGroup_GetPitch(IntPtr channelGroup, out float pitch)
        {
            fixed (float* pPitch = &pitch)
            {
                return ChannelGroup_GetPitch(channelGroup, pPitch);
            }
        }

        public abstract Result ChannelGroup_SetMute(IntPtr channelGroup, bool mute);
        
        public abstract Result ChannelGroup_GetMute(IntPtr channelGroup, out bool mute);

        public abstract Result ChannelGroup_SetReverbProperties(IntPtr channelGroup, int instance, float wet);
        
        public abstract Result ChannelGroup_GetReverbProperties(IntPtr channelGroup, int instance, float* wet);

        public Result ChannelGroup_GetReverbProperties(IntPtr channelGroup, int instance, out float wet)
        {
            fixed (float* pWet = &wet)
            {
                return ChannelGroup_GetReverbProperties(channelGroup, instance, pWet);
            }
        }

        public abstract Result ChannelGroup_SetLowPassGain(IntPtr channelGroup, float gain);
        
        public abstract Result ChannelGroup_GetLowPassGain(IntPtr channelGroup, float* gain);

        public Result ChannelGroup_GetLowPassGain(IntPtr channelGroup, out float gain)
        {
            fixed (float* pGain = &gain)
            {
                return ChannelGroup_GetLowPassGain(channelGroup, pGain);
            }
        }

        public abstract Result ChannelGroup_SetMode(IntPtr channelGroup, Mode mode);
        
        public abstract Result ChannelGroup_GetMode(IntPtr channelGroup, Mode* mode);

        public Result ChannelGroup_GetMode(IntPtr channelGroup, out Mode mode)
        {
            fixed (Mode* pMode = &mode)
            {
                return ChannelGroup_GetMode(channelGroup, pMode);
            }
        }

        public abstract Result ChannelGroup_SetCallback(IntPtr channelGroup, ChannelCallback callback);
        
        public abstract Result ChannelGroup_IsPlaying(IntPtr channelGroup, out bool isplaying);
        
        public abstract Result ChannelGroup_SetPan(IntPtr channelGroup, float pan);
        
        public abstract Result ChannelGroup_SetMixLevelsOutput(IntPtr channelGroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        
        public abstract Result ChannelGroup_SetMixLevelsInput(IntPtr channelGroup, float* levels, int levelCount);

        public Result ChannelGroup_SetMixLevelsInput(IntPtr channelGroup, Span<float> levels)
        {
            fixed (float* pLevels = levels)
            {
                return ChannelGroup_SetMixLevelsInput(channelGroup, pLevels, levels.Length);
            }
        }

        public abstract Result ChannelGroup_SetMixMatrix(IntPtr channelGroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);
        
        public abstract Result ChannelGroup_GetMixMatrix(IntPtr channelGroup, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);
        
        public abstract Result ChannelGroup_GetDSPClock(IntPtr channelGroup, ulong* dspclock, ulong* parentclock);

        public Result ChannelGroup_GetDSPClock(IntPtr channelGroup, out ulong dspclock, out ulong parentclock)
        {
            fixed (ulong* pDspClock = &dspclock, pParentClock = &parentclock)
            {
                return ChannelGroup_GetDSPClock(channelGroup, pDspClock, pParentClock);
            }
        }

        public abstract Result ChannelGroup_SetDelay(IntPtr channelGroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);
        
        public abstract Result ChannelGroup_GetDelay(IntPtr channelGroup, ulong* dspclock_start, ulong* dspclock_end, int* stopchannels);

        public Result ChannelGroup_GetDelay(IntPtr channelGroup, ulong* dspclock_start, ulong* dspclock_end, out bool stopchannels)
        {
            int stopChannelsTemp = 0;
            var res = ChannelGroup_GetDelay(channelGroup, dspclock_start, dspclock_end, &stopChannelsTemp);
            stopchannels = stopChannelsTemp != 0;
            return res;
        }

        public Result ChannelGroup_GetDelay(IntPtr channelGroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            Result res;
            int stopChannelsTemp = 0;
            fixed (ulong* pDspclock_start = &dspclock_start, pDspclock_end = &dspclock_end)
            {
                res = ChannelGroup_GetDelay(channelGroup, pDspclock_start, pDspclock_end, &stopChannelsTemp);
            }
            stopchannels = stopChannelsTemp != 0;
            return res;
        }

        public abstract Result ChannelGroup_AddFadePoint(IntPtr channelGroup, ulong dspclock, float volume);
        
        public abstract Result ChannelGroup_SetFadePointRamp(IntPtr channelGroup, ulong dspclock, float volume);
        
        public abstract Result ChannelGroup_RemoveFadePoints(IntPtr channelGroup, ulong dspclock_start, ulong dspclock_end);
        
        public abstract Result ChannelGroup_GetFadePoints(IntPtr channelGroup, uint* numpoints, ulong* point_dspclock, float* point_volume);

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

        public abstract Result ChannelGroup_Set3DAttributes(IntPtr channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Set3DAttributes(IntPtr channelGroup, in Vector3 pos, in Vector3 vel, in Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltpanpos = &alt_pan_pos)
            {
                return ChannelGroup_Set3DAttributes(channelGroup, pPos, pVel, pAltpanpos);
            }
        }

        public abstract Result ChannelGroup_Get3DAttributes(IntPtr channelGroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Get3DAttributes(IntPtr channelGroup, out Vector3 pos, out Vector3 vel, out Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltPanPos = &alt_pan_pos)
            {
                return ChannelGroup_Get3DAttributes(channelGroup, pPos, pVel, pAltPanPos);
            }
        }

        public abstract Result ChannelGroup_Set3DMinMaxDistance(IntPtr channelGroup, float mindistance, float maxdistance);
        
        public abstract Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelGroup, float* mindistance, float* maxdistance);

        public Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelGroup, out float minDistance, out float maxDistance)
        {
            fixed (float* pMinDistance = &minDistance, pMaxdistance = &maxDistance)
            {
                return ChannelGroup_Get3DMinMaxDistance(channelGroup, pMinDistance, pMaxdistance);
            }
        }

        public abstract Result ChannelGroup_Set3DConeSettings(IntPtr channelGroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        
        public abstract Result ChannelGroup_Get3DConeSettings(IntPtr channelGroup, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result ChannelGroup_Get3DConeSettings(IntPtr channelGroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideAngle = &insideconeangle, pOutsideAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return ChannelGroup_Get3DConeSettings(channelGroup, pInsideAngle, pOutsideAngle, pOutsideVolume);
            }
        }

        public abstract Result ChannelGroup_Set3DConeOrientation(IntPtr channelGroup, Vector3* orientation);

        public Result ChannelGroup_Set3DConeOrientation(IntPtr channelGroup, in Vector3 orientation)
        {
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Set3DConeOrientation(channelGroup, pOrientation);
            }
        }

        public abstract Result ChannelGroup_Get3DConeOrientation(IntPtr channelGroup, Vector3* orientation);

        public Result ChannelGroup_Get3DConeOrientation(IntPtr channelGroup, out Vector3 orientation)
        {
            orientation = default;
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Get3DConeOrientation(channelGroup, pOrientation);
            }
        }

        public abstract Result ChannelGroup_Set3DCustomRolloff(IntPtr channelGroup, IntPtr points, int numpoints);

        public abstract Result ChannelGroup_Get3DCustomRolloff(IntPtr channelGroup, IntPtr* points, int* numpoints);

        public Result ChannelGroup_Get3DCustomRolloff(IntPtr channelGroup, out IntPtr points, out int pointCount)
        {
            fixed (IntPtr* pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return ChannelGroup_Get3DCustomRolloff(channelGroup, pPoints, pPointCount);
            }
        }

        public abstract Result ChannelGroup_Set3DOcclusion(IntPtr channelGroup, float directocclusion, float reverbocclusion);
        public abstract Result ChannelGroup_Get3DOcclusion(IntPtr channelGroup, float* directocclusion, float* reverbocclusion);

        public Result ChannelGroup_Get3DOcclusion(IntPtr channelGroup, out float directOcclusion, out float reverbOcclusion)
        {
            fixed (float* pDirectOcclusion = &directOcclusion, pReverbOcclusion = &reverbOcclusion)
            {
                return ChannelGroup_Get3DOcclusion(channelGroup, pDirectOcclusion, pReverbOcclusion);
            }
        }

        public abstract Result ChannelGroup_Set3DSpread(IntPtr channelGroup, float angle);

        public abstract Result ChannelGroup_Get3DSpread(IntPtr channelGroup, float* angle);

        public Result ChannelGroup_Get3DSpread(IntPtr channelGroup, out float angle)
        {
            fixed (float* pAngle = &angle)
            {
                return ChannelGroup_Get3DSpread(channelGroup, pAngle);
            }
        }

        public abstract Result ChannelGroup_Set3DLevel(IntPtr channelGroup, float level);

        public abstract Result ChannelGroup_Get3DLevel(IntPtr channelGroup, float* level);

        public Result ChannelGroup_Get3DLevel(IntPtr channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DLevel(channelGroup, pLevel);
            }
        }

        public abstract Result ChannelGroup_Set3DDopplerLevel(IntPtr channelGroup, float level);

        public abstract Result ChannelGroup_Get3DDopplerLevel(IntPtr channelGroup, float* level);

        public Result ChannelGroup_Get3DDopplerLevel(IntPtr channelGroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DDopplerLevel(channelGroup, pLevel);
            }
        }

        public abstract Result ChannelGroup_Set3DDistanceFilter(IntPtr channelGroup, bool custom, float customLevel, float centerFreq);

        public abstract Result ChannelGroup_Get3DDistanceFilter(IntPtr channelGroup, int* custom, float* customLevel, float* centerFreq);

        public Result ChannelGroup_Get3DDistanceFilter(IntPtr channelGroup, out bool custom, out float customLevel, out float centerFreq)
        {
            fixed (float* pCustomlevel = &customLevel, pCenterFreq = &centerFreq)
            {
                int _val = 0;
                var res = ChannelGroup_Get3DDistanceFilter(channelGroup, &_val, pCustomlevel, pCenterFreq);
                custom = _val != 0;
                return res;
            }
        }

        public abstract Result ChannelGroup_GetSystemObject(IntPtr channelGroup, IntPtr* system);

        public Result ChannelGroup_GetSystemObject(IntPtr channelGroup, out IntPtr system)
        {
            fixed (IntPtr* pSystem = &system)
            {
                return ChannelGroup_GetSystemObject(channelGroup, pSystem);
            }
        }

        public abstract Result ChannelGroup_SetVolume(IntPtr channelGroup, float volume);
        
        public abstract Result ChannelGroup_GetDSP(IntPtr channelGroup, int index, IntPtr* dsp);

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

        public abstract Result ChannelGroup_AddDSP(IntPtr channelGroup, int index, IntPtr dsp);

        public Result ChannelGroup_AddDSP(IntPtr channelGroup, ChannelControlDSPIndex index, IntPtr dsp)
        {
            return ChannelGroup_AddDSP(channelGroup, (int)index, dsp);
        }

        public abstract Result ChannelGroup_RemoveDSP(IntPtr channelGroup, IntPtr dsp);
        
        public abstract Result ChannelGroup_GetNumDSPs(IntPtr channelGroup, int* dspCount);

        public Result ChannelGroup_GetNumDSPs(IntPtr channelGroup, out int dspCount)
        {
            fixed (int* pDspCount = &dspCount)
            {
                return ChannelGroup_GetNumDSPs(channelGroup, pDspCount);
            }
        }

        public abstract Result ChannelGroup_SetDSPIndex(IntPtr channelGroup, IntPtr dsp, int index);
        
        public abstract Result ChannelGroup_GetDSPIndex(IntPtr channelGroup, IntPtr dsp, int* index);

        public Result ChannelGroup_GetDSPIndex(IntPtr channelGroup, IntPtr dsp, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return ChannelGroup_GetDSPIndex(channelGroup, dsp, pIndex);
            }
        }

        [Obsolete]
        public abstract Result ChannelGroup_SetUserData(IntPtr channelGroup, IntPtr userdata);

        [Obsolete]
        public abstract Result ChannelGroup_GetUserData(IntPtr channelGroup, IntPtr* userdata);

        [Obsolete]
        public Result ChannelGroup_GetUserData(IntPtr channelGroup, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return ChannelGroup_GetUserData(channelGroup, pUserdata);
            }
        }

        public abstract Result ChannelGroup_Release(IntPtr Handle);

        public abstract Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, int propagatedspclock, IntPtr* connection);

        public Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, bool propagatedspclock, IntPtr* connection)
        {
            return ChannelGroup_AddGroup(channelGroup, group, propagatedspclock ? 1 : 0, connection);
        }

        public Result ChannelGroup_AddGroup(IntPtr channelGroup, IntPtr group, bool propagatedspclock, out IntPtr connection)
        {
            fixed (IntPtr* connectionRes = &connection)
                return ChannelGroup_AddGroup(channelGroup, group, propagatedspclock ? 1 : 0, connectionRes);
        }

        public abstract Result ChannelGroup_GetNumGroups(IntPtr channelGroup, int* groupCount);

        public Result ChannelGroup_GetNumGroups(IntPtr channelGroup, out int groupCount)
        {
            fixed (int* pGroupCount = &groupCount)
            {
                return ChannelGroup_GetNumGroups(channelGroup, pGroupCount);
            }
        }

        public abstract Result ChannelGroup_GetGroup(IntPtr channelGroup, int index, IntPtr* group);

        public Result ChannelGroup_GetGroup(IntPtr channelGroup, int index, out IntPtr group)
        {
            fixed (IntPtr* pGroup = &group)
            {
                return ChannelGroup_GetGroup(channelGroup, index, pGroup);
            }
        }

        public abstract Result ChannelGroup_GetParentGroup(IntPtr channelGroup, IntPtr* group);

        public Result ChannelGroup_GetParentGroup(IntPtr channelGroup, out IntPtr group)
        {
            fixed (IntPtr* pGroup = &group)
            {
                return ChannelGroup_GetParentGroup(channelGroup, pGroup);
            }
        }

        public abstract Result ChannelGroup_GetName(IntPtr channelGroup, byte* name, int namelen);

        public abstract Result ChannelGroup_GetNumChannels(IntPtr channelGroup, int* channelCount);
        
        public Result ChannelGroup_GetNumChannels(IntPtr channelGroup, out int channelCount)
        {
            fixed (int* pChannelCount = &channelCount)
            {
                return ChannelGroup_GetNumChannels(channelGroup, pChannelCount);
            }
        }

        public abstract Result ChannelGroup_GetChannel(IntPtr channelGroup, int index, IntPtr* channel);

        public Result ChannelGroup_GetChannel(IntPtr channelGroup, int index, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return ChannelGroup_GetChannel(channelGroup, index, pChannel);
            }
        }

        public abstract Result DSP_Release(IntPtr dsp);
        
        public abstract Result DSP_GetSystemObject(IntPtr dsp, IntPtr* system);

        public Result DSP_GetSystemObject(IntPtr dsp, out IntPtr system)
        {
            fixed (IntPtr* pSystem = &system)
            {
                return DSP_GetSystemObject(dsp, pSystem);
            }
        }

        public abstract Result DSP_AddInput(IntPtr dsp, IntPtr target, IntPtr* connection, DSPConnectionType type);

        public Result DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPConnectionType type)
        {
            fixed (IntPtr* pConnection = &connection)
            {
                return DSP_AddInput(dsp, target, pConnection, type);
            }
        }

        public abstract Result DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);
        
        public abstract Result DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);
        
        public abstract Result DSP_GetNumInputs(IntPtr dsp, int* inputCount);

        public Result DSP_GetNumInputs(IntPtr dsp, out int inputCount)
        {
            fixed (int* pInputCount = &inputCount)
            {
                return DSP_GetNumInputs(dsp, pInputCount);
            }
        }

        public abstract Result DSP_GetNumOutputs(IntPtr dsp, int* outputCount);

        public Result DSP_GetNumOutputs(IntPtr dsp, out int outputCount)
        {
            fixed (int* pOutputCount = &outputCount)
            {
                return DSP_GetNumOutputs(dsp, pOutputCount);
            }
        }

        public abstract Result DSP_GetInput(IntPtr dsp, int index, IntPtr* input, IntPtr* inputconnection);

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

        public abstract Result DSP_GetOutput(IntPtr dsp, int index, IntPtr* output, IntPtr* outputconnection);

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


        public abstract Result DSP_SetActive(IntPtr dsp, bool active);

        public abstract Result DSP_GetActive(IntPtr dsp, out bool active);

        public abstract Result DSP_SetBypass(IntPtr dsp, bool bypass);

        public abstract Result DSP_GetBypass(IntPtr dsp, out bool bypass);

        public abstract Result DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

        public abstract Result DSP_GetWetDryMix(IntPtr dsp, float* prewet, float* postwet, float* dry);

        public Result DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry)
        {
            fixed (float* pPrewet = &prewet, pPostwet = &postwet, pDry = &dry)
            {
                return DSP_GetWetDryMix(dsp, pPrewet, pPostwet, pDry);
            }
        }

        public abstract Result DSP_SetChannelFormat(IntPtr dsp, ChannelMask channelmask, int channelCount, SpeakerMode source_speakermode);

        public abstract Result DSP_GetChannelFormat(IntPtr dsp, ChannelMask* channelmask, int* channelCount, SpeakerMode* source_speakermode);

        public Result DSP_GetChannelFormat(IntPtr dsp, out ChannelMask channelmask, out int channelCount, out SpeakerMode source_speakermode)
        {
            fixed (ChannelMask* pChannelMask = &channelmask)
            fixed (int* pChannelCount = &channelCount)
            fixed (SpeakerMode* pSource = &source_speakermode)
            {
                return DSP_GetChannelFormat(dsp, pChannelMask, pChannelCount, pSource);
            }
        }

        public abstract Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, ChannelMask* outmask, int* outchannels, SpeakerMode* outspeakermode);
        
        public Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            fixed (ChannelMask* pOutMask = &outmask)
            fixed (int* pOutChannels = &outchannels)
            fixed (SpeakerMode* pOutSpeakerMode = &outspeakermode)
            {
                return DSP_GetOutputChannelFormat(dsp, inmask, inchannels, inspeakermode, pOutMask, pOutChannels, pOutSpeakerMode);
            }
        }
        
        public abstract Result DSP_Reset(IntPtr dsp);
        
        public abstract Result DSP_SetParameterFloat(IntPtr dsp, int index, float value);
        
        public abstract Result DSP_SetParameterBool(IntPtr dsp, int index, int value);

        public Result DSP_SetParameterBool(IntPtr dsp, int index, bool value)
        {
            return DSP_SetParameterBool(dsp, index, value ? 1 : 0);
        }

        public abstract Result DSP_SetParameterInt(IntPtr dsp, int index, int value);
        
        public abstract Result DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);
        
        public abstract Result DSP_GetParameterFloat(IntPtr dsp, int index, float* value, byte* valuestr, int valuestrlen);

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

        public abstract Result DSP_GetParameterBool(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);

        public Result DSP_GetParameterBool(IntPtr dsp, int index, out bool value, byte* valuestr, int valuestrlen)
        {
            int _val;
            var res = DSP_GetParameterBool(dsp, index, &_val, valuestr, valuestrlen);
            value = _val != 0;
            return res;
        }

        public Result DSP_GetParameterBool(IntPtr dsp, int index, out bool value)
        {
            return DSP_GetParameterBool(dsp, index, out value, null, 0);
        }

        public abstract Result DSP_GetParameterint(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);

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

        public abstract Result DSP_GetParameterData(IntPtr dsp, int index, IntPtr* data, uint* length, byte* valuestr, int valuestrlen);

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

        public abstract Result DSP_GetNumParameters(IntPtr dsp, int* paramCount);

        public Result DSP_GetNumParameters(IntPtr dsp, out int paramCount)
        {
            fixed (int* pParamCount = &paramCount)
            {
                return DSP_GetNumParameters(dsp, pParamCount);
            }
        }

        public abstract Result DSP_GetParameterInfo(IntPtr dsp, int index, Dsp.Interop.ParameterDescriptionStruct** desc);
        
        public abstract Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, int* index);

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

        public abstract Result DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);
        
        public abstract Result DSP_GetInfo(IntPtr dsp, byte* name, FmodVersion* version, int* channels, int* configwidth, int* configheight);

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

        public abstract Result DSP_GetType(IntPtr dsp, DSPType* type);

        public Result DSP_GetType(IntPtr dsp, out DSPType type)
        {
            fixed (DSPType* pType = &type)
            {
                return DSP_GetType(dsp, pType);
            }
        }

        public abstract Result DSP_GetIdle(IntPtr dsp, out bool idle);
        
        [Obsolete("DSP UserData pointer is used for the Managed Object's GCHandle, setting it will break managed library code. Refer to the Custom DSP Example for more information.")]
        public abstract Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);
        
        [Obsolete]
        public abstract Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);

        [Obsolete]
        public Result DSP_GetUserData(IntPtr dsp, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return DSP_GetUserData(dsp, pUserdata);
            }
        }

        public abstract Result DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);
        
        public abstract Result DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);

        public abstract Result DSP_GetMeteringInfo(IntPtr dsp, DSPMeteringInfo* inputInfo, DSPMeteringInfo* outputInfo);

        public Result DSP_GetMeteringInfo(IntPtr dsp, out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo)
        {
            fixed (DSPMeteringInfo* pInputInfo = &inputInfo, pOutputInfo = &outputInfo)
            {
                return DSP_GetMeteringInfo(dsp, pInputInfo, pOutputInfo);
            }
        }

        public abstract Result DSPConnection_GetInput(IntPtr dspconnection, IntPtr* input);

        public Result DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input)
        {
            fixed (IntPtr* pInput = &input)
            {
                return DSPConnection_GetInput(dspconnection, pInput);
            }
        }

        public abstract Result DSPConnection_GetOutput(IntPtr dspconnection, IntPtr* output);

        public Result DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output)
        {
            fixed (IntPtr* pOutput = &output)
            {
                return DSPConnection_GetOutput(dspconnection, pOutput);
            }
        }

        public abstract Result DSPConnection_SetMix(IntPtr dspconnection, float volume);
        
        public abstract Result DSPConnection_GetMix(IntPtr dspconnection, float* volume);

        public Result DSPConnection_GetMix(IntPtr dspconnection, out float volume)
        {
            fixed (float* pVolume = &volume)
            {
                return DSPConnection_GetMix(dspconnection, pVolume);
            }
        }

        public abstract Result DSPConnection_SetMixMatrix(IntPtr dspconnection, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        public abstract Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);

        public Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            fixed (int* pOutChannels = &outchannels, pInChannels = &inchannels)
            {
                return DSPConnection_GetMixMatrix(dspconnection, matrix, pOutChannels, pInChannels, inchannel_hop);
            }
        }

        public abstract Result DSPConnection_GetType(IntPtr dspconnection, DSPConnectionType* type);

        public Result DSPConnection_GetType(IntPtr dspconnection, out DSPConnectionType type)
        {
            fixed (DSPConnectionType* pType = &type)
            {
                return DSPConnection_GetType(dspconnection, pType);
            }
        }

        [Obsolete]
        public abstract Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

        [Obsolete]
        public abstract Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);

        [Obsolete]
        public Result DSPConnection_GetUserData(IntPtr dspconnection, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return DSPConnection_GetUserData(dspconnection, pUserdata);
            }
        }

        public abstract Result Geometry_Release(IntPtr geometry);
        
        public abstract Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector3* vertices, int* polygonindex);

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

        public abstract Result Geometry_GetNumPolygons(IntPtr geometry, int* polygonCount);

        public Result Geometry_GetNumPolygons(IntPtr geometry, out int polygonCount)
        {
            fixed (int* pPolygonCount = &polygonCount)
            {
                return Geometry_GetNumPolygons(geometry, pPolygonCount);
            }
        }

        public abstract Result Geometry_GetMaxPolygons(IntPtr geometry, int* maxpolygons, int* maxvertices);

        public Result Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices)
        {
            fixed (int* pMaxPolygons = &maxpolygons, pMaxVertices = &maxvertices)
            {
                return Geometry_GetMaxPolygons(geometry, pMaxPolygons, pMaxVertices);
            }
        }

        public abstract Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, int* vertexCount);

        public Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int vertexCount)
        {
            fixed (int* pVertexCount = &vertexCount)
            {
                return Geometry_GetPolygonNumVertices(geometry, index, pVertexCount);
            }
        }

        public abstract Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, in Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_SetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        public abstract Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);

        public Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out Vector3 vertex)
        {
            fixed (Vector3* pVertex = &vertex)
            {
                return Geometry_GetPolygonVertex(geometry, index, vertexindex, pVertex);
            }
        }

        public abstract Result Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);
        
        public abstract Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, int* doublesided);

        public Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, out bool doublesided)
        {
            int _dsVal = 0;
            var res = Geometry_GetPolygonAttributes(geometry, index, directocclusion, reverbocclusion, &_dsVal);
            doublesided = _dsVal != 0;
            return res;
        }

        public Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
        {
            fixed (float* pDirectOcclusion = &directocclusion, pReverbOcclusion = &reverbocclusion)
            {
                return Geometry_GetPolygonAttributes(geometry, index, pDirectOcclusion, pReverbOcclusion, out doublesided);
            }
        }

        public abstract Result Geometry_SetActive(IntPtr geometry, bool active);

        public abstract Result Geometry_GetActive(IntPtr geometry, out bool active);
        
        public abstract Result Geometry_SetRotation(IntPtr geometry, Vector3* forward, Vector3* up);

        public Result Geometry_SetRotation(IntPtr geometry, in Vector3 forward, in Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_SetRotation(geometry, pForward, pUp);
            }
        }

        public abstract Result Geometry_GetRotation(IntPtr geometry, Vector3* forward, Vector3* up);

        public Result Geometry_GetRotation(IntPtr geometry, out Vector3 forward, out Vector3 up)
        {
            fixed (Vector3* pForward = &forward, pUp = &up)
            {
                return Geometry_GetRotation(geometry, pForward, pUp);
            }
        }

        public abstract Result Geometry_SetPosition(IntPtr geometry, Vector3* position);

        public Result Geometry_SetPosition(IntPtr geometry, in Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_SetPosition(geometry, pPosition);
            }
        }

        public abstract Result Geometry_GetPosition(IntPtr geometry, Vector3* position);

        public Result Geometry_GetPosition(IntPtr geometry, out Vector3 position)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Geometry_GetPosition(geometry, pPosition);
            }
        }

        public abstract Result Geometry_SetScale(IntPtr geometry, Vector3* scale);

        public Result Geometry_SetScale(IntPtr geometry, in Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_SetScale(geometry, pScale);
            }
        }

        public abstract Result Geometry_GetScale(IntPtr geometry, Vector3* scale);

        public Result Geometry_GetScale(IntPtr geometry, out Vector3 scale)
        {
            fixed (Vector3* pScale = &scale)
            {
                return Geometry_GetScale(geometry, pScale);
            }
        }

        public abstract Result Geometry_Save(IntPtr geometry, void* data, int* datasize);

        [Obsolete]
        public abstract Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

        [Obsolete]
        public abstract Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);

        [Obsolete]
        public Result Geometry_GetUserData(IntPtr geometry, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return Geometry_GetUserData(geometry, pUserdata);
            }
        }

        public abstract Result Reverb3D_Release(IntPtr reverb3D);
        
        public abstract Result Reverb3D_Set3DAttributes(IntPtr reverb3D, Vector3* position, float minDistance, float maxDistance);

        public Result Reverb3D_Set3DAttributes(IntPtr reverb3D, in Vector3 position, float minDistance, float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            {
                return Reverb3D_Set3DAttributes(reverb3D, pPosition, minDistance, maxDistance);
            }
        }

        public abstract Result Reverb3D_Get3DAttributes(IntPtr reverb3D, Vector3* position, float* minDistance, float* maxDistance);

        public Result Reverb3D_Get3DAttributes(IntPtr reverb3D, out Vector3 position, out float minDistance, out float maxDistance)
        {
            fixed (Vector3* pPosition = &position)
            fixed (float* pMin = &minDistance, pMax = &maxDistance)
            {
                return Reverb3D_Get3DAttributes(reverb3D, pPosition, pMin, pMax);
            }
        }

        public abstract Result Reverb3D_SetProperties(IntPtr reverb3D, ReverbProperties* properties);

        public Result Reverb3D_SetProperties(IntPtr reverb3D, in ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_SetProperties(reverb3D, pProps);
            }
        }

        public abstract Result Reverb3D_GetProperties(IntPtr reverb3D, ReverbProperties* properties);

        public Result Reverb3D_GetProperties(IntPtr reverb3D, out ReverbProperties properties)
        {
            fixed (ReverbProperties* pProps = &properties)
            {
                return Reverb3D_GetProperties(reverb3D, pProps);
            }
        }

        public abstract Result Reverb3D_SetActive(IntPtr reverb3D, bool active);
        
        public abstract Result Reverb3D_GetActive(IntPtr reverb3D, out bool active);

        [Obsolete]
        public abstract Result Reverb3D_SetUserData(IntPtr reverb3D, IntPtr userdata);

        [Obsolete]
        public abstract Result Reverb3D_GetUserData(IntPtr reverb3D, IntPtr* userdata);

        [Obsolete]
        public Result Reverb3D_GetUserData(IntPtr reverb3D, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return Reverb3D_GetUserData(reverb3D, pUserdata);
            }
        }
    }
}
