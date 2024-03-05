using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class Discover_Test
{
    [Test, TestCaseSource(typeof(DiscoverTestCaseSource), "TestCasesVideo1")]
    public void Video1_Test(Discover discover)
    {
        using (discover)
        {
            discover.Execute();
            Assert.AreEqual(128, discover.AudioBitrate, "AudioBitrate");
            Assert.AreEqual(true, discover.Audio, "Audio");
            Assert.AreEqual(true, discover.Video, "Video");
            Assert.AreEqual("9:16", discover.AspectRatioString, "AspectRatioString");
            Assert.AreEqual(0.5625f, discover.AspectRatio, "AspectRatio");
            Assert.AreEqual(1, discover.AudioList.Count, "AudioList.Count");
            Assert.AreEqual(44100, discover.AudioSampleRate, "AudioSampleRate");
            Assert.AreEqual(29, discover.FPS, "FPS");
            Assert.AreEqual(1920, discover.Height, "Height");
            Assert.AreEqual(1080, discover.Width, "Width");
            Assert.AreEqual(2, discover.Length, "Length");
            Assert.AreEqual(3964, discover.VideoBitrate, "VideoBitrate");
        }
    }

    [Test, TestCaseSource(typeof(DiscoverTestCaseSource), "TestCasesVideo2")]
    public void Video2_Test(Discover discover)
    {
        using (discover)
        {
            discover.Execute();
            Assert.AreEqual(128, discover.AudioBitrate, "AudioBitrate");
            Assert.AreEqual(true, discover.Audio, "Audio");
            Assert.AreEqual(true, discover.Video, "Video");
            Assert.AreEqual(1.77777779f, discover.AspectRatio, "AspectRatio");
            Assert.AreEqual(1, discover.AudioList.Count, "AudioList.Count");
            Assert.AreEqual(44100, discover.AudioSampleRate, "AudioSampleRate");
            Assert.AreEqual(30, discover.FPS, "FPS");
            Assert.AreEqual(1080, discover.Height, "Height");
            Assert.AreEqual(1920, discover.Width, "Width");
            Assert.AreEqual(4, discover.Length, "Length");
            Assert.AreEqual(4650, discover.VideoBitrate, "VideoBitrate");
        }
    }
}