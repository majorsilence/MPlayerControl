using System;
using System.Runtime.InteropServices;

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
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows;
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.Mac;
            }

            throw new PlatformNotSupportedException();
        }

    }
}

