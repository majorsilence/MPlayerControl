using Majorsilence.Media.Videos;

namespace Majorsilence.Media.WorkerService
{
    public class Settings
    {
        public string UploadFolder { get; init; }
        public string MEncoderPath { get; init; }
        public string ConvertedFolder { get; init; }
        public string FfmpegPath { get; init; }
        public VideoAspectRatios [] AspectRatios { get; init; }
        public Dictionary<VideoType, AudioType> VideoAudioConverters { get; init; }
        public Dictionary<VideoType, string> VideoFileExtension { get; init; }
    }
}
