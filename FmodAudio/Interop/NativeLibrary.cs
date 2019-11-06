using System;
using System.Numerics;
using System.Text;
using FmodAudio.Codec;
using FmodAudio.Dsp;

using AdvancedDLSupport;

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

        //public abstract Result System_RegisterOutput(IntPtr system, Output.OutputDescription.Structure* description, uint* handle);

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
        
        public abstract Result System_CreateChannelGroup(IntPtr system, byte* name, IntPtr* channelgroup);

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

        public abstract Result System_CreateSoundGroup(IntPtr system, byte* name, IntPtr* soundgroup);

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
        
        public abstract Result System_GetMasterChannelGroup(IntPtr system, IntPtr* channelgroup);

        public Result System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup)
        {
            fixed (IntPtr* pChannelGroup = &channelgroup)
            {
                return System_GetMasterChannelGroup(system, pChannelGroup);
            }
        }

        public abstract Result System_GetMasterSoundGroup(IntPtr system, IntPtr* soundgroup);

        public Result System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup)
        {
            fixed (IntPtr* pSoundGroup = &soundgroup)
            {
                return System_GetMasterSoundGroup(system, pSoundGroup);
            }
        }

        public abstract Result System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, int passThru);
        
        public abstract Result System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);
        
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

        public abstract Result System_SetUserData(IntPtr system, IntPtr userdata);
        
        public abstract Result System_GetUserData(IntPtr system, IntPtr* userdata);

        public Result System_GetUserData(IntPtr system, out IntPtr userdata)
        {
            fixed (IntPtr* pUserdata = &userdata)
            {
                return System_GetUserData(system, pUserdata);
            }
        }

        public abstract Result Sound_Release(IntPtr sound);
        
        public abstract Result Sound_Lock(IntPtr sound, uint offset, uint length, IntPtr* ptr1, IntPtr* ptr2, uint* len1, uint* len2);
        
        public abstract Result Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);
        
        public abstract Result Sound_SetDefaults(IntPtr sound, float frequency, int priority);
        
        public abstract Result Sound_GetDefaults(IntPtr sound, float* frequency, int* priority);
        
        public abstract Result Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);
        
        public abstract Result Sound_Get3DMinMaxDistance(IntPtr sound, float* min, float* max);
        
        public abstract Result Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        
        public abstract Result Sound_Get3DConeSettings(IntPtr sound, float* insideconeangle, float* outsideconeangle, float* outsidevolume);
        
        public abstract Result Sound_Set3DCustomRolloff(IntPtr sound, IntPtr points, int numpoints);
        
        public abstract Result Sound_Get3DCustomRolloff(IntPtr sound, IntPtr* points, int* numpoints);
        
        public abstract Result Sound_GetSubSound(IntPtr sound, int index, IntPtr* subsound);
        
        public abstract Result Sound_GetSubSoundParent(IntPtr sound, IntPtr* parentsound);
        public abstract Result Sound_GetName(IntPtr sound, byte* name, int namelen);
        public abstract Result Sound_GetLength(IntPtr sound, uint* length, TimeUnit lengthtype);
        public abstract Result Sound_GetFormat(IntPtr sound, SoundType* type, SoundFormat* format, int* channels, int* bits);
        public abstract Result Sound_GetNumSubSounds(IntPtr sound, int* numsubsounds);
        public abstract Result Sound_GetNumTags(IntPtr sound, int* numtags, int* numtagsupdated);
        public abstract Result Sound_GetTag(IntPtr sound, string name, int index, Tag* tag);
        public abstract Result Sound_GetOpenState(IntPtr sound, OpenState* openstate, uint* percentbuffered, int* starving, int* diskbusy);
        public abstract Result Sound_ReadData(IntPtr sound, void* buffer, uint length, uint* read);
        public abstract Result Sound_SeekData(IntPtr sound, uint pcm);
        public abstract Result Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);
        public abstract Result Sound_GetSoundGroup(IntPtr sound, IntPtr* soundgroup);
        public abstract Result Sound_GetNumSyncPoints(IntPtr sound, int* numsyncpoints);
        public abstract Result Sound_GetSyncPoint(IntPtr sound, int index, IntPtr* point);
        public abstract Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, byte* name, int namelen, uint* offset, TimeUnit offsettype);
        public abstract Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsettype, byte* name, IntPtr* point);
        public abstract Result Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);
        public abstract Result Sound_SetMode(IntPtr sound, Mode mode);
        public abstract Result Sound_GetMode(IntPtr sound, Mode* mode);
        public abstract Result Sound_SetLoopCount(IntPtr sound, int loopcount);
        public abstract Result Sound_GetLoopCount(IntPtr sound, int* loopcount);
        public abstract Result Sound_SetLoopPoints(IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        public abstract Result Sound_GetLoopPoints(IntPtr sound, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);
        public abstract Result Sound_GetMusicNumChannels(IntPtr sound, int* numchannels);
        public abstract Result Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);
        public abstract Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, float* volume);
        public abstract Result Sound_SetMusicSpeed(IntPtr sound, float speed);
        public abstract Result Sound_GetMusicSpeed(IntPtr sound, float* speed);
        public abstract Result Sound_SetUserData(IntPtr sound, IntPtr userdata);
        public abstract Result Sound_GetUserData(IntPtr sound, IntPtr* userdata);
        public abstract Result SoundGroup_Release(IntPtr soundgroup);
        public abstract Result SoundGroup_GetSystemObject(IntPtr soundgroup, IntPtr* system);
        public abstract Result SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);
        public abstract Result SoundGroup_GetMaxAudible(IntPtr soundgroup, int* maxaudible);
        public abstract Result SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior behavior);
        public abstract Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior* behavior);
        public abstract Result SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);
        public abstract Result SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, float* speed);
        public abstract Result SoundGroup_SetVolume(IntPtr soundgroup, float volume);
        public abstract Result SoundGroup_GetVolume(IntPtr soundgroup, float* volume);
        public abstract Result SoundGroup_Stop(IntPtr soundgroup);
        public abstract Result SoundGroup_GetName(IntPtr soundgroup, byte* name, int namelen);
        public abstract Result SoundGroup_GetNumSounds(IntPtr soundgroup, int* numsounds);
        public abstract Result SoundGroup_GetSound(IntPtr soundgroup, int index, IntPtr* sound);
        public abstract Result SoundGroup_GetNumPlaying(IntPtr soundgroup, int* numplaying);
        public abstract Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);
        public abstract Result SoundGroup_GetUserData(IntPtr soundgroup, IntPtr* userdata);
        public abstract Result Channel_SetFrequency(IntPtr channel, float frequency);
        public abstract Result Channel_GetFrequency(IntPtr channel, float* frequency);
        public abstract Result Channel_SetPriority(IntPtr channel, int priority);
        public abstract Result Channel_GetPriority(IntPtr channel, int* priority);
        public abstract Result Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);
        public abstract Result Channel_GetChannelGroup(IntPtr channel, IntPtr* channelgroup);
        public abstract Result Channel_IsVirtual(IntPtr channel, int* isvirtual);
        public abstract Result Channel_GetCurrentSound(IntPtr channel, IntPtr* sound);
        public abstract Result Channel_GetIndex(IntPtr channel, int* index);
        public abstract Result Channel_SetPosition(IntPtr channel, uint position, TimeUnit postype);
        public abstract Result Channel_GetPosition(IntPtr channel, uint* position, TimeUnit postype);
        public abstract Result Channel_SetMode(IntPtr channel, Mode mode);
        public abstract Result Channel_GetMode(IntPtr channel, Mode* mode);
        public abstract Result Channel_SetLoopCount(IntPtr channel, int loopcount);
        public abstract Result Channel_GetLoopCount(IntPtr channel, int* loopcount);
        public abstract Result Channel_SetLoopPoints(IntPtr channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        public abstract Result Channel_GetLoopPoints(IntPtr channel, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);
        public abstract Result Channel_SetUserData(IntPtr channel, IntPtr userdata);
        public abstract Result Channel_GetUserData(IntPtr channel, IntPtr* userdata);
        public abstract Result ChannelGroup_Stop(IntPtr channelgroup);
        public abstract Result ChannelGroup_SetPaused(IntPtr channelgroup, int paused);
        public abstract Result ChannelGroup_GetPaused(IntPtr channelgroup, int* paused);
        public abstract Result ChannelGroup_GetVolume(IntPtr channelgroup, float* volume);
        public abstract Result ChannelGroup_SetVolumeRamp(IntPtr channelgroup, int ramp);
        public abstract Result ChannelGroup_GetVolumeRamp(IntPtr channelgroup, int* ramp);
        public abstract Result ChannelGroup_GetAudibility(IntPtr channelgroup, float* audibility);

        public Result ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility)
        {
            fixed (float* pAudibility = &audibility)
            {
                return ChannelGroup_GetAudibility(channelgroup, pAudibility);
            }
        }

        public abstract Result ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);
        public abstract Result ChannelGroup_GetPitch(IntPtr channelgroup, float* pitch);
        public abstract Result ChannelGroup_SetMute(IntPtr channelgroup, int mute);
        public abstract Result ChannelGroup_GetMute(IntPtr channelgroup, int* mute);
        public abstract Result ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);
        public abstract Result ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, float* wet);
        public abstract Result ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);
        public abstract Result ChannelGroup_GetLowPassGain(IntPtr channelgroup, float* gain);
        public abstract Result ChannelGroup_SetMode(IntPtr channelgroup, Mode mode);
        public abstract Result ChannelGroup_GetMode(IntPtr channelgroup, Mode* mode);
        public abstract Result ChannelGroup_SetCallback(IntPtr channelgroup, ChannelCallback callback);
        public abstract Result ChannelGroup_IsPlaying(IntPtr channelgroup, int* isplaying);
        public abstract Result ChannelGroup_SetPan(IntPtr channelgroup, float pan);
        public abstract Result ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        public abstract Result ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float* levels, int numlevels);
        public abstract Result ChannelGroup_SetMixMatrix(IntPtr channelgroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);
        public abstract Result ChannelGroup_GetMixMatrix(IntPtr channelgroup, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);
        public abstract Result ChannelGroup_GetDSPClock(IntPtr channelgroup, ulong* dspclock, ulong* parentclock);
        public abstract Result ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, int stopchannels);
        public abstract Result ChannelGroup_GetDelay(IntPtr channelgroup, ulong* dspclock_start, ulong* dspclock_end, int* stopchannels);

        public Result ChannelGroup_GetDelay(IntPtr channelgroup, ulong* dspclock_start, ulong* dspclock_end, out bool stopchannels)
        {
            int stopChannelsTemp = 0;
            var res = ChannelGroup_GetDelay(channelgroup, dspclock_start, dspclock_end, &stopChannelsTemp);
            stopchannels = stopChannelsTemp != 0;
            return res;
        }

        public Result ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            Result res;
            int stopChannelsTemp = 0;
            fixed (ulong* pDspclock_start = &dspclock_start, pDspclock_end = &dspclock_end)
            {
                res = ChannelGroup_GetDelay(channelgroup, pDspclock_start, pDspclock_end, &stopChannelsTemp);
            }
            stopchannels = stopChannelsTemp != 0;
            return res;
        }

        public abstract Result ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);
        public abstract Result ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);
        public abstract Result ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);
        public abstract Result ChannelGroup_GetFadePoints(IntPtr channelgroup, uint* numpoints, ulong* point_dspclock, float* point_volume);

        public Result ChannelGroup_GetFadePoints(IntPtr channelgroup, out ulong[] pointDspClocks, out float[] pointVolumes)
        {
            uint count = 0;

            Result res = ChannelGroup_GetFadePoints(channelgroup, &count, null, null);

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
                res = ChannelGroup_GetFadePoints(channelgroup, &count, pClocks, pVolumes);
            }

            pointDspClocks = clocks;
            pointVolumes = volumes;

            return res;
        }

        public abstract Result ChannelGroup_Set3DAttributes(IntPtr channelgroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public abstract Result ChannelGroup_Get3DAttributes(IntPtr channelgroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);

        public Result ChannelGroup_Get3DAttributes(IntPtr channelgroup, out Vector3 pos, out Vector3 vel, out Vector3 alt_pan_pos)
        {
            fixed (Vector3* pPos = &pos, pVel = &vel, pAltPanPos = &alt_pan_pos)
            {
                return ChannelGroup_Get3DAttributes(channelgroup, pPos, pVel, pAltPanPos);
            }
        }

        public abstract Result ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);
        public abstract Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, float* mindistance, float* maxdistance);

        public Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float minDistance, out float maxDistance)
        {
            fixed (float* pMinDistance = &minDistance, pMaxdistance = &maxDistance)
            {
                return ChannelGroup_Get3DMinMaxDistance(channelgroup, pMinDistance, pMaxdistance);
            }
        }

        public abstract Result ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        public abstract Result ChannelGroup_Get3DConeSettings(IntPtr channelgroup, float* insideconeangle, float* outsideconeangle, float* outsidevolume);

        public Result ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            fixed (float* pInsideAngle = &insideconeangle, pOutsideAngle = &outsideconeangle, pOutsideVolume = &outsidevolume)
            {
                return ChannelGroup_Get3DConeSettings(channelgroup, pInsideAngle, pOutsideAngle, pOutsideVolume);
            }
        }

        public abstract Result ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, Vector3* orientation);
        public abstract Result ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, Vector3* orientation);

        public Result ChannelGroup_Get3DConeOrientation(IntPtr channelGroup, out Vector3 orientation)
        {
            orientation = default;
            fixed (Vector3* pOrientation = &orientation)
            {
                return ChannelGroup_Get3DConeOrientation(channelGroup, pOrientation);
            }
        }

        public abstract Result ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, IntPtr points, int numpoints);
        public abstract Result ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, IntPtr* points, int* numpoints);

        public Result ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int pointCount)
        {
            fixed (IntPtr* pPoints = &points)
            fixed (int* pPointCount = &pointCount)
            {
                return ChannelGroup_Get3DCustomRolloff(channelgroup, pPoints, pPointCount);
            }
        }

        public abstract Result ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);
        public abstract Result ChannelGroup_Get3DOcclusion(IntPtr channelgroup, float* directocclusion, float* reverbocclusion);

        public Result ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directOcclusion, out float reverbOcclusion)
        {
            fixed (float* pDirectOcclusion = &directOcclusion, pReverbOcclusion = &reverbOcclusion)
            {
                return ChannelGroup_Get3DOcclusion(channelgroup, pDirectOcclusion, pReverbOcclusion);
            }
        }

        public abstract Result ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);

        public abstract Result ChannelGroup_Get3DSpread(IntPtr channelgroup, float* angle);

        public Result ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle)
        {
            fixed (float* pAngle = &angle)
            {
                return ChannelGroup_Get3DSpread(channelgroup, pAngle);
            }
        }

        public abstract Result ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);

        public abstract Result ChannelGroup_Get3DLevel(IntPtr channelgroup, float* level);

        public Result ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level)
        {
            fixed (float* pLevel = &level)
            {
                return ChannelGroup_Get3DLevel(channelgroup, pLevel);
            }
        }

        public abstract Result ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);

        public abstract Result ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, float* level);

        public abstract Result ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, int custom, float customLevel, float centerFreq);

        public abstract Result ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, int* custom, float* customLevel, float* centerFreq);

        public Result ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out int custom, out float customLevel, out float centerFreq)
        {
            fixed (int* pCustom = &custom)
            fixed (float* pCustomlevel = &customLevel, pCenterFreq = &centerFreq)
            {
                return ChannelGroup_Get3DDistanceFilter(channelgroup, pCustom, pCustomlevel, pCenterFreq);
            }
        }

        public abstract Result ChannelGroup_GetSystemObject(IntPtr channelgroup, IntPtr* system);
        public abstract Result ChannelGroup_SetVolume(IntPtr channelgroup, float volume);
        public abstract Result ChannelGroup_GetDSP(IntPtr channelgroup, int index, IntPtr* dsp);

        public Result ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock)
        {
            fixed (ulong* pDspclock = &dspclock, pParentclock = &parentclock)
            {
                return ChannelGroup_GetDSPClock(channelgroup, pDspclock, pParentclock);
            }
        }

        public abstract Result ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);
        public abstract Result ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);
        public abstract Result ChannelGroup_GetNumDSPs(IntPtr channelgroup, int* numdsps);
        public abstract Result ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);
        public abstract Result ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, int* index);

        public Result ChannelGroup_GetDSPIndex(IntPtr channelGroup, IntPtr dsp, out int index)
        {
            fixed (int* pIndex = &index)
            {
                return ChannelGroup_GetDSPIndex(channelGroup, dsp, pIndex);
            }
        }

        public abstract Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);
        public abstract Result ChannelGroup_GetUserData(IntPtr channelgroup, IntPtr* userdata);
        public abstract Result ChannelGroup_Release(IntPtr Handle);

        public abstract Result ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, int propagatedspclock, IntPtr* connection);

        public Result ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, IntPtr* connection)
        {
            return ChannelGroup_AddGroup(channelgroup, group, propagatedspclock ? 1 : 0, connection);
        }

        public Result ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection)
        {
            fixed (IntPtr* connectionRes = &connection)
                return ChannelGroup_AddGroup(channelgroup, group, propagatedspclock ? 1 : 0, connectionRes);
        }

        public abstract Result ChannelGroup_GetNumGroups(IntPtr channelgroup, int* numgroups);
        public abstract Result ChannelGroup_GetGroup(IntPtr channelgroup, int index, IntPtr* group);

        public Result ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group)
        {
            fixed (IntPtr* pGroup = &group)
            {
                return ChannelGroup_GetGroup(channelgroup, index, pGroup);
            }
        }

        public abstract Result ChannelGroup_GetParentGroup(IntPtr channelgroup, IntPtr* group);
        public abstract Result ChannelGroup_GetName(IntPtr channelgroup, byte* name, int namelen);
        public abstract Result ChannelGroup_GetNumChannels(IntPtr channelgroup, int* numchannels);
        public abstract Result ChannelGroup_GetChannel(IntPtr channelgroup, int index, IntPtr* channel);

        public Result ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel)
        {
            fixed (IntPtr* pChannel = &channel)
            {
                return ChannelGroup_GetChannel(channelgroup, index, pChannel);
            }
        }

        public abstract Result DSP_Release(IntPtr dsp);
        public abstract Result DSP_GetSystemObject(IntPtr dsp, IntPtr* system);
        public abstract Result DSP_AddInput(IntPtr dsp, IntPtr target, IntPtr* connection, DSPConnectionType type);
        public abstract Result DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);
        public abstract Result DSP_DisconnectAll(IntPtr dsp, int inputs, int outputs);
        public abstract Result DSP_GetNumInputs(IntPtr dsp, int* numinputs);
        public abstract Result DSP_GetNumOutputs(IntPtr dsp, int* numoutputs);
        public abstract Result DSP_GetInput(IntPtr dsp, int index, IntPtr* input, IntPtr* inputconnection);
        public abstract Result DSP_GetOutput(IntPtr dsp, int index, IntPtr* output, IntPtr* outputconnection);
        public abstract Result DSP_SetActive(IntPtr dsp, int active);
        public abstract Result DSP_GetActive(IntPtr dsp, int* active);
        public abstract Result DSP_SetBypass(IntPtr dsp, int bypass);
        public abstract Result DSP_GetBypass(IntPtr dsp, int* bypass);
        public abstract Result DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);
        public abstract Result DSP_GetWetDryMix(IntPtr dsp, float* prewet, float* postwet, float* dry);
        public abstract Result DSP_SetChannelFormat(IntPtr dsp, ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode);
        public abstract Result DSP_GetChannelFormat(IntPtr dsp, out ChannelMask channelmask, int* numchannels, out SpeakerMode source_speakermode);
        public abstract Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, int* outchannels, out SpeakerMode outspeakermode);
        public abstract Result DSP_Reset(IntPtr dsp);
        public abstract Result DSP_SetParameterFloat(IntPtr dsp, int index, float value);
        public abstract Result DSP_SetParameterBool(IntPtr dsp, int index, int value);
        public abstract Result DSP_SetParameterInt(IntPtr dsp, int index, int value);
        public abstract Result DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);
        public abstract Result DSP_GetParameterFloat(IntPtr dsp, int index, float* value, byte* valuestr, int valuestrlen);
        public abstract Result DSP_GetParameterBool(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);
        public abstract Result DSP_GetParameterint(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);
        public abstract Result DSP_GetParameterData(IntPtr dsp, int index, IntPtr* data, uint* length, IntPtr valuestr, int valuestrlen);
        public abstract Result DSP_GetNumParameters(IntPtr dsp, int* numparams);
        public abstract Result DSP_GetParameterInfo(IntPtr dsp, int index, IntPtr* desc);
        public abstract Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, int* index);
        public abstract Result DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, int show);
        public abstract Result DSP_GetInfo(IntPtr dsp, byte* name, FmodVersion* version, int* channels, int* configwidth, int* configheight);
        public abstract Result DSP_GetType(IntPtr dsp, DSPType* type);
        public abstract Result DSP_GetIdle(IntPtr dsp, int* idle);
        public abstract Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);
        public abstract Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);
        public abstract Result DSP_SetMeteringEnabled(IntPtr dsp, int inputEnabled, int outputEnabled);
        public abstract Result DSP_GetMeteringEnabled(IntPtr dsp, int* inputEnabled, int* outputEnabled);
        public abstract Result DSP_GetMeteringInfo(IntPtr dsp, DSPMeteringInfo* inputInfo, DSPMeteringInfo* outputInfo);
        public abstract Result DSPConnection_GetInput(IntPtr dspconnection, IntPtr* input);
        public abstract Result DSPConnection_GetOutput(IntPtr dspconnection, IntPtr* output);
        public abstract Result DSPConnection_SetMix(IntPtr dspconnection, float volume);
        public abstract Result DSPConnection_GetMix(IntPtr dspconnection, float* volume);
        public abstract Result DSPConnection_SetMixMatrix(IntPtr dspconnection, float* matrix, int outchannels, int inchannels, int inchannel_hop);
        public abstract Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);
        public abstract Result DSPConnection_GetType(IntPtr dspconnection, DSPConnectionType* type);
        public abstract Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);
        public abstract Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);
        public abstract Result Geometry_Release(IntPtr geometry);
        public abstract Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, int doublesided, int numvertices, Vector3* vertices, int* polygonindex);
        public abstract Result Geometry_GetNumPolygons(IntPtr geometry, int* numpolygons);
        public abstract Result Geometry_GetMaxPolygons(IntPtr geometry, int* maxpolygons, int* maxvertices);
        public abstract Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, int* numvertices);
        public abstract Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);
        public abstract Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);
        public abstract Result Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, int doublesided);
        public abstract Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, int* doublesided);
        public abstract Result Geometry_SetActive(IntPtr geometry, int active);
        public abstract Result Geometry_GetActive(IntPtr geometry, int* active);
        public abstract Result Geometry_SetRotation(IntPtr geometry, Vector3* forward, Vector3* up);
        public abstract Result Geometry_GetRotation(IntPtr geometry, Vector3* forward, Vector3* up);
        public abstract Result Geometry_SetPosition(IntPtr geometry, Vector3* position);
        public abstract Result Geometry_GetPosition(IntPtr geometry, Vector3* position);
        public abstract Result Geometry_SetScale(IntPtr geometry, Vector3* scale);
        public abstract Result Geometry_GetScale(IntPtr geometry, Vector3* scale);
        public abstract Result Geometry_Save(IntPtr geometry, void* data, int* datasize);
        public abstract Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);
        public abstract Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);
    }
}
