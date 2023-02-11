using System.IO;
using Majorsilence.Media.Images;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MplayerUnitTests;

[TestFixture]
public class ImageResize_Test
{
    [Test]
    public void Test1()
    {
        var a = ImageResize.ResizeBlackBar(
            Path.Combine(GlobalVariables.BasePath,
                "1.jpg"), 720, 480);

        var jpgEncoder = new JpegEncoder();
        a.SaveAsJpeg(Path.Combine(GlobalVariables.BasePath,
            "test.jpg"), jpgEncoder);
    }
}