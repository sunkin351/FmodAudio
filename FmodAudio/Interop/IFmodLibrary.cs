using AdvancedDLSupport;
using System;
using System.Runtime.InteropServices;

namespace FmodAudio.Interop
{
    using Dsp;
    using Dsp.Interop;

    public delegate Result ChannelCallbackInternal(IntPtr rawchannel, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2);

    public unsafe interface IFmodLibrary
    {
        [NativeSymbol("FMOD_Memory_Initialize")]
        Result Memory_Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags);

        [NativeSymbol("FMOD_Memory_GetStats")]
        Result Memory_GetStats(out int currentalloced, out int maxalloced, bool blocking);

        [NativeSymbol("FMOD_Debug_Initialize")]
        Result Debug_Initialize(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename);

        #region System Class Functions

        [NativeSymbol("FMOD_System_Create")]
        Result System_Create(out IntPtr System);

        [NativeSymbol("FMOD_System_Release")]
        Result System_Release(IntPtr system);

        [NativeSymbol("FMOD_System_SetOutput")]
        Result System_SetOutput(IntPtr system, OutputType output);

        [NativeSymbol("FMOD_System_GetOutput")]
        Result System_GetOutput(IntPtr system, out OutputType output);

        [NativeSymbol("FMOD_System_GetNumDrivers")]
        Result System_GetNumDrivers(IntPtr system, out int numdrivers);

        [NativeSymbol("FMOD_System_GetDriverInfo")]
        Result System_GetDriverInfo(IntPtr system, int id, byte* name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels);

        [NativeSymbol("FMOD_System_SetDriver")]
        Result System_SetDriver(IntPtr system, int driver);

        [NativeSymbol("FMOD_System_GetDriver")]
        Result System_GetDriver(IntPtr system, out int driver);

