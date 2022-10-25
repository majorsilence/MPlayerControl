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
using System.Threading.Tasks;
using System.Linq;

namespace Majorsilence.Media.Videos
{
    /// <summary>
    /// This class is used to discover information about a media file.
    /// </summary>
    public class MpvDiscover : Discover
    {
        private Mpv _mpv;
        private readonly string filePath;
        private readonly string libmpvPath;

        private MpvDiscover ()
        {
        }

        public MpvDiscover (string filePath, string libmpvPath)
        {
            this.filePath = filePath;
            this.libmpvPath = libmpvPath;
        }

        ~MpvDiscover ()
        {
            // Cleanup

            _mpv.Dispose ();

        }

        bool disposed = false;

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposed)
                return;

            if (disposing) {
                _mpv.Dispose ();
            }

            disposed = true;
        }

        private int _VideoBitrate = 0;

        /// <summary>
        /// Is the files video bitrate.  Kilobits per second.
        /// </summary>
        public int VideoBitrate {
            get {
                return _VideoBitrate;
            }
        }

        private int _AudioBitrate = 0;

        /// <summary>
        /// Is the files audio bitrate.  Kilobits per second.
        /// </summary>
        public int AudioBitrate {
            get {
                return _AudioBitrate;
            }
        }

        private int _AudioRate = 0;

        /// <summary>
        /// Is the files audio rate.  What is the difference between this and AudioBitrate.
        /// </summary>
        public int AudioSampleRate {
            get {
                return _AudioRate;
            }
        }

        private int _Width = 0;

        /// <summary>
        /// Is the videos width.
        /// </summary>
        public int Width {
            get {
                return _Width;
            }
        }

        private int _Height = 0;

        /// <summary>
        /// Is the videos height.
        /// </summary>
        public int Height {
            get {
                return _Height;
            }
        }

        private int _fps;

        /// <summary>
        /// The videos frames per second.
        /// </summary>
        public int FPS {
            get {
                return _fps;
            }
        }

        private int _Length = 0;

        /// <summary>
        /// The length of the video in seconds.
        /// </summary>
        public int Length {
            get {
                return _Length;
            }
        }

        private float _AspectRatio = 0f;

        /// <summary>
        /// The aspect ratio of the video. Could be 4/3 or 16/9.
        /// </summary>
        public float AspectRatio {
            get {
                return _AspectRatio;
            }
        }

        private List<int> _AudioList;

        /// <summary>
        /// List of audio tracks in the video.
        /// </summary>
        public List<int> AudioList {
            get {
                return _AudioList;
            }
        }

        private List<AudioTrackInfo> _AudioTracks;

        public List<AudioTrackInfo> AudioTracks {
            get {
                return _AudioTracks;
            }
        }

        private bool _Video = false;

        /// <summary>
        /// Returns true if the file contains video.
        /// </summary>
        public bool Video {
            get {
                return _Video;
            }
        }

        private bool _Audio = false;

        /// <summary>
        /// Returns true if the file contains audio.
        /// </summary>
        public bool Audio {
            get {
                return _Audio;
            }
        }

        private List<SubtitlesInfo> _SubtitleList;

        public List<SubtitlesInfo> SubtitleList {
            get {
                return _SubtitleList;
            }
        }

      
        public Task ExecuteAsync ()
        {
            return Task.Run (() => Execute ());
        }

        public void Execute ()
        {
            // As work around can run mpv commandline and parse output
            // mpv video_name.mp4 --vo null -ao null --frames 1 -v



            /*
             Reads the values of the video (width, heigth, fps...) and stores them
             into file_values.
		
             Returns (False,AUDIO) if the file is not a video (with AUDIO the number
             of audio tracks)
		 	
             Returns (True,0) if the file is a right video file 
             */
            _mpv = new Mpv (libmpvPath);

            // Must be set before initializeation
            _mpv.SetOption ("frames", MpvFormat.MPV_FORMAT_INT64, 148);

            _mpv.Initialize ();

            _mpv.SetOption ("wid", MpvFormat.MPV_FORMAT_INT64, -1);
            _mpv.SetOption ("vo", MpvFormat.MPV_FORMAT_STRING, "null");
            _mpv.SetOption ("ao", MpvFormat.MPV_FORMAT_STRING, "null");

            _mpv.DoMpvCommand ("loadfile", filePath);

            // HACK: wait for video to load
            System.Threading.Thread.Sleep (1000);
            //_mpv.SetProperty ("pause", MpvFormat.MPV_FORMAT_STRING, "yes");
           
            _Width = _mpv.GetPropertyInt ("width");
            _Height = _mpv.GetPropertyInt ("height");
            _AspectRatio = _mpv.GetPropertyFloat ("video-aspect");

            int bits = _mpv.GetPropertyInt ("audio-bitrate");
            //int bytes = Bits2Bytes (bits);
            //int kb = Bytes2Kilobytes (bytes);
            //_AudioBitrate = (int)Math.Round (bits / 1024m, 0);
            _AudioBitrate = bits;           
            _AudioRate = _mpv.GetPropertyInt ("audio-params/samplerate");
            _Length = _mpv.GetPropertyInt ("duration");

            //_fps = _mpv.GetPropertyInt ("container-fps");
            _fps = _mpv.GetPropertyInt ("fps");
            _mpv.TryGetPropertyInt ("video-bitrate", out _VideoBitrate);

            string videoFormat = _mpv.GetProperty ("video-format");
            if (!string.IsNullOrWhiteSpace (videoFormat)) {
                _Video = true;
            }


            _AudioList = new List<int> ();
            _AudioTracks = new List<AudioTrackInfo> ();
            _SubtitleList = new List<SubtitlesInfo> ();

            int trackCount = _mpv.GetPropertyInt ("track-list/count");
            foreach (int i in Enumerable.Range (0, trackCount)) {
                string trackType = _mpv.GetProperty ($"track-list/{i}/type");
                int trackId = _mpv.GetPropertyInt ($"track-list/{i}/id");
                string name;

                _mpv.TryGetProperty ($"track-list/{i}/title", out name);
                string language;
                _mpv.TryGetProperty ($"track-list/{i}/lang", out language);

                if (trackType == "audio") {
                    _AudioList.Add (trackId);

                    var info = new AudioTrackInfo () {
                        ID = trackId,
                        Name = name,
                        Language = language
                    };

                    _AudioTracks.Add (info);
                } else if (trackType == "sub") {
                    var info = new SubtitlesInfo () {
                        ID = trackId,
                        Name = name,
                        Language = language
                    };

                    _SubtitleList.Add (info);
                }
                    
            }
                

            if (_AspectRatio == 0.0) {
                _AspectRatio = ((float)_Width / (float)_Height);
                if (_AspectRatio <= 1.5) {
                    _AspectRatio = (ScreenAspectRatio.FourThree);
                }
            }

        }
            
    }
        
}
