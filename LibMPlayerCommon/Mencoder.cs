using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text;

namespace LibMPlayerCommon
{
    public class Mencoder
    {
        private BackendPrograms _backendProgram;

        /// <summary>
        /// This event is raised each time mencoder percentage complete changes
        /// </summary>
        public event MplayerEventHandler PercentCompleted;

        /// <summary>
        /// This event is raised when mencoder is finished converting a video.
        /// </summary>
        public event MplayerEventHandler ConversionComplete;

        /// <summary>
        /// The process that is running mencoder. 
        /// </summary>
        private System.Diagnostics.Process MencoderInstance { get; set; }


        private int _currentPercent = 0;


        public Mencoder()
        {

            _backendProgram = new BackendPrograms();
        }


        public void Convert(string cmd)
        {

            MencoderInstance = new System.Diagnostics.Process();
            MencoderInstance.StartInfo.CreateNoWindow = true;
            MencoderInstance.StartInfo.UseShellExecute = false;
            MencoderInstance.StartInfo.ErrorDialog = false;
            MencoderInstance.StartInfo.RedirectStandardOutput = true;
            MencoderInstance.StartInfo.RedirectStandardInput = true;
            MencoderInstance.StartInfo.RedirectStandardError = true;



            MencoderInstance.StartInfo.Arguments = cmd;
            MencoderInstance.StartInfo.FileName = this._backendProgram.MEncoder;

            MencoderInstance.Start();


            MencoderInstance.OutputDataReceived +=new System.Diagnostics.DataReceivedEventHandler(MencoderInstance_OutputDataReceived);
            MencoderInstance.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(MencoderInstance_ErrorDataReceived);
            MencoderInstance.BeginErrorReadLine();
            MencoderInstance.BeginOutputReadLine();

        }

        public enum VideoType
        {
            xvid,
            webm,
            x264,
            wmv1,
            wmv2,
            mpeg4
        }

        public enum AudioType
        {
            ac3,
            mp3,
            mp2,
            vorbis,
            flac,
            wmav1,
            wmav2
        }

        public void Convert2WebM(string videoToConvertFilePath, string outputFilePath)
        {
            Convert(VideoType.webm, AudioType.vorbis, videoToConvertFilePath, outputFilePath);
        }

        public void Convert2X264(string videoToConvertFilePath, string outputFilePath)
        {
            Convert(VideoType.x264, AudioType.mp3, videoToConvertFilePath, outputFilePath);
        }


        public void Convert(VideoType vidType, AudioType audType, string videoToConvertFilePath, string outputFilePath)
        {
            // http://www.mplayerhq.hu/DOCS/HTML/en/menc-feat-selecting-codec.html

            StringBuilder cmd = new StringBuilder();
            // mencoder.exe          


            cmd.Append("-ovc"); // video codec for encoding 
            cmd.Append(" ");

            bool lavcVideoSelected = false;

            if (vidType == VideoType.x264)
            {
                cmd.Append("x264");  
            }
            else if (vidType == VideoType.xvid)
            {
                cmd.Append("xvid");
            }
            else
            {
                lavcVideoSelected = true;
                cmd.Append("lavc"); // use one of libavcodec's video codecs
            }
            cmd.Append(" ");

            cmd.Append("-oac"); // audio codec for encoding 
            cmd.Append(" ");
            cmd.Append("lavc"); // use one of libavcodec's audio codecs
            cmd.Append(" ");

            cmd.Append('"' + videoToConvertFilePath + '"');
            cmd.Append(" ");

            if (vidType == VideoType.webm)
            {
                cmd.Append("-ffourcc");
                cmd.Append(" ");
                cmd.Append("VP80");
                cmd.Append(" ");
            }


            cmd.Append("-lavcopts");
            cmd.Append(" ");

            if (lavcVideoSelected)
            { // Using builtin codes from lavc

                // setup the selected video format
                if (vidType == VideoType.webm)
                {
                    cmd.Append("vcodec=libvpx");
                }
                else if (vidType == VideoType.mpeg4)
                {
                    cmd.Append("vcodec=mpeg4");
                }
                else if (vidType == VideoType.wmv1)
                {
                    cmd.Append("vcodec=wmv1");
                }
                else if (vidType == VideoType.wmv2)
                {
                    cmd.Append("vcodec=wmv2");
                }
                cmd.Append(" ");
            }

            // setup the selected audio format
            if (audType == AudioType.vorbis)
            {
                cmd.Append("acodec=libvorbis");
            }
            else if (audType == AudioType.mp3)
            {
                cmd.Append("acodec=libmp3lame");
            }
            else if (audType == AudioType.ac3)
            {
                cmd.Append("acodec=ac3");
            }
            else if (audType == AudioType.flac)
            {
                cmd.Append("acodec=flac");
            }
            else if (audType == AudioType.mp2)
            {
                cmd.Append("acodec=mp2");
            }
            else if (audType == AudioType.wmav1)
            {
                cmd.Append("acodec=wmav1");
            }
            else if (audType == AudioType.wmav2)
            {
                cmd.Append("acodec=wmav2");
            }
            cmd.Append(" ");

            cmd.Append("-o");
            cmd.Append(" ");
            cmd.Append('"' + outputFilePath + '"');

            Convert(cmd.ToString());

        }







        /// <summary>
        /// All mencoder standard output is read through this function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MencoderInstance_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string line = e.Data.ToString();
                try
                {

                    if (line.StartsWith("Pos:"))
                    {
                        int percent = int.Parse(line.Substring(21, 3).Replace("%", "").Trim());
                        if (percent != _currentPercent)
                        {
                            // Only riase this event once the percent has changed
                            this.PercentCompleted(this, new MplayerEvent(percent));
                        }

                    }
                    else if (line.StartsWith("Exiting") || line.ToLower().StartsWith("eof code"))
                    {

                        this.ConversionComplete(this, new MplayerEvent("Exiting File"));
                    }


                }
                catch (Exception ex)
                {
                    LibMPlayerCommon.Logging.Instance.WriteLine(ex);
                }
                //System.Console.WriteLine(line);
            }
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
                System.Console.WriteLine(e.Data.ToString());
            }
        }


    }


    

}
