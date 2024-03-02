using System.Runtime.InteropServices;
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.WorkerService.Tests;

public class Tests
{
    string _testFolder = Path.Combine(Path.GetTempPath(), "Majorsilence.Media.WorkerService.Tests");
    private string _ffmpegPath;
    string _uploadFolder;
    string _convertedFolder;

    [SetUp]
    public void Setup()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _ffmpegPath = "ffmpeg.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _ffmpegPath = "/opt/homebrew/bin/ffmpeg";
        }
        else
        {
            _ffmpegPath = "/usr/bin/ffmpeg";
        }

        _uploadFolder = Path.Combine(_testFolder, "Upload");
        _convertedFolder = Path.Combine(_testFolder, "Converted");

        if (Directory.Exists(_testFolder))
        {
            Directory.Delete(_testFolder, true);
        }

        Directory.CreateDirectory(_testFolder);
        Directory.CreateDirectory(_uploadFolder);
        Directory.CreateDirectory(_convertedFolder);
    }

    [Test]
    public async Task Test1()
    {
        string guid = Guid.NewGuid().ToString();
        File.Copy("TestVideos/video1.mp4", Path.Combine(_uploadFolder, "guid"));
        File.WriteAllText(Path.Combine(_uploadFolder, $"{guid}.startrequest"), "testtoken");
        File.WriteAllText(Path.Combine(_uploadFolder, $"{guid}.txt"), $"{guid}{Environment.NewLine}.mp4");

        var transcodingManager = new TranscodingManager(new Ffmpeg(_ffmpegPath), new Settings()
        {
            UploadFolder = _uploadFolder,
            ConvertedFolder = _convertedFolder,
            VideoAudioConverters = new Dictionary<VideoType, AudioType>()
            {
                { VideoType.x264, AudioType.aac }
            },
            AspectRatios = new VideoAspectRatios[]
            {
                VideoAspectRatios.p720,
                VideoAspectRatios.p360
            },
            VideoFileExtension = new Dictionary<VideoType, string>()
            {
                { VideoType.x264, "mp4" }
            }
        });
        int transcodeCount = await transcodingManager.Transcode(CancellationToken.None);
        Assert.That(transcodeCount, Is.EqualTo(2));
    }
}