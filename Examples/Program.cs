using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Type[] ExampleTypes = new Type[]
            {
                typeof(_3DExample),
                typeof(AsyncIOExample),
                typeof(ChannelGroupsExample),
                typeof(ConvolutionReverbExample),
                typeof(CustomDSPExample),
                typeof(DspEffectPerSpeakerExample),
                typeof(EffectsExample),
                typeof(GaplessPlaybackExample),
                typeof(GenerateToneExample),
                typeof(GranularSynthExample),
                typeof(LoadFromMemoryExample),
                typeof(MultipleSpeakerExample),
                typeof(RecordExample)
            };

            string[] ExampleNames = new string[]
            {
                "3D Example",
                "Async IO Example (Broken)",
                "Channel Group Example",
                "Convolution Reverb Example",
                "Custom DSP Example (Broken)",
                "Dsp Effect Per Speaker Example",
                "Effects Example",
                "Gapless Playback Example",
                "Generate Tone Example",
                "Granular Synth Example",
                "Load From Memory Exmaple",
                "Multiple Speaker Example",
                "Record Example"
            };

            Debug.Assert(ExampleNames.Length == ExampleTypes.Length);

            UI = new SelectionUI(ExampleNames, ExampleTypes);
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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
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
                    Console.WriteLine("Fmod Error Code: " + fe.Result);
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
