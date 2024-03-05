using System.IO;
using System.Runtime.InteropServices;

namespace MplayerUnitTests;

public class GlobalVariables
{
    // Test videos can be downloaded from:
    // http://files.majorsilence.com/TestVideos.zip
    // TODO: You must point the path to the mplayer.exe and the test videos to the location
    // were they exist on your computer.

    public static string BasePath => Path.Combine(Path.GetTempPath(), "MPlayerControl", "Tests", "TestVideos");

    private static string _mplayerPath;

    public static string MplayerPath
    {
        get
        {
            InitPath();
            return _mplayerPath;
        }
        private set { _mplayerPath = value; }
    }

    private static string _libMpvPath;

    public static string LibMpvPath
    {
        get
        {
            InitPath();
            return _libMpvPath;
        }
        private set { _libMpvPath = value; }
    }
    
    private static string _ffmpegPath;

    public static string FfmpegPath
    {
        get
        {
            InitPath();
            return _ffmpegPath;
        }
        private set { _ffmpegPath = value; }
    }
    
    private static string _ffprobePath;

    public static string FfprobePath
    {
        get
        {
            InitPath();
            return _ffprobePath;
        }
        private set { _ffprobePath = value; }
    }

    public static string Video2Path => Path.Combine(BasePath, "video2.mp4");

    public static string Video1Path => Path.Combine(BasePath, "video1.mp4");

    public static string OutputVideoWebM => Path.Combine(BasePath, "OutputVideoWebM.webm");

    public static string OutputVideoX264 => Path.Combine(BasePath, "OutputVideoX264.mp4");

    public static string OutputVideoDvdMpegPal => Path.Combine(BasePath, "OutputVideoDvdMpegPal.mpg");

    public static string OutputVideoDvdMpegNtsc => Path.Combine(BasePath, "OutputVideoDvdMpegNtsc.mpg");

    private static bool initCalled = false;

    private static void InitPath()
    {
        if (initCalled) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            MplayerPath = @"mplayer.exe";
            LibMpvPath = @"mpv-1.dll";
            FfmpegPath = @"ffmpeg.exe";
            FfprobePath = @"ffprobe.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            MplayerPath = "/opt/homebrew/bin/mplayer";
            LibMpvPath = "/opt/homebrew/lib/libmpv.2.dylib";
            FfmpegPath = "/opt/homebrew/bin/ffmpeg";
            FfprobePath = "/opt/homebrew/bin/ffprobe";
        }
        else
        {
            MplayerPath = "mplayer";
            LibMpvPath = "/usr/lib/x86_64-linux-gnu/libmpv.so.1";
            FfmpegPath = "/usr/bin/ffmpeg";
            FfprobePath = "/usr/bin/ffprobe";
        }

        initCalled = true;
    }
}