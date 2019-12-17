using System;
using System.Collections.Concurrent;

namespace FmodAudio
{
    using Dsp;

    public partial class FmodSystem
    {
        private readonly ConcurrentDictionary<IntPtr, WeakReference<Sound>> SoundLookup = new ConcurrentDictionary<IntPtr, WeakReference<Sound>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<SoundGroup>> SoundGroupLookup = new ConcurrentDictionary<IntPtr, WeakReference<SoundGroup>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<ChannelGroup>> ChannelGroupLookup = new ConcurrentDictionary<IntPtr, WeakReference<ChannelGroup>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<DSP>> DSPLookup = new ConcurrentDictionary<IntPtr, WeakReference<DSP>>();
        private readonly ConcurrentDictionary<IntPtr, WeakReference<Geometry>> GeometryLookup = new ConcurrentDictionary<IntPtr, WeakReference<Geometry>>();

        internal Sound GetSound(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            Sound sound;

            if (SoundLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out sound))
                {
                    return sound;
                }

                goto InvalidState;
            }

            sound = new Sound(this, handle);
            if (SoundLookup.TryAdd(handle, new WeakReference<Sound>(sound)))
            {
                return sound;
            }
            
            InvalidState:
            throw new InvalidOperationException();
        }

        internal void ReleaseSound(IntPtr inst)
        {
            library.Sound_Release(inst).CheckResult();
            SoundLookup.TryRemove(inst, out _);
        }

        internal SoundGroup GetSoundGroup(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            SoundGroup group;

            if (SoundGroupLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out group))
                {
                    return group;
                }

                goto InvalidState;
            }

            group = new SoundGroup(this, handle);
            if (SoundGroupLookup.TryAdd(handle, new WeakReference<SoundGroup>(group)))
            {
                return group;
            }
            
            InvalidState:
            throw new InvalidOperationException();
        }

        internal void ReleaseSoundGroup(IntPtr inst, bool isMaster)
        {
            if (!isMaster)
            {
                library.SoundGroup_Release(inst).CheckResult();
            }

            SoundGroupLookup.TryRemove(inst, out _);
        }

        internal ChannelGroup GetChannelGroup(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            ChannelGroup group;

            if (ChannelGroupLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out group))
                {
                    return group;
                }

                goto InvalidState;
            }

            group = new ChannelGroup(this, handle);
            if (ChannelGroupLookup.TryAdd(handle, new WeakReference<ChannelGroup>(group)))
            {
                return group;
            }
            
            InvalidState:
            throw new InvalidOperationException();
        }

        internal void ReleaseChannelGroup(IntPtr handle, bool isMaster)
        {
            if (!isMaster)
            {
                library.ChannelGroup_Release(handle).CheckResult();
            }

            ChannelGroupLookup.TryRemove(handle, out _);
        }

        internal Dsp.DSP GetDSP(IntPtr handle, bool ownsObjectIfNotFound = true)
        {
            if (handle == IntPtr.Zero)
                return null;

            DSP dsp;

            if (DSPLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out dsp))
                {
                    return dsp;
                }

                goto InvalidState;
            }

            dsp = new DSP(this, handle, ownsObjectIfNotFound);

            if (ownsObjectIfNotFound)
            {
                if (DSPLookup.TryAdd(handle, new WeakReference<DSP>(dsp)))
                {
                    return dsp;
                }
            }
            else
            {
                return dsp;
            }

            InvalidState:
            throw new InvalidOperationException();
        }

        internal void ReleaseDSP(IntPtr handle)
        {
            library.DSP_Release(handle).CheckResult();
            DSPLookup.TryRemove(handle, out _);
        }

        internal Geometry GetGeometry(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            Geometry geometry;
            if (GeometryLookup.TryGetValue(handle, out var wref))
            {
                if (wref.TryGetTarget(out geometry))
                {
                    return geometry;
                }

                goto InvalidState;
            }

            geometry = new Geometry(handle, library);
            if (GeometryLookup.TryAdd(handle, new WeakReference<Geometry>(geometry)))
            {
                return geometry;
            }

            InvalidState:
            throw new InvalidOperationException();
        }

        internal void ReleaseGeometry(IntPtr handle)
        {
            library.Geometry_Release(handle).CheckResult();
            GeometryLookup.TryRemove(handle, out _);
        }

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
