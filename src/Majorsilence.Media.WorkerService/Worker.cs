using System.Globalization;
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Settings _settings;
    private readonly IVideoEncoder _videoEncoder;

    public Worker(ILogger<Worker> logger, IVideoEncoder videoEncoder, Settings settings)
    {
        _logger = logger;
        _videoEncoder = videoEncoder;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int countNothingToDo = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            string extraConvertedSubFolder = System.IO.Path.Combine(_settings.ConvertedFolder,
                DateTime.UtcNow.Year.ToString(),
                DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString());
            var x = new TranscodingManager(_videoEncoder, _settings);
            int transcodingCount = await x.Transcode(extraConvertedSubFolder, stoppingToken);
            if (transcodingCount == 0)
            {
                countNothingToDo++;
                if (countNothingToDo > 60)
                {
                    await Task.Delay(60000, stoppingToken);
                    continue;
                }

                await Task.Delay(1000 * countNothingToDo, stoppingToken);
            }
            else
            {
                countNothingToDo = 0;
            }
        }
    }
}