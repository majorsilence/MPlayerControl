/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerCommon.

LibMPlayerCommon is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
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

    public class Globals
    {
        private Globals() { }



        public static string MajorSilenceLocalAppDataDirectory
        {
            get
            {
                string msDir = null;
                msDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                msDir = System.IO.Path.Combine(msDir, "MajorSilence");
                return msDir;
            }
        }

        public static string MajorSilenceMPlayerLocalAppDataDirectory
        {
            get { return System.IO.Path.Combine(MajorSilenceLocalAppDataDirectory, "MPlayer"); }
        }

        public static string MajorSilenceMEncoderLocalAppDataDirectory
        {
            get { return System.IO.Path.Combine(MajorSilenceLocalAppDataDirectory, "MEncoder"); }
        }

    }

}
