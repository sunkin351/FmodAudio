using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using FmodAudio;
using FmodAudio.Dsp;

namespace Examples
{
    using Base;

    //Warning, this example is not in working condition
    public unsafe class CustomDSPExample : Example
    {
        public class MyCustomDSP : FmodAudio.Dsp.UserDefinedDsp
        {
            private static readonly ParameterDescription[] paramDescriptions = new ParameterDescription[]
            {
                new DataParameterDescription("wave data", null, ParameterDataType.User),
                new FloatParameterDescription("volume", "%", 0, 1, 1)
            };

            private float[] buffer;
            private float volumeLinear = 1.0f;
            private int lengthSamples, channels;
            public readonly ReaderWriterLockSlim DspLock = new ReaderWriterLockSlim();

            public MyCustomDSP(FmodSystem system, string name)
                : base(system, name, new FmodVersion(1, 0, 0), 1, 1, paramDescriptions, DSPProcessType.Read)
            {
            }

            public ReadOnlySpan<float> Data => this.buffer.AsSpan(0, this.lengthSamples * this.channels);

            public float Volume
            {
                get => this.volumeLinear;
                set => this.volumeLinear = Math.Clamp(value, 0f, 1f);
            }

            public int Channels => this.channels;

            public int LengthSamples => this.lengthSamples;

            protected override Result Create(DspState* state)
            {
                var res = state->GetBlockSize(out uint size);

                if (res != Result.Ok)
                {
                    return res;
                }

                this.lengthSamples = (int)size;
                this.buffer = new float[(int)size * 8];

                return Result.Ok;
            }

            protected override unsafe Result Release(DspState* state)
            {
                DspLock.Dispose();
                return Result.Ok;
            }

            protected override Result Reset(DspState* state)
            {
                this.buffer.AsSpan().Clear();
                return Result.Ok;
            }

            // A little look into C# intrinsics, .NET Core 3+ only
            private static void AdjustVolumeAllSamples(float* inbuffer, float* outbuffer, int length, float volume)
            {
                int i = 0;

                if (Avx.IsSupported)
                {
                    Vector256<float> volVec = Vector256.Create(volume);

                    while (length - i >= Vector256<float>.Count)
                    {
                        Vector256<float> tmp = Avx.Multiply(volVec, Avx.LoadVector256(inbuffer + i)); //Load from input, multiply by volume

                        Avx.Store(outbuffer + i, tmp); //Store in output

                        i += Vector256<float>.Count; //Increment index by the number of vector elements
                    }

                    if (length - i >= Vector128<float>.Count)
                    {
                        Vector128<float> tmp = Sse.Multiply(volVec.GetLower(), Sse.LoadVector128(inbuffer + i));
                        Sse.Store(outbuffer + i, tmp);

                        i += Vector128<float>.Count;
                    }
                }
                else if (Sse.IsSupported)
                {
                    Vector128<float> volVec = Vector128.Create(volume); //Broadcast the volume value across all vector elements

                    while (length - i >= Vector128<float>.Count)
                    {
                        Vector128<float> tmp = Sse.Multiply(volVec, Sse.LoadVector128(inbuffer + i)); //Load from input, multiply by volume

                        Sse.Store(outbuffer + i, tmp); //Store in output

                        i += Vector128<float>.Count;
                    }
                }

                //process remaining, if any
                while (i < length)
                {
                    outbuffer[i] = volume * inbuffer[i];
                    i += 1;
                }
            }

            protected override unsafe Result Read(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, ref int outChannels)
            {
                Debug.Assert(inChannels == outChannels);

                int lengthInFloats = (int)length * inChannels;

                AdjustVolumeAllSamples(inBuffer, outBuffer, lengthInFloats, this.volumeLinear);

                if (inChannels <= 8 && this.DspLock.TryEnterWriteLock(0))
                {
                    try
                    {
                        var outSpan = new Span<float>(outBuffer, lengthInFloats);

                        outSpan.CopyTo(this.buffer);

                        this.channels = inChannels;
                        this.lengthSamples = (int)length;
                    }
                    finally
                    {
                        this.DspLock.ExitWriteLock();
                    }
                }

                return Result.Ok;
            }

