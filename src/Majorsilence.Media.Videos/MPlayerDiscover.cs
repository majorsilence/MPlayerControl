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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Majorsilence.Media.Videos;

/// <summary>
///     This class is used to discover information about a media file.
/// </summary>
public class MPlayerDiscover : Discover
{
    private readonly string filePath;
    private readonly string mplayerPath;

    private bool disposed;

    private MPlayerDiscover()
    {
    }

    public MPlayerDiscover(string filePath)
        : this(filePath, "")
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="mplayerPath">
    ///     If mplayerPath is left empty it will search for mplayer.exe in
    ///     "current directory\backend\mplayer.exe" on windows and mplayer in the path on linux.
    /// </param>
    public MPlayerDiscover(string filePath, string mplayerPath)
    {
        this.filePath = filePath;
        this.mplayerPath = mplayerPath;
    }

    /// <summary>
    ///     Is the files video bitrate.  Kilobits per second.
    /// </summary>
    public int VideoBitrate { get; private set; }

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

    public string AspectRatioString { get; private set; }

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
    public bool Audio { get; private set; }

    public List<SubtitlesInfo> SubtitleList { get; private set; }

    public Task ExecuteAsync()
    {
        return Task.Run(() => Execute());
    }

    public void Execute()
    {
        /*
         Reads the values of the video (width, heigth, fps...) and stores them
         into file_values.

         Returns (False,AUDIO) if the file is not a video (with AUDIO the number
         of audio tracks)

         Returns (True,0) if the file is a right video file
         */

        var mplayerLocation = new BackendPrograms(mplayerPath);

        var audio = 0;
        var video = 0;
        var nframes = 1;

        var minimum_audio = 10000;
        AudioList = new List<int>();
        AudioTracks = new List<AudioTrackInfo>();
        SubtitleList = new List<SubtitlesInfo>();
        // if CHECK_AUDIO is TRUE, we just check if it's an audio file

        //if check_audio:
        //    nframes=0
        //else:
        //    nframes=1

        using (var handle = new Process())
        {
            handle.StartInfo.UseShellExecute = false;
            handle.StartInfo.CreateNoWindow = true;
            handle.StartInfo.RedirectStandardOutput = true;
            handle.StartInfo.RedirectStandardError = true;

            handle.StartInfo.FileName = mplayerLocation.MPlayer;
            handle.StartInfo.Arguments = string.Format("-loop 1 -identify -ao null -vo null -frames 0 {0} \"{1}\"",
                nframes.ToString(), filePath);
            handle.Start();
            var line = "";
            var strReader = new StringReader(handle.StandardOutput.ReadToEnd());

            while ((line = strReader.ReadLine()) != null)
            {
                //while (handle.HasExited == false)

                if (line.Trim() == "") continue;
                var position = line.IndexOf("ID_");
                if (position == -1) continue;
                line = line.Substring(position);
                if (line.StartsWith("ID_VIDEO_BITRATE"))
                {
                    VideoBitrate = Globals.IntParse(line.Substring(17)) / 1000; // kilobits per second
                }
                else if (line.StartsWith("ID_VIDEO_WIDTH"))
                {
                    Width = Globals.IntParse(line.Substring(15));
                }
                else if (line.StartsWith("ID_VIDEO_HEIGHT"))
                {
                    Height = Globals.IntParse(line.Substring(16));
                }
                else if (line.StartsWith("ID_VIDEO_ASPECT"))
                {
                    AspectRatio = Globals.FloatParse(line.Substring(16));
                }
                else if (line.StartsWith("ID_VIDEO_FPS"))
                {
                    FPS = (int)Globals.FloatParse(line.Substring(13));
                }
                else if (line.StartsWith("ID_AUDIO_BITRATE"))
                {
                    AudioBitrate = Globals.IntParse(line.Substring(17)) / 1000; // kilobits per second
                }
                else if (line.StartsWith("ID_AUDIO_RATE"))
                {
                    AudioSampleRate = Globals.IntParse(line.Substring(14));
                }
                else if (line.StartsWith("ID_LENGTH"))
                {
                    Length = (int)Globals.FloatParse(line.Substring(10));
                }
                else if (line.StartsWith("ID_VIDEO_ID"))
                {
                    video += 1;
                    Video = true;
                }
                else if (line.StartsWith("ID_AUDIO_ID"))
                {
                    audio += 1;
                    Audio = true;
                    var audio_track = Globals.IntParse(line.Substring(12));
                    if (minimum_audio > audio_track) minimum_audio = audio_track;
                    AudioList.Add(audio_track);

                    var info = new AudioTrackInfo();
                    info.ID = audio_track;
                    AudioTracks.Add(info);
                }
                else if (line.StartsWith("ID_AID_") && line.Substring(9, 4) == "LANG")
                {
                    if (AudioTracks.Count > 0)
                    {
                        var value = line.Substring(14);

                        AudioTracks[AudioTracks.Count - 1].Language = value;
                    }
                }
                else if (line.StartsWith("ID_AID_") && line.Substring(9, 4) == "NAME")
                {
                    if (AudioTracks.Count > 0)
                    {
                        var value = line.Substring(14);

                        AudioTracks[AudioTracks.Count - 1].Name = value;
                    }
                }
                else if (line.StartsWith("ID_SUBTITLE_ID"))
                {
                    var value = Globals.IntParse(line.Substring(15));

                    var info = new SubtitlesInfo();
                    info.ID = value;
                    SubtitleList.Add(info);
                }
                else if (line.StartsWith("ID_SID_") && line.Substring(9, 4) == "LANG")
                {
                    if (SubtitleList.Count > 0)
                    {
                        var value = line.Substring(14);

                        SubtitleList[SubtitleList.Count - 1].Language = value;
                    }
                }
                else if (line.StartsWith("ID_SID_") && line.Substring(9, 4) == "NAME")
                {
                    if (SubtitleList.Count > 0)
                    {
                        var value = line.Substring(14);

                        SubtitleList[SubtitleList.Count - 1].Name = value;
                    }
                }
            }

            handle.WaitForExit();
            handle.Close();
        }
        
        int gcd = GCD(Width, Height);

        int aspectWidth = Width / gcd;
        int aspectHeight = Height / gcd;
        if(AspectRatio== 0)
            AspectRatio = (float)aspectWidth / aspectHeight;

        AspectRatioString = $"{aspectWidth}:{aspectHeight}";
    }

    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            //MediaPlayer.Dispose ();
        }

        disposed = true;
    }
}

public class AudioTrackInfo
{
    public int ID { get; set; }

    public string Language { get; set; }

    public string Name { get; set; }
}

public class SubtitlesInfo
{
    public int ID { get; set; }

    public string Language { get; set; }

    public string Name { get; set; }
}