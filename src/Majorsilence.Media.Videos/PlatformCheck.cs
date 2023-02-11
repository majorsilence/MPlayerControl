using System;
using System.Runtime.InteropServices;

namespace Majorsilence.Media.Videos;

public enum Platform
{
    Windows,
    Linux,
    Mac
}

public static class PlatformCheck
{
    public static Platform RunningPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Platform.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Platform.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return Platform.Mac;

        throw new PlatformNotSupportedException();
    }
}