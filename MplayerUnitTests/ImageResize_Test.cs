using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

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
                            "12195978_10153226237613441_9028549466281839298_n.jpg"), 720, 480);
            a.Save(Path.Combine(GlobalVariables.BasePath, 
                    "test.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
        }

    }
}
