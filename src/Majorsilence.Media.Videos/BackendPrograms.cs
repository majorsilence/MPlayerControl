/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

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
using System.IO;
using System.Reflection;

namespace Majorsilence.Media.Videos;

public class BackendPrograms
{
    private readonly string _mencoderPath;

    private readonly string _mplayerPath;

    public BackendPrograms()
    {
        _mplayerPath = "";
        _mencoderPath = "";
    }

    public BackendPrograms(string mplayerPath)
    {
        _mplayerPath = mplayerPath;
        _mencoderPath = "";
    }

    public BackendPrograms(string mplayerPath, string mencoderPath)
    {
        _mplayerPath = mplayerPath;
        _mencoderPath = mencoderPath;
    }

    public string MPlayer
    {
        get
        {
            if (OSPlatform() == "windows")
            {
                if (_mplayerPath != "") return _mplayerPath;
                return Path.Combine(CurrentAssemblyDirectory(), "mplayer.exe");
            }

            if (_mplayerPath != "") return _mplayerPath;

            return "mplayer";
        }
    }

    public string MEncoder
    {
        get
        {
            if (OSPlatform() == "windows")
            {
                if (_mencoderPath != "") return _mencoderPath;

                return Path.Combine(CurrentAssemblyDirectory(), "mencoder.exe");
            }

            if (_mencoderPath != "") return _mencoderPath;

            return "mencoder";
        }
    }


    public static string OSPlatform()
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix ||
            Environment.OSVersion.Platform == PlatformID.MacOSX) return "unix";
        return "windows";
    }

    /// <summary>
    ///     Return the directory of the current executing assembly
    /// </summary>
    /// <returns></returns>
    private static string CurrentAssemblyDirectory()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        location = Path.GetDirectoryName(location);
        return location;
    }
}