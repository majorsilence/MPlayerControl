/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerCommon.

LibMPlayerCommon is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

LibMPlayerCommon is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Text;

namespace LibMPlayerCommon
{
    public class BackendPrograms
    {

        private static string OSPlatform()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix || System.Environment.OSVersion.Platform == System.PlatformID.MacOSX)
            {
                return "unix";
            }
            return "windows";
        }

        /// <summary>
        /// Return the directory of the current executing assembly
        /// </summary>
        /// <returns></returns>
        private static string CurrentAssemblyDirectory()
        {
            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            location = System.IO.Path.GetDirectoryName(location);
            return location;

        }

        public static string MPlayer
        {
            get
            {
                if (OSPlatform() == "windows")
                {
                    string t = System.IO.Path.Combine(CurrentAssemblyDirectory(), "backend");
                    return System.IO.Path.Combine(t, "mplayer.exe");
                }
                else
                {
                    return "mplayer";
                }
            }
        }

        public static string MEncoder
        {
            get
            {
                if (OSPlatform() == "windows")
                {
                    string t = System.IO.Path.Combine(CurrentAssemblyDirectory(), "backend");
                    return System.IO.Path.Combine(t, "mencoder.exe");
                }
                else
                {
                    return "mencoder";
                }
            }
        }

    }
}
