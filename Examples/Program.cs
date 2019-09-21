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

            IExample example = null;

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
                case 18:
                    example = new RecordExample();
                    break;
                case 30:
                    example = new TestExample();
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

                Console.ReadKey();
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
