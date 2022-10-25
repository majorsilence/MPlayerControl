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
using System.Collections.Generic;
using System.Text;

namespace Majorsilence.Media.Videos
{
    public class Logging
    {
        // TODO: replace with real logging framework

        private static volatile Logging instance;
        private static object syncRoot = new Object();

        private string filePath;


        private Logging() 
        { 
            this.filePath = System.IO.Path.Combine(Globals.MajorSilenceMPlayerLocalAppDataDirectory, "MajorSilence-Debug.txt");
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(this.filePath));
            System.Diagnostics.Trace.AutoFlush = true;
        }

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

                if (System.IO.Directory.Exists(Globals.MajorSilenceMPlayerLocalAppDataDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(Globals.MajorSilenceMPlayerLocalAppDataDirectory);
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
            string output = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + System.Environment.NewLine;
            output += msg + System.Environment.NewLine;
            output += System.Environment.NewLine + System.Environment.NewLine;

            System.Diagnostics.Trace.WriteLine(output, category);
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
