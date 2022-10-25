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
            var a = new Majorsilence.Media.Videos.SlideShow();

            var b = new List<Majorsilence.Media.Videos.SlideShowInfo>();
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath, 
                        "1.jpg"), 
                    Majorsilence.Media.Videos.SlideShowEffect.TimeWarp));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                        "2.jpg"), 
                    Majorsilence.Media.Videos.SlideShowEffect.Moire));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,  
                        "3.jpg"),
                    Majorsilence.Media.Videos.SlideShowEffect.Water));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                        "4.jpg"),
                    Majorsilence.Media.Videos.SlideShowEffect.RandomJitter));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "5.jpg"),
                Majorsilence.Media.Videos.SlideShowEffect.Pixelate));
            a.CreateSlideShow(b, 
                Path.Combine(GlobalVariables.BasePath, "helloworld.mpg"),
                Path.Combine (GlobalVariables.BasePath,@"doxent_-_Sunset_Boulevard-edit.mp3"),
                5);
        }

        [Test ()]
        public async Task Async_Test1 ()
        {
            var a = new Majorsilence.Media.Videos.SlideShow ();

            var b = new List<Majorsilence.Media.Videos.SlideShowInfo> ();
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath, 
                    "1.jpg"), 
                Majorsilence.Media.Videos.SlideShowEffect.TimeWarp));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "2.jpg"), 
                Majorsilence.Media.Videos.SlideShowEffect.Moire));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,  
                    "3.jpg"),
                Majorsilence.Media.Videos.SlideShowEffect.Water));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "4.jpg"),
                Majorsilence.Media.Videos.SlideShowEffect.RandomJitter));
            b.Add(new Majorsilence.Media.Videos.SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                    "5.jpg"),
                Majorsilence.Media.Videos.SlideShowEffect.Pixelate));
            await a.CreateSlideShowAsync (b,
                Path.Combine (GlobalVariables.BasePath, "helloworld_async_test.mpg"),
                Path.Combine (GlobalVariables.BasePath,@"doxent_-_Sunset_Boulevard-edit.mp3"),
                5);
        }


    }
}