        [NativeSymbol("FMOD_System_SetSoftwareChannels")]
        Result System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels);

        [NativeSymbol("FMOD_System_GetSoftwareChannels")]
        Result System_GetSoftwareChannels(IntPtr system, out int numsoftwarechannels);

        [NativeSymbol("FMOD_System_SetSoftwareFormat")]
        Result System_SetSoftwareFormat(IntPtr system, int samplerate, SpeakerMode speakermode, int numrawspeakers);

        [NativeSymbol("FMOD_System_GetSoftwareFormat")]
        Result System_GetSoftwareFormat(IntPtr system, out int samplerate, out SpeakerMode speakermode, out int numrawspeakers);

        [NativeSymbol("FMOD_System_SetDSPBufferSize")]
        Result System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers);

        [NativeSymbol("FMOD_System_GetDSPBufferSize")]
        Result System_GetDSPBufferSize(IntPtr system, out uint bufferlength, out int numbuffers);

        [NativeSymbol("FMOD_System_SetFileSystem")]
        Result System_SetFileSystem(IntPtr system, FileOpenCallbackImpl useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek, FileAsyncReadCallback userasyncread, FileAsyncCancelCallback userasynccancel, int blockalign);

        [NativeSymbol("FMOD_System_AttachFileSystem")]
        Result System_AttachFileSystem(IntPtr system, FileOpenCallback useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek);

        [NativeSymbol("FMOD_System_SetPluginPath")]
        Result System_SetPluginPath(IntPtr system, [In, Out] byte[] path);

        [NativeSymbol("FMOD_System_LoadPlugin")]
        Result System_LoadPlugin(IntPtr system, [In, Out] byte[] filename, out uint handle, uint priority);

        [NativeSymbol("FMOD_System_UnloadPlugin")]
        Result System_UnloadPlugin(IntPtr system, uint handle);

        [NativeSymbol("FMOD_System_GetNumNestedPlugins")]
        Result System_GetNumNestedPlugins(IntPtr system, uint handle, out int count);

        [NativeSymbol("FMOD_System_GetNestedPlugin")]
        Result System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle);

        [NativeSymbol("FMOD_System_GetNumPlugins")]
        Result System_GetNumPlugins(IntPtr system, PluginType plugintype, out int numplugins);

        [NativeSymbol("FMOD_System_GetPluginHandle")]
        Result System_GetPluginHandle(IntPtr system, PluginType plugintype, int index, out uint handle);

        [NativeSymbol("FMOD_System_GetPluginInfo")]
        Result System_GetPluginInfo(IntPtr system, uint handle, out PluginType plugintype, byte* name, int namelen, out uint version);

        [NativeSymbol("FMOD_System_CreateDSPByPlugin")]
        Result System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp);

        [NativeSymbol("FMOD_System_SetOutputByPlugin")]
        Result System_SetOutputByPlugin(IntPtr system, uint handle);

        [NativeSymbol("FMOD_System_GetOutputByPlugin")]
        Result System_GetOutputByPlugin(IntPtr system, out uint handle);

        [NativeSymbol("FMOD_System_GetDSPInfoByPlugin")]
        Result System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description);

        [NativeSymbol("FMOD_System_RegisterDSP")]
        Result System_RegisterDSP(IntPtr system, [In] ref DspDescription.Structure description, out uint handle);

        [NativeSymbol("FMOD_System_Init")]
        Result System_Init(IntPtr system, int maxchannels, InitFlags flags, IntPtr extradriverdata);

        [NativeSymbol("FMOD_System_Close")]
        Result System_Close(IntPtr system);

        [NativeSymbol("FMOD_System_Update")]
        Result System_Update(IntPtr system);

        [NativeSymbol("FMOD_System_SetAdvancedSettings")]
        Result System_SetAdvancedSettings(IntPtr system, ref AdvancedSettings._interopStruct settings);

        [NativeSymbol("FMOD_System_GetAdvancedSettings")]
        Result System_GetAdvancedSettings(IntPtr system, ref AdvancedSettings._interopStruct settings);

        [NativeSymbol("FMOD_System_Set3DRolloffCallback")]
        Result System_Set3DRolloffCallback(IntPtr system, CB_3D_RolloffCallback callback);

        [NativeSymbol("FMOD_System_MixerSuspend")]
        Result System_MixerSuspend(IntPtr system);

        [NativeSymbol("FMOD_System_MixerResume")]
        Result System_MixerResume(IntPtr system);

        [NativeSymbol("FMOD_System_GetDefaultMixMatrix")]
        Result System_GetDefaultMixMatrix(IntPtr system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, [In, Out] float[] matrix, int matrixhop);

        [NativeSymbol("FMOD_System_GetSpeakerModeChannels")]
        Result System_GetSpeakerModeChannels(IntPtr system, SpeakerMode mode, out int channels);

        [NativeSymbol("FMOD_System_SetCallback")]
        Result System_SetCallback(IntPtr system, SystemCallback callback, SystemCallbackType callbackmask);

        [NativeSymbol("FMOD_System_SetSpeakerPosition")]
        Result System_SetSpeakerPosition(IntPtr system, Speaker speaker, float x, float y, bool active);

        [NativeSymbol("FMOD_System_GetSpeakerPosition")]
        Result System_GetSpeakerPosition(IntPtr system, Speaker speaker, out float x, out float y, out bool active);

        [NativeSymbol("FMOD_System_Set3DSettings")]
        Result System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);

        [NativeSymbol("FMOD_System_Get3DSettings")]
        Result System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);

        [NativeSymbol("FMOD_System_Set3DNumListeners")]
        Result System_Set3DNumListeners(IntPtr system, int numlisteners);

        [NativeSymbol("FMOD_System_Get3DNumListeners")]
        Result System_Get3DNumListeners(IntPtr system, out int numlisteners);

        [NativeSymbol("FMOD_System_Set3DListenerAttributes")]
        Result System_Set3DListenerAttributes(IntPtr system, int listener, ref Vector pos, ref Vector vel, ref Vector forward, ref Vector up);

        [NativeSymbol("FMOD_System_Get3DListenerAttributes")]
        Result System_Get3DListenerAttributes(IntPtr system, int listener, out Vector pos, out Vector vel, out Vector forward, out Vector up);

        [NativeSymbol("FMOD_System_SetStreamBufferSize")]
        Result System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TimeUnit filebuffersizetype);

        [NativeSymbol("FMOD_System_GetStreamBufferSize")]
        Result System_GetStreamBufferSize(IntPtr system, out uint filebuffersize, out TimeUnit filebuffersizetype);

        [NativeSymbol("FMOD_System_GetVersion")]
        Result System_GetVersion(IntPtr system, out uint version);

        [NativeSymbol("FMOD_System_GetOutputHandle")]
        Result System_GetOutputHandle(IntPtr system, out IntPtr handle);

        [NativeSymbol("FMOD_System_GetChannelsPlaying")]
        Result System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels);

        [NativeSymbol("FMOD_System_GetCPUUsage")]
        Result System_GetCPUUsage(IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total);

        [NativeSymbol("FMOD_System_GetFileUsage")]
        Result System_GetFileUsage(IntPtr system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead);

        [NativeSymbol("FMOD_System_GetSoundRAM")]
        Result System_GetSoundRAM(IntPtr system, out int currentalloced, out int maxalloced, out int total);

        [NativeSymbol("FMOD_System_CreateSound")]
        Result System_CreateSound(IntPtr system, [In, Out] byte[] name_or_data, Mode mode, ref CreateSoundInfo._interopStruct exinfo, out IntPtr sound);

        [NativeSymbol("FMOD_System_CreateStream")]
        Result System_CreateStream(IntPtr system, [In, Out] byte[] name_or_data, Mode mode, ref CreateSoundInfo._interopStruct exinfo, out IntPtr sound);

        [NativeSymbol("FMOD_System_CreateDSP")]
        Result System_CreateDSP(IntPtr system, [In] ref DspDescription.Structure description, out IntPtr dspHandle);

        [NativeSymbol("FMOD_System_CreateDSPByType")]
        Result System_CreateDSPByType(IntPtr system, DSPType type, out IntPtr dspHandle);

        [NativeSymbol("FMOD_System_CreateChannelGroup")]
        Result System_CreateChannelGroup(IntPtr system, [In, Out] byte[] name, out IntPtr channelgroup);

        [NativeSymbol("FMOD_System_CreateSoundGroup")]
        Result System_CreateSoundGroup(IntPtr system, [In, Out] byte[] name, out IntPtr soundgroup);

        [NativeSymbol("FMOD_System_CreateReverb3D")]
        Result System_CreateReverb3D(IntPtr system, out IntPtr reverb);

        [NativeSymbol("FMOD_System_PlaySound")]
        Result System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel);

        [NativeSymbol("FMOD_System_PlayDSP")]
        Result System_PlayDSP(IntPtr systemHandle, IntPtr dspHandle, IntPtr channelGroupHandle, bool paused, out IntPtr channel);

        [NativeSymbol("FMOD_System_GetChannel")]
        Result System_GetChannel(IntPtr system, int channelid, out IntPtr channel);

        [NativeSymbol("FMOD_System_GetMasterChannelGroup")]
        Result System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup);

        [NativeSymbol("FMOD_System_GetMasterSoundGroup")]
        Result System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup);

        [NativeSymbol("FMOD_System_AttachChannelGroupToPort")]
        Result System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);

        [NativeSymbol("FMOD_System_DetachChannelGroupFromPort")]
        Result System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);

        [NativeSymbol("FMOD_System_SetReverbProperties")]
        Result System_SetReverbProperties(IntPtr system, int instance, ref ReverbProperties prop);

        [NativeSymbol("FMOD_System_GetReverbProperties")]
        Result System_GetReverbProperties(IntPtr system, int instance, out ReverbProperties prop);

        [NativeSymbol("FMOD_System_LockDSP")]
        Result System_LockDSP(IntPtr system);

        [NativeSymbol("FMOD_System_UnlockDSP")]
        Result System_UnlockDSP(IntPtr system);

        [NativeSymbol("FMOD_System_GetRecordNumDrivers")]
        Result System_GetRecordNumDrivers(IntPtr system, out int numdrivers, out int numconnected);

        [NativeSymbol("FMOD_System_GetRecordDriverInfo")]
        Result System_GetRecordDriverInfo(IntPtr system, int id, byte* name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels, out DriverState state);

        [NativeSymbol("FMOD_System_GetRecordPosition")]
        Result System_GetRecordPosition(IntPtr system, int id, out uint position);

        [NativeSymbol("FMOD_System_RecordStart")]
        Result System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);

        [NativeSymbol("FMOD_System_RecordStop")]
        Result System_RecordStop(IntPtr system, int id);

        [NativeSymbol("FMOD_System_IsRecording")]
        Result System_IsRecording(IntPtr system, int id, out bool recording);

        [NativeSymbol("FMOD_System_CreateGeometry")]
        Result System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);

        [NativeSymbol("FMOD_System_SetGeometrySettings")]
        Result System_SetGeometrySettings(IntPtr system, float maxworldsize);

        [NativeSymbol("FMOD_System_GetGeometrySettings")]
        Result System_GetGeometrySettings(IntPtr system, out float maxworldsize);

        [NativeSymbol("FMOD_System_LoadGeometry")]
        Result System_LoadGeometry(IntPtr system, byte* data, int datasize, out IntPtr geometry);

        [NativeSymbol("FMOD_System_GetGeometryOcclusion")]
        Result System_GetGeometryOcclusion(IntPtr system, ref Vector listener, ref Vector source, out float direct, out float reverb);

        [NativeSymbol("FMOD_System_SetNetworkProxy")]
        Result System_SetNetworkProxy(IntPtr system, byte[] proxy);

        [NativeSymbol("FMOD_System_GetNetworkProxy")]
        Result System_GetNetworkProxy(IntPtr system, byte* proxy, int proxylen);

        [NativeSymbol("FMOD_System_SetNetworkTimeout")]
        Result System_SetNetworkTimeout(IntPtr system, int timeout);

        [NativeSymbol("FMOD_System_GetNetworkTimeout")]
        Result System_GetNetworkTimeout(IntPtr system, out int timeout);

        [NativeSymbol("FMOD_System_SetUserData")]
        Result System_SetUserData(IntPtr system, IntPtr userdata);

        [NativeSymbol("FMOD_System_GetUserData")]
        Result System_GetUserData(IntPtr system, out IntPtr userdata);
        
        #endregion

        #region Sound Class Functions

        [NativeSymbol("FMOD_Sound_Release")]
        Result Sound_Release(IntPtr sound);

        [NativeSymbol("FMOD_Sound_Lock")]
        Result Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);

        [NativeSymbol("FMOD_Sound_Unlock")]
        Result Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);

        [NativeSymbol("FMOD_Sound_SetDefaults")]
        Result Sound_SetDefaults(IntPtr sound, float frequency, int priority);

        [NativeSymbol("FMOD_Sound_GetDefaults")]
        Result Sound_GetDefaults(IntPtr sound, out float frequency, out int priority);

        [NativeSymbol("FMOD_Sound_Set3DMinMaxDistance")]
        Result Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);

        [NativeSymbol("FMOD_Sound_Get3DMinMaxDistance")]
        Result Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max);

        [NativeSymbol("FMOD_Sound_Set3DConeSettings")]
        Result Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);

        [NativeSymbol("FMOD_Sound_Get3DConeSettings")]
        Result Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

        [NativeSymbol("FMOD_Sound_Set3DCustomRolloff")]
        Result Sound_Set3DCustomRolloff(IntPtr sound, IntPtr points, int numpoints);

        [NativeSymbol("FMOD_Sound_Get3DCustomRolloff")]
        Result Sound_Get3DCustomRolloff(IntPtr sound, out IntPtr points, out int numpoints);

        [NativeSymbol("FMOD_Sound_GetSubSound")]
        Result Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound);

        [NativeSymbol("FMOD_Sound_GetSubSoundParent")]
        Result Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound);

        [NativeSymbol("FMOD_Sound_GetName")]
        Result Sound_GetName(IntPtr sound, IntPtr name, int namelen);

        [NativeSymbol("FMOD_Sound_GetLength")]
        Result Sound_GetLength(IntPtr sound, out uint length, TimeUnit lengthtype);

        [NativeSymbol("FMOD_Sound_GetFormat")]
        Result Sound_GetFormat(IntPtr sound, out SoundType type, out SoundFormat format, out int channels, out int bits);

        [NativeSymbol("FMOD_Sound_GetNumSubSounds")]
        Result Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds);

        [NativeSymbol("FMOD_Sound_GetNumTags")]
        Result Sound_GetNumTags(IntPtr sound, out int numtags, out int numtagsupdated);

        [NativeSymbol("FMOD_Sound_GetTag")]
        Result Sound_GetTag(IntPtr sound, string name, int index, out Tag tag);

        [NativeSymbol("FMOD_Sound_GetOpenState")]
        Result Sound_GetOpenState(IntPtr sound, out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy);

        [NativeSymbol("FMOD_Sound_ReadData")]
        Result Sound_ReadData(IntPtr sound, void* buffer, uint length, out uint read);

        [NativeSymbol("FMOD_Sound_SeekData")]
        Result Sound_SeekData(IntPtr sound, uint pcm);

        [NativeSymbol("FMOD_Sound_SetSoundGroup")]
        Result Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);

        [NativeSymbol("FMOD_Sound_GetSoundGroup")]
        Result Sound_GetSoundGroup(IntPtr sound, out IntPtr soundgroup);

        [NativeSymbol("FMOD_Sound_GetNumSyncPoints")]
        Result Sound_GetNumSyncPoints(IntPtr sound, out int numsyncpoints);

        [NativeSymbol("FMOD_Sound_GetSyncPoint")]
        Result Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point);

        [NativeSymbol("FMOD_Sound_GetSyncPointInfo")]
        Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TimeUnit offsettype);

        [NativeSymbol("FMOD_Sound_AddSyncPoint")]
        Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsettype, string name, out IntPtr point);

        [NativeSymbol("FMOD_Sound_DeleteSyncPoint")]
        Result Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);

        [NativeSymbol("FMOD_Sound_SetMode")]
        Result Sound_SetMode(IntPtr sound, Mode mode);

        [NativeSymbol("FMOD_Sound_GetMode")]
        Result Sound_GetMode(IntPtr sound, out Mode mode);

        [NativeSymbol("FMOD_Sound_SetLoopCount")]
        Result Sound_SetLoopCount(IntPtr sound, int loopcount);

        [NativeSymbol("FMOD_Sound_GetLoopCount")]
        Result Sound_GetLoopCount(IntPtr sound, out int loopcount);

        [NativeSymbol("FMOD_Sound_SetLoopPoints")]
        Result Sound_SetLoopPoints(IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [NativeSymbol("FMOD_Sound_GetLoopPoints")]
        Result Sound_GetLoopPoints(IntPtr sound, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);

        [NativeSymbol("FMOD_Sound_GetMusicNumChannels")]
        Result Sound_GetMusicNumChannels(IntPtr sound, out int numchannels);

        [NativeSymbol("FMOD_Sound_SetMusicChannelVolume")]
        Result Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);

        [NativeSymbol("FMOD_Sound_GetMusicChannelVolume")]
        Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume);

        [NativeSymbol("FMOD_Sound_SetMusicSpeed")]
        Result Sound_SetMusicSpeed(IntPtr sound, float speed);

        [NativeSymbol("FMOD_Sound_GetMusicSpeed")]
        Result Sound_GetMusicSpeed(IntPtr sound, out float speed);

        [NativeSymbol("FMOD_Sound_SetUserData")]
        Result Sound_SetUserData(IntPtr sound, IntPtr userdata);

        [NativeSymbol("FMOD_Sound_GetUserData")]
        Result Sound_GetUserData(IntPtr sound, out IntPtr userdata);

        #endregion

        #region SoundGroup Class Functions

        [NativeSymbol("FMOD_SoundGroup_Release")]
        Result SoundGroup_Release(IntPtr soundgroup);

        [NativeSymbol("FMOD_SoundGroup_GetSystemObject")]
        Result SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system);

        [NativeSymbol("FMOD_SoundGroup_SetMaxAudible")]
        Result SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);

        [NativeSymbol("FMOD_SoundGroup_GetMaxAudible")]
        Result SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible);

        [NativeSymbol("FMOD_SoundGroup_SetMaxAudibleBehavior")]
        Result SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior behavior);

        [NativeSymbol("FMOD_SoundGroup_GetMaxAudibleBehavior")]
        Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SoundGroupBehavior behavior);

        [NativeSymbol("FMOD_SoundGroup_SetMuteFadeSpeed")]
        Result SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);

        [NativeSymbol("FMOD_SoundGroup_GetMuteFadeSpeed")]
        Result SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed);

        [NativeSymbol("FMOD_SoundGroup_SetVolume")]
        Result SoundGroup_SetVolume(IntPtr soundgroup, float volume);

        [NativeSymbol("FMOD_SoundGroup_GetVolume")]
        Result SoundGroup_GetVolume(IntPtr soundgroup, out float volume);

        [NativeSymbol("FMOD_SoundGroup_Stop")]
        Result SoundGroup_Stop(IntPtr soundgroup);

        [NativeSymbol("FMOD_SoundGroup_GetName")]
        Result SoundGroup_GetName(IntPtr soundgroup, byte* name, int namelen);

        [NativeSymbol("FMOD_SoundGroup_GetNumSounds")]
        Result SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds);

        [NativeSymbol("FMOD_SoundGroup_GetSound")]
        Result SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound);

        [NativeSymbol("FMOD_SoundGroup_GetNumPlaying")]
        Result SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying);

        [NativeSymbol("FMOD_SoundGroup_SetUserData")]
        Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

        [NativeSymbol("FMOD_SoundGroup_GetUserData")]
        Result SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata);

        #endregion

        #region Channel Class Functions

        [NativeSymbol("FMOD_Channel_SetFrequency")]
        Result Channel_SetFrequency(IntPtr channel, float frequency);

        [NativeSymbol("FMOD_Channel_GetFrequency")]
        Result Channel_GetFrequency(IntPtr channel, out float frequency);

        [NativeSymbol("FMOD_Channel_SetPriority")]
        Result Channel_SetPriority(IntPtr channel, int priority);

        [NativeSymbol("FMOD_Channel_GetPriority")]
        Result Channel_GetPriority(IntPtr channel, out int priority);

        [NativeSymbol("FMOD_Channel_SetChannelGroup")]
        Result Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);

        [NativeSymbol("FMOD_Channel_GetChannelGroup")]
        Result Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup);

        [NativeSymbol("FMOD_Channel_IsVirtual")]
        Result Channel_IsVirtual(IntPtr channel, out bool isvirtual);

        [NativeSymbol("FMOD_Channel_GetCurrentSound")]
        Result Channel_GetCurrentSound(IntPtr channel, out IntPtr sound);

        [NativeSymbol("FMOD_Channel_GetIndex")]
        Result Channel_GetIndex(IntPtr channel, out int index);

        [NativeSymbol("FMOD_Channel_SetPosition")]
        Result Channel_SetPosition(IntPtr channel, uint position, TimeUnit postype);

        [NativeSymbol("FMOD_Channel_GetPosition")]
        Result Channel_GetPosition(IntPtr channel, out uint position, TimeUnit postype);

        [NativeSymbol("FMOD_Channel_SetMode")]
        Result Channel_SetMode(IntPtr channel, Mode mode);

        [NativeSymbol("FMOD_Channel_GetMode")]
        Result Channel_GetMode(IntPtr channel, out Mode mode);

        [NativeSymbol("FMOD_Channel_SetLoopCount")]
        Result Channel_SetLoopCount(IntPtr channel, int loopcount);

        [NativeSymbol("FMOD_Channel_GetLoopCount")]
        Result Channel_GetLoopCount(IntPtr channel, out int loopcount);

        [NativeSymbol("FMOD_Channel_SetLoopPoints")]
        Result Channel_SetLoopPoints(IntPtr channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);

        [NativeSymbol("FMOD_Channel_GetLoopPoints")]
        Result Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);

        [NativeSymbol("FMOD_Channel_SetUserData")]
        Result Channel_SetUserData(IntPtr channel, IntPtr userdata);

        [NativeSymbol("FMOD_Channel_GetUserData")]
        Result Channel_GetUserData(IntPtr channel, out IntPtr userdata);

        #endregion

        #region ChannelControl Class Functions

        [NativeSymbol("FMOD_ChannelGroup_Stop")]
        Result ChannelGroup_Stop(IntPtr channelgroup);

        [NativeSymbol("FMOD_ChannelGroup_SetPaused")]
        Result ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);

        [NativeSymbol("FMOD_ChannelGroup_GetPaused")]
        Result ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);

        [NativeSymbol("FMOD_ChannelGroup_GetVolume")]
        Result ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);

        [NativeSymbol("FMOD_ChannelGroup_SetVolumeRamp")]
        Result ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);

        [NativeSymbol("FMOD_ChannelGroup_GetVolumeRamp")]
        Result ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);

        [NativeSymbol("FMOD_ChannelGroup_GetAudibility")]
        Result ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);

        [NativeSymbol("FMOD_ChannelGroup_SetPitch")]
        Result ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);

        [NativeSymbol("FMOD_ChannelGroup_GetPitch")]
        Result ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);

        [NativeSymbol("FMOD_ChannelGroup_SetMute")]
        Result ChannelGroup_SetMute(IntPtr channelgroup, bool mute);

        [NativeSymbol("FMOD_ChannelGroup_GetMute")]
        Result ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);

        [NativeSymbol("FMOD_ChannelGroup_SetReverbProperties")]
        Result ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);

        [NativeSymbol("FMOD_ChannelGroup_GetReverbProperties")]
        Result ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);

        [NativeSymbol("FMOD_ChannelGroup_SetLowPassGain")]
        Result ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);

        [NativeSymbol("FMOD_ChannelGroup_GetLowPassGain")]
        Result ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);

        [NativeSymbol("FMOD_ChannelGroup_SetMode")]
        Result ChannelGroup_SetMode(IntPtr channelgroup, Mode mode);

        [NativeSymbol("FMOD_ChannelGroup_GetMode")]
        Result ChannelGroup_GetMode(IntPtr channelgroup, out Mode mode);

        [NativeSymbol("FMOD_ChannelGroup_SetCallback")]
        Result ChannelGroup_SetCallback(IntPtr channelgroup, ChannelCallbackInternal callback);

        [NativeSymbol("FMOD_ChannelGroup_IsPlaying")]
        Result ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);

        [NativeSymbol("FMOD_ChannelGroup_SetPan")]
        Result ChannelGroup_SetPan(IntPtr channelgroup, float pan);

        [NativeSymbol("FMOD_ChannelGroup_SetMixLevelsOutput")]
        Result ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

        [NativeSymbol("FMOD_ChannelGroup_SetMixLevelsInput")]
        Result ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float* levels, int numlevels);

        [NativeSymbol("FMOD_ChannelGroup_SetMixMatrix")]
        Result ChannelGroup_SetMixMatrix(IntPtr channelgroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);

        [NativeSymbol("FMOD_ChannelGroup_GetMixMatrix")]
        Result ChannelGroup_GetMixMatrix(IntPtr channelgroup, float* matrix, out int outchannels, out int inchannels, int inchannel_hop);

        [NativeSymbol("FMOD_ChannelGroup_GetDSPClock")]
        Result ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);

        [NativeSymbol("FMOD_ChannelGroup_SetDelay")]
        Result ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

        [NativeSymbol("FMOD_ChannelGroup_GetDelay")]
        Result ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

        [NativeSymbol("FMOD_ChannelGroup_AddFadePoint")]
        Result ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);

        [NativeSymbol("FMOD_ChannelGroup_SetFadePointRamp")]
        Result ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);

        [NativeSymbol("FMOD_ChannelGroup_RemoveFadePoints")]
        Result ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);

        [NativeSymbol("FMOD_ChannelGroup_GetFadePoints")]
        Result ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong* point_dspclock, float* point_volume);

        [NativeSymbol("FMOD_ChannelGroup_Set3DAttributes")]
        Result ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref Vector pos, ref Vector vel, ref Vector alt_pan_pos);

        [NativeSymbol("FMOD_ChannelGroup_Get3DAttributes")]
        Result ChannelGroup_Get3DAttributes(IntPtr channelgroup, out Vector pos, out Vector vel, out Vector alt_pan_pos);

        [NativeSymbol("FMOD_ChannelGroup_Set3DMinMaxDistance")]
        Result ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);

        [NativeSymbol("FMOD_ChannelGroup_Get3DMinMaxDistance")]
        Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);

        [NativeSymbol("FMOD_ChannelGroup_Set3DConeSettings")]
        Result ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);

        [NativeSymbol("FMOD_ChannelGroup_Get3DConeSettings")]
        Result ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

        [NativeSymbol("FMOD_ChannelGroup_Set3DConeOrientation")]
        Result ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref Vector orientation);

        [NativeSymbol("FMOD_ChannelGroup_Get3DConeOrientation")]
        Result ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out Vector orientation);

        [NativeSymbol("FMOD_ChannelGroup_Set3DCustomRolloff")]
        Result ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, IntPtr points, int numpoints);

        [NativeSymbol("FMOD_ChannelGroup_Get3DCustomRolloff")]
        Result ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);

        [NativeSymbol("FMOD_ChannelGroup_Set3DOcclusion")]
        Result ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);

        [NativeSymbol("FMOD_ChannelGroup_Get3DOcclusion")]
        Result ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);

        [NativeSymbol("FMOD_ChannelGroup_Set3DSpread")]
        Result ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);

        [NativeSymbol("FMOD_ChannelGroup_Get3DSpread")]
        Result ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);

        [NativeSymbol("FMOD_ChannelGroup_Set3DLevel")]
        Result ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);

        [NativeSymbol("FMOD_ChannelGroup_Get3DLevel")]
        Result ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);

        [NativeSymbol("FMOD_ChannelGroup_Set3DDopplerLevel")]
        Result ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);

        [NativeSymbol("FMOD_ChannelGroup_Get3DDopplerLevel")]
        Result ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);

        [NativeSymbol("FMOD_ChannelGroup_Set3DDistanceFilter")]
        Result ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);

        [NativeSymbol("FMOD_ChannelGroup_Get3DDistanceFilter")]
        Result ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);

        [NativeSymbol("FMOD_ChannelGroup_GetSystemObject")]
        Result ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system); //Unused

        [NativeSymbol("FMOD_ChannelGroup_SetVolume")]
        Result ChannelGroup_SetVolume(IntPtr channelgroup, float volume);

        [NativeSymbol("FMOD_ChannelGroup_GetDSP")]
        Result ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);

        [NativeSymbol("FMOD_ChannelGroup_AddDSP")]
        Result ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);

        [NativeSymbol("FMOD_ChannelGroup_RemoveDSP")]
        Result ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);

        [NativeSymbol("FMOD_ChannelGroup_GetNumDSPs")]
        Result ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);

        [NativeSymbol("FMOD_ChannelGroup_SetDSPIndex")]
        Result ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);

        [NativeSymbol("FMOD_ChannelGroup_GetDSPIndex")]
        Result ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);

        [NativeSymbol("FMOD_ChannelGroup_SetUserData")]
        Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

        [NativeSymbol("FMOD_ChannelGroup_GetUserData")]
        Result ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata);

        #endregion

        #region ChannelGroup Class Functions
        
        [NativeSymbol("FMOD_ChannelGroup_Release")]
        Result ChannelGroup_Release(IntPtr Handle);

        [NativeSymbol("FMOD_ChannelGroup_AddGroup")]
        Result ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection);

        [NativeSymbol("FMOD_ChannelGroup_GetNumGroups")]
        Result ChannelGroup_GetNumGroups(IntPtr channelgroup, out int numgroups);

        [NativeSymbol("FMOD_ChannelGroup_GetGroup")]
        Result ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group);

        [NativeSymbol("FMOD_ChannelGroup_GetParentGroup")]
        Result ChannelGroup_GetParentGroup(IntPtr channelgroup, out IntPtr group);

        [NativeSymbol("FMOD_ChannelGroup_GetName")]
        Result ChannelGroup_GetName(IntPtr channelgroup, byte* name, int namelen);

        [NativeSymbol("FMOD_ChannelGroup_GetNumChannels")]
        Result ChannelGroup_GetNumChannels(IntPtr channelgroup, out int numchannels);

        [NativeSymbol("FMOD_ChannelGroup_GetChannel")]
        Result ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel);

        #endregion

        #region DSP Class Functions

        [NativeSymbol("FMOD_DSP_Release")]
        Result DSP_Release(IntPtr dsp);

        [NativeSymbol("FMOD_DSP_GetSystemObject")]
        Result DSP_GetSystemObject(IntPtr dsp, out IntPtr system);

        [NativeSymbol("FMOD_DSP_AddInput")]
        Result DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPConnectionType type);

        [NativeSymbol("FMOD_DSP_DisconnectFrom")]
        Result DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);

        [NativeSymbol("FMOD_DSP_DisconnectAll")]
        Result DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);

        [NativeSymbol("FMOD_DSP_GetNumInputs")]
        Result DSP_GetNumInputs(IntPtr dsp, out int numinputs);

        [NativeSymbol("FMOD_DSP_GetNumOutputs")]
        Result DSP_GetNumOutputs(IntPtr dsp, out int numoutputs);

        [NativeSymbol("FMOD_DSP_GetInput")]
        Result DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);

        [NativeSymbol("FMOD_DSP_GetOutput")]
        Result DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);

        [NativeSymbol("FMOD_DSP_SetActive")]
        Result DSP_SetActive(IntPtr dsp, bool active);

        [NativeSymbol("FMOD_DSP_GetActive")]
        Result DSP_GetActive(IntPtr dsp, out bool active);

        [NativeSymbol("FMOD_DSP_SetBypass")]
        Result DSP_SetBypass(IntPtr dsp, bool bypass);

        [NativeSymbol("FMOD_DSP_GetBypass")]
        Result DSP_GetBypass(IntPtr dsp, out bool bypass);

        [NativeSymbol("FMOD_DSP_SetWetDryMix")]
        Result DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

        [NativeSymbol("FMOD_DSP_GetWetDryMix")]
        Result DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry);

        [NativeSymbol("FMOD_DSP_SetChannelFormat")]
        Result DSP_SetChannelFormat(IntPtr dsp, ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode);

        [NativeSymbol("FMOD_DSP_GetChannelFormat")]
        Result DSP_GetChannelFormat(IntPtr dsp, out ChannelMask channelmask, out int numchannels, out SpeakerMode source_speakermode);

        [NativeSymbol("FMOD_DSP_GetOutputChannelFormat")]
        Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode);

        [NativeSymbol("FMOD_DSP_Reset")]
        Result DSP_Reset(IntPtr dsp);

        [NativeSymbol("FMOD_DSP_SetParameterFloat")]
        Result DSP_SetParameterFloat(IntPtr dsp, int index, float value);

        [NativeSymbol("FMOD_DSP_SetParameterInt")]
        Result DSP_SetParameterInt(IntPtr dsp, int index, int value);

        [NativeSymbol("FMOD_DSP_SetParameterBool")]
        Result DSP_SetParameterBool(IntPtr dsp, int index, bool value);

        [NativeSymbol("FMOD_DSP_SetParameterData")]
        Result DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);

        [NativeSymbol("FMOD_DSP_GetParameterFloat")]
        Result DSP_GetParameterFloat(IntPtr dsp, int index, out float value, byte* valuestr, int valuestrlen);

        [NativeSymbol("FMOD_DSP_GetParameterInt")]
        Result DSP_GetParameterInt(IntPtr dsp, int index, out int value, byte* valuestr, int valuestrlen);

        [NativeSymbol("FMOD_DSP_GetParameterBool")]
        Result DSP_GetParameterBool(IntPtr dsp, int index, out bool value, byte* valuestr, int valuestrlen);

        [NativeSymbol("FMOD_DSP_GetParameterData")]
        Result DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);

        [NativeSymbol("FMOD_DSP_GetNumParameters")]
        Result DSP_GetNumParameters(IntPtr dsp, out int numparams);

        [NativeSymbol("FMOD_DSP_GetParameterInfo")]
        Result DSP_GetParameterInfo(IntPtr dsp, int index, out IntPtr desc);

        [NativeSymbol("FMOD_DSP_GetDataParameterIndex")]
        Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index);

        [NativeSymbol("FMOD_DSP_ShowConfigDialog")]
        Result DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);

        [NativeSymbol("FMOD_DSP_GetInfo")]
        Result DSP_GetInfo(IntPtr dsp, IntPtr name, out FmodVersion version, out int channels, out int configwidth, out int configheight);

        [NativeSymbol("FMOD_DSP_GetType")]
        Result DSP_GetType(IntPtr dsp, out DSPType type);

        [NativeSymbol("FMOD_DSP_GetIdle")]
        Result DSP_GetIdle(IntPtr dsp, out bool idle);

        [NativeSymbol("FMOD_DSP_SetUserData")]
        Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);

        [NativeSymbol("FMOD_DSP_GetUserData")]
        Result DSP_GetUserData(IntPtr dsp, out IntPtr userdata);

        [NativeSymbol("FMOD_DSP_SetMeteringEnabled")]
        Result DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);

        [NativeSymbol("FMOD_DSP_GetMeteringEnabled")]
        Result DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);

        [NativeSymbol("FMOD_DSP_GetMeteringInfo")]
        Result DSP_GetMeteringInfo(IntPtr dsp, out DSPMeteringInfo inputInfo, out DSPMeteringInfo outputInfo);

        #endregion

        #region DSPConnection Class Functions

        [NativeSymbol("FMOD_DSPConnection_GetInput")]
        Result DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input);

        [NativeSymbol("FMOD_DSPConnection_GetOutput")]
        Result DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output);

        [NativeSymbol("FMOD_DSPConnection_SetMix")]
        Result DSPConnection_SetMix(IntPtr dspconnection, float volume);

        [NativeSymbol("FMOD_DSPConnection_GetMix")]
        Result DSPConnection_GetMix(IntPtr dspconnection, out float volume);

        [NativeSymbol("FMOD_DSPConnection_SetMixMatrix")]
        Result DSPConnection_SetMixMatrix(IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

        [NativeSymbol("FMOD_DSPConnection_GetMixMatrix")]
        Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

        [NativeSymbol("FMOD_DSPConnection_GetType")]
        Result DSPConnection_GetType(IntPtr dspconnection, out DSPConnectionType type);

        [NativeSymbol("FMOD_DSPConnection_SetUserData")]
        Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

        [NativeSymbol("FMOD_DSPConnection_GetUserData")]
        Result DSPConnection_GetUserData(IntPtr dspconnection, out IntPtr userdata);

        #endregion

        #region Geometry Class Functions

        [NativeSymbol("FMOD_Geometry_Release")]
        Result Geometry_Release(IntPtr geometry);

        [NativeSymbol("FMOD_Geometry_AddPolygon")]
        Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector* vertices, out int polygonindex);

        [NativeSymbol("FMOD_Geometry_GetNumPolygons")]
        Result Geometry_GetNumPolygons(IntPtr geometry, out int numpolygons);

        [NativeSymbol("FMOD_Geometry_GetMaxPolygons")]
        Result Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices);

        [NativeSymbol("FMOD_Geometry_GetPolygonNumVertices")]
        Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);

        [NativeSymbol("FMOD_Geometry_SetPolygonVertex")]
        Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, ref Vector vertex);

        [NativeSymbol("FMOD_Geometry_GetPolygonVertex")]
        Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out Vector vertex);

        [NativeSymbol("FMOD_Geometry_SetPolygonAttributes")]
        Result Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

        [NativeSymbol("FMOD_Geometry_GetPolygonAttributes")]
        Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);

        [NativeSymbol("FMOD_Geometry_SetActive")]
        Result Geometry_SetActive(IntPtr geometry, bool active);

        [NativeSymbol("FMOD_Geometry_GetActive")]
        Result Geometry_GetActive(IntPtr geometry, out bool active);

        [NativeSymbol("FMOD_Geometry_SetRotation")]
        Result Geometry_SetRotation(IntPtr geometry, ref Vector forward, ref Vector up);

        [NativeSymbol("FMOD_Geometry_GetRotation")]
        Result Geometry_GetRotation(IntPtr geometry, out Vector forward, out Vector up);

        [NativeSymbol("FMOD_Geometry_SetPosition")]
        Result Geometry_SetPosition(IntPtr geometry, ref Vector position);

        [NativeSymbol("FMOD_Geometry_GetPosition")]
        Result Geometry_GetPosition(IntPtr geometry, out Vector position);

        [NativeSymbol("FMOD_Geometry_SetScale")]
        Result Geometry_SetScale(IntPtr geometry, ref Vector scale);

        [NativeSymbol("FMOD_Geometry_GetScale")]
        Result Geometry_GetScale(IntPtr geometry, out Vector scale);

        [NativeSymbol("FMOD_Geometry_Save")]
        Result Geometry_Save(IntPtr geometry, void* data, out int datasize);

        [NativeSymbol("FMOD_Geometry_SetUserData")]
        Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

        [NativeSymbol("FMOD_Geometry_GetUserData")]
        Result Geometry_GetUserData(IntPtr geometry, out IntPtr userdata);

        #endregion
    }
}
