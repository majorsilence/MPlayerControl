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
using System.Timers;


using System.Diagnostics;


namespace LibMPlayerCommon
{

    public class MPlayer
    {
        private int _wid;
        private bool _fullscreen;
        private int _mplayerProcessID = -1;
        private MplayerBackends _mplayerBackend;

        // Current position in seconds in stream.
        private int _currentPosition = 0;

        // The total length that the video is in seconds.
        private int _totalTime = 0;
       
        private string currentFilePath;
        private string _mplayerPath;

        private BackendPrograms _backendProgram;

        public event MplayerEventHandler VideoExited;
        /// <summary>
        /// This event is the most accurate way to get the current position of the current playing file.
        /// Whenever the postion changes this event will notify that the positon has changed with the value
        /// being the new position (seconds into the file).
        /// </summary>
        public event MplayerEventHandler CurrentPosition;
        public event MplayerEventHandler Cache;
        public event MplayerEventHandler Finalfile;

        public event MplayerEventHandler Scanfonts;
        public event MplayerEventHandler Filesub;
        public event MplayerEventHandler Audiochannel;
        public event MplayerEventHandler Setaudiolang;
        private System.Timers.Timer _currentPostionTimer;
        
        ///vars for mplayer info
        private string _getfileinfofilename;
        private string _cache;
        private string _percentpos;
        private string _getinfotitle;
        private string _finalfilecode;
        private string _scanning;
        private string _filesub;
        private string _audiochannel;
        private string _setaudiolang;
        ///
        private MPlayer()
        {
        }

        public MPlayer(int wid, MplayerBackends backend)
            : this(wid, backend, "")
        {
        }

        public MPlayer(int wid, MplayerBackends backend, string mplayerPath)
            : this(wid, backend, mplayerPath, false)
        {
        }

        /// <summary>
        /// Create a new instance of mplayer class.
        /// </summary>
        /// <param name="wid">Window ID that mplayer should attach itself</param>
        /// <param name="backend">The video output backend that mplayer will use.</param>
        /// <param name="mplayerPath">The full filepath to mplayer.exe.  If mplayerPath is left empty it will search for mplayer.exe in 
        /// "current directory\backend\mplayer.exe" on windows and mplayer in the path on linux.</param>
        /// <param name="loadMplayer">If true mplayer will immediately be loaded and you should not attempt to 
        /// play any files until MplayerRunning is true.</param>
        public MPlayer(int wid, MplayerBackends backend, string mplayerPath, bool loadMplayer)
        { 
            this._wid = wid;
            this._fullscreen = false;
            this.MplayerRunning = false;
            this._mplayerBackend = backend;
            this._mplayerPath = mplayerPath;
            this.CurrentStatus = MediaStatus.Stopped;

            this._backendProgram = new BackendPrograms(mplayerPath);

            MediaPlayer = new System.Diagnostics.Process();

            // This timer will send an event every second with the current video postion when video
            // is in play mode.
            this._currentPostionTimer = new System.Timers.Timer(1000);
            this._currentPostionTimer.Elapsed += new ElapsedEventHandler(_currentPostionTimer_Elapsed);
            this._currentPostionTimer.Enabled = true;

            if (loadMplayer)
            {
                Action caller = new Action(InitializeMplayer);
                caller.BeginInvoke(null, null);
            }

        }

        /// <summary>
        /// Cleanup resources.  Currently this means that mplayer is closed if it is still running.
        /// </summary>
        ~MPlayer()
        {
            // Cleanup

            if (this._mplayerProcessID != -1)
            {
                try
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(this._mplayerProcessID);
                    if (p.HasExited == false)
                    {
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Instance.WriteLine(ex);
                }
            }
        }


        /// <summary>
        /// Is mplayer alreadying running?  True or False.
        /// </summary>
        public bool MplayerRunning { get; set; }

        /// <summary>
        /// The current status of the player.
        /// </summary>
        public MediaStatus CurrentStatus { get; set; }

        public bool HardwareAccelerated { get; set; }


        /// <summary>
        /// The process that is running mplayer.
        /// </summary>
        private System.Diagnostics.Process MediaPlayer { get; set; }

