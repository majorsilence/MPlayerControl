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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace LibMPlayerCommon
{
    /// <summary>
    /// This class is used to discover information about a media file.
    /// </summary>
    public class Discover
    {
        private Discover() { }

        private int _VideoBitrate=0;
        /// <summary>
        /// Is the files video bitrate.  Kilobits per second.
        /// </summary>
        public int VideoBitrate 
        {
            get
            {
                return _VideoBitrate;
            }
        }

        private int _AudioBitrate=0;
        /// <summary>
        /// Is the files audio bitrate.  Kilobits per second.
        /// </summary>
        public int AudioBitrate
        {
            get
            {
                return _AudioBitrate;
            }
        }

        private int _AudioRate=0;
        /// <summary>
        /// Is the files audio rate.  What is the difference between this and AudioBitrate.
        /// </summary>
        public int AudioSampleRate
        {
            get
            {
                return _AudioRate;
            }
        }

        private int _Width=0;
        /// <summary>
        /// Is the videos width.
        /// </summary>
        public int Width
        {
            get
            {
                return _Width;
            }
        }

        private int _Height=0;
        /// <summary>
        /// Is the videos height.
        /// </summary>
        public int Height
        {
            get
            {
                return _Height;
            }
        }

        private int _fps;
        /// <summary>
        /// The videos frames per second.
        /// </summary>
        public int FPS
        {
            get
            {
                return _fps;
            }
        }

        private int _Length=0;
        /// <summary>
        /// The length of the video in seconds.
        /// </summary>
        public int Length
        {
            get
            {
                return _Length;
            }
        }

        private float _AspectRatio = 0f;
        /// <summary>
        /// The aspect ratio of the video. Could be 4/3 or 16/9.
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return _AspectRatio;
            }
        }

        private List<int> _AudioList;
        /// <summary>
        /// List of audio tracks in the video.
        /// </summary>
        public List<int> AudioList
        {
            get
            {
                return _AudioList;
            }
        }

        private Dictionary<int, string> _AudioTracks;
        public Dictionary<int, string> AudioTracks
        {
            get 
            {
                return _AudioTracks;
            }
        }

        private bool _Video = false;
        /// <summary>
        /// Returns true if the file contains video.
        /// </summary>
        public bool Video
        {
            get
            {
                return _Video;
            }
        }

        private bool _Audio = false;
        /// <summary>
        /// Returns true if the file contains audio.
        /// </summary>
        public bool Audio
        {
            get
            {
                return _Audio;
            }
        }


        public Discover(string filePath) : this(filePath, "") { }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mplayerPath">If mplayerPath is left empty it will search for mplayer.exe in 
        /// "current directory\backend\mplayer.exe" on windows and mplayer in the path on linux.</param>
        public Discover(string filePath, string mplayerPath)
        {

            /*
             Reads the values of the video (width, heigth, fps...) and stores them
             into file_values.
		
             Returns (False,AUDIO) if the file is not a video (with AUDIO the number
             of audio tracks)
		 	
             Returns (True,0) if the file is a right video file 
             */

            BackendPrograms mplayerLocation = new BackendPrograms(mplayerPath);

            int audio = 0;
            int video = 0;
            int nframes = 1;

            int minimum_audio = 10000;
            _AudioList = new List<int>();
            _AudioTracks = new Dictionary<int, string>();
            // if CHECK_AUDIO is TRUE, we just check if it's an audio file

            //if check_audio:
            //    nframes=0
            //else:
            //    nframes=1

            System.Diagnostics.Process handle = new System.Diagnostics.Process();

            handle.StartInfo.UseShellExecute = false;
            handle.StartInfo.CreateNoWindow = true;
            handle.StartInfo.RedirectStandardOutput = true;
            handle.StartInfo.RedirectStandardError = true;

            handle.StartInfo.FileName = mplayerLocation.MPlayer;
            handle.StartInfo.Arguments = string.Format("-loop 1 -identify -ao null -vo null -frames 0 {0} \"{1}\"", nframes.ToString(), filePath);
            handle.Start();
            string line = "";
            StringReader strReader = new StringReader(handle.StandardOutput.ReadToEnd());


            int previousAudioTrack = -1;

            while ((line = strReader.ReadLine()) != null)
            //while (handle.HasExited == false)
            {

                if (line.Trim() == "")
                {
                    continue;
                }
                int position = line.IndexOf("ID_");
                if (position == -1)
                {
                    continue;
                }
                line = line.Substring(position);
                if (line.StartsWith("ID_VIDEO_BITRATE"))
                {
                    _VideoBitrate = Globals.IntParse(line.Substring(17)) / 1000; // kilobits per second
                }
                else if (line.StartsWith("ID_VIDEO_WIDTH"))
                {
                    _Width = Globals.IntParse(line.Substring(15));
                }
                else if (line.StartsWith("ID_VIDEO_HEIGHT"))
                {
                    _Height = Globals.IntParse(line.Substring(16));
                }
                else if (line.StartsWith("ID_VIDEO_ASPECT"))
                {
                    _AspectRatio = Globals.FloatParse(line.Substring(16));
                }
                else if (line.StartsWith("ID_VIDEO_FPS"))
                {
                    _fps = (int)Globals.FloatParse(line.Substring(13));
                }
                else if (line.StartsWith("ID_AUDIO_BITRATE"))
                {
                    _AudioBitrate = Globals.IntParse(line.Substring(17)) / 1000; // kilobits per second
                }
                else if (line.StartsWith("ID_AUDIO_RATE"))
                {
                    _AudioRate = Globals.IntParse(line.Substring(14));
                }
                else if (line.StartsWith("ID_LENGTH"))
                {
                    _Length = (int)Globals.FloatParse(line.Substring(10));
                }
                else if (line.StartsWith("ID_VIDEO_ID"))
                {
                    video += 1;
                    _Video = true;
                }
                else if (line.StartsWith("ID_AUDIO_ID"))
                {
                    audio += 1;
                    _Audio = true;
                    int audio_track = Globals.IntParse(line.Substring(12));
                    if (minimum_audio > audio_track)
                    {
                        minimum_audio = audio_track;
                    }
                    _AudioList.Add(audio_track);

                    previousAudioTrack = audio_track;
                    
                }
                else if (line.StartsWith("ID_AID_0_LANG"))
                {
                    if (previousAudioTrack != -1)
                    {
                        string language = line.Substring(14);
                        _AudioTracks.Add(previousAudioTrack, language);
                    }
                }
            }

            handle.WaitForExit();
            handle.Close();

            if (_AspectRatio == 0.0)
            {
                _AspectRatio = ((float)_Width / (float)_Height);
                if (_AspectRatio <= 1.5)
                {
                    _AspectRatio = (ScreenAspectRatio.FourThree);
                }
            }

        }
    

    }
}
