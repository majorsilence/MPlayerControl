using System.Collections;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

public static class VideoEncoderTestCaseSource
{
    public static IEnumerable TestCasesEncoders
    {
        get
        {
            yield return new TestCaseData(new Mencoder(GlobalVariables.MplayerPath));
            yield return new TestCaseData(new Ffmpeg(GlobalVariables.FfprobePath));
        }
    }

    public static IEnumerable TestCasesEncodersConvertAsync
    {
        get
        {
            yield return new TestCaseData(new Mencoder(GlobalVariables.MplayerPath), VideoType.vp9, AudioType.opus,
                VideoAspectRatios.p240);
            yield return new TestCaseData(new Mencoder(GlobalVariables.MplayerPath), VideoType.x264, AudioType.aac,
                VideoAspectRatios.p240);
            yield return new TestCaseData(new Ffmpeg(GlobalVariables.FfprobePath), VideoType.vp9, AudioType.opus,
                VideoAspectRatios.p240);
            yield return new TestCaseData(new Ffmpeg(GlobalVariables.FfprobePath), VideoType.x264, AudioType.aac,
                VideoAspectRatios.p240);
        }
    }
}