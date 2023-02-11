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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Majorsilence.Media.Videos;

public class MpvPlayer : IDisposable, Player
{
    // Current position in seconds in stream.
    private float _currentPosition;

    private readonly Timer _currentPostionTimer;
    private bool _fullscreen;
    private readonly Mpv _mpv;

    // The total length that the video is in seconds.
    private int _totalTime;

    // *******************************************

    private long _wid;

    private bool disposed;


    public MpvPlayer(long wid)
        : this(wid, "")
    {
    }

    public MpvPlayer(long wid, string libMpvPath)
        : this(wid, libMpvPath, false, TimeSpan.FromMilliseconds(1000))
    {
    }

    /// <summary>
    ///     Create a new instance of mplayer class.
    /// </summary>
    /// <param name="wid">Window ID that mplayer should attach itself</param>
    /// <param name="libMpvPath">The full filepath to libmpv. </param>
    /// <param name="loadMplayer">
    ///     If true mplayer will immediately be loaded and you should not attempt to
    ///     play any files until MplayerRunning is true.
    /// </param>
    /// <param name="positionUpdateInterval">Interval of periodical position updates</param>
    public MpvPlayer(long wid, string libMpvPath, bool loadMplayer, TimeSpan positionUpdateInterval)
    {
        _wid = wid;
        _fullscreen = false;
        MplayerRunning = false;

        _mpv = new Mpv(libMpvPath);
        _mpv.Initialize();

        CurrentStatus = MediaStatus.Stopped;

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
    ///     Load and start playing a video.
    /// </summary>
    /// <param name="filePath"></param>
    public void Play(string filePath)
    {
        LoadFile(filePath);

        _mpv.SetProperty("pause", MpvFormat.MPV_FORMAT_STRING, "no");

        CurrentStatus = MediaStatus.Playing;
    }

    public void SetHandle(long wid)
    {
        _wid = wid;
        //int mpvFormatInt64 = 4;
        //_mpvSetOption (_mpvHandle, GetUtf8Bytes ("wid"), mpvFormatInt64, ref _wid);
    }


    /// <summary>
    ///     Move to a new position in the video.
    /// </summary>
    /// <param name="timePosition">Seconds.  The position to seek move to.</param>
    public void MovePosition(int timePosition)
    {
        _mpv.DoMpvCommand("seek", timePosition.ToString(CultureInfo.InvariantCulture), "absolute");
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
        _mpv.DoMpvCommand("seek", value.ToString(CultureInfo.InvariantCulture), type.ToString().ToLower());
    }

    public void SetSize(int width, int height)
    {
        if (CurrentStatus != MediaStatus.Playing) return;

        _mpv.SetProperty("width", MpvFormat.MPV_FORMAT_STRING, width.ToString(CultureInfo.InvariantCulture));
        _mpv.SetProperty("height", MpvFormat.MPV_FORMAT_STRING, height.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Pause the current video.  If paused it will unpause.
    /// </summary>
    public void Pause()
    {
        if (CurrentStatus != MediaStatus.Playing && CurrentStatus != MediaStatus.Paused) return;

        try
        {
            _mpv.SetProperty("pause", MpvFormat.MPV_FORMAT_STRING, "yes");
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
            Pause();
            Seek(0, Videos.Seek.Absolute);
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

            var data = _fullscreen ? "yes" : "no";
            _mpv.SetProperty("fullscreen", MpvFormat.MPV_FORMAT_STRING, data);
        }
    }

    /// <summary>
    ///     Toggle Fullscreen.
    /// </summary>
    public void ToggleFullScreen()
    {
        if (MplayerRunning)
        {
            var lpBuffer = IntPtr.Zero;
            var isFullScreen = _mpv.GetProperty("fullscreen");

            if (isFullScreen == "yes")
                _mpv.SetProperty("fullscreen", MpvFormat.MPV_FORMAT_STRING, "no");
            else
                _mpv.SetProperty("fullscreen", MpvFormat.MPV_FORMAT_STRING, "yes");
        }
    }

    /// <summary>
    ///     Toggle Mute.
    /// </summary>
    public void Mute()
    {
        var isMuted = _mpv.GetProperty("ao-mute");

        if (isMuted == "yes")
            _mpv.SetProperty("ao-mute", MpvFormat.MPV_FORMAT_STRING, "no");
        else
            _mpv.SetProperty("ao-mute", MpvFormat.MPV_FORMAT_STRING, "yes");
    }


    /// <summary>
    ///     Accepts a volume value of 0 - 100.
    /// </summary>
    /// <param name="volume"></param>
    public void Volume(int volume)
    {
        Debug.Assert(volume >= 0 && volume <= 100);

        _mpv.SetProperty("ao-volume", MpvFormat.MPV_FORMAT_STRING, volume.ToString());
    }

    public void SwitchAudioTrack(int track)
    {
        _mpv.SetProperty("ao-reload", MpvFormat.MPV_FORMAT_STRING, track.ToString());
    }

    public void SwitchSubtitle(int sub)
    {
        _mpv.SetProperty("sub-reload", MpvFormat.MPV_FORMAT_STRING, sub.ToString());
    }

    /// <summary>
    ///     Cleanup resources.  Currently this means that mplayer is closed if it is still running.
    /// </summary>
    /// <remarks>Alternatively call the Dispose method.</remarks>
    ~MpvPlayer()
    {
        // Cleanup

        _mpv.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            _currentPostionTimer.Dispose();
            _mpv.Dispose();
        }

        disposed = true;
    }


    private void InitializeMplayer()
    {
        _currentPostionTimer.Start();
        MplayerRunning = true;
    }


    /// <summary>
    ///     Starts a new video/audio file immediatly.  Requires that Play has been called.
    /// </summary>
    /// <param name="filePath">string</param>
    public void LoadFile(string filePath)
    {
        CurrentFilePath = filePath;

        _mpv.SetOption("wid", MpvFormat.MPV_FORMAT_INT64, _wid);
        _mpv.DoMpvCommand("loadfile", filePath);

        // HACK: wait for video to load
        Thread.Sleep(1000);
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

    // Sets in motions events to set this._totalTime.  Is called as soon as the video starts.
    private void LoadCurrentPlayingFileLength()
    {
        _totalTime = (int)_mpv.GetPropertyFloat("duration");
    }

    private void _currentPostionTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (CurrentStatus == MediaStatus.Playing)
        {
            _currentPosition = _mpv.GetPropertyFloat("time-pos");

            CurrentPosition?.Invoke(this, new MplayerEvent(_currentPosition));
        }
    }
}

// from https://github.com/mpv-player/mpv/blob/master/libmpv/client.h
public enum MpvFormat
{
    /**
     * Invalid. Sometimes used for empty values.
     */
    MPV_FORMAT_NONE = 0,

    /**
     * * The basic type is char*. It returns the raw property string, like
     * * using ${=property} in input.conf (see input.rst).
     * *
     * * NULL isn't an allowed value.
     * *
     * * Warning: although the encoding is usually UTF-8, this is not always the
     * *          case. File tags often store strings in some legacy codepage,
     * *          and even filenames don't necessarily have to be in UTF-8 (at
     * *          least on Linux). If you pass the strings to code that requires
     * *          valid UTF-8, you have to sanitize it in some way.
     * *          On Windows, filenames are always UTF-8, and libmpv converts
     * *          between UTF-8 and UTF-16 when using win32 API functions. See
     * *          the "Encoding of filenames" section for details.
     * *
     * * Example for reading:
     * *
     * *     char *result = NULL;
     * *     if (mpv_get_property(ctx, "property", MPV_FORMAT_STRING, &result)
     * < 0)
     *     *         goto error;
     *     * printf("%s\n", result);
     *     * mpv_free( result);
     *     *
     *     * Or just use mpv_get_property_string().
     *     *
     *     * Example for writing:
     *     *
     *     * char * value="the new value";
     *     *     // yep, you pass the address to the variable
     *     *     // ( needed for symmetry with other types and mpv_get_property)
     *     * mpv_set_property( ctx, "property", MPV_FORMAT_STRING, & value);
     *     *
     *     * Or just use mpv_set_property_string
     * (
     * )
     * .
     * *
     */
    MPV_FORMAT_STRING = 1,

    /**
     * The basic type is char*. It returns the OSD property string, like
     * using ${property} in input.conf (see input.rst). In many cases, this
     * is the same as the raw string, but in other cases it's formatted for
     * display on OSD. It's intended to be human readable. Do not attempt to
     * parse these strings.
     *
     * Only valid when doing read access. The rest works like MPV_FORMAT_STRING.
     */
    MPV_FORMAT_OSD_STRING = 2,

    /**
     * The basic type is int. The only allowed values are 0 ("no")
     * and 1 ("yes").
     * 
     * Example for reading:
     * 
     * int result;
     * if (mpv_get_property(ctx, "property", MPV_FORMAT_FLAG, &result)
     * < 0)
     *     goto error;
     *     printf("%s\n", result ? "true" : "false");
     *     Example for writing:
     *     int flag= 1;
     *     mpv_set_property( ctx, "property", MPV_FORMAT_FLAG, & flag
     * )
     * ;
     */
    MPV_FORMAT_FLAG = 3,

    /**
     * The basic type is int64_t.
     */
    MPV_FORMAT_INT64 = 4,

    /**
     * The basic type is double.
     */
    MPV_FORMAT_DOUBLE = 5,

    /**
     * The type is mpv_node.
     * 
     * For reading, you usually would pass a pointer to a stack-allocated
     * mpv_node value to mpv, and when you're done you call
     * mpv_free_node_contents(&node).
     * You're expected not to write to the data - if you have to, copy it
     * first (which you have to do manually).
     * 
     * For writing, you construct your own mpv_node, and pass a pointer to the
     * API. The API will never write to your data (and copy it if needed), so
     * you're free to use any form of allocation or memory management you like.
     * 
     * Warning: when reading, always check the mpv_node.format member. For
     * example, properties might change their type in future versions
     * of mpv, or sometimes even during runtime.
     * 
     * Example for reading:
     * 
     * mpv_node result;
     * if (mpv_get_property(ctx, "property", MPV_FORMAT_NODE, &result)
     * < 0)
     *     goto error;
     *     printf("format=%d\n", ( int) result.format);
     *     mpv_free_node_contents(& result).
     *     Example for writing:
     *     mpv_node value;
     *     value.format= MPV_FORMAT_STRING;
     *     value.u.string="hello";
     *     mpv_set_property( ctx, "property", MPV_FORMAT_NODE, & value
     * )
     * ;
     */
    MPV_FORMAT_NODE = 6,

    /**
     * Used with mpv_node only. Can usually not be used directly.
     */
    MPV_FORMAT_NODE_ARRAY = 7,

    /**
     * See MPV_FORMAT_NODE_ARRAY.
     */
    MPV_FORMAT_NODE_MAP = 8,

    /**
     * A raw, untyped byte array. Only used only with mpv_node, and only in
     * some very special situations. (Currently, only for the screenshot-raw
     * command.)
     */
    MPV_FORMAT_BYTE_ARRAY = 9
}