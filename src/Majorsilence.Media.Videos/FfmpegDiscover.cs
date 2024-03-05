using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Majorsilence.Media.Videos
{
    public class FfmpegDiscover : Discover
    {
        private readonly string _filePath;
        private readonly string _ffprobePath;
        private JsonDocument _mediaInfo;

        public FfmpegDiscover(string filePath, string ffprobePath)
        {
            _filePath = filePath;
            _ffprobePath = ffprobePath;
        }

        // video bit rate is in bits per second, convert to kilobits per second
        public int VideoBitrate => GetVideoStreamProperty<int>("bit_rate")/1000;

        // audio bit rate is in bits per second, convert to kilobits per second
        public int AudioBitrate => GetAudioStreamProperty<int>("bit_rate") / 1000;

        public int AudioSampleRate => GetAudioStreamProperty<int>("sample_rate");

        public int Width => GetVideoStreamProperty<int>("width");

        public int Height => GetVideoStreamProperty<int>("height");

        public int FPS
        {
            get
            {
                var raw = GetVideoStreamProperty<string>("r_frame_rate");
                string[] parts = raw.Split('/');
                return int.Parse(parts[0]) / int.Parse(parts[1]);
            }
        }

        public int Length
        {
            get
            {
                var raw = GetFormatProperty<string>("duration");
                return (int)float.Parse(raw);
            }
        }

        public float AspectRatio
        {
            get
            {
                var aspect = GetVideoStreamProperty<string>("display_aspect_ratio");
                string[] parts = aspect.Split(':');
                return float.Parse(parts[0]) / float.Parse(parts[1]);
            }
        }

        public string AspectRatioString => GetVideoStreamProperty<string>("display_aspect_ratio");

        public List<int> AudioList
        {
            get
            {
                var audioList = new List<int>();
                foreach (var stream in _mediaInfo.RootElement.GetProperty("streams").EnumerateArray())
                {
                    if (stream.GetProperty("codec_type").GetString() == "audio")
                    {
                        audioList.Add(stream.GetProperty("index").GetInt32());
                    }
                }

                return audioList;
            }
        }

        public List<AudioTrackInfo> AudioTracks => throw new NotImplementedException();

        public bool Video => HasStream("video");

        public bool Audio => HasStream("audio");

        public List<SubtitlesInfo> SubtitleList => throw new NotImplementedException();

        public void Dispose()
        {
            _mediaInfo?.Dispose();
        }

        public void Execute()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _ffprobePath,
                Arguments = $"-v quiet -print_format json -show_format -show_streams -i \"{_filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            _mediaInfo = JsonDocument.Parse(output);
        }

        public async Task ExecuteAsync()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _ffprobePath,
                Arguments = $"-v quiet -print_format json -show_format -show_streams -i \"{_filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);
            var output = await process.StandardOutput.ReadToEndAsync();
            _mediaInfo = JsonDocument.Parse(output);
        }

        private T GetFormatProperty<T>(string propertyName)
        {
            return Get<T>(_mediaInfo.RootElement.GetProperty("format").GetProperty(propertyName));
        }

        private T GetStreamProperty<T>(string streamType, string propertyName)
        {
            foreach (var stream in _mediaInfo.RootElement.GetProperty("streams").EnumerateArray())
            {
                if (stream.GetProperty("codec_type").GetString() == streamType)
                {
                    return Get<T>(stream.GetProperty(propertyName));
                }
            }

            throw new Exception($"No {streamType} stream found");
        }

        private T GetVideoStreamProperty<T>(string propertyName)
        {
            return GetStreamProperty<T>("video", propertyName);
        }

        private T GetAudioStreamProperty<T>(string propertyName)
        {
            return GetStreamProperty<T>("audio", propertyName);
        }

        private bool HasStream(string streamType)
        {
            foreach (var stream in _mediaInfo.RootElement.GetProperty("streams").EnumerateArray())
            {
                if (stream.GetProperty("codec_type").GetString() == streamType)
                {
                    return true;
                }
            }

            return false;
        }

        private T Get<T>(JsonElement element)
        {
            if (typeof(T) == typeof(int))
            {
                if (element.ValueKind == JsonValueKind.Number)
                    return (T)(object)element.GetInt32();
                return (T)(object)int.Parse(element.GetString());
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)element.GetString();
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)element.GetSingle();
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)element.GetDouble();
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)element.GetBoolean();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
            }
        }
    }
}