        private string MplayerBackend()
        {
            string backend;
            if (this._mplayerBackend == MplayerBackends.Direct3D)
            {
                backend = "direct3d";
            }
            else if (this._mplayerBackend == MplayerBackends.X11)
            {
                backend = "x11";
            }
            else if (this._mplayerBackend == MplayerBackends.Vdpau)
            {
                backend = "vdpau";
            }
            else if (this._mplayerBackend == MplayerBackends.XV)
            {
                backend = "xv";
            }
            else if (this._mplayerBackend == MplayerBackends.Quartz)
            {
                backend = "quartz";
            }
            else if (this._mplayerBackend == MplayerBackends.CoreVideo)
            {
                backend = "corevideo";
            }
            else if (this._mplayerBackend == MplayerBackends.SDL)
            {
                backend = "sdl";
            }
            else if (this._mplayerBackend == MplayerBackends.GL)
            {
                backend = "gl";
            }
            else if (this._mplayerBackend == MplayerBackends.GL2)
            {
                backend = "gl2";
            }
            else if (this._mplayerBackend == MplayerBackends.ASCII)
            {
                backend = "aa";
            }
            else if (this._mplayerBackend == MplayerBackends.ColorASCII)
            {
                backend = "caca";
            }
            else if (this._mplayerBackend == MplayerBackends.Directfb)
            {
                backend = "directfb";
            }
            else if (this._mplayerBackend == MplayerBackends.Wii)
            {
                backend = "wii";
            }
            else if (this._mplayerBackend == MplayerBackends.V4l2)
            {
                backend = "v4l2";
            }
            else if (this._mplayerBackend == MplayerBackends.VESA)
            {
                backend = "vesa";
            }
            else
            {
                backend = "opengl";
            }

            return backend;
        }

        private void InitializeMplayer()
        {

            MediaPlayer.StartInfo.CreateNoWindow = true;
            MediaPlayer.StartInfo.UseShellExecute = false;
            MediaPlayer.StartInfo.ErrorDialog = false;
            MediaPlayer.StartInfo.RedirectStandardOutput = true;
            MediaPlayer.StartInfo.RedirectStandardInput = true;
            MediaPlayer.StartInfo.RedirectStandardError = true;

            this._currentPostionTimer.Start();


            //
            //slave
            //    mandatory; tells MPlayer to start in slave mode
            //quiet
            //    optional; reduces the amount of messages that MPlayer will output
            //idle
            //    optional; it doesn’t close MPlayer after a file finished playing; 
            //    this is quite useful as you don’t want to start a new process everytime 
            //    you want to play a file, but rather loading the file into the existing 
            //    already started process (for performance reasons)
            //
            // -ss HH:MM:SS  seek the position. Works with webm.
            //
            // -bandwidth <bytes>    Specify the maximum bandwidth for network streaming (for servers
            //  that are able to send content in different bitrates).
            //
            // -cache
            //
            // -prefer-ipv4   Use  IPv4  on network connections.  Falls back on IPv6 automatically.
            //
            // -user <username>   Specify username for HTTP authentication.
            // -passwd <password> Specify password for HTTP authentication.
            //
            // -user-agent <string>
            //  Use <string> as user agent for HTTP streaming.
            //
            // -wid <window ID> (also see -guiwid) (X11, OpenGL and DirectX only)
            //
            // -vc <[-|+]codec1,[-|+]codec2,...[,]>
            //  Specify a priority list of video codecs to be used, according to
            //  their  codec  name  in  codecs.conf. 


            string backend = MplayerBackend();
            /*
                if you like you can parse more args
                MediaPlayer.StartInfo.Arguments = string.Format("-slave -quiet -idle -priority abovenormal -nodr -double -nokeepaspect -cache 8192 -nofs -autosync 100 -mc 2.0 -nomouseinput -framedrop -osdlevel 0 -lavdopts threads=4 -ao dsound -v -monitorpixelaspect 1 -ontop -font \"{0}\" -subfont-autoscale {1} -subfont-text-scale {2} -subcp {3} -subpos {4} -volume {5} -vo {6} -wid {7} \"{8}\"", this._font, this._fontautoscale, this._textscale, this._subcp, this._subpos, this.volumemain, backend, this._wid, filePath);

            */
            MediaPlayer.StartInfo.Arguments = string.Format("-slave -quiet -idle -aspect 4/3 -v -ontop -vo {0} -wid {1}", backend, this._wid);
            MediaPlayer.StartInfo.FileName = this._backendProgram.MPlayer;

            MediaPlayer.Start();


            this.MplayerRunning = true;
            this._mplayerProcessID = MediaPlayer.Id;

            //System.IO.StreamWriter mw = MediaPlayer.StandardInput;
            //mw.AutoFlush = true;

            MediaPlayer.OutputDataReceived += HandleMediaPlayerOutputDataReceived;
            MediaPlayer.ErrorDataReceived += HandleMediaPlayerErrorDataReceived;
            MediaPlayer.BeginErrorReadLine();
            MediaPlayer.BeginOutputReadLine();



        }

