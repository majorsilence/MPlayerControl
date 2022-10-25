using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MplayerUnitTests
{

    [TestFixture()]
    public class Mencoder_Test
    {


        [Test()]
        public void Convert2WebMTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                a.Convert2WebM(GlobalVariables.Video1Path, GlobalVariables.OutputVideoWebM);
            }
        }

        [Test()]
        public async Task Convert2WebMAsyncTest()
        {
            var a = new Majorsilence.Media.Videos.Mencoder();
            await a.Convert2WebMAsync(GlobalVariables.Video1Path, GlobalVariables.OutputVideoWebM);
        }


        [Test()]
        public void Convert2X264Test()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                a.Convert2X264(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX264);
            }
        }

        [Test()]
        public async Task Convert2X264AsyncTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                await a.Convert2X264Async(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX264);
            }
        }

        [Test()]
        public void Convert2DvdMpegPalTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                a.Convert2DvdMpeg(Majorsilence.Media.Videos.Mencoder.RegionType.PAL, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegPal);
            }
        }

        [Test()]
        public async Task Convert2DvdMpegPalAsyncTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                await a.Convert2DvdMpegAsync(Majorsilence.Media.Videos.Mencoder.RegionType.PAL, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegPal);
            }
        }

        [Test()]
        public void Convert2DvdMpegNtscTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                a.Convert2DvdMpeg(Majorsilence.Media.Videos.Mencoder.RegionType.NTSC, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegNtsc);

                a.ConversionComplete += a_ConversionComplete;
            }
        }

        [Test()]
        public async Task Convert2DvdMpegNtscAsyncTest()
        {
            using (var a = new Majorsilence.Media.Videos.Mencoder())
            {
                await a.Convert2DvdMpegAsync(Majorsilence.Media.Videos.Mencoder.RegionType.NTSC, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegNtsc);
            }
        }

        private void a_ConversionComplete(object sender, Majorsilence.Media.Videos.MplayerEvent e)
        {
            using (var a = new Majorsilence.Media.Videos.MPlayerDiscover(GlobalVariables.OutputVideoDvdMpegNtsc, GlobalVariables.MplayerPath))
            {
                Assert.AreEqual(192, a.AudioBitrate);
                Assert.AreEqual(720, a.Width);
                Assert.AreEqual(480, a.Height);
                Assert.AreEqual(48000, a.AudioSampleRate);
            }

        }
    }
}
