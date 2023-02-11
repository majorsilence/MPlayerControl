using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Majorsilence.Media.Videos;

internal class FfmepgDiscover : Discover
{
    public int VideoBitrate => throw new NotImplementedException();

    public int AudioBitrate => throw new NotImplementedException();

    public int AudioSampleRate => throw new NotImplementedException();

    public int Width => throw new NotImplementedException();

    public int Height => throw new NotImplementedException();

    public int FPS => throw new NotImplementedException();

    public int Length => throw new NotImplementedException();

    public float AspectRatio => throw new NotImplementedException();

    public List<int> AudioList => throw new NotImplementedException();

    public List<AudioTrackInfo> AudioTracks => throw new NotImplementedException();

    public bool Video => throw new NotImplementedException();

    public bool Audio => throw new NotImplementedException();

    public List<SubtitlesInfo> SubtitleList => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Execute()
    {
        // .\ffprobe.exe -v quiet -print_format json -show_format  -show_streams -i C:\Users\Example\Path\20220716_104936.mp4
    }

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}