        /// <summary>
        /// Load and start playing a video.
        /// </summary>
        /// <param name="filePath"></param>
        public void Play(string filePath)
        {
            this.currentFilePath = filePath;


            if (this.MplayerRunning == false)
            {
                InitializeMplayer();
            }

            LoadFile(filePath);
            this.CurrentStatus = MediaStatus.Playing;



        }

        /// <summary>
        /// Starts a new video/audio file immediatly.  Requires that Play has been called.
        /// </summary>
        /// <param name="filePath">string</param>
        public void LoadFile(string filePath)
        {
            string LoadCommand = @"" + string.Format("loadfile \"{0}\"", PrepareFilePath(filePath));
            MediaPlayer.StandardInput.WriteLine(LoadCommand);
            //MediaPlayer.StandardInput.WriteLine("play");
            MediaPlayer.StandardInput.Flush();
            this.LoadCurrentPlayingFileLength();
        }

        /// <summary>
        /// Prepare filepaths to be used witht the loadfile command.  
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// For some reason it strips the DirectorySeperatorChar so we double it up here.
        /// </remarks>
        private string PrepareFilePath(string filePath)
        {

            string preparedPath = filePath.Replace("" + System.IO.Path.DirectorySeparatorChar, "" + System.IO.Path.DirectorySeparatorChar + System.IO.Path.DirectorySeparatorChar);

            return preparedPath;
        }


        /// <summary>
        /// Move to a new position in the video.
        /// </summary>
        /// <param name="timePosition">Seconds.  The position to seek move to.</param>
        public void MovePosition(int timePosition)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property time_pos {0}", timePosition));
            MediaPlayer.StandardInput.Flush();
        }



