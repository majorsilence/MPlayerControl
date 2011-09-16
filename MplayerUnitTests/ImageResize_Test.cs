using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MplayerUnitTests
{
    [TestFixture()]
    public class ImageResize_Test
    {

        [Test()]
        public void Test1()
        {
            System.Drawing.Image a = LibImages.ImageResize.ResizeBlackBar(@"C:\Users\Peter\Desktop\Vacation 2011\100_0315.JPG", 720, 480);
            a.Save(@"C:\Users\Peter\Desktop\Testing\test.jpg",  System.Drawing.Imaging.ImageFormat.Jpeg);
        }

    }
}
