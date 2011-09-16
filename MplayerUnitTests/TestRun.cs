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

            //SlideShow_Test st = new SlideShow_Test();
            //st.Test1();

            ImageResize_Test it = new ImageResize_Test();
            it.Test1();

            Console.WriteLine("Push any key to exit...");
            //Console.ReadLine();

            /*
            Mencoder_Test a = new Mencoder_Test();
            a.Convert_Test1();
            a.Convert_Test2();
            a.Convert_Test3();
            a.Convert_Test4();

            
            Discover_Test a = new Discover_Test();
            a.AudioBitrate_Test1();

            a.AudioBitrate_Test2();

              
            */
            
        }

    }
}
