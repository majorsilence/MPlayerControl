﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibMPlayerCommon
{
    public interface IDiscover
    {
        int VideoBitrate { get; }
        int AudioBitrate { get; }
        int AudioSampleRate { get; }
        int Width { get; }
        int Height { get; }
        int FPS { get; }
        int Length { get; }
        float AspectRatio { get; }
        List<int> AudioList { get; }
        List<AudioTrackInfo> AudioTracks { get; }
        bool Video { get; }
        bool Audio { get; }
        List<SubtitlesInfo> SubtitleList { get; }

        void Execute ();
        Task ExecuteAsync ();
    }
}