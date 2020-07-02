#pragma warning disable IDE0059, CA1034

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable

namespace FmodAudio
{
    public sealed unsafe class Sound : HandleBase
    {
        internal static Sound FromNewHandle(FmodSystem system, IntPtr handle, bool ownsHandle = true)
        {
            return FromHandle(handle) ?? new Sound(system, handle, ownsHandle);
        }

        internal static unsafe Sound? FromHandle(IntPtr handle)
        {
            IntPtr ptr;
            Fmod.UserDataMethods.Sound_GetUserData(handle, &ptr).CheckResult();

            if (ptr != default)
            {
                GCHandle gcHandle = (GCHandle)ptr;

                if (gcHandle.IsAllocated && gcHandle.Target is Sound sound)
                {
                    return sound;
                }
            }

            return null;
        }

        private readonly Interop.NativeLibrary library = Fmod.Library;

        public FmodSystem SystemObject { get; }

        public Sound? Parent { get; internal set; } = null;

        private string? name;
        private Memory.SaferPointer? CustomRolloff3D = null; //Pointer to an unmanaged array of Vector
        private readonly List<Sound>? Subsounds = null;
        internal SoundGroup? soundGroup = null;

        internal Sound(FmodSystem sys, IntPtr inst, bool ownsHandle = true) : base(inst, ownsHandle)
        {
            SystemObject = sys;

            if (!ownsHandle)
            {
                return;
            }

            int count;
            library.Sound_GetNumSubSounds(inst, &count).CheckResult();

            if (count != 0)
            {
                Subsounds = new List<Sound>(count);

                for (int i = 0; i < count; ++i)
                {
                    library.Sound_GetSubSound(Handle, i, out IntPtr subsoundHandle).CheckResult();

                    Subsounds.Add(new Sound(sys, subsoundHandle, false) { Parent = this });
                }
            }
        }

        protected override void ReleaseImpl()
        {
            if (!IsSubsound)
            {
                library.Sound_Release(Handle).CheckResult();
            }
        }

        public bool IsSubsound => Parent != null;

        public SoundLock Lock(uint offset, uint length)
        {
            var loc = new SoundLock(this);
            library.Sound_Lock(Handle, offset, length, out loc.Ptr1, out loc.Ptr2, out loc.Len1, out loc.Len2).CheckResult();

            return loc;
        }

        public void Unlock(SoundLock Lock)
        {
            if (Lock.Parent != this)
            {
                throw new ArgumentException("Lock does not belong to this Sound object");
            }

            library.Sound_Unlock(Handle, Lock.Ptr1, Lock.Ptr2, Lock.Len1, Lock.Len2).CheckResult();

            Lock.IsValid = false;
        }

        public void SetDefaults(float frequency, int priority)
        {
            library.Sound_SetDefaults(Handle, frequency, priority).CheckResult();
        }

        public void GetDefaults(out float frequency, out int priority)
        {
            library.Sound_GetDefaults(Handle, out frequency, out priority).CheckResult();
        }

        public void Set3DMinMaxDistance(float min, float max)
        {
            library.Sound_Set3DMinMaxDistance(Handle, min, max).CheckResult();
        }

        public void Get3DMinMaxDistance(out float min, out float max)
        {
            library.Sound_Get3DMinMaxDistance(Handle, out min, out max).CheckResult();
        }

        public void Set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            library.Sound_Set3DConeSettings(Handle, insideconeangle, outsideconeangle, outsidevolume).CheckResult();
        }

