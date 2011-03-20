/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerCommon.

LibMPlayerCommon is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

LibMPlayerCommon is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

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

        private string _mplayerPath;
        private string _mencoderPath;

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

        public string MPlayer
        {
            get
            {
                if (OSPlatform() == "windows")
                {
                    if (_mplayerPath != "")
                    {
                        return _mplayerPath;
                    }
                    return  System.IO.Path.Combine(CurrentAssemblyDirectory(), "mplayer.exe");
                }
                else
                {
                    if (_mplayerPath != "")
                    {
                        return _mplayerPath;
                    }

                    return "mplayer";
                }
            }
        }

        public string MEncoder
        {
            get
            {
                if (OSPlatform() == "windows")
                {

                    if (_mencoderPath != "")
                    {
                        return _mencoderPath;
                    }

                    return System.IO.Path.Combine(CurrentAssemblyDirectory(), "mencoder.exe");
                }
                else
                {
                    if (_mencoderPath != "")
                    {
                        return _mencoderPath;
                    }

                    return "mencoder";
                }
            }
        }

    }
}