            protected override Result ShouldIProcess(DspState* state, bool inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode)
            {
                return Result.Ok;
            }
        }

        //static unsafe Result MyDSPGetParameterDataCallback(DspState* state, int index, out IntPtr data, out uint length, IntPtr _)
        //{
        //    data = default;
        //    length = default;

        //    if (index != 0)
        //    {
        //        return Result.Err_Invalid_Param;
        //    }
            
        //    var res = state->Functions->GetBlockSize.Invoke(state, out uint blockSize);
        //    if (res != Result.Ok)
        //    {
        //        return res;
        //    }

        //    data = state->plugindata;
        //    length = blockSize * 2 * sizeof(float);

        //    return Result.Ok;
        //}

        //static unsafe Result MyDSPGetParameterFloat(DspState* state, int index, out float value, IntPtr _)
        //{
        //    if (index != 1)
        //    {
        //        value = default;
        //        return Result.Err_Invalid_Param;
        //    }

        //    value = ((MyDSPData*)state->plugindata)->VolumeLinear;
        //    return Result.Ok;
        //}

        //static unsafe Result MyDSPSetParameterFloat(DspState* state, int index, float value)
        //{
        //    if (index != 1)
        //        return Result.Err_Invalid_Param;

        //    ((MyDSPData*)state->plugindata)->VolumeLinear = value;
        //    return Result.Ok;
        //}

        //private unsafe DSP CreateCustomDSP()
        //{
        //    ParameterDescription WaveDataDesc= new DataParameterDescription("wave data", null, ParameterDataType.User);
        //    ParameterDescription VolumeDesc = new FloatParameterDescription("volume", "%", 0, 1, 1);

        //    var dspDesc = new DspDescription()
        //    {
        //        Name = "My Dsp",
        //        PluginSDKVersion = Fmod.PluginSDKVersion,
        //        Version = new FmodVersion(1, 0, 0),
                
        //        InputBufferCount = 1,
        //        OutputBufferCount = 1,

        //        CreateCallback = MyDSPCreateCallback,
        //        ReleaseCallback = MyDSPReleaseCallback,
        //        ReadCallback = MyDSPCallback,
        //        GetParamDataCallback = MyDSPGetParameterDataCallback,
        //        SetParamFloatCallback = MyDSPSetParameterFloat,
        //        GetParamFloatCallback = MyDSPGetParameterFloat
        //    };

        //    dspDesc.SetParameterDescriptions(WaveDataDesc, VolumeDesc);

        //    return System.CreateDSP(dspDesc);
        //}

        private Sound sound;
        private Channel channel;
        private MyCustomDSP dsp;
        private ChannelGroup masterGroup;

        public CustomDSPExample() : base("Fmod Custom DSP Example")
        {
            RegisterCommand(ConsoleKey.D1, () => dsp.Bypass = !dsp.Bypass);
            RegisterCommand(ConsoleKey.D2, () => AdjustVolume(dsp, -0.1f));
            RegisterCommand(ConsoleKey.D3, () => AdjustVolume(dsp, 0.1f));
            RegisterCommand(ConsoleKey.Spacebar, () => channel.Paused = !channel.Paused);
        }

        public override void Initialize()
        {
            base.Initialize();

            System.Init(32);

            sound = System.CreateSound(MediaPath("stereo.ogg"), Mode.Loop_Normal);

            channel = System.PlaySound(sound, paused: true);

            dsp = new MyCustomDSP(System, "MyDSP");

            masterGroup = System.MasterChannelGroup;

            masterGroup.AddDSP(0, dsp);

            dsp.Bypass = true;
        }

