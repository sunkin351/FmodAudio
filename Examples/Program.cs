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
            }

            using (example)
            {
                example.Run();
            }

            Console.ReadKey();
        }

        public static void PrintUsage()
        {

        }
    }
}
