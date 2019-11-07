using AdvancedDLSupport;
using System;
using System.Runtime.InteropServices;
using System.Numerics;
namespace FmodAudio.Interop
{
    using Dsp;
    [NativeSymbols(Prefix = "FMOD_")]
    public unsafe interface INativeLibrary
    {
        //Global Functions
        Result Memory_Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags);
        
        Result Memory_GetStats(int* currentalloced, int* maxalloced, bool blocking);
        
        Result Debug_Initialize(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename);
        
        Result File_SetDiskBusy(int busy);
        
        Result File_GetDiskBusy(int* busy);

        //System Class Functions

        Result System_Create(IntPtr* System);
        
        Result System_Release(IntPtr system);
        
        Result System_SetOutput(IntPtr system, OutputType output);
        
        Result System_GetOutput(IntPtr system, OutputType* output);
        
        Result System_GetNumDrivers(IntPtr system, int* numdrivers);
        
        Result System_GetDriverInfo(IntPtr system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels);
        
        Result System_SetDriver(IntPtr system, int driver);
        
        Result System_GetDriver(IntPtr system, int* driver);
        
        Result System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels);
        
        Result System_GetSoftwareChannels(IntPtr system, int* numsoftwarechannels);
        
        Result System_SetSoftwareFormat(IntPtr system, int samplerate, SpeakerMode speakermode, int numrawspeakers);
        
        Result System_GetSoftwareFormat(IntPtr system, int* samplerate, SpeakerMode* speakermode, int* numrawspeakers);
        
        Result System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers);
        
        Result System_GetDSPBufferSize(IntPtr system, uint* bufferlength, int* numbuffers);
        
        Result System_SetFileSystem(IntPtr system, FileOpenCallback useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek, FileAsyncReadCallback userasyncread, FileAsyncCancelCallback userasynccancel, int blockalign);
        
        Result System_AttachFileSystem(IntPtr system, FileOpenCallback useropen, FileCloseCallback userclose, FileReadCallback userread, FileSeekCallback userseek);
        
        Result System_SetAdvancedSettings(IntPtr system, AdvancedSettings.Structure* settings);
        
        Result System_GetAdvancedSettings(IntPtr system, AdvancedSettings.Structure* settings);
        
        Result System_SetCallback(IntPtr system, SystemCallback callback, SystemCallbackType callbackmask);
        
        Result System_SetPluginPath(IntPtr system, byte* path);
        
        Result System_LoadPlugin(IntPtr system, byte* filename, uint* handle, uint priority);
        
        Result System_UnloadPlugin(IntPtr system, uint handle);
        
        Result System_GetNumNestedPlugins(IntPtr system, uint handle, int* count);
        
        Result System_GetNestedPlugin(IntPtr system, uint handle, int index, uint* nestedhandle);
        
        Result System_GetNumPlugins(IntPtr system, PluginType plugintype, int* numplugins);
        
        Result System_GetPluginHandle(IntPtr system, PluginType plugintype, int index, uint* handle);
        
        Result System_GetPluginInfo(IntPtr system, uint handle, PluginType* plugintype, byte* name, int namelen, FmodVersion* version);
        
        Result System_CreateDSPByPlugin(IntPtr system, uint handle, IntPtr* dsp);
        
        Result System_SetOutputByPlugin(IntPtr system, uint handle);
        
        Result System_GetOutputByPlugin(IntPtr system, uint* handle);
        
        Result System_GetDSPInfoByPlugin(IntPtr system, uint handle, IntPtr* description);
        
        Result System_RegisterCodec(IntPtr system, Codec.CodecDescription.Structure* description, uint* handle, uint priority);
        
        Result System_RegisterDSP(IntPtr system, DspDescription.Structure* description, uint* handle);
        
        //Result System_RegisterOutput(IntPtr system, Output.OutputDescription.Structure* description, uint* handle);
        
        Result System_Init(IntPtr system, int maxchannels, InitFlags flags, IntPtr extradriverdata);
        
        Result System_Close(IntPtr system);
        
        Result System_Update(IntPtr system);
        
        Result System_SetSpeakerPosition(IntPtr system, Speaker speaker, float x, float y, bool active);
        
        Result System_GetSpeakerPosition(IntPtr system, Speaker speaker, float* x, float* y, out bool active);
        
        Result System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TimeUnit filebuffersizetype);
        
        Result System_GetStreamBufferSize(IntPtr system, uint* filebuffersize, TimeUnit* filebuffersizetype);
        
        Result System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);
        
        Result System_Get3DSettings(IntPtr system, float* dopplerscale, float* distancefactor, float* rolloffscale);
        
        Result System_Set3DNumListeners(IntPtr system, int numlisteners);
        
        Result System_Get3DNumListeners(IntPtr system, int* numlisteners);
        
        Result System_Set3DListenerAttributes(IntPtr system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);
        
        Result System_Get3DListenerAttributes(IntPtr system, int listener, Vector3* pos, Vector3* vel, Vector3* forward, Vector3* up);
        
        Result System_Set3DRolloffCallback(IntPtr system, _3DRolloffCallback callback);
        
        Result System_MixerSuspend(IntPtr system);
        
        Result System_MixerResume(IntPtr system);
        
        Result System_GetDefaultMixMatrix(IntPtr system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float* matrix, int matrixhop);
        
        Result System_GetSpeakerModeChannels(IntPtr system, SpeakerMode mode, int* channels);
        
        Result System_GetVersion(IntPtr system, FmodVersion* version);
        
        Result System_GetOutputHandle(IntPtr system, IntPtr* handle);
        
        Result System_GetChannelsPlaying(IntPtr system, int* channels, int* realchannels);
        
        Result System_GetCPUUsage(IntPtr system, float* dsp, float* stream, float* geometry, float* update, float* total);
        
        Result System_GetFileUsage(IntPtr system, long* sampleBytesRead, long* streamBytesRead, long* otherBytesRead);
        
        Result System_CreateSound(IntPtr system, byte* name_or_data, Mode mode, CreateSoundInfo.Structure* exinfo, IntPtr* sound);
        
        Result System_CreateStream(IntPtr system, byte* name_or_data, Mode mode, CreateSoundInfo.Structure* exinfo, IntPtr* sound);
        
        Result System_CreateDSP(IntPtr system, DspDescription.Structure* description, IntPtr* dspHandle);
        
        Result System_CreateDSPByType(IntPtr system, DSPType type, IntPtr* dspHandle);
        
        Result System_CreateChannelGroup(IntPtr system, byte* name, IntPtr* channelgroup);
        
        Result System_CreateSoundGroup(IntPtr system, byte* name, IntPtr* soundgroup);
        
        Result System_CreateReverb3D(IntPtr system, IntPtr* reverb);
        
        Result System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, IntPtr* channel);
        
        Result System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, IntPtr* channel);

        Result System_GetChannel(IntPtr system, int channelid, IntPtr* channel);

        Result System_GetMasterChannelGroup(IntPtr system, IntPtr* channelgroup);

        Result System_GetMasterSoundGroup(IntPtr system, IntPtr* soundgroup);

        Result System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);

        Result System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);

        Result System_SetReverbProperties(IntPtr system, int instance, ReverbProperties* prop);

        Result System_GetReverbProperties(IntPtr system, int instance, ReverbProperties* prop);

        Result System_LockDSP(IntPtr system);

        Result System_UnlockDSP(IntPtr system);

        Result System_GetRecordNumDrivers(IntPtr system, int* numdrivers, int* numconnected);

        Result System_GetRecordDriverInfo(IntPtr system, int id, byte* name, int namelen, Guid* guid, int* systemrate, SpeakerMode* speakermode, int* speakermodechannels, DriverState* state);

        Result System_GetRecordPosition(IntPtr system, int id, uint* position);

        Result System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);

        Result System_RecordStop(IntPtr system, int id);

        Result System_IsRecording(IntPtr system, int id, out bool recording);

        Result System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, IntPtr* geometry);
        
        Result System_SetGeometrySettings(IntPtr system, float maxworldsize);
        
        Result System_GetGeometrySettings(IntPtr system, float* maxworldsize);
        
        Result System_LoadGeometry(IntPtr system, byte* data, int datasize, IntPtr* geometry);
        
        Result System_GetGeometryOcclusion(IntPtr system, Vector3* listener, Vector3* source, float* direct, float* reverb);
        
        Result System_SetNetworkProxy(IntPtr system, byte* proxy);
        
        Result System_GetNetworkProxy(IntPtr system, byte* proxy, int proxylen);
        
        Result System_SetNetworkTimeout(IntPtr system, int timeout);
        
        Result System_GetNetworkTimeout(IntPtr system, int* timeout);
        
        Result System_SetUserData(IntPtr system, IntPtr userdata);
        
        Result System_GetUserData(IntPtr system, IntPtr* userdata);
        
        //Sound Class Functions

        Result Sound_Release(IntPtr sound);
        
        Result Sound_Lock(IntPtr sound, uint offset, uint length, IntPtr* ptr1, IntPtr* ptr2, uint* len1, uint* len2);
        
        Result Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);
        
        Result Sound_SetDefaults(IntPtr sound, float frequency, int priority);
        
        Result Sound_GetDefaults(IntPtr sound, float* frequency, int* priority);
        
        Result Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);
        
        Result Sound_Get3DMinMaxDistance(IntPtr sound, float* min, float* max);
        
        Result Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        
        Result Sound_Get3DConeSettings(IntPtr sound, float* insideconeangle, float* outsideconeangle, float* outsidevolume);
        
        Result Sound_Set3DCustomRolloff(IntPtr sound, IntPtr points, int numpoints);
        
        Result Sound_Get3DCustomRolloff(IntPtr sound, IntPtr* points, int* numpoints);
        
        Result Sound_GetSubSound(IntPtr sound, int index, IntPtr* subsound);
        
        Result Sound_GetSubSoundParent(IntPtr sound, IntPtr* parentsound);
        
        Result Sound_GetName(IntPtr sound, byte* name, int namelen);
        
        Result Sound_GetLength(IntPtr sound, uint* length, TimeUnit lengthtype);
        
        Result Sound_GetFormat(IntPtr sound, SoundType* type, SoundFormat* format, int* channels, int* bits);
        
        Result Sound_GetNumSubSounds(IntPtr sound, int* numsubsounds);
        
        Result Sound_GetNumTags(IntPtr sound, int* numtags, int* numtagsupdated);
        
        Result Sound_GetTag(IntPtr sound, string name, int index, Tag* tag);
        
        Result Sound_GetOpenState(IntPtr sound, OpenState* openstate, uint* percentbuffered, int* starving, int* diskbusy);
        
        Result Sound_ReadData(IntPtr sound, void* buffer, uint length, uint* read);
        
        Result Sound_SeekData(IntPtr sound, uint pcm);
        
        Result Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);
        
        Result Sound_GetSoundGroup(IntPtr sound, IntPtr* soundgroup);
        
        Result Sound_GetNumSyncPoints(IntPtr sound, int* numsyncpoints);
        
        Result Sound_GetSyncPoint(IntPtr sound, int index, IntPtr* point);
        
        Result Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, byte* name, int namelen, uint* offset, TimeUnit offsettype);
        
        Result Sound_AddSyncPoint(IntPtr sound, uint offset, TimeUnit offsettype, byte* name, IntPtr* point);
        
        Result Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);
        
        Result Sound_SetMode(IntPtr sound, Mode mode);
        
        Result Sound_GetMode(IntPtr sound, Mode* mode);
        
        Result Sound_SetLoopCount(IntPtr sound, int loopcount);
        
        Result Sound_GetLoopCount(IntPtr sound, int* loopcount);
        
        Result Sound_SetLoopPoints(IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        
        Result Sound_GetLoopPoints(IntPtr sound, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);
        
        Result Sound_GetMusicNumChannels(IntPtr sound, int* numchannels);
        
        Result Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);
        
        Result Sound_GetMusicChannelVolume(IntPtr sound, int channel, float* volume);
        
        Result Sound_SetMusicSpeed(IntPtr sound, float speed);
        
        Result Sound_GetMusicSpeed(IntPtr sound, float* speed);
        
        Result Sound_SetUserData(IntPtr sound, IntPtr userdata);
        
        Result Sound_GetUserData(IntPtr sound, IntPtr* userdata);

        // SoundGroup Class Functions

        Result SoundGroup_Release(IntPtr soundgroup);
        
        Result SoundGroup_GetSystemObject(IntPtr soundgroup, IntPtr* system);
        
        Result SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);
        
        Result SoundGroup_GetMaxAudible(IntPtr soundgroup, int* maxaudible);
        
        Result SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior behavior);
        
        Result SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior* behavior);
        
        Result SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);
        
        Result SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, float* speed);
        
        Result SoundGroup_SetVolume(IntPtr soundgroup, float volume);
        
        Result SoundGroup_GetVolume(IntPtr soundgroup, float* volume);
        
        Result SoundGroup_Stop(IntPtr soundgroup);
        
        Result SoundGroup_GetName(IntPtr soundgroup, byte* name, int namelen);
        
        Result SoundGroup_GetNumSounds(IntPtr soundgroup, int* numsounds);
        
        Result SoundGroup_GetSound(IntPtr soundgroup, int index, IntPtr* sound);
        
        Result SoundGroup_GetNumPlaying(IntPtr soundgroup, int* numplaying);
        
        Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);
        
        Result SoundGroup_GetUserData(IntPtr soundgroup, IntPtr* userdata);
        
        //Channel Class Functions

        Result Channel_SetFrequency(IntPtr channel, float frequency);
        
        Result Channel_GetFrequency(IntPtr channel, float* frequency);
        
        Result Channel_SetPriority(IntPtr channel, int priority);
        
        Result Channel_GetPriority(IntPtr channel, int* priority);
        
        Result Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);
        
        Result Channel_GetChannelGroup(IntPtr channel, IntPtr* channelgroup);
        
        Result Channel_IsVirtual(IntPtr channel, out bool isvirtual);
        
        Result Channel_GetCurrentSound(IntPtr channel, IntPtr* sound);
        
        Result Channel_GetIndex(IntPtr channel, int* index);
        
        Result Channel_SetPosition(IntPtr channel, uint position, TimeUnit postype);
        
        Result Channel_GetPosition(IntPtr channel, uint* position, TimeUnit postype);
        
        Result Channel_SetMode(IntPtr channel, Mode mode);
        
        Result Channel_GetMode(IntPtr channel, Mode* mode);
        
        Result Channel_SetLoopCount(IntPtr channel, int loopcount);
        
        Result Channel_GetLoopCount(IntPtr channel, int* loopcount);
        
        Result Channel_SetLoopPoints(IntPtr channel, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        
        Result Channel_GetLoopPoints(IntPtr channel, uint* loopstart, TimeUnit loopstarttype, uint* loopend, TimeUnit loopendtype);
        
        Result Channel_SetUserData(IntPtr channel, IntPtr userdata);
        
        Result Channel_GetUserData(IntPtr channel, IntPtr* userdata);
        
        //ChannelControl Class Functions

        Result ChannelGroup_Stop(IntPtr channelgroup);
        
        Result ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);
        
        Result ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);
        
        Result ChannelGroup_GetVolume(IntPtr channelgroup, float* volume);
        
        Result ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);
        
        Result ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);
        
        Result ChannelGroup_GetAudibility(IntPtr channelgroup, float* audibility);
        
        Result ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);
        
        Result ChannelGroup_GetPitch(IntPtr channelgroup, float* pitch);
        
        Result ChannelGroup_SetMute(IntPtr channelgroup, bool mute);
        
        Result ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);
        
        Result ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);
        
        Result ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, float* wet);
        
        Result ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);
        
        Result ChannelGroup_GetLowPassGain(IntPtr channelgroup, float* gain);
        
        Result ChannelGroup_SetMode(IntPtr channelgroup, Mode mode);
        
        Result ChannelGroup_GetMode(IntPtr channelgroup, Mode* mode);
        
        Result ChannelGroup_SetCallback(IntPtr channelgroup, ChannelCallback callback);
        
        Result ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);
        
        Result ChannelGroup_SetPan(IntPtr channelgroup, float pan);
        
        Result ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        
        Result ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float* levels, int numlevels);
        
        Result ChannelGroup_SetMixMatrix(IntPtr channelgroup, float* matrix, int outchannels, int inchannels, int inchannel_hop);
        
        Result ChannelGroup_GetMixMatrix(IntPtr channelgroup, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);
        
        Result ChannelGroup_GetDSPClock(IntPtr channelgroup, ulong* dspclock, ulong* parentclock);
        
        Result ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);
        
        Result ChannelGroup_GetDelay(IntPtr channelgroup, ulong* dspclock_start, ulong* dspclock_end, int* stopchannels);
        
        Result ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);
        
        Result ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);
        
        Result ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);
        
        Result ChannelGroup_GetFadePoints(IntPtr channelgroup, uint* numpoints, ulong* point_dspclock, float* point_volume);
        
        Result ChannelGroup_Set3DAttributes(IntPtr channelgroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);
        
        Result ChannelGroup_Get3DAttributes(IntPtr channelgroup, Vector3* pos, Vector3* vel, Vector3* alt_pan_pos);
        
        Result ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);
        
        Result ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, float* mindistance, float* maxdistance);
        
        Result ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        
        Result ChannelGroup_Get3DConeSettings(IntPtr channelgroup, float* insideconeangle, float* outsideconeangle, float* outsidevolume);
        
        Result ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, Vector3* orientation);
        
        Result ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, Vector3* orientation);
        
        Result ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, IntPtr points, int numpoints);
        
        Result ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, IntPtr* points, int* numpoints);
        
        Result ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);
        
        Result ChannelGroup_Get3DOcclusion(IntPtr channelgroup, float* directocclusion, float* reverbocclusion);
        
        Result ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);
        
        Result ChannelGroup_Get3DSpread(IntPtr channelgroup, float* angle);
        
        Result ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);
        
        Result ChannelGroup_Get3DLevel(IntPtr channelgroup, float* level);
        
        Result ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);
        
        Result ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, float* level);
        
        Result ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, int custom, float customLevel, float centerFreq);
        
        Result ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, int* custom, float* customLevel, float* centerFreq);
        
        Result ChannelGroup_GetSystemObject(IntPtr channelgroup, IntPtr* system);

        
        Result ChannelGroup_SetVolume(IntPtr channelgroup, float volume);
        
        Result ChannelGroup_GetDSP(IntPtr channelgroup, int index, IntPtr* dsp);
        
        Result ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);
        
        Result ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);
        
        Result ChannelGroup_GetNumDSPs(IntPtr channelgroup, int* numdsps);
        
        Result ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);
        
        Result ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, int* index);
        
        Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);
        
        Result ChannelGroup_GetUserData(IntPtr channelgroup, IntPtr* userdata);
        
        //ChannelGroup Specific Functions
        
        Result ChannelGroup_Release(IntPtr Handle);
        
        Result ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, int propagatedspclock, IntPtr* connection);
        
        Result ChannelGroup_GetNumGroups(IntPtr channelgroup, int* numgroups);
        
        Result ChannelGroup_GetGroup(IntPtr channelgroup, int index, IntPtr* group);
        
        Result ChannelGroup_GetParentGroup(IntPtr channelgroup, IntPtr* group);
        
        Result ChannelGroup_GetName(IntPtr channelgroup, byte* name, int namelen);
        
        Result ChannelGroup_GetNumChannels(IntPtr channelgroup, int* numchannels);
        
        Result ChannelGroup_GetChannel(IntPtr channelgroup, int index, IntPtr* channel);
        
        //DSP Class Functions

        Result DSP_Release(IntPtr dsp);
        
        Result DSP_GetSystemObject(IntPtr dsp, IntPtr* system);
        
        Result DSP_AddInput(IntPtr dsp, IntPtr target, IntPtr* connection, DSPConnectionType type);
        
        Result DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);
        
        Result DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);
        
        Result DSP_GetNumInputs(IntPtr dsp, int* numinputs);
        
        Result DSP_GetNumOutputs(IntPtr dsp, int* numoutputs);
        
        Result DSP_GetInput(IntPtr dsp, int index, IntPtr* input, IntPtr* inputconnection);
        
        Result DSP_GetOutput(IntPtr dsp, int index, IntPtr* output, IntPtr* outputconnection);
        
        Result DSP_SetActive(IntPtr dsp, bool active);
        
        Result DSP_GetActive(IntPtr dsp, out bool active);
        
        Result DSP_SetBypass(IntPtr dsp, bool bypass);
        
        Result DSP_GetBypass(IntPtr dsp, out bool bypass);
        
        Result DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);
        
        Result DSP_GetWetDryMix(IntPtr dsp, float* prewet, float* postwet, float* dry);
        
        Result DSP_SetChannelFormat(IntPtr dsp, ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode);
        
        Result DSP_GetChannelFormat(IntPtr dsp, ChannelMask* channelmask, int* numchannels, SpeakerMode* source_speakermode);
        
        Result DSP_GetOutputChannelFormat(IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, int* outchannels, out SpeakerMode outspeakermode);
        
        Result DSP_Reset(IntPtr dsp);
        
        Result DSP_SetParameterFloat(IntPtr dsp, int index, float value);
        
        Result DSP_SetParameterBool(IntPtr dsp, int index, int value);
        
        Result DSP_SetParameterInt(IntPtr dsp, int index, int value);
        
        Result DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);
        
        Result DSP_GetParameterFloat(IntPtr dsp, int index, float* value, byte* valuestr, int valuestrlen);
        
        Result DSP_GetParameterBool(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);
        
        Result DSP_GetParameterint(IntPtr dsp, int index, int* value, byte* valuestr, int valuestrlen);
        
        Result DSP_GetParameterData(IntPtr dsp, int index, IntPtr* data, uint* length, byte* valuestr, int valuestrlen);
        
        Result DSP_GetNumParameters(IntPtr dsp, int* numparams);
        
        Result DSP_GetParameterInfo(IntPtr dsp, int index, Dsp.Interop.ParameterDescriptionStruct** desc);
        
        Result DSP_GetDataParameterIndex(IntPtr dsp, int datatype, int* index);
        
        Result DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);
        
        Result DSP_GetInfo(IntPtr dsp, byte* name, FmodVersion* version, int* channels, int* configwidth, int* configheight);
        
        Result DSP_GetType(IntPtr dsp, DSPType* type);
        
        Result DSP_GetIdle(IntPtr dsp, out bool idle);
        
        Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);
        
        Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);
        
        Result DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);
        
        Result DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);
        
        Result DSP_GetMeteringInfo(IntPtr dsp, DSPMeteringInfo* inputInfo, DSPMeteringInfo* outputInfo);
        
        //DSPConnection Class Functions

        Result DSPConnection_GetInput(IntPtr dspconnection, IntPtr* input);
        
        Result DSPConnection_GetOutput(IntPtr dspconnection, IntPtr* output);
        
        Result DSPConnection_SetMix(IntPtr dspconnection, float volume);
        
        Result DSPConnection_GetMix(IntPtr dspconnection, float* volume);
        
        Result DSPConnection_SetMixMatrix(IntPtr dspconnection, float* matrix, int outchannels, int inchannels, int inchannel_hop);
        
        Result DSPConnection_GetMixMatrix(IntPtr dspconnection, float* matrix, int* outchannels, int* inchannels, int inchannel_hop);
        
        Result DSPConnection_GetType(IntPtr dspconnection, DSPConnectionType* type);
        
        Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);
        
        Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);

        //Geometry Class Functions
        
        Result Geometry_Release(IntPtr geometry);
        
        Result Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector3* vertices, int* polygonindex);
        
        Result Geometry_GetNumPolygons(IntPtr geometry, int* numpolygons);
        
        Result Geometry_GetMaxPolygons(IntPtr geometry, int* maxpolygons, int* maxvertices);
        
        Result Geometry_GetPolygonNumVertices(IntPtr geometry, int index, int* numvertices);
        
        Result Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);
        
        Result Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, Vector3* vertex);
        
        Result Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);
        
        Result Geometry_GetPolygonAttributes(IntPtr geometry, int index, float* directocclusion, float* reverbocclusion, int* doublesided);
        
        Result Geometry_SetActive(IntPtr geometry, bool active);
        
        Result Geometry_GetActive(IntPtr geometry, out bool active);
        
        Result Geometry_SetRotation(IntPtr geometry, Vector3* forward, Vector3* up);
        
        Result Geometry_GetRotation(IntPtr geometry, Vector3* forward, Vector3* up);
        
        Result Geometry_SetPosition(IntPtr geometry, Vector3* position);
        
        Result Geometry_GetPosition(IntPtr geometry, Vector3* position);
        
        Result Geometry_SetScale(IntPtr geometry, Vector3* scale);
        
        Result Geometry_GetScale(IntPtr geometry, Vector3* scale);
        
        Result Geometry_Save(IntPtr geometry, void* data, int* datasize);
        
        Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);
        
        Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);
    }
}
