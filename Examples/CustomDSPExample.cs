using FmodAudio;
using FmodAudio.Dsp;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            Debug.Assert(ptr != null);

            return ref Unsafe.AsRef<T>(ptr);
        }

        static float FunAbs(float val) //A fun implementation of Abs for floats that is branchless
        {
            Unsafe.As<float, int>(ref val) &= int.MaxValue;
            return val;
        }

        static unsafe Result MyDSPCallback(DspState* state, IntPtr inBuffer, IntPtr outBuffer, uint length, int inChannels, ref int outChannels)
        {
            Debug.Assert(inChannels == outChannels);

            ref MyDSPData data = ref AsRef<MyDSPData>(state->plugindata);

            int LengthInFloats = (int)length / sizeof(float);

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
            value = default;

            if (index != 1)
                return Result.Err_Invalid_Param;

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

        static unsafe Plugin CreateDSPPlugin(FmodSystem system)
        {
            //Save some heap space by allocating everything on the stack
            ParameterDescription WaveDataDesc = ParameterDescription.CreateDataDescription("wave data", null, ParameterDataType.User);
            ParameterDescription VolumeDesc = ParameterDescription.CreateFloatDescription("volume", "%", 0, 1, 1);
            
            ParameterDescription** list = stackalloc ParameterDescription*[2] { &WaveDataDesc, &VolumeDesc };

            var dspDesc = new DspDescription.Structure()
            {
                PluginSDKVersion = FmodSystem.BindingVersion,

                InputBufferCount = 1,
                OutputBufferCount = 1,

                Read = MyDSPCallback,
                Create = MyDSPCreateCallback,
                Release = MyDSPReleaseCallback,
                GetParamData = MyDSPGetParameterDataCallback,
                SetParamFloat = MyDSPSetParameterFloat,
                GetParamFloat = MyDSPGetParameterFloat,

                ParameterCount = 2,
                ParameterDescriptions = list
            };

            return system.RegisterDSP(ref dspDesc);
        }

        readonly FmodSystem system;

        public CustomDSPExample()
        {
            system = new FmodSystem();

            TestVersion(system);
        }

        public unsafe override void Run()
        {
            Sound sound;
            Channel channel;
            DSP dsp;
            ChannelGroup masterGroup;
            
            system.Init(32);

            sound = system.CreateSound(MediaPath("stereo.ogg"), Mode.Loop_Normal);

            channel = system.PlaySound(sound, null, true);

            var plugin = CreateDSPPlugin(system);

            dsp = system.CreateDSPByPlugin(plugin);

            masterGroup = system.MasterChannelGroup;

            masterGroup.AddDSP(0, dsp);
            
            Span<char> display = stackalloc char[50];

            var ParamIndex = dsp.GetDataParameterIndex(ParameterDataType.User);

            channel.IsPaused = false;

            do
            {
                OnUpdate();

                bool bypass = dsp.Bypass;

                if (!Commands.IsEmpty)
                {
                    while(Commands.TryDequeue(out Button btn))
                    {
                        switch (btn)
                        {
                            case Button.Action1:
                                dsp.Bypass = !bypass;
                                break;

                            case Button.Action2:
                                AdjustVolume(dsp, -0.1f);
                                break;

                            case Button.Action3:
                                AdjustVolume(dsp, 0.1f);
                                break;
                            case Button.More:
                                channel.IsPaused = !channel.IsPaused;
                                break;
                            case Button.Quit:
                                goto Exit;
                        }
                    }
                }

                system.Update();

                ref readonly ParameterDescription desc = ref dsp.GetParameterInfo(1);

                ref MyDSPData data = ref AsRef<MyDSPData>(dsp.GetParameterData(0, out _));

                string volstr = dsp.GetParameterFloat(1).ToString("N2");

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

                    Span<float> Buffer = new Span<float>((void*)data.Buffer, data.Channels * data.LengthSamples);

                    for (int chan = 0; chan < data.Channels; ++chan)
                    {
                        int count, level;
                        float max = 0;

                        for(count = 0; count < data.LengthSamples; ++count)
                        {
                            float tmp = Math.Abs(Buffer[count * data.Channels + chan]);
                            if (tmp > max)
                                max = tmp;
                        }

                        level = (int)(max * 40f);

                        display.Clear();

                        chan.TryFormat(display.Slice(0, 2), out _);

                        display.Slice(3, Math.Min(level, display.Length - 3)).Fill('=');

                        DrawText(display);
                    }
                }
                
                Sleep(50);
            } while (true);

            Exit:
            sound.Release();
            masterGroup.RemoveDSP(dsp);
            dsp.Release();
            masterGroup = null;
        }

        static void AdjustVolume(DSP dsp, float adjustment)
        {
            var vol = dsp.GetParameterFloat(1);

            var vol2 = Clamp(0, 1, vol + adjustment);

            if (vol.CompareTo(vol2) != 0) //Optimization
            {
                dsp.SetParameterFloat(1, vol);
            }
        }

        public override void Dispose()
        {
            system.Release();
            base.Dispose();
        }
    }
}
