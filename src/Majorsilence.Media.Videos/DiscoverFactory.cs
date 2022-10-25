using System;

namespace Majorsilence.Media.Videos
{
    public static class DiscoverFactory
    {
        /// <summary>
        /// Attempt to auto detect prefered backends.
        /// </summary>
        /// <param name="file">The multimedia file to discover</param>
        /// <param name="path">Path to mplayer executable or mpv library</param>
        public static Discover Get (string file, string path)
        {
            if (PlatformCheck.RunningPlatform () == Platform.Windows) {
                return DiscoverFactory.Windows (file, path);
            } else if (PlatformCheck.RunningPlatform () == Platform.Linux) {
                return DiscoverFactory.Linux (file, path);
            } else if (PlatformCheck.RunningPlatform () == Platform.Mac) {
                return DiscoverFactory.Mac (file, path);
            } else {
                throw new NotImplementedException ();
            }
        }

        private static Majorsilence.Media.Videos.Discover Windows (string file, string path)
        {
            if (path.Contains ("mplayer")) {
                return new MPlayerDiscover (file, path);
            } else if (path.Contains ("mpv")) {
                return new MpvDiscover (file, path);
            }

            return null;
        }

        private static Majorsilence.Media.Videos.Discover Linux (string file, string path)
        {
            if (path.Contains ("mplayer")) {
                return new MPlayerDiscover (file, path);
            } else if (path.Contains ("libmpv")) {
                return new MpvDiscover (file, path);
            }

            return null;
        }

        private static Majorsilence.Media.Videos.Discover Mac (string file, string path)
        {
            if (path.Contains ("mplayer")) {
                return new MPlayerDiscover (file, path);
            } else if (path.Contains ("libmpv")) {
                return new MpvDiscover (file, path);
            }

            return null;
        }
    }
}

