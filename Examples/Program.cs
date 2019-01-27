using FmodAudio;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

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

            IExample test = null;

            switch (value)
            {
                case 1:
                    test = new _3DExample();
                    break;
                case 2:
                    test = new AsyncIOExample();
                    break;
                case 3:
                    test = new ChannelGroupsExample();
                    break;
                case 4:
                    test = new ConvolutionReverbExample();
                    break;
                case 5:
                    test = new CustomDSPExample();
                    break;
                case 18:
                    test = new RecordExample();
                    break;
            }

            using (test)
            {
                test.Run();
            }

            Console.ReadKey();
        }

        public static void PrintUsage()
        {

        }
    }
}
