using System;
using AdvancedDLSupport;
using AdvancedDLSupport.Loaders;

namespace FmodAudio.Interop
{
    public unsafe abstract class UserDataMethods : NativeLibraryBase, IUserDataMethods
    {
        protected UserDataMethods(string path, ImplementationOptions options, ILibraryLoader libLoader = null, ISymbolLoader symLoader = null) : base(path, options, libLoader, symLoader)
        {
        }

        public abstract Result ChannelGroup_GetUserData(IntPtr channelgroup, IntPtr* userdata);
        public abstract Result ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);
        public abstract Result DSPConnection_GetUserData(IntPtr dspconnection, IntPtr* userdata);
        public abstract Result DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);
        public abstract Result DSP_GetUserData(IntPtr dsp, IntPtr* userdata);
        public abstract Result DSP_SetUserData(IntPtr dsp, IntPtr userdata);
        public abstract Result Geometry_GetUserData(IntPtr geometry, IntPtr* userdata);
        public abstract Result Geometry_SetUserData(IntPtr geometry, IntPtr userdata);
        public abstract Result Reverb3D_GetUserData(IntPtr reverb3D, IntPtr* userdata);
        public abstract Result Reverb3D_SetUserData(IntPtr reverb3D, IntPtr userdata);
        public abstract Result SoundGroup_GetUserData(IntPtr soundgroup, IntPtr* userdata);
        public abstract Result SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);
        public abstract Result Sound_GetUserData(IntPtr sound, IntPtr* userdata);
        public abstract Result Sound_SetUserData(IntPtr sound, IntPtr userdata);
        public abstract Result System_GetUserData(IntPtr system, IntPtr* userdata);
        public abstract Result System_SetUserData(IntPtr system, IntPtr userdata);
    }
}
