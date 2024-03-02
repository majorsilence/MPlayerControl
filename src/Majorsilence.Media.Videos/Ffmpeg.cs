using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
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
        using var p = Convert_Internal(cmd, workingDirectory);
        p.WaitForExit();
    }
    
    public async Task ConvertAsync(string cmd, string workingDirectory, CancellationToken stoppingToken)
    {
        using var p = Convert_Internal(cmd, workingDirectory);
        await p.WaitForExitAsync(stoppingToken);
    }
    
    private Process Convert_Internal(string cmd, string workingDirectory = "")
    {
        var p = new Process();
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.ErrorDialog = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardError = true;
        if (!string.IsNullOrWhiteSpace(workingDirectory)) p.StartInfo.WorkingDirectory = workingDirectory;

        p.StartInfo.Arguments = cmd;
        p.StartInfo.FileName = _ffmpegPath;

        p.Start();

        p.OutputDataReceived += MencoderInstance_ErrorDataReceived;
        p.ErrorDataReceived += MencoderInstance_ErrorDataReceived;
        p.BeginErrorReadLine();
        p.BeginOutputReadLine();
        p.EnableRaisingEvents = true;
        var called = false;
        p.Exited += (s, e) =>
        {
            // hack around mono bug where when Process is disposed it sometimes retriggers the Exited event.
            if (called) return;

            called = true;
        };

        return p;
    }

    public void Convert(VideoType vidType, AudioType audType, string videoToConvertFilePath,
        string outputFilePath)
    {
        Convert(vidType, audType, VideoAspectRatios.original, videoToConvertFilePath, outputFilePath);
    }

    public void Convert(VideoType vidType, AudioType audType, VideoAspectRatios aspectRatios,
        string videoToConvertFilePath,
        string outputFilePath)
    {
        var cmd = Convert_internal(vidType, audType, aspectRatios, videoToConvertFilePath, outputFilePath);
        Convert(cmd);
    }
    
    public async Task ConvertAsync(VideoType vidType, AudioType audType, VideoAspectRatios aspectRatios,
        string videoToConvertFilePath,
        string outputFilePath, CancellationToken stoppingToken)
    {
        var cmd = Convert_internal(vidType, audType, aspectRatios, videoToConvertFilePath, outputFilePath);
        await ConvertAsync(cmd, "", stoppingToken);
    }
    
    private string Convert_internal(VideoType vidType, AudioType audType, VideoAspectRatios aspectRatios,
        string videoToConvertFilePath,
        string outputFilePath)
    {
        var cmd = new StringBuilder();

        cmd.Append("-nostdin -i ");
        cmd.Append(videoToConvertFilePath);

        // scale -vf scale=$w:$h
        var scale = "";
        if (aspectRatios == VideoAspectRatios.p240)
            scale = "scale=426:-2";
        else if (aspectRatios == VideoAspectRatios.p360)
            scale = "scale=640:-2";
        else if (aspectRatios == VideoAspectRatios.p480)
            scale = "scale=854:-2";
        else if (aspectRatios == VideoAspectRatios.p720)
            scale = "scale=1280:-2";
        else if (aspectRatios == VideoAspectRatios.p1080)
            scale = "scale=1920:-2";
        else if (aspectRatios == VideoAspectRatios.p1440)
            scale = "scale=2560:-2";
        else if (aspectRatios == VideoAspectRatios.p2160)
            scale = "scale=3840:-2";
        else if (aspectRatios == VideoAspectRatios.p7680) scale = "scale=7680:-2";

        var audio = "";
        if (audType == AudioType.aac)
            audio = "aac";
        else if (audType == AudioType.opus)
            audio = "libopus";
        else
            throw new NotImplementedException("use aac or opus.");

        if (vidType == VideoType.x264)
        {
            // https://trac.ffmpeg.org/wiki/Encode/H.264
            // https://superuser.com/questions/1551072/ffmpeg-how-do-i-re-encode-a-video-to-h-264-video-and-aac-audio
            // ffmpeg -i input.avi -c:v libx264 -preset slow -crf 20 -c:a aac -b:a 160k -vf format=yuv420p -movflags +faststart output.mp4
            cmd.Append(
                $" -c:v libx264 -preset slow -crf 20 -c:a {audio} -b:a 128k -vf \"format=yuv420p,{scale}\" ");
        }
        else if (vidType == VideoType.x265)
        {
            // https://trac.ffmpeg.org/wiki/Encode/H.265
            // ffmpeg -i input -c:v libx265 -crf 26 -preset fast -c:a aac -b:a 128k output.mp4
            cmd.Append($" -c:v libx265 -crf 26 -preset fast -c:a {audio} -b:a 128k ");
            if (!string.IsNullOrWhiteSpace(scale)) cmd.Append($"-vf {scale} ");
        }
        else if (vidType == VideoType.av1)
        {
            // https://trac.ffmpeg.org/wiki/Encode/AV1
            // ffmpeg -i input.mp4 -c:v libaom-av1 -crf 30 -b:v 0 av1_test.mkv
            cmd.Append($" -c:v libaom-av1 -crf 30 -b:v 0 -c:a {audio} ");
            if (!string.IsNullOrWhiteSpace(scale)) cmd.Append($"-vf {scale} ");
        }
        else if (vidType == VideoType.vp9)
        {
            // https://trac.ffmpeg.org/wiki/Encode/VP9
            // https://stackoverflow.com/questions/47510489/ffmpeg-convert-mp4-to-webm-poor-results
            // ffmpeg -i input.mp4 -c:v libvpx-vp9 -crf 30 -b:v 0 -b:a 128k -c:a libopus output.webm
            // TODO: two pass encoding
            cmd.Append($" -c:v libvpx-vp9 -crf 30 -b:v 0 -b:a 128k -c:a {audio} ");
            if (!string.IsNullOrWhiteSpace(scale)) cmd.Append($"-vf {scale} ");
        }
        else
        {
            throw new NotImplementedException();
        }

        cmd.Append(outputFilePath);

        return cmd.ToString();
    }

    public void Convert2WebM(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(VideoType.vp9, AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2X264(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(VideoType.x264, AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2X265(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(VideoType.x265, AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }

    public void Convert2Av1(string videoToConvertFilePath, string outputFilePath)
    {
        Convert(VideoType.av1, AudioType.implementation_detail, videoToConvertFilePath,
            outputFilePath);
    }


    /// <summary>
    ///     All mencoder error output is read through this function.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MencoderInstance_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null) Console.Error.WriteLine(e.Data);
    }
    
    public async Task ThumbnailAsync(string videoToConvertFilePath, string outputFilePath, CancellationToken stoppingToken)
    {
        var cmd = new StringBuilder();
        cmd.Append("-i ");
        cmd.Append(videoToConvertFilePath);
        cmd.Append(" -ss 00:00:01.000 -vframes 1 ");
        cmd.Append(outputFilePath);
        await ConvertAsync(cmd.ToString(), "", stoppingToken);
    }
}