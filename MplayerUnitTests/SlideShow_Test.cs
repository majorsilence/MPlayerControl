using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MplayerUnitTests
{
     [TestFixture()]
    public class SlideShow_Test
    {
         [Test()]
         public void Test1()
         {
             LibMPlayerCommon.SlideShow a = new LibMPlayerCommon.SlideShow();

             List<LibMPlayerCommon.SlideShowInfo> b = new List<LibMPlayerCommon.SlideShowInfo>();
             b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Blue hills.jpg", LibMPlayerCommon.SlideShowEffect.TimeWarp));
             b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Sunset.jpg", LibMPlayerCommon.SlideShowEffect.Moire));
             b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Water lilies.jpg", LibMPlayerCommon.SlideShowEffect.Water));
             b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Winter.jpg", LibMPlayerCommon.SlideShowEffect.RandomJitter));
             a.CreateSlideShow(b, @"C:\Documents and Settings\Peter\Desktop\helloworld.mpg",
                 @"C:\Documents and Settings\All Users\Documents\My Music\Magnatune Compilation\Rock\16. TranceVision_ Alpha.mp3",
                 5);
         }


    }
}
