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

using Majorsilence.Media.Videos.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Majorsilence.Media.Videos
{
    public class TimeConversion
    {

        /// <summary>
        /// return time in Hours:Minutes:Seconds format.
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <returns></returns>
        public static string ConvertTimeHHMMSS (int timeInSeconds)
        {

            if (timeInSeconds < 0) {
                throw new MPlayerControlException ("Invalid time.  Seconds must be greated then >= 0.  Seconds passed in was: " + timeInSeconds.ToString ());
            }

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            string time_string = "";

            if (timeInSeconds >= 3600) {
                hours = timeInSeconds / 3600;
                timeInSeconds = timeInSeconds - (hours * 3600);
            }

            if (timeInSeconds >= 60) {
                minutes = timeInSeconds / 60;
                timeInSeconds = timeInSeconds - (minutes * 60);
            }

            //remaining time is seconds
            seconds = timeInSeconds;

            time_string = time_string + hours.ToString ().PadLeft (2, '0') + ":" +
            minutes.ToString ().PadLeft (2, '0') + ":" + seconds.ToString ().PadLeft (2, '0');

            //return time in Hours:Minutes:Seconds format 
            return time_string;
        }

    }
}
