using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace MplayerUnitTests;

[SetUpFixture]
public class SetupInitialize
{
    private static string finalPath;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var path = Path.Combine(Path.GetTempPath(), "MPlayerControl", "Tests");
        finalPath = Path.Combine(path, "TestVideos");

        FinalTearDown();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);


        var files = Directory.GetFiles(Path.Combine(ExecutingDirectory(), "TestVideos"));
        foreach (var file in files) File.Copy(file, Path.Combine(finalPath, Path.GetFileName(file)));

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            GlobalVariables.InitPath(finalPath,
                @"C:\Users\peter\Downloads\MPlayer-x86_64-r38154+g9fe07908c3\mplayer.exe",
                @"C:\Users\peter\Downloads\mpv-dev-x86_64-20200202-git-77a74d9\mpv-1.dll");
        else
            GlobalVariables.InitPath(finalPath, "mplayer", "/usr/lib/x86_64-linux-gnu/libmpv.so.1");
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        if (Directory.Exists(finalPath)) Directory.Delete(finalPath, true);
    }

    private static string ExecutingDirectory()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        location = Path.GetDirectoryName(location);
        return location;
    }
}