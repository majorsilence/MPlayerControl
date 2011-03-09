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
    public class Logging
    {
        private static volatile Logging instance;
        private static object syncRoot = new Object();

        private Logging() { }

        public static Logging Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Logging();
                        }
                    }
                }

                return instance;
            }
        }

        public void WriteLine(string msg)
        {
            WriteLine(msg, "UNKNOWN");
        }
        public void WriteLine(string msg, string category)
        {

            string filePath = System.IO.Path.Combine(Globals.MajorSilenceMPlayerLocalAppDataDirectory, "MajorSilence-Debug.txt");

            if (System.IO.Directory.Exists(Globals.MajorSilenceMPlayerLocalAppDataDirectory) == false)
            {
                System.IO.Directory.CreateDirectory(Globals.MajorSilenceMPlayerLocalAppDataDirectory);
            }



            System.IO.File.AppendAllText(filePath, DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + System.Environment.NewLine);
            System.IO.File.AppendAllText(filePath, category + System.Environment.NewLine);
            System.IO.File.AppendAllText(filePath, msg + System.Environment.NewLine);
            System.IO.File.AppendAllText(filePath, System.Environment.NewLine + System.Environment.NewLine);

        }

        public void WriteLine(Exception value)
        {
            WriteLine(value, "UNKNOWN");
        }
        public void WriteLine(Exception value, string category)
        {
            WriteLine(value.Message + System.Environment.NewLine + value.StackTrace, category);
        }

    }
}
