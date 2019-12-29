using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FmodAudio;
using FmodAudio.Dsp;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Examples
{
    using Base;

    //Warning, this example is not in working condition
    public class CustomDSPExample : Example
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MyDSPData
        {
            public IntPtr Buffer;
            public float VolumeLinear;
            public int LengthSamples;
            public int Channels;
        }

        static unsafe ref T AsRef<T>(IntPtr ptr) where T: unmanaged
        {
            return ref AsRef<T>((void*)ptr);
        }

        static unsafe ref T AsRef<T>(void* ptr) where T: unmanaged
        {
            Debug.Assert(ptr != null, "Pointer was null");

            return ref Unsafe.AsRef<T>(ptr);
        }

        static unsafe Result MyDSPCallback(DspState* state, IntPtr inBuffer, IntPtr outBuffer, uint length, int inChannels, ref int outChannels)
        {
            Debug.Assert(inChannels == outChannels);

            if (state->plugindata == default)
            {
                new Span<byte>((void*)inBuffer, (int)length).CopyTo(new Span<byte>((void*)outBuffer, (int)length));

                return Result.Ok;
            }

            ref MyDSPData data = ref AsRef<MyDSPData>(state->plugindata);

            int LengthInFloats = (int)length / sizeof(float);

            //Span comes with bounding checks for bug detection. For greater speed, use pure pointers instead.
            var BufferData = new Span<float>((void*)data.Buffer, LengthInFloats);
            var InData = new Span<float>((void*)inBuffer, LengthInFloats);
            var OutData = new Span<float>((void*)outBuffer, LengthInFloats);

            int SampleCount = LengthInFloats / inChannels;


            //Feel free to unroll
            for(uint samp = 0; samp < SampleCount; ++samp)
            {
                for (int chan = 0; chan < outChannels; ++chan)
                {
                    int index = (int)samp * outChannels + chan;
                    BufferData[index] = OutData[index] = InData[index] * data.VolumeLinear;
                }
            }

            data.Channels = inChannels;

            return Result.Ok;
        }

        static unsafe Result MyDSPCreateCallback(DspState* state)
        {
            if (state->plugindata != default)
            {
                return Result.Ok;
            }

            //Writing this method sure felt like I was using C...
            var functions = state->Functions;
            var res = functions->GetBlockSize.Invoke(state, out uint blockSize);

            if (res != Result.Ok)
                return res;

            IntPtr dataPtr = functions->Alloc.Invoke((uint)sizeof(MyDSPData), MemoryType.Normal, IntPtr.Zero); //The original example used calloc, 
            if (dataPtr == IntPtr.Zero)
                return Result.Err_Memory;

            ref MyDSPData data = ref AsRef<MyDSPData>(dataPtr);

            data = default;

            state->plugindata = dataPtr;
            data.VolumeLinear = 1.0f;
            data.LengthSamples = (int)blockSize;

            data.Buffer = functions->Alloc.Invoke(blockSize * 8 * sizeof(float), MemoryType.Normal, IntPtr.Zero);

            if (data.Buffer == IntPtr.Zero)
            {
                state->plugindata = IntPtr.Zero;
                functions->Free.Invoke(dataPtr, MemoryType.Normal, IntPtr.Zero);
                return Result.Err_Memory;
            }

            return Result.Ok;
        }

        static unsafe Result MyDSPReleaseCallback(DspState* state)
        {
            if (state->plugindata != default)
            {
                var functions = state->Functions;
                var ptr = (MyDSPData*)state->plugindata;
                if (ptr->Buffer != default)
                {
                    functions->Free.Invoke(ptr->Buffer, MemoryType.Normal, default);
                }

                functions->Free.Invoke(state->plugindata, MemoryType.Normal, default);
            }

            return Result.Ok;
        }

        static unsafe Result MyDSPGetParameterDataCallback(DspState* state, int index, out IntPtr data, out uint length, IntPtr _)
        {
            data = default;
            length = default;

            if (index != 0)
            {
                return Result.Err_Invalid_Param;
            }
            
            var res = state->Functions->GetBlockSize.Invoke(state, out uint blockSize);
            if (res != Result.Ok)
            {
                return res;
            }

            data = state->plugindata;
            length = blockSize * 2 * sizeof(float);

            return Result.Ok;
        }

        static unsafe Result MyDSPGetParameterFloat(DspState* state, int index, out float value, IntPtr _)
        {
            if (index != 1)
            {
                value = default;
                return Result.Err_Invalid_Param;
            }

            value = ((MyDSPData*)state->plugindata)->VolumeLinear;
            return Result.Ok;
        }

        static unsafe Result MyDSPSetParameterFloat(DspState* state, int index, float value)
        {
            if (index != 1)
                return Result.Err_Invalid_Param;

            ((MyDSPData*)state->plugindata)->VolumeLinear = value;
            return Result.Ok;
        }

        private unsafe DSP CreateCustomDSP()
        {
            ParameterDescription WaveDataDesc = new DataParameterDescription("wave data", null, ParameterDataType.User);
            ParameterDescription VolumeDesc = new FloatParameterDescription("volume", "%", 0, 1, 1);

            var dspDesc = new DspDescription()
            {
                PluginSDKVersion = Fmod.PluginSDKVersion,
                Version = new FmodVersion(1, 0, 0),
                
                InputBufferCount = 1,
                OutputBufferCount = 1,

                CreateCallback = MyDSPCreateCallback,
                ReleaseCallback = MyDSPReleaseCallback,
                ReadCallback = MyDSPCallback,
                GetParamDataCallback = MyDSPGetParameterDataCallback,
                SetParamFloatCallback = MyDSPSetParameterFloat,
                GetParamFloatCallback = MyDSPGetParameterFloat
            };

            dspDesc.SetParameterDescriptions(WaveDataDesc, VolumeDesc);

            return System.CreateDSP(dspDesc);
        }

        private Sound sound;
        private Channel channel;
        private DSP dsp;
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

            dsp = CreateCustomDSP();

            masterGroup = System.MasterChannelGroup;

            masterGroup.AddDSP(0, dsp);
        }

        public unsafe override void Run()
        {
            Span<char> display = stackalloc char[50];

            var ParamIndex = dsp.GetDataParameterIndex(ParameterDataType.User);

            channel.Paused = false;

            do
            {
                OnUpdate();

                ProcessInput();

                bool bypass = dsp.Bypass;

                System.Update();

                ParameterDescription desc = dsp.GetParameterInfo(1);

                ref MyDSPData data = ref AsRef<MyDSPData>(dsp.GetParameterData(0, out _));

                string volstr = dsp.GetParameterFloat(1).ToString("N1");

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

                if (data.Channels != 0)
                {
                    Debug.Assert((uint)data.Channels <= 10);

                    int[] levels = DetermineChannelLevels((float*)data.Buffer, data.Channels, data.LengthSamples);

                    int channel = 0;

                    do
                    {
                        display.Clear();

                        channel.TryFormat(display.Slice(0, 2), out _);

                        display.Slice(3, Math.Min(levels[channel], display.Length - 3)).Fill('=');

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

        private static void AdjustVolume(DSP dsp, float adjustment)
        {
            var vol = dsp.GetParameterFloat(1);

            var vol2 = Math.Clamp(vol + adjustment, 0, 1);

            if (vol != vol2) //Optimization
            {
                dsp.SetParameterFloat(1, vol2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int[] DetermineChannelLevels(float* buffer, int channels, int sampleCount)
        {
            if (Sse2.IsSupported)
            {
                return SSE2(buffer, channels, sampleCount);
            }

            return Software(buffer, channels, sampleCount);

            static int[] SSE2(float* buffer, int channels, int sampleCount)
            {
                int channelIndex = 0;

                int[] levels = new int[channels];

                fixed (int* pLevels = levels)
                {
                    if (channels >= Vector128<float>.Count)
                    {
                        Vector128<float> AbsConst = Vector128.Create(int.MaxValue).AsSingle(), MulConst = Vector128.Create(40f);

                        do
                        {
                            Vector128<float> max = Vector128<float>.Zero;

                            for (int sample = 0; sample < sampleCount; ++sample)
                            {
                                Vector128<float> tmp = Sse.And(AbsConst, Sse.LoadVector128(buffer + (channels * sample + channelIndex)));

                                max = Sse.Max(max, tmp);
                            }

                            max = Sse.Multiply(max, MulConst);

                            Sse2.Store(pLevels + channelIndex, Sse2.ConvertToVector128Int32(max));

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

                        pLevels[channelIndex] = (int)(max * 40f);

                        channelIndex += 1;
                    }
                }

                return levels;
            }

            static int[] Software(float* buffer, int channels, int sampleCount)
            {
                int[] levels = new int[channels];

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

                    levels[chan] = (int)(max * 40f);
                }

                return levels;
            }
        }
    }
}
