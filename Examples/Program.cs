using System;

namespace Examples
{
    using Base;
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int value))
            {
                PrintUsage();
                return;
            }

            Example example = null;

            switch (value)
            {
                case 1:
                    example = new _3DExample();
                    break;
                case 2:
                    example = new AsyncIOExample();
                    break;
                case 3:
                    example = new ChannelGroupsExample();
                    break;
                case 4:
                    example = new ConvolutionReverbExample();
                    break;
                case 5:
                    example = new CustomDSPExample();
                    break;
                case 6:
                    example = new DspEffectPerSpeakerExample();
                    break;
                case 7:
                    Console.WriteLine("DSP Inspector Example not written yet.");
                    return;
                case 8:
                    example = new EffectsExample();
                    break;
                case 9:
                    example = new GaplessPlaybackExample();
                    break;
                case 18:
                    example = new RecordExample();
                    break;
            }

            if (example != null)
            {
                ConsoleHelpers.Initialize(example.Title);

                try
                {
                    example.Run();
                }
                catch (Exception e)
                {
                    ConsoleHelpers.OnError();
                    Console.WriteLine(e);
                }
                finally
                {
                    ConsoleHelpers.OnExit();
                }
            }
            else
            {
                Console.WriteLine("Invalid Example Number");
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: Examples {Example Number}");
        }
    }
}