        public void Get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            library.Sound_Get3DConeSettings(Handle, out insideconeangle, out outsideconeangle, out outsidevolume).CheckResult();
        }
        
        public unsafe Span<Vector3> GetCustomRolloff3D()
        {
            library.Sound_Get3DCustomRolloff(Handle, out IntPtr points, out int Number).CheckResult();

            if (points == IntPtr.Zero)
            {
                return default;
            }
            else
            {
                return new Span<Vector3>((void*)points, Number);
            }
        }

        public void SetCustomRolloff3D(ReadOnlySpan<Vector3> points)
        {
            int num = 0;
            Memory.SaferPointer arrptr = FmodHelpers.AllocateCustomRolloff(points);

            if (arrptr != null)
            {
                num = points.Length;
            }
            else if(CustomRolloff3D == null)
            {
                return;
            }

            library.Sound_Set3DCustomRolloff(Handle, arrptr, num).CheckResult();

            CustomRolloff3D = arrptr;
        }
        
        public int SubSoundCount
        {
            get
            {
                if (Subsounds != null)
                {
                    return Subsounds.Count;
                }

                library.Sound_GetNumSubSounds(Handle, out int number).CheckResult();

                return number;
            }
        }

        public Sound GetSubSound(int index)
        {
            if (Subsounds != null)
            {
                return Subsounds[index];
            }

            int total = SubSoundCount;

            if ((uint)index >= (uint)total)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            library.Sound_GetSubSound(Handle, index, out IntPtr handle).CheckResult();

            return new Sound(SystemObject, handle, false)
            {
                Parent = this
            };
        }

        public unsafe string Name
        {
            get
            {
                if (name == null)
                {
                    const int buflen = FmodSystem.MaxInteropNameStringLength;

                    byte* buf = stackalloc byte[buflen];

                    library.Sound_GetName(Handle, buf, buflen).CheckResult();

                    name = FmodHelpers.PtrToString(buf, buflen);
                }

                return name;
            }
        }

        public uint GetLength(TimeUnit unit)
        {
            library.Sound_GetLength(Handle, out uint value, unit).CheckResult();

            return value;
        }

        public void GetFormat(out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            library.Sound_GetFormat(Handle, out type, out format, out channels, out bits).CheckResult();
        }

        public void GetTagCount(out int TagCount, out int UpdatedTagCount)
        {
            library.Sound_GetNumTags(Handle, out TagCount, out UpdatedTagCount).CheckResult();
        }

        public void GetTag(string name, int index, out Tag tag)
        {
            library.Sound_GetTag(Handle, name, index, out tag).CheckResult();
        }

        public void GetOpenState(out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
        {
            openstate = default;
            percentbuffered = default;
            starving = default;
            diskbusy = default;

            library.Sound_GetOpenState(Handle, out openstate, out percentbuffered, out starving, out diskbusy).CheckResult();
        }

        public unsafe uint ReadData<T>(Span<T> buffer) where T: unmanaged
        {
            if (buffer.IsEmpty)
                return 0;

            fixed(T* ptr = buffer)
            {
                return ReadDataInternal(ptr, checked((uint)buffer.Length * (uint)sizeof(T)));
            }
        }

        public unsafe uint ReadData(IntPtr buffer, int blength)
        {
            if (blength < 0)
                throw new ArgumentOutOfRangeException(nameof(blength));

            if (blength == 0)
                return 0;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return ReadDataInternal((void*)buffer, (uint)blength);
        }

        /// <summary>
        /// Reads data from this sound object into the provided buffer
        /// </summary>
        /// <param name="buffer"> Buffer to read the data into </param>
        /// <param name="blength"> Length of the buffer in bytes </param>
        /// <returns>The number of bytes read</returns>
        private unsafe uint ReadDataInternal(void* buffer, uint blength)
        {
            library.Sound_ReadData(Handle, buffer, blength, out uint read).CheckResult();

            return read;
        }

        public void SeekData(uint pcm)
        {
            library.Sound_SeekData(Handle, pcm).CheckResult();
        }

        public SoundGroup? SoundGroup
        {
            get
            {
                library.Sound_GetSoundGroup(Handle, out IntPtr handle).CheckResult();

                return handle == default ? null : (SoundGroup.FromHandle(handle) ?? new SoundGroup(SystemObject, handle, false));
            }

            set
            {
                var handle = value != null ? value.Handle : IntPtr.Zero;

                library.Sound_SetSoundGroup(Handle, handle).CheckResult();

                soundGroup = value;
            }
        }

        public int SyncPointCount
        {
            get
            {
                library.Sound_GetNumSyncPoints(Handle, out int syncpoints).CheckResult();

                return syncpoints;
            }
        }

        public SyncPoint GetSyncPoint(int index)
        {
            library.Sound_GetSyncPoint(Handle, index, out IntPtr handle).CheckResult();

            return new SyncPoint(this, handle);
        }
        
        public unsafe void GetSyncPointInfo(SyncPoint point, TimeUnit unit, out string name, out uint offset)
        {
            const int buflen = FmodSystem.MaxInteropNameStringLength;
            
            if(point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            else if(point.ParentSound != this)
            {
                throw new ArgumentException("SyncPoint is not of this Sound object.");
            }

            byte* buffer = stackalloc byte[buflen];

            library.Sound_GetSyncPointInfo(Handle, point.Handle, buffer, buflen, out offset, unit).CheckResult();

            name = FmodHelpers.PtrToString(buffer, buflen);
        }

        public unsafe SyncPoint CreateSyncPoint(string name, TimeUnit unit, uint offset)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            library.Sound_AddSyncPoint(Handle, offset, unit, name, out IntPtr handle).CheckResult();
            return new SyncPoint(this, handle);
        }

        public void DeleteSyncPoint(SyncPoint point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            else if (point.ParentSound != this)
            {
                throw new ArgumentException("SyncPoint is not of this Sound object.");
            }

            library.Sound_DeleteSyncPoint(Handle, point.Handle).CheckResult();
        }

        // Functions also in Channel class but here they are the 'default' to save having to change it in Channel all the time.
        public Mode Mode
        {
            get
            {
                library.Sound_GetMode(Handle, out Mode mode).CheckResult();

                return mode;
            }

            set
            {
                library.Sound_SetMode(Handle, value).CheckResult();
            }
        }

        public int LoopCount
        {
            get
            {
                library.Sound_GetLoopCount(Handle, out int value).CheckResult();

                return value;
            }

            set
            {
                library.Sound_SetLoopCount(Handle, value).CheckResult();
            }
        }

        public void SetLoopPoints(TimeUnit startUnit, uint start, TimeUnit endUnit, uint end)
        {
            library.Sound_SetLoopPoints(Handle, start, startUnit, end, endUnit).CheckResult();
        }

        public void GetLoopPoints(TimeUnit startunit, out uint start, TimeUnit endunit, out uint end)
        {
            library.Sound_GetLoopPoints(Handle, out start, startunit, out end, endunit).CheckResult();
        }

        //For MOD/S3M/XM/IT/MID sequenced formats only.
        public int MusicChannelCount
        {
            get
            {
                library.Sound_GetMusicNumChannels(Handle, out int value).CheckResult();

                return value;
            }
        }

        public float GetMusicChannelVolume(int channel)
        {
            library.Sound_GetMusicChannelVolume(Handle, channel, out float volume).CheckResult();

            return volume;
        }

        public void SetMusicChannelVolume(int channel, float volume)
        {
            library.Sound_SetMusicChannelVolume(Handle, channel, volume).CheckResult();
        }

        public float MusicSpeed
        {
            get
            {
                library.Sound_GetMusicSpeed(Handle, out float speed).CheckResult();

                return speed;
            }

            set
            {
                library.Sound_SetMusicSpeed(Handle, value).CheckResult();
            }
        }

        internal override unsafe IntPtr UserData
        {
            get
            {
                IntPtr userData;
                Fmod.UserDataMethods.Sound_GetUserData(Handle, &userData).CheckResult();

                return userData;
            }

            set
            {
                Fmod.UserDataMethods.Sound_SetUserData(Handle, value).CheckResult();
            }
        }

        public sealed class SoundLock
        {
            public Sound Parent { get; }
            internal IntPtr Ptr1, Ptr2;
            internal uint Len1, Len2;
            internal bool IsValid;

            internal SoundLock(Sound parent)
            {
                Parent = parent;
                IsValid = true;
            }

            public IntPtr Pointer1
            {
                get
                {
                    if (!IsValid)
                        ThrowInvalid();

                    return Ptr1;
                }
            }
            public IntPtr Pointer2
            {
                get
                {
                    if (!IsValid)
                        ThrowInvalid();

                    return Ptr2;
                }
            }

            /// <summary>
            /// Pointer1's Length in bytes
            /// </summary>
            public uint Length1 => Len1;

            /// <summary>
            /// Pointer2's Length in bytes
            /// </summary>
            public uint Length2 => Len2;

            public unsafe Span<T> Span1<T>() where T: unmanaged
            {
                if (!IsValid)
                    ThrowInvalid();

                return new Span<T>((void*)Ptr1, (int)(Len1 / (uint)sizeof(T)));
            }

            public unsafe Span<T> Span2<T>() where T: unmanaged
            {
                if (!IsValid)
                    ThrowInvalid();

                if (Length2 == 0)
                    return default;

                return new Span<T>((void*)Ptr2, (int)(Len2 / (uint)sizeof(T)));
            }

            private static void ThrowInvalid()
            {
                throw new InvalidOperationException("Sound Lock no longer valid");
            }
        }

        public sealed class SyncPoint
        {
            public Sound ParentSound { get; }
            internal IntPtr Handle;

            internal SyncPoint(Sound parent, IntPtr handle)
            {
                ParentSound = parent;
                Handle = handle;
            }
        }
    }
}
