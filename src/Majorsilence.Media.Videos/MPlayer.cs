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
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Majorsilence.Media.Videos;

public class MPlayer : IDisposable, Player
{
    private string _audiochannel;

    private readonly BackendPrograms _backendProgram;

    private string _cache;

    // Current position in seconds in stream.
    private float _currentPosition;

    private readonly Timer _currentPostionTimer;
    private string _filesub;
    private string _finalfilecode;
    private bool _fullscreen;

    ///vars for mplayer info
    private string _getfileinfofilename;

    private string _getinfotitle;
    private readonly MplayerBackends _mplayerBackend;
    private string _mplayerPath;
    private int _mplayerProcessID = -1;
    private string _percentpos;
    private string _scanning;
    private string _setaudiolang;

    // The total length that the video is in seconds.
    private int _totalTime;
    private long _wid;

    private readonly string consoleArguments;

    private bool disposed;

    public EventHandler<DataReceivedEventArgs> MplayerError;

    public EventHandler<DataReceivedEventArgs> MplayerOutput;

    ///
    private MPlayer()
    {
    }

    public MPlayer(long wid, MplayerBackends backend)
        : this(wid, backend, "")
    {
    }

    public MPlayer(long wid, MplayerBackends backend, string mplayerPath)
        : this(wid, backend, mplayerPath, false, TimeSpan.FromMilliseconds(1000))
    {
    }

    /// <summary>
    ///     Create a new instance of mplayer class.
    /// </summary>
    /// <param name="wid">Window ID that mplayer should attach itself</param>
    /// <param name="backend">The video output backend that mplayer will use.</param>
    /// <param name="mplayerPath">
    ///     The full filepath to mplayer.exe.  If mplayerPath is left empty it will search for mplayer.exe in
    ///     "current directory\backend\mplayer.exe" on windows and mplayer in the path on linux.
    /// </param>
    /// <param name="loadMplayer">
    ///     If true mplayer will immediately be loaded and you should not attempt to
    ///     play any files until MplayerRunning is true.
    /// </param>
    /// <param name="positionUpdateInterval">Interval of periodical position updates</param>
    /// <param name="consoleArguments">
    ///     Specify custom console arguments here; default "-slave -quiet -idle -v -ontop" ( do not
    ///     set -vo and -wid )
    /// </param>
    public MPlayer(long wid, MplayerBackends backend, string mplayerPath, bool loadMplayer,
        TimeSpan positionUpdateInterval, string consoleArguments = "-slave -quiet -idle -v -ontop")
    {
        _wid = wid;
        _fullscreen = false;
        MplayerRunning = false;
        _mplayerBackend = backend;
        _mplayerPath = mplayerPath;
        CurrentStatus = MediaStatus.Stopped;

        this.consoleArguments = consoleArguments;

        _backendProgram = new BackendPrograms(mplayerPath);

        MediaPlayer = new Process();

        // This timer will send an event every second with the current video postion when video
        // is in play mode.
        _currentPostionTimer = new Timer(positionUpdateInterval.TotalMilliseconds);
        _currentPostionTimer.Elapsed += _currentPostionTimer_Elapsed;
        _currentPostionTimer.Enabled = false;

        if (loadMplayer)
        {
            var caller = InitializeMplayer;
            caller.BeginInvoke(null, null);
        }
    }

    /// <summary>
    ///     Is mplayer alreadying running?  True or False.
    /// </summary>
    public bool MplayerRunning { get; set; }

    public bool HardwareAccelerated { get; set; }


