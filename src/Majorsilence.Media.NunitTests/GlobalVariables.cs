using System.IO;

namespace MplayerUnitTests;

public class GlobalVariables
{
    // Test videos can be downloaded from:
    // http://files.majorsilence.com/TestVideos.zip
    // TODO: You must point the path to the mplayer.exe and the test videos to the location
    // were they exist on your computer.

    public static string BasePath { get; private set; }

    public static string MplayerPath { get; private set; }

    public static string LibMpvPath { get; private set; }

    public static string Video2Path => Path.Combine(BasePath, "video2.mp4");

    public static string Video1Path => Path.Combine(BasePath, "video1.mp4");

    public static string OutputVideoWebM => Path.Combine(BasePath, "OutputVideoWebM.webm");

    public static string OutputVideoX264 => Path.Combine(BasePath, "OutputVideoX264.mp4");

    public static string OutputVideoDvdMpegPal => Path.Combine(BasePath, "OutputVideoDvdMpegPal.mpg");

    public static string OutputVideoDvdMpegNtsc => Path.Combine(BasePath, "OutputVideoDvdMpegNtsc.mpg");

    public static void InitPath(string basePath, string mplayerPath, string libMpvPath)
    {
        BasePath = basePath;
        MplayerPath = mplayerPath;
        LibMpvPath = libMpvPath;
    }
}