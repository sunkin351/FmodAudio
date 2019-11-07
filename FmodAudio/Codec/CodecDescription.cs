#pragma warning disable CA1815, CA1034

using System;
using System.Collections.Generic;
using System.Text;

namespace FmodAudio.Codec
{
    public class CodecDescription
    {
        internal Structure Struct;

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
                    Struct.Name = default;
                }
                else
                {
                    NameBuffer = FmodHelpers.StringToPointer(value, Encoding.UTF8);
                    managedName = value;
                    Struct.Name = NameBuffer;
                }
            }
        }

        public FmodVersion Version { get => Struct.Version; set => Struct.Version = value; }

        public bool DefaultAsStream { get => Struct.DefaultAsStream != 0; set => Struct.DefaultAsStream = value ? 1 : 0; }

        public TimeUnit TimeUnits { get => Struct.TimeUnits; set => Struct.TimeUnits = value; }

        public CodecOpenCallback OpenCallback
        {
            get => openCallback;

            set => FmodHelpers.UpdateCallback(value, out openCallback, out Struct.Open);
        }

        public CodecCloseCallback CloseCallback
        {
            get => closeCallback;
            set => FmodHelpers.UpdateCallback(value, out closeCallback, out Struct.Close);
        }

        public CodecReadCallback ReadCallback
        {
            get => readCallback;
            set => FmodHelpers.UpdateCallback(value, out readCallback, out Struct.Read);
        }

        public CodecGetLengthCallback GetLengthCallback
        {
            get => getLengthCallback;
            set => FmodHelpers.UpdateCallback(value, out getLengthCallback, out Struct.GetLength);
        }

        public CodecSetPositionCallback SetPositionCallback
        {
            get => setPositionCallback;
            set => FmodHelpers.UpdateCallback(value, out setPositionCallback, out Struct.SetPosition);
        }

        public CodecGetPositionCallback GetPositionCallback
        {
            get => getPositionCallback;
            set => FmodHelpers.UpdateCallback(value, out getPositionCallback, out Struct.GetPosition);
        }

        public CodecSoundCreateCallback CreateSoundFormat
        {
            get => soundCreateCallback;
            set => FmodHelpers.UpdateCallback(value, out soundCreateCallback, out Struct.SoundCreate);
        }

        public CodecGetWaveFormatCallback GetWaveFormatCallback
        {
            get => getWaveFormatCallback;
            set => FmodHelpers.UpdateCallback(value, out getWaveFormatCallback, out Struct.GetWaveFormat);
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
