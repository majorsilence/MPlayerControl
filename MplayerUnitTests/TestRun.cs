using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MplayerUnitTests
{
    public class TestRun
    {

        static void Main(string[] args)
        {

            Mencoder_Test a = new Mencoder_Test();
            a.Convert_Test2();

            /*
            Discover_Test a = new Discover_Test();
            a.AudioBitrate_Test1();

            a.AudioBitrate_Test2();

              
            */
            Console.WriteLine("Push any key to exit...");
            Console.ReadLine();
        }

    }
}
