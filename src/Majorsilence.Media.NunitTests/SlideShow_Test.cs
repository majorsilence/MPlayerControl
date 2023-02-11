using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class SlideShow_Test
{
    [Test]
    public void Test1()
    {
        var a = new SlideShow();

        var b = new List<SlideShowInfo>();
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "1.jpg"),
            SlideShowEffect.TimeWarp));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "2.jpg"),
            SlideShowEffect.Moire));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "3.jpg"),
            SlideShowEffect.Water));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "4.jpg"),
            SlideShowEffect.RandomJitter));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "5.jpg"),
            SlideShowEffect.Pixelate));
        a.CreateSlideShow(b,
            Path.Combine(GlobalVariables.BasePath, "helloworld.mpg"),
            Path.Combine(GlobalVariables.BasePath, @"doxent_-_Sunset_Boulevard-edit.mp3"),
            5);
    }

    [Test]
    public async Task Async_Test1()
    {
        var a = new SlideShow();

        var b = new List<SlideShowInfo>();
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "1.jpg"),
            SlideShowEffect.TimeWarp));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "2.jpg"),
            SlideShowEffect.Moire));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "3.jpg"),
            SlideShowEffect.Water));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "4.jpg"),
            SlideShowEffect.RandomJitter));
        b.Add(new SlideShowInfo(Path.Combine(GlobalVariables.BasePath,
                "5.jpg"),
            SlideShowEffect.Pixelate));
        await a.CreateSlideShowAsync(b,
            Path.Combine(GlobalVariables.BasePath, "helloworld_async_test.mpg"),
            Path.Combine(GlobalVariables.BasePath, @"doxent_-_Sunset_Boulevard-edit.mp3"),
            5);
    }
}