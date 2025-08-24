using System.Threading.Tasks;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class Mencoder2_Test
{
    [Test]
    public void Convert2WebMTest()
    {
        using (var a = new Mencoder())
        {
            a.PercentCompleted += (s, e) =>
            {
                // Console.WriteLine ($"Convert2WebMTest Percent: {e.Value}");
            };
            a.Convert2WebM(GlobalVariables.Video1Path, GlobalVariables.OutputVideoWebM);
        }
    }

    [Test]
    public async Task Convert2WebMAsyncTest()
    {
        var a = new Mencoder();
        a.PercentCompleted += (s, e) =>
        {
            // Console.WriteLine ($"Convert2WebMAsyncTest Percent: {e.Value}");
        };
        await a.Convert2WebMAsync(GlobalVariables.Video1Path, GlobalVariables.OutputVideoWebM);
    }


    [Test]
    public void Convert2X264Test()
    {
        using (var a = new Mencoder())
        {
            a.Convert2X264(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX264);
        }
    }

    [Test]
    public async Task Convert2X264AsyncTest()
    {
        using (var a = new Mencoder())
        {
            await a.Convert2X264Async(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX264);
        }
    }

    [Test]
    public void Convert2DvdMpegPalTest()
    {
        using (var a = new Mencoder())
        {
            a.Convert2DvdMpeg(RegionType.PAL, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegPal);
        }
    }

    [Test]
    public async Task Convert2DvdMpegPalAsyncTest()
    {
        using (var a = new Mencoder())
        {
            await a.Convert2DvdMpegAsync(RegionType.PAL, GlobalVariables.Video1Path,
                GlobalVariables.OutputVideoDvdMpegPal);
        }
    }

    [Test]
    public void Convert2DvdMpegNtscTest()
    {
        using (var a = new Mencoder())
        {
            a.Convert2DvdMpeg(RegionType.NTSC, GlobalVariables.Video1Path, GlobalVariables.OutputVideoDvdMpegNtsc);

            a.ConversionComplete += a_ConversionComplete;
        }
    }

    [Test]
    public async Task Convert2DvdMpegNtscAsyncTest()
    {
        using (var a = new Mencoder())
        {
            await a.Convert2DvdMpegAsync(RegionType.NTSC, GlobalVariables.Video1Path,
                GlobalVariables.OutputVideoDvdMpegNtsc);
        }
    }

    private void a_ConversionComplete(object sender, MplayerEvent e)
    {
        using (var a = new MPlayerDiscover(GlobalVariables.OutputVideoDvdMpegNtsc, GlobalVariables.MplayerPath))
        {
            Assert.That(a.AudioBitrate, Is.EqualTo(192));
            Assert.That(a.Width, Is.EqualTo(720));
            Assert.That(a.Height, Is.EqualTo(480));
            Assert.That(a.AudioSampleRate, Is.EqualTo(48000));
        }
    }
}