using System.Globalization;
using Majorsilence.Media.Videos;
using System.Text;

namespace Majorsilence.Media.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IVideoEncoder _videoEncoder;
        private readonly Settings _settings;

        public Worker(ILogger<Worker> logger, IVideoEncoder videoEncoder, Settings settings)
        {
            _logger = logger;
            _videoEncoder = videoEncoder;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var fileDetails = await FindFileToProcess(stoppingToken);
                if (string.IsNullOrEmpty(fileDetails.DetailFilePath))
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                string processingStartTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                await System.IO.File.AppendAllTextAsync(fileDetails.DetailFilePath,
                    $"{Environment.NewLine}{processingStartTime}", stoppingToken);

                string srcVideo = System.IO.Path.Combine(_settings.UploadFolder,
                    $"{fileDetails.VideoId}");

                string destVideo =
                    System.IO.Path.Combine(_settings.ConvertedFolder, $"{fileDetails.VideoId}_[placeholder]");

                foreach (var audVidType in _settings.VideoAudioConverters)
                {
                    foreach (var aspect in _settings.AspectRatios)
                    {
                        string ext = _settings.VideoFileExtension
                            .FirstOrDefault(p => p.Key == audVidType.Key)
                            .Value
                            .ToString();
                        _videoEncoder.Convert(audVidType.Key, audVidType.Value, aspect, srcVideo,
                            destVideo.Replace("[placeholder]",
                                $"{audVidType.Key.ToString()}_{aspect.ToString()}.{ext}"));
                    }
                }

                File.Delete(srcVideo);
                File.Delete(fileDetails.DetailFilePath);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<(string VideoId, string DetailFilePath)> FindFileToProcess(
            CancellationToken stoppingToken)
        {
            var foundFiles = System.IO.Directory.GetFiles(_settings.UploadFolder, "*.txt");

            foreach (var uploadDetailFile in foundFiles)
            {
                if (string.IsNullOrWhiteSpace(uploadDetailFile))
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var lines = await System.IO.File.ReadAllLinesAsync(uploadDetailFile);
                string videoId = lines[0];


                if (lines.Count() > 2)
                {
                    // already processing
                    string processingTime = lines[2];
                    Console.WriteLine($"Skipping value that started processing at {processingTime}");
                    continue;
                }

                return (videoId, uploadDetailFile);
            }

            return (null, null);
        }
    }
}