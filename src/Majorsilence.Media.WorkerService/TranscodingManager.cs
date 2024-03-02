using System.Globalization;
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.WorkerService;

public class TranscodingManager
{
    private readonly Settings _settings;
    private readonly IVideoEncoder _videoEncoder;

    public TranscodingManager(IVideoEncoder videoEncoder, Settings settings)
    {
        _videoEncoder = videoEncoder;
        _settings = settings;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns>Number of transcoding loops</returns>
    public async Task<int> Transcode(CancellationToken stoppingToken)
    {
        var fileDetails = await FindFileToProcess(stoppingToken);
        if (string.IsNullOrEmpty(fileDetails.DetailFilePath))
        {
            return 0;
        }

        var processingStartTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        await File.AppendAllTextAsync(fileDetails.DetailFilePath,
            $"{Environment.NewLine}{processingStartTime}", stoppingToken);

        var srcVideo = Path.Combine(_settings.UploadFolder,
            $"{fileDetails.VideoId}");

        string destFolder = System.IO.Path.Combine(_settings.ConvertedFolder, DateTime.UtcNow.Year.ToString(),
            DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString());
        if (!Directory.Exists(destFolder))
        {
            Directory.CreateDirectory(destFolder);
        }

        var destVideo =
            Path.Combine(destFolder, $"{fileDetails.VideoId}_[placeholder]");

        int transcodingCount = 0;
        foreach (var audVidType in _settings.VideoAudioConverters)
        foreach (var aspect in _settings.AspectRatios)
        {
            var ext = _settings.VideoFileExtension
                .FirstOrDefault(p => p.Key == audVidType.Key)
                .Value
                .ToString();
            await _videoEncoder.ConvertAsync(audVidType.Key, audVidType.Value, aspect, srcVideo,
                destVideo.Replace("[placeholder]",
                    $"{audVidType.Key.ToString()}_{aspect.ToString()}.{ext}"),
                stoppingToken);
            transcodingCount++;
        }

        File.Delete(srcVideo);
        File.Delete(fileDetails.DetailFilePath);
        File.Delete($"{srcVideo}.txt");
        File.Delete(fileDetails.StartRequestFilePath);
        File.Create(Path.Combine(destFolder, $"{fileDetails.VideoId}.done"));
        return transcodingCount;
    }

    private async Task<(string VideoId, string DetailFilePath, string StartRequestFilePath)> FindFileToProcess(
        CancellationToken stoppingToken)
    {
        var dInfo = new DirectoryInfo(_settings.UploadFolder);
        var foundFiles = dInfo.GetFiles("*.txt").OrderBy(p => p.CreationTimeUtc).Select(p => p.FullName).ToArray();

        foreach (var uploadDetailFile in foundFiles)
        {
            if (string.IsNullOrWhiteSpace(uploadDetailFile))
            {
                continue;
            }

            var lines = await File.ReadAllLinesAsync(uploadDetailFile, stoppingToken);
            var videoId = lines[0];

            if (lines.Length > 2)
            {
                // already processing
                var processingTime = lines[2];
                Console.WriteLine($"Skipping value that started processing at {processingTime}");
                continue;
            }

            string startRequestFilePath =
                System.IO.Path.Combine(_settings.UploadFolder,
                    System.IO.Path.GetFileNameWithoutExtension(uploadDetailFile) + ".startrequest");
            return (videoId, uploadDetailFile, startRequestFilePath);
        }

        return (null, null, null);
    }
}