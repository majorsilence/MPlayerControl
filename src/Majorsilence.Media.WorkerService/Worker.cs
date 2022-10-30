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
                    $"{fileDetails.VideoId}{fileDetails.VideoExt}");

                string destVideo =
                    System.IO.Path.Combine(_settings.ConvertedFolder, $"{fileDetails.VideoId}_[placeholder]");

                _videoEncoder.Convert2WebM(srcVideo, 
                    destVideo.Replace("[placeholder]", "vp9.webm"));
                _videoEncoder.Convert(VideoType.webm, AudioType.opus, VideoAspectRatios.p240, srcVideo, 
                    destVideo.Replace("[placeholder]", "vp9_240p.webm"));
                _videoEncoder.Convert(VideoType.webm, AudioType.opus, VideoAspectRatios.p360, srcVideo, 
                    destVideo.Replace("[placeholder]", "vp9_360p.webm"));
                _videoEncoder.Convert(VideoType.webm, AudioType.opus, VideoAspectRatios.p480, srcVideo, 
                    destVideo.Replace("[placeholder]", "vp9_480p.webm"));
                _videoEncoder.Convert(VideoType.webm, AudioType.opus, VideoAspectRatios.p720, srcVideo, 
                    destVideo.Replace("[placeholder]", "vp9_720p.webm"));

                _videoEncoder.Convert2X264(srcVideo,
                    destVideo.Replace("[placeholder]", "x264.mp4"));
                
                _videoEncoder.Convert2X265(srcVideo, 
                    destVideo.Replace("[placeholder]", "x265.mp4"));
                
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<(string VideoId, string VideoExt, string DetailFilePath)> FindFileToProcess(
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
                string videoExt = lines[1];

                if (lines.Count() > 2)
                {
                    // already processing
                    string processingTime = lines[2];
                    Console.WriteLine($"Skipping value that started processing at {processingTime}");
                    continue;
                }

                return (videoId, videoExt, uploadDetailFile);
            }

            return (null, null, null);
        }
    }
}