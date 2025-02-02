using Avalonia;
using System;

namespace Majorsilence.Media.Desktop.UI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            string path = "";
            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i] == "-path")
                {
                    path = Environment.GetCommandLineArgs()[i + 1].Trim();
                }
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                Properties.Settings.Default.MPlayerPath = path;
                Properties.Settings.Default.Save();
            }

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}