using Majorsilence.Media.Videos;
using Majorsilence.Media.WorkerService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<Settings>(s =>
        {
            return s.GetService<IConfiguration>()
                .GetSection("ApiSettings")
                .Get<Settings>();
        });

        services.AddSingleton<IVideoEncoder>(s => { return new Ffmpeg(s.GetService<Settings>().FfmpegPath); });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();