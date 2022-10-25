using System;

namespace Majorsilence.Media.Videos
{
    public interface Player : IDisposable
    {
        event MplayerEventHandler VideoExited;
        event MplayerEventHandler CurrentPosition;

        bool FullScreen { get; set; }

        MediaStatus CurrentStatus { get; set; }
        
        string CurrentFilePath { get; }

        void ToggleFullScreen ();

        void Stop ();

        void Pause ();

        void Mute ();

        /// <summary>
        /// The window id that the video will play in
        /// </summary>
        /// <param name="wid">Wid.</param>
        void SetHandle (long wid);

        void MovePosition (int timePosition);

        void Play (string filePath);

        void Seek (int value, Seek type);

        void SetSize (int width, int height);

        void SwitchAudioTrack (int track);

        void SwitchSubtitle (int sub);

        int CurrentPlayingFileLength ();

        void Volume (int volume);

    }
}

