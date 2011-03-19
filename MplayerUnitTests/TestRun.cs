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

            Discover_Test a = new Discover_Test();
            a.AudioBitrate_Test1();

            a.AudioBitrate_Test2();

            Console.ReadLine();
        }

    }
}
