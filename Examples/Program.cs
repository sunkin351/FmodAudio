using System;

namespace Examples
{
    using Base;
    static class Program
    {
        static void Main(string[] args)
        {
            SelectionUI ui = new SelectionUI(new Example[]
            {
                new _3DExample(),
                new AsyncIOExample(),
                new ChannelGroupsExample(),
                new ConvolutionReverbExample(),
                new CustomDSPExample(),
                new DspEffectPerSpeakerExample(),
                new EffectsExample(),
                new GaplessPlaybackExample(),
                new GenerateToneExample(),
                new RecordExample()
            });

            ConsoleHelpers.Initialize();

            while (true)
            {
                Console.Title = "Fmod Example Selection";

                Example example = ui.Select();

                if (example is null)
                {
                    break;
                }

                Console.Title = example.Title;

                try
                {
                    example.Initialize();
                    example.Run();
                }
                catch (Exception e)
                {
                    ConsoleHelpers.OnError();
                    Console.WriteLine(e);
                    break;
                }
                finally
                {
                    example.Dispose();
                }
            }
        }
    }
}
