using System.Collections;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

public static class DiscoverTestCaseSource
{
    public static IEnumerable TestCasesVideo1
    {
        get
        {
            yield return new TestCaseData(new MPlayerDiscover(GlobalVariables.Video1Path, GlobalVariables.MplayerPath));
#if !MACOS
            yield return new TestCaseData(new MpvDiscover(GlobalVariables.Video1Path, GlobalVariables.LibMpvPath));
#endif
            yield return new TestCaseData(new FfmpegDiscover(GlobalVariables.Video1Path, GlobalVariables.FfprobePath));
        }
    }

    public static IEnumerable TestCasesVideo2
    {
        get
        {
            yield return new TestCaseData(new MPlayerDiscover(GlobalVariables.Video2Path, GlobalVariables.MplayerPath));
#if !MACOS
            yield return new TestCaseData(new MpvDiscover(GlobalVariables.Video2Path, GlobalVariables.LibMpvPath));
#endif
            yield return new TestCaseData(new FfmpegDiscover(GlobalVariables.Video2Path, GlobalVariables.FfprobePath));
        }
    }
}