﻿using System;

namespace LibMPlayerCommon
{
    public static class PlayerFactory
    {
        /// <summary>
        /// Attempt to auto detect prefered backends.
        /// </summary>
        /// <param name="handle">Handle.</param>
        /// <param name="path">Path to mplayer executable or mpv library</param>
        public static Player Get(long handle, string path)
        {
            if (PlatformCheck.RunningPlatform() == Platform.Windows)
            {
                return PlayerFactory.Windows(handle, path);
            }
            else if (PlatformCheck.RunningPlatform() == Platform.Linux)
            {
                return PlayerFactory.Linux(handle, path);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static LibMPlayerCommon.Player Windows(long handle, string path)
        {
            if (path.Contains("mplayer"))
            {
                return new MPlayer(handle, MplayerBackends.Direct3D, path);
            }
            else if (path.Contains("libmpv"))
            {
                return new MpvPlayer(handle, path);
            }

            return null;
        }

        private static LibMPlayerCommon.Player Linux(long handle, string path)
        {
            if (path.Contains("mplayer"))
            {
                return new MPlayer(handle, MplayerBackends.XV, path);
            }
            else if (path.Contains("libmpv"))
            {
                return new MpvPlayer(handle, path);
            }

            return null;
        }
    }
}
