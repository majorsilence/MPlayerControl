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
using System.Diagnostics;
using System.IO;

namespace Majorsilence.Media.Videos;

public class Logging
{
    // TODO: replace with real logging framework

    private static volatile Logging instance;
    private static readonly object syncRoot = new();

    private readonly string filePath;


    private Logging()
    {
        filePath = Path.Combine(Globals.MajorSilenceMPlayerLocalAppDataDirectory, "MajorSilence-Debug.txt");
        Trace.Listeners.Add(new TextWriterTraceListener(filePath));
        Trace.AutoFlush = true;
    }

    public static Logging Instance
    {
        get
        {
            if (instance == null)
                lock (syncRoot)
                {
                    if (instance == null) instance = new Logging();
                }

            if (Directory.Exists(Globals.MajorSilenceMPlayerLocalAppDataDirectory) == false)
                Directory.CreateDirectory(Globals.MajorSilenceMPlayerLocalAppDataDirectory);

            return instance;
        }
    }

    public void WriteLine(string msg)
    {
        WriteLine(msg, "UNKNOWN");
    }

    public void WriteLine(string msg, string category)
    {
        var output = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
        output += msg + Environment.NewLine;
        output += Environment.NewLine + Environment.NewLine;

        Trace.WriteLine(output, category);
    }

    public void WriteLine(Exception value)
    {
        WriteLine(value, "UNKNOWN");
    }

    public void WriteLine(Exception value, string category)
    {
        WriteLine(value.Message + Environment.NewLine + value.StackTrace, category);
    }
}