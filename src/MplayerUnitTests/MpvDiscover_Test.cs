using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace MplayerUnitTests
{

    [Ignore("temp ignore until travis ci setup with libmpv")]
    [TestFixture()]
    public class MpvDiscover_Test
    {

        #region Test Video1

        [Test()]
        public void AudioBitrate_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(8, a.AudioBitrate);
            }
        }

        [Test()]
        [Ignore("fixme")]
        public void IsAudio_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(true, a.Audio);
            }
        }


        [Test()]
        public void IsVideo_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(true, a.Video);
            }
        }

        [Test()]
        public void AspectRatio_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(1.33333302f, a.AspectRatio);
            }
        }

        [Test()]
        public void AudioList_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(1, a.AudioList.Count);
            }
        }

        [Test()]
        public void AudioSampleRate_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(22050, a.AudioSampleRate);
            }
        }

        [Test()]
        [Ignore("fixme")]
        public void FPS_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(29, a.FPS);
            }
        }


        [Test()]
        public void Height_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(240, a.Height);
            }
        }

        [Test()]
        public void Width_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(320, a.Width);
            }
        }

        [Test()]
        public void Length_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(208, a.Length);
            }
        }

        [Test()]
        [Ignore("fixme")]
        public void VideoBitrate_Test1()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(339, a.VideoBitrate);
            }
        }

        #endregion


        #region Test Video2

        [Test()]
        [Ignore("fixme")]
        public void AudioBitrate_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(8, a.AudioBitrate);
            }
        }

        [Test()]
        [Ignore("fixme")]
        public void IsAudio_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(true, a.Audio);
            }
        }


        [Test()]
        public void IsVideo_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(true, a.Video);
            }
        }

        [Test()]
        public void AspectRatio_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(1.33333302f, a.AspectRatio);
            }
        }

        [Test()]
        public void AudioList_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(1, a.AudioList.Count);
            }
        }

        [Test()]
        public void AudioSampleRate_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(22050, a.AudioSampleRate);
            }
        }

        [Test()]
        public void FPS_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(29, a.FPS);
            }
        }


        [Test()]
        public void Height_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(240, a.Height);
            }
        }

        [Test()]
        public void Width_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(320, a.Width);
            }

        }

        [Test()]
        public void Length_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(255, a.Length);
            }
        }

        [Test()]
        [Ignore("fixme")]
        public void VideoBitrate_Test2()
        {
            using (var a = new LibMPlayerCommon.MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath))
            {
                a.Execute();
                Assert.AreEqual(330, a.VideoBitrate);
            }
        }

        #endregion
    }
}
