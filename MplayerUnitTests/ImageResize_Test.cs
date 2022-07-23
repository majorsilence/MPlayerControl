using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SixLabors.ImageSharp;

namespace MplayerUnitTests
{
    [TestFixture()]
    public class ImageResize_Test
    {

        [Test()]
        public void Test1()
        {
            var a = LibImages.ImageResize.ResizeBlackBar(
                        Path.Combine(GlobalVariables.BasePath,
                            "1.jpg"), 720, 480);

            var jpgEncoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
            a.SaveAsJpeg(Path.Combine(GlobalVariables.BasePath, 
                    "test.jpg"),jpgEncoder);
        }

    }
}
