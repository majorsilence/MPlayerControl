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

            LibMPlayerCommon.SlideShow a = new LibMPlayerCommon.SlideShow();

            List<LibMPlayerCommon.SlideShowInfo> b = new  List<LibMPlayerCommon.SlideShowInfo>();
            b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Blue hills.jpg",  LibMPlayerCommon.SlideShowEffect.TimeWarp));
            b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Sunset.jpg",  LibMPlayerCommon.SlideShowEffect.Sphere));
            b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Water lilies.jpg",  LibMPlayerCommon.SlideShowEffect.Water));
            b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Winter.jpg",  LibMPlayerCommon.SlideShowEffect.RandomJitter));
            a.CreateSlideShow(b, @"C:\Documents and Settings\Peter\Desktop\hellworld.mpg", 
                @"C:\Documents and Settings\All Users\Documents\My Music\Magnatune Compilation\Rock\16. TranceVision_ Alpha.mp3", 
                5);


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
