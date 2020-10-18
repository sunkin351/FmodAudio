using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using FmodAudio;
using FmodAudio.DigitalSignalProcessing;

namespace Examples
{
    using System.Runtime.InteropServices;
    using Base;

    public unsafe class CustomDSPExample : Example
    {
        #region Dsp Implementation
        public class MyCustomDSP : IDisposable
        {
            public static Dsp CreateDsp(FmodSystem system)
            {
                return system.CreateDSP(in Description);
            }

            private static readonly ParameterDescription[] paramDescriptions = new ParameterDescription[]
            {
                new DataParameterDescription("wave data", null, ParameterDataType.User),
                new FloatParameterDescription("volume", "%", 0, 1, 1)
            };

            private static ParameterDescriptionList paramList = new ParameterDescriptionList(paramDescriptions);

            private static readonly DspDescriptionStruct Description = new DspDescriptionStruct
            {
                PluginSDKVersion = Fmod.PluginSDKVersion,
                Name = "MyDsp",
                Version = new FmodVersion(1, 0, 0),
                InputBufferCount = 1,
                OutputBufferCount = 1,

                //Function Pointers
                Create = &_myCreate,
                Release = &_myRelease,
                Reset = &_myReset,
                Read = &_myRead,
                ShouldIProcess = &_myShouldIProcess,
                GetParamFloat = &_myGetParameterFloat,
                SetParamFloat = &_mySetParameterFloat,
                GetParamData = &_myGetParameterData
            };

            static MyCustomDSP()
            {
                paramList.GetPointerAndCount(out Description.ParameterDescriptions, out Description.ParameterCount);
            }

            #region Dsp Implementation Details

            private static MyCustomDSP? GetManagedState(DspState* state)
            {
                if (state->plugindata != default)
                {
                    return GCHandle.FromIntPtr(state->plugindata).Target as MyCustomDSP;
                }

                return null;
            }

#pragma warning disable IDE1006

            [UnmanagedCallersOnly]
            private static Result _myCreate(DspState* state)
            {
                var res = state->GetBlockSize(out uint size);

                if (res != Result.Ok)
                {
                    return res;
                }

                var managedState = new MyCustomDSP((int)size);

                var gchandle = GCHandle.Alloc(managedState, GCHandleType.Normal);

                state->plugindata = GCHandle.ToIntPtr(gchandle);

                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _myRelease(DspState* state)
            {
                var gchandle = GCHandle.FromIntPtr(state->plugindata);

                if (gchandle.IsAllocated && gchandle.Target is MyCustomDSP managedState)
                {
                    managedState.Dispose();
                    gchandle.Free();
                }

                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _myReset(DspState* state)
            {
                var managedState = GetManagedState(state);

                if (managedState is null)
                    return Result.Err_Internal;

                managedState.buffer.AsSpan().Clear();
                managedState.volumeLinear = 1.0f;

                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _myRead(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, int* outChannels)
            {
                var managedState = GetManagedState(state);

                if (managedState is null)
                    return Result.Err_Internal;

                return managedState.Read(state, inBuffer, outBuffer, length, inChannels, ref *outChannels);
            }

            [UnmanagedCallersOnly]
            private static Result _myShouldIProcess(DspState* state, int inputsIdle, uint length, ChannelMask inMask, int inChannels, SpeakerMode speakerMode)
            {
                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _myGetParameterData(DspState* state, int index, void** data, uint* length, byte* valueStr)
            {
                if (index != 0)
                {
                    return Result.Err_Invalid_Param;
                }

                var managedState = GetManagedState(state);

                if (managedState is null)
                    return Result.Err_Internal;

                *data = Unsafe.AsPointer(ref managedState.buffer[0]); //Safe because the array is already pinned
                *length = (uint)(managedState.channels * managedState.lengthSamples);

                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _myGetParameterFloat(DspState* state, int index, float* value, byte* valueStr)
            {
                if (index != 1)
                {
                    return Result.Err_Invalid_Param;
                }

                var managedState = GetManagedState(state);

                *value = managedState.Volume;
                return Result.Ok;
            }

            [UnmanagedCallersOnly]
            private static Result _mySetParameterFloat(DspState* state, int index, float value)
            {
                if (index != 1)
                {
                    return Result.Err_Invalid_Param;
                }

                var managedState = GetManagedState(state);

                managedState.Volume = value;

                return Result.Ok;
            }

#pragma warning restore IDE1006

            #endregion

            private float[] buffer;
            private float volumeLinear = 1.0f;
            private int lengthSamples, channels;
            private readonly ReaderWriterLockSlim DspLock = new ReaderWriterLockSlim();

            private MyCustomDSP(int size)
            {
                lengthSamples = size;
                buffer = GC.AllocateArray<float>(size * 8, pinned: true);
            }

            public void Dispose()
            {
                DspLock.Dispose();
            }

            public ReadOnlySpan<float> Data => this.buffer.AsSpan(0, this.lengthSamples * this.channels);

            public float Volume
            {
                get => this.volumeLinear;
                set => this.volumeLinear = Math.Clamp(value, 0f, 1f);
            }

            public int Channels => this.channels;

            public int LengthSamples => this.lengthSamples;

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

            private unsafe Result Read(DspState* state, float* inBuffer, float* outBuffer, uint length, int inChannels, ref int outChannels)
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
                    catch
                    {
                        return Result.Err_Internal;
                    }
                    finally
                    {
                        this.DspLock.ExitWriteLock();
                    }
                }

                return Result.Ok;
            }
        }

        #endregion

        private Sound sound;
        private Channel channel;
        private Dsp dsp;
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

            dsp = MyCustomDSP.CreateDsp(System);

            masterGroup = System.MasterChannelGroup;

            masterGroup.AddDSP(0, dsp);

            dsp.Bypass = true;
        }

        public unsafe override void Run()
        {
            Span<char> display = stackalloc char[50];

            channel.Paused = false;

            System.GetDSPBufferSize(out var sampleLen, out _);
            dsp.GetChannelFormat(out _, out var channels, out _);

            do
            {
                OnUpdate();

                System.Update();

                bool bypass = dsp.Bypass;
                var volume = dsp.GetParameterFloat(1);
                var ptr = dsp.GetParameterData(0, out var len);

                var data = new ReadOnlySpan<float>(ptr, (int)len);

                string volstr = (volume * 100).ToString("N1");

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
                    Debug.Assert((uint)channels <= 10);

                    float[] levels = DetermineChannelLevels(data, channels, (int)sampleLen);

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
            if (sound != default)
                sound.Dispose();

            if ((FmodAudio.Base.DspHandle)dsp != default)
            {
                masterGroup.RemoveDSP(dsp);
                dsp.Dispose();
            }

            base.Dispose();
        }

        private static void AdjustVolume(Dsp dsp, float adjustment)
        {
            dsp.SetParameterFloat(1, Math.Clamp(dsp.GetParameterFloat(1) + adjustment, 0, 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe float[] DetermineChannelLevels(ReadOnlySpan<float> buffer, int channels, int sampleCount)
        {
            if (channels * sampleCount > buffer.Length)
            {
                throw new ArgumentException("");
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
