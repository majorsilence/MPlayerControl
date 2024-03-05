using System;
using System.Threading;
using System.Threading.Tasks;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class VideoEncoder_Test
{
    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncoders")]
    public void Convert2WebMTest(IVideoEncoder encoder)
    {
        using (encoder as IDisposable)
        {
            encoder.Convert2WebM(GlobalVariables.Video1Path, GlobalVariables.OutputVideoWebM);
        }
    }

    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncoders")]
    public void Convert2X264Test(IVideoEncoder encoder)
    {
        using (encoder as IDisposable)
        {
            encoder.Convert2X264(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX264);
        }
    }

    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncoders")]
    public void Convert2X265Test(IVideoEncoder encoder)
    {
        using (encoder as IDisposable)
        {
            encoder.Convert2X265(GlobalVariables.Video1Path, GlobalVariables.OutputVideoX265);
        }
    }

    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncoders")]
    public void Convert2Av1Test(IVideoEncoder encoder)
    {
        using (encoder as IDisposable)
        {
            encoder.Convert2Av1(GlobalVariables.Video1Path, GlobalVariables.OutputVideoAv1);
        }
    }

    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncoders")]
    public async Task ThumbnailTest(IVideoEncoder encoder)
    {
        using (encoder as IDisposable)
        {
            await encoder.ThumbnailAsync(GlobalVariables.Video1Path, GlobalVariables.OutputThumbnail,
                CancellationToken.None);
        }
    }

    [Test, TestCaseSource(typeof(VideoEncoderTestCaseSource), "TestCasesEncodersConvertAsync")]
    public async Task ConvertAsync(IVideoEncoder encoder, VideoType videoType, AudioType audioType,
        VideoAspectRatios aspectRatio)
    {
        using (encoder as IDisposable)
        {
            await encoder.ConvertAsync(videoType, audioType, aspectRatio, GlobalVariables.Video1Path,
                GlobalVariables.OutputVideoWebM,
                CancellationToken.None);
        }
    }
}