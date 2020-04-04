using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace MplayerUnitTests
{
    [TestFixture()]
    public class SlideShow_Test
    {
        [Test()]
        public void Test1()
        {
            var a = new LibMPlayerCommon.SlideShow();

            var b = new List<LibMPlayerCommon.SlideShowInfo>();
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath, 
                        "1.jpg"), 
                    LibMPlayerCommon.SlideShowEffect.TimeWarp));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                        "2.jpg"), 
                    LibMPlayerCommon.SlideShowEffect.Moire));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,  
                        "3.jpg"),
                    LibMPlayerCommon.SlideShowEffect.Water));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                        "4.jpg"),
                    LibMPlayerCommon.SlideShowEffect.RandomJitter));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "5.jpg"),
                LibMPlayerCommon.SlideShowEffect.Flip));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "6.jpg"),
                LibMPlayerCommon.SlideShowEffect.Pixelate));
            a.CreateSlideShow(b, 
                Path.Combine(GlobalVariables.BasePath, "helloworld.mpg"),
                Path.Combine (GlobalVariables.BasePath,@"doxent_-_Sunset_Boulevard-edit.mp3"),
                5);
        }

        [Test ()]
        public async Task Async_Test1 ()
        {
            var a = new LibMPlayerCommon.SlideShow ();

            var b = new List<LibMPlayerCommon.SlideShowInfo> ();
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath, 
                    "1.jpg"), 
                LibMPlayerCommon.SlideShowEffect.TimeWarp));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "2.jpg"), 
                LibMPlayerCommon.SlideShowEffect.Moire));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,  
                    "3.jpg"),
                LibMPlayerCommon.SlideShowEffect.Water));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "4.jpg"),
                LibMPlayerCommon.SlideShowEffect.RandomJitter));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "5.jpg"),
                LibMPlayerCommon.SlideShowEffect.Flip));
            b.Add(new LibMPlayerCommon.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "6.jpg"),
                LibMPlayerCommon.SlideShowEffect.Pixelate));
            await a.CreateSlideShowAsync (b,
                Path.Combine (GlobalVariables.BasePath, "helloworld_async_test.mpg"),
                Path.Combine (GlobalVariables.BasePath,@"doxent_-_Sunset_Boulevard-edit.mp3"),
                5);
        }


    }
}
