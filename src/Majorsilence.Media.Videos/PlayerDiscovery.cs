/*

Copyright 2026 (C) Peter Gill <peter@majorsilence.com>

This file is part of Majorsilence.Media.Videos.

Majorsilence.Media.Videos is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

Majorsilence.Media.Videos is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Majorsilence.Media.Videos;

/// <summary>
///     Locates an installed playback backend so a first run doesn't have to ask the user where
///     mplayer or libmpv lives.
/// </summary>
public static class PlayerDiscovery
{
    // PlayerFactory picks its backend by looking for "mplayer" or "libmpv" in the path, so only
    // those two names are worth returning -- a bare "mpv" executable would come back unusable.
    private static readonly string[] MPlayerNames = { "mplayer", "mplayer.exe" };

    private static readonly string[] LibMpvNames =
    {
        "libmpv.so.2", "libmpv.so.1", "libmpv.so", "libmpv.dylib", "libmpv-2.dll", "libmpv-1.dll"
    };

    private static readonly string[] ExecutableDirectories =
    {
        "/usr/bin", "/usr/local/bin", "/bin", "/opt/homebrew/bin", "/snap/bin", "/var/lib/flatpak/exports/bin"
    };

    private static readonly string[] LibraryDirectories =
    {
        "/usr/lib/x86_64-linux-gnu", "/usr/lib/aarch64-linux-gnu", "/usr/lib64", "/usr/lib",
        "/usr/local/lib", "/lib/x86_64-linux-gnu", "/opt/homebrew/lib"
    };

    /// <summary>
    ///     Looks through the usual install locations (and PATH) for mplayer, then for the libmpv
    ///     shared library, and returns the first one that exists. Returns null when neither is
    ///     installed, in which case the caller still has to ask the user.
    /// </summary>
    public static string FindPlayerPath()
    {
        return FindMPlayer() ?? FindLibMpv();
    }

    /// <summary>
    ///     Returns the path to an installed mplayer executable, or null if there isn't one.
    /// </summary>
    public static string FindMPlayer()
    {
        return FindFirst(ExecutableDirectoriesWithPath(), MPlayerNames);
    }

    /// <summary>
    ///     Returns the path to an installed libmpv shared library, or null if there isn't one.
    /// </summary>
    public static string FindLibMpv()
    {
        return FindFirst(LibraryDirectories, LibMpvNames);
    }

    private static string FindFirst(IEnumerable<string> directories, string[] fileNames)
    {
        foreach (var directory in directories)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                continue;
            }

            foreach (var fileName in fileNames)
            {
                string candidate;
                try
                {
                    candidate = Path.Combine(directory, fileName);
                }
                catch (ArgumentException)
                {
                    // A malformed PATH entry -- skip it rather than fail the whole search.
                    continue;
                }

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    private static IEnumerable<string> ExecutableDirectoriesWithPath()
    {
        foreach (var directory in ExecutableDirectories)
        {
            yield return directory;
        }

        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path))
        {
            yield break;
        }

        foreach (var directory in path.Split(Path.PathSeparator))
        {
            yield return directory;
        }
    }
}
