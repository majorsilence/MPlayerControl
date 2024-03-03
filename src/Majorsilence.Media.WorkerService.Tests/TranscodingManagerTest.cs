using System.Diagnostics.Contracts;
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

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testFolder))
        {
            Directory.Delete(_testFolder, true);
        }
    }

    [Test]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p240)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p360)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p480)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p720)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p1080)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p1440)]
    [TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p2160)]
    //[TestCase(VideoType.vp9, AudioType.opus, VideoAspectRatios.p7680)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p240)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p360)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p480)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p720)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p1080)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p1440)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p2160)]
    //[TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.p7680)]
    [TestCase(VideoType.x264, AudioType.aac, VideoAspectRatios.original)]
    public async Task Test1(VideoType videoType, AudioType audioType, VideoAspectRatios aspectRatio)
    {
        string guid = Guid.NewGuid().ToString();
        File.Copy("TestVideos/video1.mp4", Path.Combine(_uploadFolder, guid));
        await File.WriteAllTextAsync(Path.Combine(_uploadFolder, $"{guid}.startrequest"), "testtoken");
        await File.WriteAllTextAsync(Path.Combine(_uploadFolder, $"{guid}.txt"), $"{guid}{Environment.NewLine}.mp4");

        var formatExtensions = new Dictionary<VideoType, string>()
        {
            { VideoType.x264, "mp4" },
            { VideoType.vp9, "webm" },
            { VideoType.x265, "mp4" },
            { VideoType.av1, "webm" },
            { VideoType.mpeg4, "mp4" }
        };
        var transcodingManager = new TranscodingManager(new Ffmpeg(_ffmpegPath), new Settings()
        {
            UploadFolder = _uploadFolder,
            ConvertedFolder = _convertedFolder,
            VideoAudioConverters = new Dictionary<VideoType, AudioType>()
            {
                { videoType, audioType }
            },
            AspectRatios = new VideoAspectRatios[]
            {
                aspectRatio
            },
            VideoFileExtension = formatExtensions
        });
        string extraConvertedSubFolder = System.IO.Path.Combine(DateTime.UtcNow.Year.ToString(),
            DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString());
        int transcodeCount = await transcodingManager.Transcode(extraConvertedSubFolder, CancellationToken.None);
        Assert.That(transcodeCount, Is.EqualTo(1));

        string fileExtension = formatExtensions[videoType];
        Assert.That(File.Exists(System.IO.Path.Combine(_convertedFolder, extraConvertedSubFolder, guid,
            $"{guid}_thumbnail.jpg")));
        Assert.That(File.Exists(System.IO.Path.Combine(_convertedFolder, extraConvertedSubFolder, guid,
            $"{guid}_{videoType.ToString()}_{aspectRatio}.{fileExtension}")));
    }
}