    /// <summary>
    ///     The process that is running mplayer.
    /// </summary>
    private Process MediaPlayer { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public string CurrentFilePath { get; private set; }

    public event MplayerEventHandler VideoExited;

    /// <summary>
    ///     This event is the most accurate way to get the current position of the current playing file.
    ///     Whenever the postion changes this event will notify that the positon has changed with the value
    ///     being the new position (seconds into the file).
    /// </summary>
    public event MplayerEventHandler CurrentPosition;

    /// <summary>
    ///     The current status of the player.
    /// </summary>
    public MediaStatus CurrentStatus { get; set; }

    /// <summary>
    ///     This is only useful if mplayer has not yet been initialized
    /// </summary>
    public void SetHandle(long wid)
    {
        _wid = wid;
    }

    /// <summary>
    ///     Load and start playing a video.
    /// </summary>
    /// <param name="filePath"></param>
    public void Play(string filePath)
    {
        CurrentFilePath = filePath;


        if (MplayerRunning == false) InitializeMplayer();

        LoadFile(filePath);
        CurrentStatus = MediaStatus.Playing;
    }


    /// <summary>
    ///     Move to a new position in the video.
    /// </summary>
    /// <param name="timePosition">Seconds.  The position to seek move to.</param>
    public void MovePosition(int timePosition)
    {
        WriteLineWithDebug(string.Format("set_property time_pos {0}", timePosition));
        MediaPlayer.StandardInput.Flush();
    }


    /// <summary>
    ///     Seek a new postion.
    ///     Seek to some place in the movie.
    ///     Seek.Relative is a relative seek of +/- value seconds (default).
    ///     Seek.Percentage is a seek to value % in the movie.
    ///     Seek.Absolute is a seek to an absolute position of value seconds.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    public void Seek(int value, Seek type)
    {
        WriteLineWithDebug(string.Format("pausing_keep_force seek {0} {1}", value, (int)type));
        MediaPlayer.StandardInput.Flush();
    }

    public void SetSize(int width, int height)
    {
        if (CurrentStatus != MediaStatus.Playing) return;

        WriteLineWithDebug(string.Format("set_property width {0}", width));
        WriteLineWithDebug(string.Format("set_property height {0}", height));
        MediaPlayer.StandardInput.Flush();
    }

    /// <summary>
    ///     Pause the current video.  If paused it will unpause.
    /// </summary>
    public void Pause()
    {
        if (CurrentStatus != MediaStatus.Playing && CurrentStatus != MediaStatus.Paused) return;

        try
        {
            WriteLineWithDebug("pause");
            MediaPlayer.StandardInput.Flush();
        }
        catch (Exception ex)
        {
            Logging.Instance.WriteLine(ex);
            return;
        }

        if (CurrentStatus == MediaStatus.Paused)
            CurrentStatus = MediaStatus.Playing;
        else
            CurrentStatus = MediaStatus.Paused;
    }

    /// <summary>
    ///     Stop the current video.
    /// </summary>
    public void Stop()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("stop");
            MediaPlayer.StandardInput.Flush();
        }

