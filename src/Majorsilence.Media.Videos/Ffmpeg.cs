using System;
using System.Text;
using System.Threading.Tasks;

namespace Majorsilence.Media.Videos;

public class Ffmpeg : IVideoEncoder
{
    private readonly string _ffmpegPath;

    public Ffmpeg(string ffmpegPath)
    {
        _ffmpegPath = ffmpegPath;
    }

    public void Convert(string cmd, string workingDirectory = "")
    {
        using var p = new System.Diagnostics.Process();
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.ErrorDialog = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardError = true;
        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            p.StartInfo.WorkingDirectory = workingDirectory;
        }

        p.StartInfo.Arguments = cmd;
        p.StartInfo.FileName = _ffmpegPath;

        p.Start();

        p.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(MencoderInstance_ErrorDataReceived);
        p.BeginErrorReadLine();
        p.BeginOutputReadLine();
        p.EnableRaisingEvents = true;
        bool called = false;
        p.Exited += (s, e) =>
        {
            // hack around mono bug where when Process is disposed it sometimes retriggers the Exited event.
            if (called)
            {
                return;
            }

            called = true;
        };

        p.WaitForExit();
    }

    public void Convert(Mencoder.VideoType vidType, Mencoder.AudioType audType, string videoToConvertFilePath,
        string outputFilePath)
    {
        StringBuilder cmd = new StringBuilder();

        cmd.Append("-i ");
        cmd.Append(videoToConvertFilePath);

        if (vidType == Mencoder.VideoType.x264)
        {
            // https://trac.ffmpeg.org/wiki/Encode/H.264
            // https://superuser.com/questions/1551072/ffmpeg-how-do-i-re-encode-a-video-to-h-264-video-and-aac-audio
            // ffmpeg -i input.avi -c:v libx264 -preset slow -crf 20 -c:a aac -b:a 160k -vf format=yuv420p -movflags +faststart output.mp4
            cmd.Append(
                " -c:v libx264 -preset slow -crf 20 -c:a aac -b:a 160k -vf format=yuv420p -movflags +faststart ");
        }
        else if (vidType == Mencoder.VideoType.x265)
        {
            // https://trac.ffmpeg.org/wiki/Encode/H.265
            // ffmpeg -i input -c:v libx265 -crf 26 -preset fast -c:a aac -b:a 128k output.mp4
            cmd.Append(" -c:v libx265 -crf 26 -preset fast -c:a aac -b:a 128k ");
        }
        else if (vidType == Mencoder.VideoType.av1)
        {
            // https://trac.ffmpeg.org/wiki/Encode/AV1
            // ffmpeg -i input.mp4 -c:v libaom-av1 -crf 30 -b:v 0 av1_test.mkv
            cmd.Append(" -c:v libaom-av1 -crf 30 -b:v 0 -c:a libopus ");
        }
        else if (vidType == Mencoder.VideoType.webm)
        {
            // https://trac.ffmpeg.org/wiki/Encode/VP9
            // https://stackoverflow.com/questions/47510489/ffmpeg-convert-mp4-to-webm-poor-results
            // ffmpeg -i input.mp4 -c:v libvpx-vp9 -crf 30 -b:v 0 -b:a 128k -c:a libopus output.webm
            // TODO: two pass encoding
            cmd.Append(" -c:v libvpx-vp9 -crf 30 -b:v 0 -b:a 128k -c:a libopus ");
        }
        else
        {
            throw new NotImplementedException();
        }

        cmd.Append(outputFilePath);

        Convert(cmd.ToString());
    }

    public void Convert2WebM(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(Mencoder.VideoType.webm, Mencoder.AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2X264(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(Mencoder.VideoType.x264, Mencoder.AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2X265(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(Mencoder.VideoType.x265, Mencoder.AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2Av1(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(Mencoder.VideoType.av1, Mencoder.AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }


    /// <summary>
    /// All mencoder error output is read through this function.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MencoderInstance_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Console.Error.WriteLine(e.Data);
        }
    }
}