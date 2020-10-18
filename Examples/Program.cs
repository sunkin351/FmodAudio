using System;
using System.Runtime.CompilerServices;

namespace Examples
{
    using Base;
    using FmodAudio;

    static class Program
    {
        private static readonly SelectionUI UI;

        static Program()
        {
            SelectionUI.ExampleInfo[] examples = new SelectionUI.ExampleInfo[]
            {
                new SelectionUI.ExampleInfo()
                {
                    Name = "3D Example",
                    ExampleType = typeof(_3DExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Async IO Example (Broken)",
                    ExampleType = typeof(AsyncIOExample),
                    Enabled = false
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Channel Group Example",
                    ExampleType = typeof(ChannelGroupsExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Convolution Reverb Example",
                    ExampleType = typeof(ConvolutionReverbExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Custom DSP Example",
                    ExampleType = typeof(CustomDSPExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Dsp Effect Per Speaker Example",
                    ExampleType = typeof(DspEffectPerSpeakerExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Effects Example",
                    ExampleType = typeof(EffectsExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Gapless Playback Example",
                    ExampleType = typeof(GaplessPlaybackExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Generate Tone Example",
                    ExampleType = typeof(GenerateToneExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                { 
                    Name = "Granular Synth Example",
                    ExampleType = typeof(GranularSynthExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                { 
                    Name = "Load From Memory Example",
                    ExampleType = typeof(LoadFromMemoryExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                { 
                    Name = "Multiple Speaker Example", 
                    ExampleType = typeof(MultipleSpeakerExample), 
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Record Example",
                    ExampleType = typeof(RecordExample),
                    Enabled = true
                },
                new SelectionUI.ExampleInfo()
                {
                    Name = "Music Player",
                    ExampleType = typeof(MusicPlayer),
                    Enabled = true
                }
            };

            UI = new SelectionUI(examples);
        }

        private static void Main(string[] args)
        {
            ConsoleHelpers.Initialize();

            Fmod.SetLibraryLocation(@"D:\Programming\FMOD2\api\core\lib\x64");

            while (true)
            {
                Type example = UI.Select();

                if (example is null || !StartExample(example))
                {
                    break;
                }

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static bool StartExample(Type exampleType)
        {
            if (!typeof(Example).IsAssignableFrom(exampleType))
                throw new ArgumentException("Type not assignable to typeof(Example)", nameof(exampleType));

            var example = (Example)Activator.CreateInstance(exampleType);

            Console.Title = example.Title;

            try
            {
                example.Initialize();
                example.Run();
            }
            catch (Exception e)
            {
                ConsoleHelpers.OnError();

                if (e is FmodException fe && fe.Result != null)
                {
                    Console.WriteLine("Fmod Error Code: " + fe.Result.Value);
                }

                Console.WriteLine(e);
                
                return false;
            }
            finally
            {
                example.Dispose();
            }

            return true;
        }
    }
}