        CurrentStatus = MediaStatus.Stopped;
    }

    /// <summary>
    ///     Retrieves the number of seconds of the current playing video.
    /// </summary>
    public int CurrentPlayingFileLength()
    {
        return _totalTime;
    }

    /// <summary>
    ///     Get if the video is full is screen or not.  Set video to play in fullscreen.
    /// </summary>
    public bool FullScreen
    {
        get => _fullscreen;
        set
        {
            _fullscreen = value;
            WriteLineWithDebug(string.Format("set_property fullscreen {0}", Convert.ToInt32(_fullscreen)));
            MediaPlayer.StandardInput.Flush();
        }
    }

    /// <summary>
    ///     Toggle Fullscreen.
    /// </summary>
    public void ToggleFullScreen()
    {
        if (MplayerRunning)
        {
            WriteLineWithDebug("vo_fullscreen");
            MediaPlayer.StandardInput.Flush();
        }
    }

    /// <summary>
    ///     Toggle Mute.
    /// </summary>
    public void Mute()
    {
        WriteLineWithDebug("mute");
        MediaPlayer.StandardInput.Flush();
    }


    /// <summary>
    ///     Accepts a volume value of 0 - 100.
    /// </summary>
    /// <param name="volume"></param>
    public void Volume(int volume)
    {
        Debug.Assert(volume >= 0 && volume <= 100);

        WriteLineWithDebug(string.Format("pausing_keep_force volume {0} 1", volume));
        MediaPlayer.StandardInput.Flush();
    }

    public void SwitchAudioTrack(int track)
    {
        WriteLineWithDebug(string.Format("switch_audio {0}", track));
        MediaPlayer.StandardInput.Flush();
    }

    public void SwitchSubtitle(int sub)
    {
        WriteLineWithDebug(string.Format("sub_select {0}", sub));
        MediaPlayer.StandardInput.Flush();
    }

    public event MplayerEventHandler Cache;
    public event MplayerEventHandler Finalfile;

    public event MplayerEventHandler Scanfonts;
    public event MplayerEventHandler Filesub;
    public event MplayerEventHandler Audiochannel;
    public event MplayerEventHandler Setaudiolang;

    /// <summary>
    ///     Cleanup resources.  Currently this means that mplayer is closed if it is still running.
    /// </summary>
    /// <remarks>Alternatively call the Dispose method.</remarks>
    ~MPlayer()
    {
        // Cleanup
        if (_mplayerProcessID != -1)
            try
            {
                var p = Process.GetProcessById(_mplayerProcessID);
                if (p.HasExited == false) p.Kill();
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(ex);
            }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            _currentPostionTimer.Elapsed -= _currentPostionTimer_Elapsed;
            _currentPostionTimer.Enabled = false;

            if (MediaPlayer != null)
            {
                try
                {
                    if (MediaPlayer.HasExited == false) MediaPlayer.Kill();
                }
                catch (Exception ex)
                {
                    Logging.Instance.WriteLine(ex);
                }

                MediaPlayer.Dispose();
                MediaPlayer = null;
            }
        }

        disposed = true;
    }

    private string MplayerBackend()
    {
        string backend;
        if (_mplayerBackend == MplayerBackends.Direct3D)
            backend = "direct3d";
        else if (_mplayerBackend == MplayerBackends.X11)
            backend = "x11";
        else if (_mplayerBackend == MplayerBackends.Vdpau)
            backend = "vdpau";
        else if (_mplayerBackend == MplayerBackends.XV)
            backend = "xv";
        else if (_mplayerBackend == MplayerBackends.Quartz)
            backend = "quartz";
        else if (_mplayerBackend == MplayerBackends.CoreVideo)
            backend = "corevideo";
        else if (_mplayerBackend == MplayerBackends.SDL)
            backend = "sdl";
        else if (_mplayerBackend == MplayerBackends.GL)
            backend = "gl";
        else if (_mplayerBackend == MplayerBackends.GL2)
            backend = "gl2";
        else if (_mplayerBackend == MplayerBackends.ASCII)
            backend = "aa";
        else if (_mplayerBackend == MplayerBackends.ColorASCII)
            backend = "caca";
        else if (_mplayerBackend == MplayerBackends.Directfb)
            backend = "directfb";
        else if (_mplayerBackend == MplayerBackends.Wii)
            backend = "wii";
        else if (_mplayerBackend == MplayerBackends.V4l2)
            backend = "v4l2";
        else if (_mplayerBackend == MplayerBackends.VESA)
            backend = "vesa";
        else
            backend = "opengl";

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

        _currentPostionTimer.Start();


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


        var backend = MplayerBackend();
        /*
            if you like you can parse more args
            MediaPlayer.StartInfo.Arguments = string.Format("-slave -quiet -idle -priority abovenormal -nodr -double -nokeepaspect -cache 8192 -nofs -autosync 100 -mc 2.0 -nomouseinput -framedrop -osdlevel 0 -lavdopts threads=4 -ao dsound -v -monitorpixelaspect 1 -ontop -font \"{0}\" -subfont-autoscale {1} -subfont-text-scale {2} -subcp {3} -subpos {4} -volume {5} -vo {6} -wid {7} \"{8}\"", this._font, this._fontautoscale, this._textscale, this._subcp, this._subpos, this.volumemain, backend, this._wid, filePath);

        */
        MediaPlayer.StartInfo.Arguments =
            string.Format("{0} -vo {1} -wid {2}", consoleArguments, backend, _wid);
        MediaPlayer.StartInfo.FileName = _backendProgram.MPlayer;

        MediaPlayer.Start();


        MplayerRunning = true;
        _mplayerProcessID = MediaPlayer.Id;

        //System.IO.StreamWriter mw = MediaPlayer.StandardInput;
        //mw.AutoFlush = true;

        MediaPlayer.OutputDataReceived += HandleMediaPlayerOutputDataReceived;
        MediaPlayer.ErrorDataReceived += HandleMediaPlayerErrorDataReceived;
        MediaPlayer.BeginErrorReadLine();
        MediaPlayer.BeginOutputReadLine();
    }

    /// <summary>
    ///     Starts a new video/audio file immediatly.  Requires that Play has been called.
    /// </summary>
    /// <param name="filePath">string</param>
    public void LoadFile(string filePath)
    {
        var LoadCommand = @"" + string.Format("loadfile \"{0}\"", PrepareFilePath(filePath));
        WriteLineWithDebug(LoadCommand);
        //WriteLineWithDebug("play");
        MediaPlayer.StandardInput.Flush();
        LoadCurrentPlayingFileLength();
    }

    /// <summary>
    ///     Prepare filepaths to be used witht the loadfile command.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <remarks>
    ///     For some reason it strips the DirectorySeperatorChar so we double it up here.
    /// </remarks>
    private string PrepareFilePath(string filePath)
    {
        var preparedPath = filePath;

        if (Environment.OSVersion.ToString().IndexOf("Windows") > -1)
            preparedPath = filePath.Replace("" + Path.DirectorySeparatorChar,
                "" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar);

        return preparedPath;
    }

    private void WriteLineWithDebug(string line)
    {
#if DEBUG_LINES
            Console.WriteLine(line);
#endif
        MediaPlayer.StandardInput.WriteLine(line);
    }

    public void EnableFramedropping()
    {
        WriteLineWithDebug("set_property framedropping 1");
        MediaPlayer.StandardInput.Flush();
    }

    public void DisableFramedropping()
    {
        WriteLineWithDebug("set_property framedropping 0");
        MediaPlayer.StandardInput.Flush();
    }

    public void SetSpeed(double speed)
    {
        WriteLineWithDebug(string.Format("speed_set {0}", speed));
        MediaPlayer.StandardInput.Flush();
    }

    /// <summary>
    ///     Close MPlayer instance.
    /// </summary>
    public void Quit()
    {
        try
        {
            WriteLineWithDebug("quit");
            MediaPlayer.StandardInput.Flush();
        }
        catch (ObjectDisposedException ex)
        {
            Logging.Instance.WriteLine(ex);
        }
    }

    /// <summary>
    ///     set percent position of mplayer
    /// </summary>
    public void SetPercent(int v)
    {
        WriteLineWithDebug(string.Format("set_property percent_pos {0}", v));
        MediaPlayer.StandardInput.Flush();
    }

    // Sets in motions events to set this._totalTime.  Is called as soon as the video starts.
    private void LoadCurrentPlayingFileLength()
    {
        // This works even with streaming.
        var file = new MPlayerDiscover(CurrentFilePath, _backendProgram.MPlayer);
        file.Execute();
        _totalTime = file.Length;
    }


    private void _currentPostionTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (CurrentStatus == MediaStatus.Playing)
        {
            WriteLineWithDebug("pausing_keep_force get_time_pos");
            MediaPlayer.StandardInput.Flush();
        }
    }

    /// <summary>
    ///     get percent positiob
    /// </summary>
    public string GetPercentPos()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("pausing_keep_force get_percent_pos");
            MediaPlayer.StandardInput.Flush();
            return _percentpos;
        }

        return "";
        _percentpos = "";
    }

    /// <summary>
    ///     Get the current postion in the file being played.
    /// </summary>
    /// <remarks>It is highly recommended to use the CurrentPostion event instead.</remarks>
    /// <returns></returns>
    public float GetCurrentPosition()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("pausing_keep_force get_time_pos");
            MediaPlayer.StandardInput.Flush();

            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _currentPosition;
        }

        return -1;
    }

    /// <summary>
    ///     Insert subtitles, change visibility, position of subtitle, and some another functions
    /// </summary>
    public void InsertSubtitles(string filepath)
    {
        WriteLineWithDebug(string.Format("sub_load \"{0}\"", PrepareFilePath(filepath)));
        WriteLineWithDebug("sub_file 0");
        MediaPlayer.StandardInput.Flush();
    }

    //visibility of subtitles
    public void VisibilitySubtitle(int v)
    {
        WriteLineWithDebug(string.Format("set_property sub_visibility {0}", v));
        MediaPlayer.StandardInput.Flush();
    }

    //position of subtitle bottom//center//up
    public void SubPos(int v)
    {
        WriteLineWithDebug(string.Format("set_property sub_pos {0}", v));
        MediaPlayer.StandardInput.Flush();
    }

    //subtitle font scale
    public void SubScale(int v)
    {
        WriteLineWithDebug(string.Format("set_property sub_scale {0}", v));
        MediaPlayer.StandardInput.Flush();
    }

    //remove subtitles
    public void RemoveSubtitle()
    {
        WriteLineWithDebug("sub_remove -1");
        MediaPlayer.StandardInput.Flush();
    }

    //subtitle delay
    public void SubDelay(int v)
    {
        WriteLineWithDebug(string.Format("sub_delay {0}", v));
        MediaPlayer.StandardInput.Flush();
    }

    /// <summary>
    ///     Change aspect ratio at runtime. [value] is the new aspect ratio expressed
    ///     as a float (e.g. 1.77778 for 16/9), or special value -1 to reset to
    ///     original aspect ratio (ditto if [value] is missing), or special value 0
    ///     to disable automatic movie aspect ratio compensation.
    ///     There might be problems with some video filters.
    /// </summary>
    /// <param name="ratio"></param>
    public void SwitchRatio(string ratio)
    {
        WriteLineWithDebug(string.Format("switch_ratio {0}", ratio));
        MediaPlayer.StandardInput.Flush();
    }

    /// <summary>
    ///     All mplayer standard output is read through this function.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleMediaPlayerOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            MplayerOutput?.Invoke(sender, e);

            var line = e.Data;

            if (line.StartsWith("EOF code:", StringComparison.Ordinal))
            {
                _finalfilecode = line.Substring("EOF code:".Length);
                if (_finalfilecode != null) Finalfile?.Invoke(this, new MplayerEvent(_finalfilecode));
            }
            else if (line.Contains("Scanning file") || line.Contains("get_path"))
            {
                _scanning = line;
                if (_scanning != null) Scanfonts?.Invoke(this, new MplayerEvent(_scanning));
            }
            else if (line.StartsWith("ID_FILE_SUB_FILENAME=", StringComparison.Ordinal))
            {
                _filesub = line.Substring("ID_FILE_SUB_FILENAME=".Length);
                if (_filesub != null) Filesub?.Invoke(this, new MplayerEvent(_filesub));
            }
            else if (line.StartsWith("ID_AID_", StringComparison.Ordinal))
            {
                _audiochannel = line.Substring("ID_AID_".Length);
                if (_audiochannel != null) Audiochannel?.Invoke(this, new MplayerEvent(_audiochannel));
            }
            else if (line.StartsWith("ID_SID_", StringComparison.Ordinal))
            {
                _setaudiolang = line.Substring("ID_SID_".Length);
                if (_setaudiolang != null) Setaudiolang?.Invoke(this, new MplayerEvent(_setaudiolang));
            }
            else if (line.StartsWith("ANS_PERCENT_POSITION=", StringComparison.Ordinal))
            {
                _percentpos = line.Substring("ANS_PERCENT_POSITION=".Length);
            }
            else if (line.StartsWith("Cache fill:", StringComparison.Ordinal))
            {
                _cache = line.Substring("Cache fill:".Length);
                if (_cache != null) Cache?.Invoke(this, new MplayerEvent(_cache));
            }
            else if (line.StartsWith("ANS_AUDIO_BITRATE=", StringComparison.Ordinal)) //audio bitrate
            {
                _getfileinfofilename = line.Substring("ANS_AUDIO_BITRATE=".Length);
            }
            else if (line.StartsWith("ANS_AUDIO_CODEC=", StringComparison.Ordinal)) //audio codec
            {
                _getfileinfofilename = line.Substring("ANS_AUDIO_CODEC=".Length);
            }
            else if (line.StartsWith("ANS_AUDIO_SAMPLES=", StringComparison.Ordinal)) //audio sample
            {
                _getfileinfofilename = line.Substring("ANS_AUDIO_SAMPLES=".Length);
            }
            else if (line.StartsWith("ANS_FILENAME=")) //audio filename
            {
                _getfileinfofilename = line.Substring("ANS_FILENAME=".Length);
            }
            else if (line.StartsWith("ANS_META_ALBUM=")) //album
            {
                _getfileinfofilename = line.Substring("ANS_META_ALBUM=".Length);
            }
            else if (line.StartsWith("ANS_META_ARTIST=", StringComparison.Ordinal)) //artista
            {
                _getfileinfofilename = line.Substring("ANS_META_ARTIST=".Length);
            }
            else if (line.StartsWith("ANS_META_COMMENT=", StringComparison.Ordinal)) //comentarios
            {
                _getfileinfofilename = line.Substring("ANS_META_COMMENT=".Length);
            }
            else if (line.StartsWith("ANS_META_GENRE=", StringComparison.Ordinal)) //genero
            {
                _getfileinfofilename = line.Substring("ANS_META_GENRE=".Length);
            }
            else if (line.StartsWith("ANS_META_TITLE=", StringComparison.Ordinal)) //titulo
            {
                _getinfotitle = line.Substring("ANS_META_TITLE=".Length);
            }
            else if (line.StartsWith("ANS_META_TRACK=", StringComparison.Ordinal)) //track
            {
                _getfileinfofilename = line.Substring("ANS_META_TRACK=".Length);
            }
            else if (line.StartsWith("ANS_META_YEAR=", StringComparison.Ordinal)) //ano
            {
                _getfileinfofilename = line.Substring("ANS_META_YEAR=".Length);
            }
            else if (line.StartsWith("ANS_VIDEO_BITRATE=", StringComparison.Ordinal)) //video bitrate
            {
                _getfileinfofilename = line.Substring("ANS_VIDEO_BITRATE=".Length);
            }
            else if (line.StartsWith("ANS_VIDEO_RESOLUTION=", StringComparison.Ordinal)) //video resoluçao
            {
                _getfileinfofilename = line.Substring("ANS_VIDEO_RESOLUTION=".Length);
            }
            else if (line.StartsWith("ANS_TIME_POSITION=", StringComparison.Ordinal))
            {
                _currentPosition = Globals.FloatParse(line.Substring("ANS_TIME_POSITION=".Length));

                CurrentPosition?.Invoke(this, new MplayerEvent(_currentPosition));
            }
            else if (line.StartsWith("ANS_length=", StringComparison.Ordinal))
            {
                _totalTime = (int)Globals.FloatParse(line.Substring("ANS_length=".Length));
            }
            else if (line.StartsWith("Exiting", StringComparison.Ordinal) ||
                     line.ToLower().StartsWith("eof code", StringComparison.Ordinal))
            {
                VideoExited?.Invoke(this, new MplayerEvent("Exiting File"));
            }

            //System.Console.WriteLine(line);
        }
    }

    /// <summary>
    ///     All mplayer error output is read through this function.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleMediaPlayerErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null) MplayerError?.Invoke(sender, e);
    }

    /// <summary>
    ///     get info about the file
    /// </summary>

    #region get info

    //get audio bitrate
    public string GetAudioBitrate()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_audio_bitrate");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get audio codec
    public string GetAudioCodec()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_audio_codec");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get audio samples
    public string GetAudioSamples()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_audio_samples");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get filename
    public string GetfileName()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_file_name");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get album
    public string getalbum()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_album");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get artist
    public string GetArtist()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_artist");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get comment
    public string GetComment()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_comment");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get comment
    public string GetGenre()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_genre");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get title
    public string GetTitle()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_title");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getinfotitle;
        }

        return "";
    }

    //get meta track
    public string GetTrack()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_track");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get year
    public string GetYear()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_meta_year");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get videobitrate
    public string GetVideoBitrate()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_video_bitrate");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    //get video resolution
    public string GetVideoResolution()
    {
        if (CurrentStatus != MediaStatus.Stopped)
        {
            WriteLineWithDebug("get_video_resolution");
            MediaPlayer.StandardInput.Flush();
            // This is to give the HandleMediaPlayerOutputDataReceived enought time to process and set the currentPosition.
            Thread.Sleep(100);
            return _getfileinfofilename;
        }

        return "";
    }

    #endregion
}