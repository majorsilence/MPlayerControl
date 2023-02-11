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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Majorsilence.Media.Videos;

/// <summary>
///     This class is used to discover information about a media file.
/// </summary>
public class MpvDiscover : Discover
{
    private readonly string filePath;
    private readonly string libmpvPath;

    private Mpv _mpv;

    private int _VideoBitrate;

    private bool disposed;

    private MpvDiscover()
    {
    }

    public MpvDiscover(string filePath, string libmpvPath)
    {
        this.filePath = filePath;
        this.libmpvPath = libmpvPath;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Is the files video bitrate.  Kilobits per second.
    /// </summary>
    public int VideoBitrate => _VideoBitrate;

    /// <summary>
    ///     Is the files audio bitrate.  Kilobits per second.
    /// </summary>
    public int AudioBitrate { get; private set; }

    /// <summary>
    ///     Is the files audio rate.  What is the difference between this and AudioBitrate.
    /// </summary>
    public int AudioSampleRate { get; private set; }

    /// <summary>
    ///     Is the videos width.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Is the videos height.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     The videos frames per second.
    /// </summary>
    public int FPS { get; private set; }

    /// <summary>
    ///     The length of the video in seconds.
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    ///     The aspect ratio of the video. Could be 4/3 or 16/9.
    /// </summary>
    public float AspectRatio { get; private set; }

    /// <summary>
    ///     List of audio tracks in the video.
    /// </summary>
    public List<int> AudioList { get; private set; }

    public List<AudioTrackInfo> AudioTracks { get; private set; }

    /// <summary>
    ///     Returns true if the file contains video.
    /// </summary>
    public bool Video { get; private set; }

    /// <summary>
    ///     Returns true if the file contains audio.
    /// </summary>
    public bool Audio { get; } = false;

    public List<SubtitlesInfo> SubtitleList { get; private set; }


    public Task ExecuteAsync()
    {
        return Task.Run(() => Execute());
    }

    public void Execute()
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
        _mpv = new Mpv(libmpvPath);

        // Must be set before initializeation
        _mpv.SetOption("frames", MpvFormat.MPV_FORMAT_INT64, 148);

        _mpv.Initialize();

        _mpv.SetOption("wid", MpvFormat.MPV_FORMAT_INT64, -1);
        _mpv.SetOption("vo", MpvFormat.MPV_FORMAT_STRING, "null");
        _mpv.SetOption("ao", MpvFormat.MPV_FORMAT_STRING, "null");

        _mpv.DoMpvCommand("loadfile", filePath);

        // HACK: wait for video to load
        Thread.Sleep(1000);
        //_mpv.SetProperty ("pause", MpvFormat.MPV_FORMAT_STRING, "yes");

        Width = _mpv.GetPropertyInt("width");
        Height = _mpv.GetPropertyInt("height");
        AspectRatio = _mpv.GetPropertyFloat("video-aspect");

        var bits = _mpv.GetPropertyInt("audio-bitrate");
        //int bytes = Bits2Bytes (bits);
        //int kb = Bytes2Kilobytes (bytes);
        //_AudioBitrate = (int)Math.Round (bits / 1024m, 0);
        AudioBitrate = bits;
        AudioSampleRate = _mpv.GetPropertyInt("audio-params/samplerate");
        Length = _mpv.GetPropertyInt("duration");

        //_fps = _mpv.GetPropertyInt ("container-fps");
        FPS = _mpv.GetPropertyInt("fps");
        _mpv.TryGetPropertyInt("video-bitrate", out _VideoBitrate);

        var videoFormat = _mpv.GetProperty("video-format");
        if (!string.IsNullOrWhiteSpace(videoFormat)) Video = true;


        AudioList = new List<int>();
        AudioTracks = new List<AudioTrackInfo>();
        SubtitleList = new List<SubtitlesInfo>();

        var trackCount = _mpv.GetPropertyInt("track-list/count");
        foreach (var i in Enumerable.Range(0, trackCount))
        {
            var trackType = _mpv.GetProperty($"track-list/{i}/type");
            var trackId = _mpv.GetPropertyInt($"track-list/{i}/id");
            string name;

            _mpv.TryGetProperty($"track-list/{i}/title", out name);
            string language;
            _mpv.TryGetProperty($"track-list/{i}/lang", out language);

            if (trackType == "audio")
            {
                AudioList.Add(trackId);

                var info = new AudioTrackInfo
                {
                    ID = trackId,
                    Name = name,
                    Language = language
                };

                AudioTracks.Add(info);
            }
            else if (trackType == "sub")
            {
                var info = new SubtitlesInfo
                {
                    ID = trackId,
                    Name = name,
                    Language = language
                };

                SubtitleList.Add(info);
            }
        }


        if (AspectRatio == 0.0)
        {
            AspectRatio = Width / (float)Height;
            if (AspectRatio <= 1.5) AspectRatio = ScreenAspectRatio.FourThree;
        }
    }

    ~MpvDiscover()
    {
        // Cleanup

        _mpv.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing) _mpv.Dispose();

        disposed = true;
    }
}