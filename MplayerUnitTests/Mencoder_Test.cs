using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace MplayerUnitTests
{

    [TestFixture()]
    public class Mencoder_Test
    {


        [Test()]
        public void Convert_Test1()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2WebM(GlobalVariables.Video8Path, GlobalVariables.OutputVideoWebM);
        }

        [Test()]
        public void Convert_Test2()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2X264(GlobalVariables.Video8Path, GlobalVariables.OutputVideoX264);
        }

        [Test()]
        public void Convert_Test3()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2DvdMpeg(LibMPlayerCommon.Mencoder.RegionType.PAL, GlobalVariables.Video8Path, GlobalVariables.OutputVideoDvdMpegPal);
        }

        [Test()]
        public void Convert_Test4()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2DvdMpeg(LibMPlayerCommon.Mencoder.RegionType.NTSC, GlobalVariables.Video8Path, GlobalVariables.OutputVideoDvdMpegNtsc);

            a.ConversionComplete+=new LibMPlayerCommon.MplayerEventHandler(a_ConversionComplete);
        }

        private void a_ConversionComplete(object sender, LibMPlayerCommon.MplayerEvent e)
        {
            LibMPlayerCommon.Discover a = new LibMPlayerCommon.Discover(GlobalVariables.OutputVideoDvdMpegNtsc, GlobalVariables.MplayerPath);
            NUnit.Framework.Assert.AreEqual(192, a.AudioBitrate);
            NUnit.Framework.Assert.AreEqual(720, a.Width);
            NUnit.Framework.Assert.AreEqual(480, a.Height);
            NUnit.Framework.Assert.AreEqual(48000, a.AudioSampleRate);

        }
    }
}