        public unsafe override void Run()
        {
            Span<char> display = stackalloc char[50];

            var ParamIndex = dsp.GetDataParameterIndex(ParameterDataType.User);

            channel.Paused = false;

            do
            {
                OnUpdate();

                System.Update();

                bool bypass = dsp.Bypass;

                ParameterDescription desc = dsp.GetParameterInfo(1);

                var data = dsp.Data;

                string volstr = (dsp.Volume * 100).ToString("N1");

                DrawText("==================================================");
                DrawText("Custom DSP Example.");
                DrawText("Copyright (c) Firelight Technologies 2004-2018.");
                DrawText("==================================================");
                DrawText();
                DrawText("Press 1 to toggle filter bypass");
                DrawText("Press 2 to decrease volume 10%");
                DrawText("Press 3 to increase volume 10%");
                DrawText("Press Esc to Quit");
                DrawText();
                DrawText("Filter is " + (bypass ? "inactive" : "active"));
                DrawText("Volume is " + volstr + "%");

                if (data.Length != 0)
                {
                    Debug.Assert((uint)dsp.Channels <= 10);

                    float[] levels;

                    dsp.DspLock.EnterReadLock();

                    try
                    {
                        levels = DetermineChannelLevels(data, dsp.Channels, dsp.LengthSamples);
                    }
                    finally
                    {
                        dsp.DspLock.ExitReadLock();
                    }

                    int channel = 0;

                    do
                    {
                        display.Clear();

                        channel.TryFormat(display.Slice(0, 2), out _);

                        display.Slice(3, Math.Min((int)(levels[channel] * 40f), display.Length - 3)).Fill('=');

                        DrawText(display);
                    }
                    while (++channel < levels.Length);
                }
                
                Sleep(50);
            }
            while (!ShouldExit);
        }

        public override void Dispose()
        {
            sound?.Dispose();

            if (dsp != null)
            {
                masterGroup.RemoveDSP(dsp);
                dsp.Dispose();
            }

            base.Dispose();
        }

        private static void AdjustVolume(MyCustomDSP dsp, float adjustment)
        {
            dsp.Volume = Math.Clamp(dsp.Volume + adjustment, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe float[] DetermineChannelLevels(ReadOnlySpan<float> buffer, int channels, int sampleCount)
        {
            if (channels * sampleCount > buffer.Length)
            {
                //Throw some error here
            }

            fixed (float* pBuffer = buffer)
            {
                if (Sse2.IsSupported)
                {
                    return SSE2(pBuffer, channels, sampleCount);
                }

                return Software(pBuffer, channels, sampleCount);
            }

            static float[] SSE2(float* buffer, int channels, int sampleCount)
            {
                int channelIndex = 0;

                float[] levels = new float[channels];

                fixed (float* pLevels = levels)
                {
                    if (channels >= Vector128<float>.Count)
                    {
                        Vector128<float> AbsConst = Vector128.Create(int.MaxValue).AsSingle();

                        do
                        {
                            Vector128<float> max = Vector128<float>.Zero;

                            for (int sample = 0; sample < sampleCount; ++sample)
                            {
                                Vector128<float> tmp = Sse.And(AbsConst, Sse.LoadVector128(buffer + (channels * sample + channelIndex)));

                                max = Sse.Max(max, tmp);
                            }

                            Sse.Store(pLevels + channelIndex, max);

                            channelIndex += Vector128<float>.Count;
                        }
                        while (channels - channelIndex >= Vector128<float>.Count);
                    }

                    while (channelIndex < channels)
                    {
                        float max = 0f;

                        for (int sample = 0; sample < sampleCount; ++sample)
                        {
                            float tmp = Math.Abs(buffer[channels * sample + channelIndex]);

                            if (tmp > max)
                                max = tmp;
                        }

                        pLevels[channelIndex] = max;

                        channelIndex += 1;
                    }
                }

                return levels;
            }

            static float[] Software(float* buffer, int channels, int sampleCount)
            {
                float[] levels = new float[channels];

                for (int chan = 0; chan < channels; ++chan)
                {
                    float max = 0f;

                    for (int sample = 0; sample < sampleCount; ++sample)
                    {
                        float tmp = Math.Abs(buffer[channels * sample + chan]);

                        if (tmp > max)
                        {
                            max = tmp;
                        }
                    }

                    levels[chan] = max;
                }

                return levels;
            }
        }
    }
}
