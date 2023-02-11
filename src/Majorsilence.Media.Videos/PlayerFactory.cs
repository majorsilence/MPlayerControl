using System;

namespace Majorsilence.Media.Videos;

public static class PlayerFactory
{
    /// <summary>
    ///     Attempt to auto detect prefered backends.
    /// </summary>
    /// <param name="handle">Handle.</param>
    /// <param name="path">Path to mplayer executable or mpv library</param>
    public static Player Get(long handle, string path)
    {
        // -path "/usr/lib/x86_64-linux-gnu/libmpv.so.1"

        if (PlatformCheck.RunningPlatform() == Platform.Windows)
            return Windows(handle, path);
        if (PlatformCheck.RunningPlatform() == Platform.Linux)
            return Linux(handle, path);
        if (PlatformCheck.RunningPlatform() == Platform.Mac)
            return Mac(handle, path);
        throw new NotImplementedException();
    }

    private static Player Windows(long handle, string path)
    {
        if (path.Contains("mplayer"))
            return new MPlayer(handle, MplayerBackends.Direct3D, path);
        if (path.Contains("mpv")) return new MpvPlayer(handle, path);

        return null;
    }

    private static Player Linux(long handle, string path)
    {
        if (path.Contains("mplayer"))
            return new MPlayer(handle, MplayerBackends.XV, path);
        if (path.Contains("libmpv")) return new MpvPlayer(handle, path);

        return null;
    }

    private static Player Mac(long handle, string path)
    {
        if (path.Contains("mplayer"))
            return new MPlayer(handle, MplayerBackends.OpenGL, path);
        if (path.Contains("libmpv")) return new MpvPlayer(handle, path);

        return null;
    }
}