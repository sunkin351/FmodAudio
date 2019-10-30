#pragma warning disable IDE1006
#pragma warning disable IDE0052 // Remove unread private members

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace FmodAudio
{
    public class CreateSoundInfo
    {
        internal _interopStruct Struct = default;

        private Memory.SaferPointer InclusionListMemory = null;

        private int[] InclusionListManaged = null;
        
        private Encoding encoding = Encoding.UTF8;

        private string DLSNameManaged = null;

        private Memory.SaferPointer DLSNameMemory = null;
        
        private string encryptionKey = null;

        private Memory.SaferPointer encryptionKeyMemory = null;

        private SoundGroup initSoundGroup;

        /// <summary>
        /// Affects how the encryption key will be translated to bytes, Default is UTF8
        /// </summary>
        public Encoding EncryptionKeyEncoding
        {
            get => encoding;
            set
            {
                if (value is null)
                    value = Encoding.UTF8;

                if (ReferenceEquals(encoding, value) || encoding.EncodingName == value.EncodingName)
                {
                    return;
                }
                
                encoding = value;

                if (encryptionKey != null)
                {
                    UpdateEncryptionKey();
                }
            }
        }
        
        public CreateSoundInfo()
        {
            Struct.cbsize = Unsafe.SizeOf<_interopStruct>();
        }

        /// <summary>
        /// [w] Optional. Size in bytes of file to load, or sound to create (in this case only if FMOD_OPENUSER is used).  Required if loading from memory.  If 0 is specified, then it will use the size of the file (unless loading from memory then an error will be returned).
        /// </summary>
        public uint Length { get => Struct.Length; set => Struct.Length = value; }
        /// <summary>
        /// [w] Optional. Offset from start of the file to start loading from.  This is useful for loading files from inside big data files.
        /// </summary>
        public uint FileOffset { get => Struct.FileOffset; set => Struct.FileOffset = value; }
        /// <summary>
        /// [w] Optional. Number of channels in a sound specified only if OPENUSER is used.
        /// </summary>
        public int ChannelCount { get => Struct.ChannelCount; set => Struct.ChannelCount = value; }
        /// <summary>
        /// [w] Optional. Default frequency of sound in a sound specified only if OPENUSER is used.  Other formats use the frequency determined by the file format.
        /// </summary>
        public int DefaultFrequency { get => Struct.DefaultFrequency; set => Struct.DefaultFrequency = value; }
        /// <summary>
        /// [w] Optional. Format of the sound specified only if OPENUSER is used.  Other formats use the format determined by the file format.
        /// </summary>
        public SoundFormat Format { get => Struct.Format; set => Struct.Format = value; }
        /// <summary>
        /// [w] Optional. For streams.
        /// This determines the size of the double buffer (in PCM samples) that a stream uses.
        /// Use this for user created streams if you want to determine the size of the callback buffer passed to you.
        /// Specify 0 to use FMOD's default size which is currently equivalent to 400ms of the sound format created/loaded.
        /// </summary>
        public uint DecodeBufferSize { get => Struct.DecodeBufferSize; set => Struct.DecodeBufferSize = value; }
        /// <summary>
        /// [w] Optional.
        /// In a multi-sample file format such as .FSB/.DLS/.SF2, specify the initial subsound to seek to, only if CREATESTREAM is used.
        /// </summary>
        public int InitialSubsound { get => Struct.InitialSubsound; set => Struct.InitialSubsound = value; }
        /// <summary>
        /// [w] Optional. Specify 0 to ignore or have no subsounds.
        /// In a user created multi-sample sound, specify the number of subsounds within the sound that are accessable with Sound.GetSubSound() / SoundGetSubSound().
        /// </summary>
        public int SubsoundCount { get => Struct.SubsoundCount; set => Struct.SubsoundCount = value; }
        /// <summary>
        /// [w] Optional.
        /// In a multi-sample format such as .FSB/.DLS/.SF2 it may be desirable to specify only a subset of sounds to be loaded out of the whole file.
        /// This is an array of subsound indicies to load into memory when created.
        /// </summary>
        public int[] InclusionList
        {
            get => InclusionListManaged.ArrayClone();
            set
            {
                if (value == null || value.Length == 0)
                {
                    InclusionListManaged = null;
                    InclusionListMemory = null;
                    Struct.InclusionList = IntPtr.Zero;
                    Struct.InclusionListCount = 0;
                    return;
                }

                int count = value.Length;
                InclusionListManaged = value;
                InclusionListMemory = Memory.Allocate(count * sizeof(int));
                value.CopyTo(InclusionListMemory.AsSpan<int>());
                Struct.InclusionList = InclusionListMemory;
                Struct.InclusionListCount = count;
            }
        }

        private SoundPCMReadCallback pcmReadCallback;

        /// <summary>
        /// [w] Optional.
        /// Callback to 'piggyback' on FMOD's read functions and accept or even write PCM data while FMOD is opening the sound.
        /// Used for user sounds created with OPENUSER or for capturing decoded data as FMOD reads it.
        /// </summary>
        public SoundPCMReadCallback PCMReadCallback
        {
            get => pcmReadCallback;
            set
            {
                Helpers.UpdateCallback(value, out pcmReadCallback, out Struct.PCMReadCallback);
            }
        }

        private SoundPCMSetPosCallback pcmSetPosCallback;

        /// <summary>
        /// [w] Optional. Callback for when the user calls a seeking function such as Channel::setPosition within a multi-sample sound, and for when it is opened.
        /// </summary>
        public SoundPCMSetPosCallback PCMSetPosCallback
        {
            get => pcmSetPosCallback;
            set
            {
                Helpers.UpdateCallback(value, out pcmSetPosCallback, out Struct.PCMSetPosCallback);
            }
        }

        private SoundNonBlockCallback nonBlockCallback;

        /// <summary>
        /// [w] Optional. Callback for successful completion, or error while loading a sound that used the FMOD_NONBLOCKING flag.
        /// </summary>
        public SoundNonBlockCallback NonBlockCallback
        {
            get => nonBlockCallback;
            set
            {
                Helpers.UpdateCallback(value, out nonBlockCallback, out Struct.NonBlockCallback);
            }
        }
        /// <summary>
        /// [w] Optional.
        /// Filename for a DLS or SF2 sample set when loading a MIDI file.
        /// If not specified, on windows it will attempt to open /windows/system32/drivers/gm.dls, otherwise the MIDI will fail to open.
        /// </summary>
        public string DLSName
        {
            get => DLSNameManaged;
            set
            {
                if (DLSNameManaged is null)
                {
                    if (string.IsNullOrEmpty(value))
                        return;
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        DLSNameManaged = null;
                        DLSNameMemory = null;
                        Struct.DLSName = IntPtr.Zero;
                        return;
                    }
                    else if (DLSNameManaged.Equals(value, StringComparison.Ordinal))
                        return;
                }

                DLSNameManaged = value;
                
                if (Helpers.StringToPointer(value, Encoding.UTF8, ref DLSNameMemory))
                {
                    Struct.DLSName = DLSNameMemory;
                }
            }
        }
        /// <summary>
        /// [w] Optional. Key for encrypted FSB file.  Without this key an encrypted FSB file will not load.
        /// </summary>
        public string EncryptionKey
        {
            get => encryptionKey;
            set
            {
                if (string.Equals(encryptionKey, value))
                    return;

                encryptionKey = string.IsNullOrEmpty(value) ? null : value;

                UpdateEncryptionKey();
            }
        }
        /// <summary>
        /// [w] Optional.
        /// For sequenced formats with dynamic channel allocation such as .MID and .IT, this specifies the maximum voice count allowed while playing.
        /// .IT defaults to 64.  .MID defaults to 32.
        /// </summary>
        public int MaxPolyphony { get => Struct.MaxPolyphony; set => Struct.MaxPolyphony = value; }
        /// <summary>
        /// [w] Optional. This is user data to be attached to the sound during creation.  Access via Sound.UserData.
        /// </summary>
        public IntPtr UserData { get => Struct.UserData; set => Struct.UserData = value; }
        /// <summary>
        /// [w] Optional. Instead of scanning all codec types, use this to speed up loading by making it jump straight to this codec.
        /// </summary>
        public SoundType SuggestedSoundType { get => Struct.SuggestedSoundType; set => Struct.SuggestedSoundType = value; }

        private FileOpenCallbackImpl fileOpenImpl;
        private FileOpenCallback fileOpen;

        /// <summary>
        /// [w] Optional. Callback for opening this file.
        /// </summary>
        public FileOpenCallback FileUserOpen
        {
            get => fileOpen;
            set
            {
                Helpers.UpdateCallback(value?.Wrap(), out fileOpenImpl, out Struct.FileUserOpen);
                fileOpen = value;
            }
        }

        private FileCloseCallback fileUserClose;

        /// <summary>
        /// [w] Optional. Callback for closing this file.
        /// </summary>
        public FileCloseCallback FileUserClose
        {
            get => fileUserClose;
            set
            {
                Helpers.UpdateCallback(value, out fileUserClose, out Struct.FileUserClose);
            }
        }

        private FileReadCallback fileUserRead;

        /// <summary>
        /// [w] Optional. Callback for reading from this file.
        /// </summary>
        public FileReadCallback FileUserRead
        {
            get => fileUserRead;
            set
            {
                Helpers.UpdateCallback(value, out fileUserRead, out Struct.FileUserRead);
            }
        }

        private FileSeekCallback fileUserSeek;

        /// <summary>
        /// [w] Optional. Callback for seeking within this file.
        /// </summary>
        public FileSeekCallback FileUserSeek
        {
            get => fileUserSeek;
            set
            {
                Helpers.UpdateCallback(value, out fileUserSeek, out Struct.FileUserSeek);
            }
        }

        private FileAsyncReadCallback fileUserAsyncRead;

        /// <summary>
        /// [w] Optional. Callback for asyncronously reading from this file.
        /// </summary>
        public FileAsyncReadCallback FileUserAsyncRead
        {
            get => fileUserAsyncRead;
            set
            {
                Helpers.UpdateCallback(value, out fileUserAsyncRead, out Struct.FileUserAsyncRead);
            }
        }

        private FileAsyncCancelCallback fileUserAsyncCancel;

        /// <summary>
        /// [w] Optional. Callback for cancelling an asyncronous read.
        /// </summary>
        public FileAsyncCancelCallback FileUserAsyncCancel
        {
            get => fileUserAsyncCancel;
            set
            {
                Helpers.UpdateCallback(value, out fileUserAsyncCancel, out Struct.FileUserAsyncCancel);
            }
        }
        
        /// <summary>
        /// [w] Optional. User data to be passed into the file callbacks.
        /// </summary>
        public IntPtr FileUserData { get => Struct.FileUserData; set => Struct.FileUserData = value; }
        
        /// <summary>
        /// [w] Optional. Buffer size for reading the file, -1 to disable buffering, or 0 for system default.
        /// </summary>
        public int FileBufferSize { get => Struct.FileBufferSize; set => Struct.FileBufferSize = value; }
        
        /// <summary>
        /// [w] Optional. Use this to differ the way fmod maps multichannel sounds to speakers.  See FMOD_CHANNELORDER for more.
        /// </summary>
        public ChannelOrder ChannelOrder { get => Struct.ChannelOrder; set => Struct.ChannelOrder = value; }

        /// <summary>
        /// [w] Optional. Specify a sound group if required, to put sound in as it is created.
        /// </summary>
        public SoundGroup InitialSoundGroup
        {
            get => initSoundGroup;
            set
            {
                initSoundGroup = value;
                Struct.InitialSoundGroup = value?.Handle ?? IntPtr.Zero;
            }
        }

        /// <summary>
        /// [w] Optional. For streams. Specify an initial position to seek the stream to.
        /// </summary>
        public uint InitialSeekPosition { get => Struct.InitialSeekPosition; set => Struct.InitialSeekPosition = value; }

        /// <summary>
        /// Optional. For streams. Specify the time unit for the position set in initialseekposition.
        /// </summary>
        public TimeUnit IntialSeekPosType { get => Struct.InitialSeekPosType; set => Struct.InitialSeekPosType = value; }

        /// <summary>
        /// [w] Optional. Set to true to use fmod's built in file system.
        /// Ignores setFileSystem callbacks and also FMOD_CREATESOUNEXINFO file callbacks.
        /// Useful for specific cases where you don't want to use your own file system but want to use fmod's file system (ie net streaming).
        /// </summary>
        public bool IgnoreSetFileSystem { get => Struct.IgnoreSetFilesystem > 0; set => Struct.IgnoreSetFilesystem = value ? 1 : 0; }

        /// <summary>
        /// [w] Optional. Specify 0 or FMOD_AUDIOQUEUE_CODECPOLICY_DEFAULT to ignore.
        /// Policy used to determine whether hardware or software is used for decoding.
        /// See FMOD_AUDIOQUEUE_CODECPOLICY for options (iOS >= 3.0 required, otherwise only hardware is available)
        /// </summary>
        public uint AudioQueuePolicy { get => Struct.AudioQueuePolicy; set => Struct.AudioQueuePolicy = value; }

        /// <summary>
        /// [w] Optional. Allows you to set a minimum desired MIDI mixer granularity.
        /// Values smaller than 512 give greater than default accuracy at the cost of more CPU and vise versa.
        /// Specify 0 for default (512 samples).
        /// </summary>
        public uint MinMidiGranularity { get => Struct.MinMidiGranularity; set => Struct.MinMidiGranularity = value; }

        /// <summary>
        /// [w] Optional. Specify 0 to ignore. Specifies a thread index to execute non blocking load on.
        /// Allows for up to 5 threads to be used for loading at once.  This is to avoid one load blocking another.  Maximum value = 4.
        /// </summary>
        public int NonblockingThreadID
        {
            get => Struct.Nonblockthreadid;
            set
            {
                if ((uint)value >= 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "NonBlocking Thread ID cannot be >= 5");
                }

                Struct.Nonblockthreadid = value;
            }
        }

        /// <summary>
        /// [r/w] Optional. Specify 0 to ignore. Allows you to provide the GUID lookup for cached FSB header info.
        /// Once loaded the GUID will be written back to the pointer. This is to avoid seeking and reading the FSB header.
        /// </summary>
        public IntPtr FSBGuid { get => Struct.FSBGUID; set => Struct.FSBGUID = value; }

        /// <summary>
        /// Optimized for changing both the value and encoding of the FSB Encryption key at once.
        /// </summary>
        /// <param name="value">The new string value for the key</param>
        /// <param name="encoding">The new encoding for the key</param>
        public void ChangeEncryptionKey(string value, Encoding encoding)
        {
            encryptionKey = string.IsNullOrEmpty(value) ? null : value;

            if (!ReferenceEquals(this.encoding, encoding) && this.encoding.EncodingName != encoding.EncodingName)
            {
                this.encoding = encoding;
            }

            UpdateEncryptionKey();
        }

        private void UpdateEncryptionKey()
        {
            if (encryptionKey is null)
            {
                encryptionKeyMemory = null;
                Struct.EncryptionKey = default;
                return;
            }

            bool tmp = Helpers.StringToPointer(encryptionKey, encoding, ref encryptionKeyMemory);

            if (tmp)
            {
                Struct.EncryptionKey = encryptionKeyMemory;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _interopStruct
        {
            /// <summary>
            /// Size of this structure.  This is used so the structure can be expanded in the future and still work on older versions of FMOD Ex.
            /// </summary>
            public int cbsize;
            
            public uint Length;
            
            public uint FileOffset;

            public int ChannelCount;

            public int DefaultFrequency;
            
            public SoundFormat Format;
            
            public uint DecodeBufferSize;

            public int InitialSubsound;
            
            public int SubsoundCount;

            public IntPtr InclusionList;

            public int InclusionListCount;
            
            public IntPtr PCMReadCallback;

            public IntPtr PCMSetPosCallback;

            public IntPtr NonBlockCallback;

            public IntPtr DLSName;

            public IntPtr EncryptionKey;

            public int MaxPolyphony;

            public IntPtr UserData;

            public SoundType SuggestedSoundType;
            
            public IntPtr FileUserOpen;
            
            public IntPtr FileUserClose;
            
            public IntPtr FileUserRead;
            
            public IntPtr FileUserSeek;
            
            public IntPtr FileUserAsyncRead;

            public IntPtr FileUserAsyncCancel;

            public IntPtr FileUserData;

            public int FileBufferSize;

            public ChannelOrder ChannelOrder;

            public IntPtr InitialSoundGroup;

            public uint InitialSeekPosition;

            public TimeUnit InitialSeekPosType;

            public int IgnoreSetFilesystem;

            public uint AudioQueuePolicy;

            public uint MinMidiGranularity;

            public int Nonblockthreadid;
            
            public IntPtr FSBGUID;
        }
    }
}
