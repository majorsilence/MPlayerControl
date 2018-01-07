using System;

namespace LibMPlayerCommon
{
    public interface Player
    {
        event MplayerEventHandler VideoExited;
        event MplayerEventHandler CurrentPosition;

        MediaStatus CurrentStatus { get; set; }

        void Stop();

        void Pause();

        void Mute();

        void Play(string filePath);

        void Seek(int value, Seek type);

        void SetSize(int width, int height);

        void SwitchAudioTrack(int track);

        void SwitchSubtitle(int sub);

        int CurrentPlayingFileLength();
    }
}

