using System;

using AdvancedDLSupport;

namespace FmodAudio.Interop
{
    [NativeSymbols(Prefix = "FMOD_")]
    public unsafe interface IUserDataMethods
    {
        Result System_SetUserData(IntPtr system, IntPtr userdata);

        Result System_GetUserData(IntPtr system, IntPtr* userdata);

        Result Sound_SetUserData(IntPtr sound, IntPtr userdata);

        Result Sound_GetUserData(IntPtr sound, IntPtr* userdata);

        Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

        Result SoundGroup_GetUserData(IntPtr soundgroup, IntPtr* userdata);

        Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

        Result ChannelGroup_GetUserData(IntPtr channelgroup, IntPtr* userdata);

        Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);

        Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);

        Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

        Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);

        Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

        Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);

        Result Reverb3D_SetUserData(IntPtr reverb3D, IntPtr userdata);

        Result Reverb3D_GetUserData(IntPtr reverb3D, IntPtr* userdata);
    }
}