        /// <summary>
        /// Seek a new postion.
        /// Seek to some place in the movie.
        /// Seek.Relative is a relative seek of +/- value seconds (default).
        /// Seek.Percentage is a seek to value % in the movie.
        /// Seek.Absolute is a seek to an absolute position of value seconds.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void Seek(int value, Seek type)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("seek {0} {1}", value, type));
            MediaPlayer.StandardInput.Flush();
        }

        public void SetSize(int width, int height)
        {
            if (this.CurrentStatus != MediaStatus.Playing)
            {
                return;
            }
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property width {0}", width));
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property height {0}", height));
            MediaPlayer.StandardInput.Flush();
        }

        /// <summary>
        /// Pause the current video.  If paused it will unpause.
        /// </summary>
        public void Pause()
        {
            if (this.CurrentStatus != MediaStatus.Playing && this.CurrentStatus != MediaStatus.Paused)
            {
                return;
            }

            try
            {
                MediaPlayer.StandardInput.WriteLine("pause");
                MediaPlayer.StandardInput.Flush();
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(ex);
                return;
            }

            if (this.CurrentStatus == MediaStatus.Paused)
            {
                this.CurrentStatus = MediaStatus.Playing;
            }
            else
            {
                this.CurrentStatus = MediaStatus.Paused;
            }
        }

        /// <summary>
        /// Stop the current video.
        /// </summary>
        public void Stop()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("stop");
                MediaPlayer.StandardInput.Flush();
            }
            this.CurrentStatus = MediaStatus.Stopped;
        }

        /// <summary>
        /// Close MPlayer instance.
        /// </summary>
        public void Quit()
        {
            try
            {
                MediaPlayer.StandardInput.WriteLine("quit");
                MediaPlayer.StandardInput.Flush();
            }
            catch (ObjectDisposedException ex)
            {
                Logging.Instance.WriteLine(ex);
            }
        }
        /// <summary>
        /// set percent position of mplayer 
        /// </summary>
        public void SetPercent(int v)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property percent_pos {0}", v));
            MediaPlayer.StandardInput.Flush();
        }

        /// <summary>
        /// Retrieves the number of seconds of the current playing video.
        /// </summary>
        public int CurrentPlayingFileLength()
        {
            return this._totalTime;
        }
        // Sets in motions events to set this._totalTime.  Is called as soon as the video starts.
        private void LoadCurrentPlayingFileLength()
        {
            // This works even with streaming.
            Discover file = new Discover(this.currentFilePath, this._backendProgram.MPlayer);
            file.Execute();
            this._totalTime = file.Length;
        }


        private void _currentPostionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.CurrentStatus == MediaStatus.Playing)
            {
                MediaPlayer.StandardInput.WriteLine("get_time_pos");
                MediaPlayer.StandardInput.Flush();
            }
        }
        /// <summary>
        /// get percent positiob
        /// </summary>
        public string GetPercentPos()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_percent_pos");
                MediaPlayer.StandardInput.Flush();
                return this._percentpos;
            }
            return "";
        this._percentpos = "";
        }
        /// <summary>
        /// Get the current postion in the file being played.
        /// </summary>
        /// <remarks>It is highly recommended to use the CurrentPostion event instead.</remarks>
        /// <returns></returns>
        public int GetCurrentPosition()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_time_pos");
                MediaPlayer.StandardInput.Flush();

                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._currentPosition;
            }
            return -1;
        }

        /// <summary>
        /// Get if the video is full is screen or not.  Set video to play in fullscreen.
        /// </summary>
        public bool FullScreen
        {
            get { return _fullscreen; }
            set
            {
                _fullscreen = value;
                MediaPlayer.StandardInput.WriteLine(string.Format("set_property fullscreen {0}", Convert.ToInt32(_fullscreen)));
                MediaPlayer.StandardInput.Flush();
            }
        }

        /// <summary>
        /// Toggle Fullscreen.
        /// </summary>
        public void ToggleFullScreen()
        {
            if (this.MplayerRunning)
            {
                MediaPlayer.StandardInput.WriteLine("vo_fullscreen");
                MediaPlayer.StandardInput.Flush();
            }
        }

        /// <summary>
        /// Toggle Mute.  
        /// </summary>
        public void Mute()
        {
            MediaPlayer.StandardInput.WriteLine("mute");
            MediaPlayer.StandardInput.Flush();
        }


        /// <summary>
        /// Accepts a volume value of 0 - 100.
        /// </summary>
        /// <param name="volume"></param>
        public void Volume(int volume)
        {
            Debug.Assert(volume >= 0 && volume <= 100);

            MediaPlayer.StandardInput.WriteLine(string.Format("volume {0} 1", volume));
            MediaPlayer.StandardInput.Flush();

        }

        public void SwitchAudioTrack(int track)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("switch_audio {0}", track));
            MediaPlayer.StandardInput.Flush();
        }

        public void SwitchSubtitle(int sub)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("sub_select {0}", sub));
            MediaPlayer.StandardInput.Flush();
        }

        /// <summary>
        /// Insert subtitles, change visibility, position of subtitle, and some another functions
        /// </sumamry>
        public void InsertSubtitles(string filepath)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("sub_load \"{0}\"", PrepareFilePath(filepath)));
            MediaPlayer.StandardInput.WriteLine(string.Format("sub_file 0"));
            MediaPlayer.StandardInput.Flush();
        }
        //visibility of subtitles
        public void VisibilitySubtitle(int v)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property sub_visibility {0}", v));
            MediaPlayer.StandardInput.Flush();
        }
        //position of subtitle bottom//center//up
        public void SubPos(int v)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property sub_pos {0}", v));
            MediaPlayer.StandardInput.Flush();
        }
        //subtitle font scale
        public void SubScale(int v)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("set_property sub_scale {0}", v));
            MediaPlayer.StandardInput.Flush();
        }
        //remove subtitles
        public void RemoveSubtitle()
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("sub_remove -1"));
            MediaPlayer.StandardInput.Flush();
        }
        //subtitle delay
        public void SubDelay(int v)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("sub_delay {0}", v));
            MediaPlayer.StandardInput.Flush();
        }
        /// <summary>
        /// Change aspect ratio at runtime. [value] is the new aspect ratio expressed
        /// as a float (e.g. 1.77778 for 16/9), or special value -1 to reset to
        /// original aspect ratio (ditto if [value] is missing), or special value 0
        /// to disable automatic movie aspect ratio compensation.
        /// There might be problems with some video filters.
        /// </summary>
        /// <param name="ratio"></param>
        public void SwitchRatio(string ratio)
        {
            MediaPlayer.StandardInput.WriteLine(string.Format("switch_ratio {0}", ratio));
            MediaPlayer.StandardInput.Flush();
        }
        
        /// <summary>
        /// get info about the file 
        /// </summary>
