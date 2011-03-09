using System;
using System.Collections.Generic;
using System.Text;

namespace LibMPlayerCommon
{
    public class TimeConversion
    {

        /// <summary>
        /// return time in Hours:Minutes:Seconds format.
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <returns></returns>
        public static string ConvertTimeHHMMSS(int timeInSeconds)
        {

            if (timeInSeconds < 0)
            {
                throw new Exception("Invalid time.  Seconds must be greated then >= 0.  Seconds passed in was: " + timeInSeconds.ToString());
            }

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            string time_string = "";

            if (timeInSeconds >= 3600)
            {
                hours = timeInSeconds / 3600;
                timeInSeconds = timeInSeconds - (hours * 3600);
            }

            if (timeInSeconds >= 60)
            {
                minutes = timeInSeconds / 60;
                timeInSeconds = timeInSeconds - (minutes * 60);
            }

            //remaining time is seconds
            seconds = timeInSeconds;

            time_string = time_string + hours.ToString().PadLeft(2, '0') + ":" + 
                minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');

            //return time in Hours:Minutes:Seconds format 
            return time_string;
      }

    }
}
