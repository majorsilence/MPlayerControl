using Majorsilence.Media.Videos;
using Majorsilence.Media.WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<Settings>((s) =>
        {
            return s.GetService<IConfiguration>()
                .GetSection("ApiSettings")
                .Get<Settings>();
        });

        services.AddSingleton<IVideoEncoder>((s) =>
        {
            return new Mencoder(s.GetService<Settings>().MEncoderPath);
        });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
