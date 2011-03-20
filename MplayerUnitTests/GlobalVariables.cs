using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MplayerUnitTests
{
    public class GlobalVariables
    {

        // Test videos can be downloaded from:
        // http://files.majorsilence.com/TestVideos.zip
        // TODO: You must point the path to the mplayer.exe and the test videos to the location
        // were they exist on your computer.

        public static string MplayerPath
        {
            get { return @"C:\Program Files\MajorSilence\DeVeDe\bin\mplayer.exe"; }
        }


        public static string Video1Path
        {
            get { return @"C:\Documents and Settings\Peter\My Documents\Projects\TestVideos\TestVideo1-á.flv"; }
        }

        public static string Video2Path
        {
            get { return @"C:\Documents and Settings\Peter\My Documents\Projects\TestVideos\TestVideo2.flv"; }
        }

        public static string Video8Path
        {
            get { return @"C:\Documents and Settings\Peter\My Documents\Projects\TestVideos\TestVideo8.flv"; }
        }

        public static string OutputVideoWebM
        {
            get { return @"C:\Documents and Settings\Peter\My Documents\Projects\TestVideos\OutputVideoWebM.webm"; }
        }

        public static string OutputVideoX264
        {
            get { return @"C:\Documents and Settings\Peter\My Documents\Projects\TestVideos\OutputVideoX264.mp4"; }
        }

    }
}
