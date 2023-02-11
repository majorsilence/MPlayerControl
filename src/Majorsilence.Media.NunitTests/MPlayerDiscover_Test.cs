﻿using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class MPlayerDiscover_Test
{
    [Test]
    public void AudioBitrate_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(128, a.AudioBitrate);
        }
    }

    [Test]
    public void IsAudio_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(true, a.Audio);
        }
    }


    [Test]
    public void IsVideo_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(true, a.Video);
        }
    }

    [Test]
    public void AspectRatio_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1.33333337f, a.AspectRatio);
        }
    }

    [Test]
    public void AudioList_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1, a.AudioList.Count);
        }
    }

    [Test]
    public void AudioSampleRate_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(44100, a.AudioSampleRate);
        }
    }

    [Test]
    public void FPS_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(29, a.FPS);
        }
    }


    [Test]
    public void Height_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1920, a.Height);
        }
    }

    [Test]
    public void Width_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1080, a.Width);
        }
    }

    [Test]
    public void Length_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(2, a.Length);
        }
    }

    [Test]
    public void VideoBitrate_Test1()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(3964, a.VideoBitrate);
        }
    }

    [Test]
    public void AudioBitrate_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(128, a.AudioBitrate);
        }
    }

    [Test]
    public void IsAudio_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(true, a.Audio);
        }
    }


    [Test]
    public void IsVideo_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(true, a.Video);
        }
    }

    [Test]
    public void AspectRatio_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1.77777779f, a.AspectRatio);
        }
    }

    [Test]
    public void AudioList_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1, a.AudioList.Count);
        }
    }

    [Test]
    public void AudioSampleRate_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(44100, a.AudioSampleRate);
        }
    }

    [Test]
    public void FPS_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(30, a.FPS);
        }
    }


    [Test]
    public void Height_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1080, a.Height);
        }
    }

    [Test]
    public void Width_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(1920, a.Width);
        }
    }

    [Test]
    public void Length_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(4, a.Length);
        }
    }

    [Test]
    public void VideoBitrate_Test2()
    {
        using (var a = new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath))
        {
            a.Execute();
            Assert.AreEqual(4650, a.VideoBitrate);
        }
    }
}