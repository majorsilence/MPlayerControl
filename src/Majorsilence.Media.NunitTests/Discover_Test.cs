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
            Assert.That(discover.AudioBitrate, Is.EqualTo(128), "AudioBitrate");
            Assert.That(discover.Audio, Is.EqualTo(true), "Audio");
            Assert.That(discover.Video, Is.EqualTo(true), "Video");
            Assert.That(discover.AspectRatioString, Is.EqualTo("9:16"), "AspectRatioString");
            Assert.That(discover.AspectRatio, Is.EqualTo(0.5625f), "AspectRatio");
            Assert.That(discover.AudioList.Count, Is.EqualTo(1), "AudioList.Count");
            Assert.That(discover.AudioSampleRate, Is.EqualTo(44100), "AudioSampleRate");
            Assert.That(discover.FPS, Is.EqualTo(29), "FPS");
            Assert.That(discover.Height, Is.EqualTo(1920), "Height");
            Assert.That(discover.Width, Is.EqualTo(1080), "Width");
            Assert.That(discover.Length, Is.EqualTo(2), "Length");
            Assert.That(discover.VideoBitrate, Is.EqualTo(3964), "VideoBitrate");
        }
    }

    [Test, TestCaseSource(typeof(DiscoverTestCaseSource), "TestCasesVideo2")]
    public void Video2_Test(Discover discover)
    {
        using (discover)
        {
            discover.Execute();
            Assert.That(discover.AudioBitrate, Is.EqualTo(128), "AudioBitrate");
            Assert.That(discover.Audio, Is.EqualTo(true), "Audio");
            Assert.That(discover.Video, Is.EqualTo(true), "Video");
            Assert.That(discover.AspectRatio, Is.EqualTo(1.77777779f), "AspectRatio");
            Assert.That(discover.AudioList.Count, Is.EqualTo(1), "AudioList.Count");
            Assert.That(discover.AudioSampleRate, Is.EqualTo(44100), "AudioSampleRate");
            Assert.That(discover.FPS, Is.EqualTo(30), "FPS");
            Assert.That(discover.Height, Is.EqualTo(1080), "Height");
            Assert.That(discover.Width, Is.EqualTo(1920), "Width");
            Assert.That(discover.Length, Is.EqualTo(4), "Length");
            Assert.That(discover.VideoBitrate, Is.EqualTo(4650), "VideoBitrate");
        }
    }
}