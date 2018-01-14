using System;

namespace LibMPlayerCommon
{
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
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (System.IO.Directory.Exists("/Applications")
                        & System.IO.Directory.Exists("/System")
                        & System.IO.Directory.Exists("/Users")
                        & System.IO.Directory.Exists("/Volumes"))
                    {
                        return Platform.Mac;
                    }
                    else
                    {
                        return Platform.Linux;
                    }

                case PlatformID.MacOSX:
                    return Platform.Mac;
                default:
                    return Platform.Windows;
            }
        }

    }
}

