/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of MediaPlayer.

MediaPlayer is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

MediaPlayer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/


using System;
using System.Windows.Forms;

namespace MediaPlayer
{
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {

            string path = "";
            for (int i = 0; i <= Environment.GetCommandLineArgs().Length - 1; i++)
            {
                if (Environment.GetCommandLineArgs()[i] == "-path")
                {
                    path = Environment.GetCommandLineArgs()[i + 1].Trim();
                }
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                MediaPlayer.Properties.Settings.Default.MPlayerPath = path;
                MediaPlayer.Properties.Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Player("", false, false));
        }
        
    }
}