#region get info
        //get audio bitrate
        public string GetAudioBitrate()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_audio_bitrate");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get audio codec
        public string GetAudioCodec()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_audio_codec");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get audio samples
        public string GetAudioSamples()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_audio_samples");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
            return this._getfileinfofilename;
            }
        return "";
        }
        //get filename
        public string GetfileName()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_file_name");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get album
        public string getalbum()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_album");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get artist
        public string GetArtist()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_artist");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get comment
        public string GetComment()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_comment");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get comment
        public string GetGenre()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_genre");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
            return "";
        }
        //get title
        public string GetTitle()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_title");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getinfotitle;
            }
        return "";
        }
        //get meta track
        public string GetTrack()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_track");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get year
        public string GetYear()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_meta_year");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
            return this._getfileinfofilename;
            }
        return "";
        }
        //get videobitrate
        public string GetVideoBitrate()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_video_bitrate");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
        //get video resolution
        public string GetVideoResolution()
        {
            if (this.CurrentStatus != MediaStatus.Stopped)
            {
                MediaPlayer.StandardInput.WriteLine("get_video_resolution");
                MediaPlayer.StandardInput.Flush();
                // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
                System.Threading.Thread.Sleep(100);
                return this._getfileinfofilename;
            }
        return "";
        }
#endregion

        /// <summary>
        /// All mplayer standard output is read through this function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMediaPlayerOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string line = e.Data.ToString();

                if (line.StartsWith("EOF code:", StringComparison.Ordinal))
                {
                    this._finalfilecode = line.Substring("EOF code:".Length);
                    if (this._finalfilecode != null)
                    {
                    this.Finalfile(this, new MplayerEvent(this._finalfilecode));
                    }
                }
                else if (line.Contains("Scanning file") || line.Contains("get_path"))
                {
                    this._scanning = line;
                    if (this._scanning != null)
                    {
                        this.Scanfonts(this, new MplayerEvent(this._scanning));
                    }
                }
                else if (line.StartsWith("ID_FILE_SUB_FILENAME=", StringComparison.Ordinal))
                {
                    this._filesub = line.Substring("ID_FILE_SUB_FILENAME=".Length);
                    if (this._filesub != null)
                    {
                        this.Filesub(this, new MplayerEvent(this._filesub));
                    }
                }
                else if (line.StartsWith("ID_AID_", StringComparison.Ordinal))
                {
                    this._audiochannel = line.Substring("ID_AID_".Length);
                    if (this._audiochannel != null)
                    {
                        this.Audiochannel(this, new MplayerEvent(this._audiochannel));
                    }
                }
                else if (line.StartsWith("ID_SID_", StringComparison.Ordinal))
                {
                    this._setaudiolang = line.Substring("ID_SID_".Length);
                    if (this._setaudiolang != null)
                    {
                        this.Setaudiolang(this, new MplayerEvent(this._setaudiolang));
                    }
                }
                else if (line.StartsWith("ANS_PERCENT_POSITION=", StringComparison.Ordinal))
                {
                    this._percentpos = line.Substring("ANS_PERCENT_POSITION=".Length);
                }
                else if (line.StartsWith("Cache fill:", StringComparison.Ordinal))
                {
                    this._cache = line.Substring("Cache fill:".Length);
                    if (this._cache != null)
                    {
                        this.Cache(this, new MplayerEvent(this._cache));
                    }
                }
                else if (line.StartsWith("ANS_AUDIO_BITRATE=", StringComparison.Ordinal)) //audio bitrate
                {
                this._getfileinfofilename = line.Substring("ANS_AUDIO_BITRATE=".Length);
                }
                else if (line.StartsWith("ANS_AUDIO_CODEC=", StringComparison.Ordinal)) //audio codec
                {
                this._getfileinfofilename = line.Substring("ANS_AUDIO_CODEC=".Length);
                }
                else if (line.StartsWith("ANS_AUDIO_SAMPLES=", StringComparison.Ordinal)) //audio sample
                {
                this._getfileinfofilename = line.Substring("ANS_AUDIO_SAMPLES=".Length);
                }
                else if (line.StartsWith("ANS_FILENAME=")) //audio filename
                {
                this._getfileinfofilename = line.Substring("ANS_FILENAME=".Length);
                }
                else if (line.StartsWith("ANS_META_ALBUM=")) //album
                {
                this._getfileinfofilename = line.Substring("ANS_META_ALBUM=".Length);
                }
                else if (line.StartsWith("ANS_META_ARTIST=", StringComparison.Ordinal)) //artista
                {
                this._getfileinfofilename = line.Substring("ANS_META_ARTIST=".Length);
                }
                else if (line.StartsWith("ANS_META_COMMENT=", StringComparison.Ordinal)) //comentarios
                {
                this._getfileinfofilename = line.Substring("ANS_META_COMMENT=".Length);
                }
                else if (line.StartsWith("ANS_META_GENRE=", StringComparison.Ordinal)) //genero
                {
                this._getfileinfofilename = line.Substring("ANS_META_GENRE=".Length);
                }
                else if (line.StartsWith("ANS_META_TITLE=", StringComparison.Ordinal)) //titulo
                {
                this._getinfotitle = line.Substring("ANS_META_TITLE=".Length);
                }
                else if (line.StartsWith("ANS_META_TRACK=", StringComparison.Ordinal)) //track
                {
                this._getfileinfofilename = line.Substring("ANS_META_TRACK=".Length);
                }
                else if (line.StartsWith("ANS_META_YEAR=", StringComparison.Ordinal)) //ano
                {
                    this._getfileinfofilename = line.Substring("ANS_META_YEAR=".Length);
                }
                else if (line.StartsWith("ANS_VIDEO_BITRATE=", StringComparison.Ordinal)) //video bitrate
                {
                    this._getfileinfofilename = line.Substring("ANS_VIDEO_BITRATE=".Length);
                }
                else if (line.StartsWith("ANS_VIDEO_RESOLUTION=", StringComparison.Ordinal)) //video resoluçao
                {
                    this._getfileinfofilename = line.Substring("ANS_VIDEO_RESOLUTION=".Length);
                }
                else if (line.StartsWith("ANS_TIME_POSITION=", StringComparison.Ordinal))
                {
                    this._currentPosition = (int)Globals.FloatParse(line.Substring("ANS_TIME_POSITION=".Length));

                    if (this.CurrentPosition != null)
                    {
                        this.CurrentPosition(this, new MplayerEvent(this._currentPosition));
                    }
                }
                else if (line.StartsWith("ANS_length=", StringComparison.Ordinal))
                {
                    this._totalTime = (int)Globals.FloatParse(line.Substring("ANS_length=".Length));
                }
                else if (line.StartsWith("Exiting", StringComparison.Ordinal) || line.ToLower().StartsWith("eof code", StringComparison.Ordinal))
                {
                    if (this.VideoExited != null)
                    {
                        this.VideoExited(this, new MplayerEvent("Exiting File"));
                    }
                }

                //System.Console.WriteLine(line);
            }
        }

        /// <summary>
        /// All mplayer error output is read through this function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMediaPlayerErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                System.Console.WriteLine(e.Data.ToString());
            }
        }

    }
}
