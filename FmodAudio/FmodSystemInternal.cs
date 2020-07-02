using System;
using System.Collections.Concurrent;

namespace FmodAudio
{
    using System.Runtime.InteropServices;
    using Dsp;

    public partial class FmodSystem
    {
        private readonly ConcurrentDictionary<IntPtr, WeakReference<SoundGroup>> SoundGroupLookup = new ConcurrentDictionary<IntPtr, WeakReference<SoundGroup>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<ChannelGroup>> ChannelGroupLookup = new ConcurrentDictionary<IntPtr, WeakReference<ChannelGroup>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<Geometry>> GeometryLookup = new ConcurrentDictionary<IntPtr, WeakReference<Geometry>>();

        /// <summary>
        /// This is to make sure all Dsp Callbacks are kept alive for the lifetime of the plugin.
        /// </summary>
        private readonly ConcurrentDictionary<Plugin, DspDescription> UserRegisteredDSPs = new ConcurrentDictionary<Plugin, DspDescription>();

        /// <summary>
        /// Same purpose as UserRegisteredDSPs
        /// </summary>
        private readonly ConcurrentDictionary<Plugin, Codec.CodecDescription> UserRegisteredCodecs = new ConcurrentDictionary<Plugin, Codec.CodecDescription>();

    }
}
