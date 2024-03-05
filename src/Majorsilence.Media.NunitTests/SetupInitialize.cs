using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace MplayerUnitTests;

[SetUpFixture]
public class SetupInitialize
{
    private static string finalPath;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        finalPath = GlobalVariables.BasePath;
        FinalTearDown();
        if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
        
        var files = Directory.GetFiles(Path.Combine(ExecutingDirectory(), "TestVideos"));
        foreach (var file in files) File.Copy(file, Path.Combine(finalPath, Path.GetFileName(file)));
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