#pragma warning disable CA1815, CA1034

using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Codec
{
    public class CodecDescription
    {
        internal Structure _struct;

        private string managedName;
        private Memory.SaferPointer NameBuffer;

        private CodecOpenCallback openCallback;
        private CodecCloseCallback closeCallback;
        private CodecReadCallback readCallback;
        private CodecGetLengthCallback getLengthCallback;
        private CodecSetPositionCallback setPositionCallback;
        private CodecGetPositionCallback getPositionCallback;
        private CodecSoundCreateCallback soundCreateCallback;
        private CodecGetWaveFormatCallback getWaveFormatCallback;

        public string Name
        {
            get => managedName;

            set
            {
                if (value is null)
                {
                    managedName = null;
                    NameBuffer = null;
                    _struct.Name = default;
                }
                else
                {
                    NameBuffer = FmodHelpers.StringToPointer(value, Encoding.UTF8);
                    managedName = value;
                    _struct.Name = NameBuffer;
                }
            }
        }

        public FmodVersion Version { get => _struct.Version; set => _struct.Version = value; }

        public bool DefaultAsStream { get => _struct.DefaultAsStream != 0; set => _struct.DefaultAsStream = value ? 1 : 0; }

        public TimeUnit TimeUnits { get => _struct.TimeUnits; set => _struct.TimeUnits = value; }

        public CodecOpenCallback OpenCallback
        {
            get => openCallback;

            set => FmodHelpers.UpdateCallback(value, out openCallback, out _struct.Open);
        }

        public CodecCloseCallback CloseCallback
        {
            get => closeCallback;
            set => FmodHelpers.UpdateCallback(value, out closeCallback, out _struct.Close);
        }

        public CodecReadCallback ReadCallback
        {
            get => readCallback;
            set => FmodHelpers.UpdateCallback(value, out readCallback, out _struct.Read);
        }

        public CodecGetLengthCallback GetLengthCallback
        {
            get => getLengthCallback;
            set => FmodHelpers.UpdateCallback(value, out getLengthCallback, out _struct.GetLength);
        }

        public CodecSetPositionCallback SetPositionCallback
        {
            get => setPositionCallback;
            set => FmodHelpers.UpdateCallback(value, out setPositionCallback, out _struct.SetPosition);
        }

        public CodecGetPositionCallback GetPositionCallback
        {
            get => getPositionCallback;
            set => FmodHelpers.UpdateCallback(value, out getPositionCallback, out _struct.GetPosition);
        }

        public CodecSoundCreateCallback CreateSoundFormat
        {
            get => soundCreateCallback;
            set => FmodHelpers.UpdateCallback(value, out soundCreateCallback, out _struct.SoundCreate);
        }

        public CodecGetWaveFormatCallback GetWaveFormatCallback
        {
            get => getWaveFormatCallback;
            set => FmodHelpers.UpdateCallback(value, out getWaveFormatCallback, out _struct.GetWaveFormat);
        }

        public struct Structure
        {
            public IntPtr Name;
            public FmodVersion Version;
            public int DefaultAsStream;
            public TimeUnit TimeUnits;
            public IntPtr Open;
            public IntPtr Close;
            public IntPtr Read;
            public IntPtr GetLength;
            public IntPtr SetPosition;
            public IntPtr GetPosition;
            public IntPtr SoundCreate;
            public IntPtr GetWaveFormat;
        }
    }
}
