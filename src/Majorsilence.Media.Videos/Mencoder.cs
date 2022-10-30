/*

Copyright 2012 (C) Peter Gill <peter@majorsilence.com>

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Majorsilence.Media.Videos
{
    [Obsolete("Use Ffmpeg class or custom built class that implements IVideoEncoder.")]
    public class Mencoder : IDisposable, IVideoEncoder
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

        private int _currentPercent = 0;


        public Mencoder()
        {

            _backendProgram = new BackendPrograms();
        }

        public Mencoder(string mencoderPath)
        {

            _backendProgram = new BackendPrograms("", mencoderPath);
        }


        bool disposed = false;

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
            }

            disposed = true;
        }

        public Task ConvertAsync(string cmd)
        {
            return Task.Run(() => Convert(cmd));
        }


        public void Convert(string cmd, string workingDirectory = "")
        {
            using (var MencoderInstance = new System.Diagnostics.Process())
            {
                MencoderInstance.StartInfo.CreateNoWindow = true;
                MencoderInstance.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                MencoderInstance.StartInfo.UseShellExecute = false;
                MencoderInstance.StartInfo.ErrorDialog = false;
                MencoderInstance.StartInfo.RedirectStandardOutput = true;
                MencoderInstance.StartInfo.RedirectStandardInput = true;
                MencoderInstance.StartInfo.RedirectStandardError = true;
                if (!string.IsNullOrWhiteSpace(workingDirectory))
                {
                    MencoderInstance.StartInfo.WorkingDirectory = workingDirectory;
                }

                MencoderInstance.StartInfo.Arguments = cmd;
                MencoderInstance.StartInfo.FileName = this._backendProgram.MEncoder;

                MencoderInstance.Start();

                MencoderInstance.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(MencoderInstance_OutputDataReceived);
                MencoderInstance.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(MencoderInstance_ErrorDataReceived);
                MencoderInstance.BeginErrorReadLine();
                MencoderInstance.BeginOutputReadLine();
                MencoderInstance.EnableRaisingEvents = true;
                bool called = false;
                MencoderInstance.Exited += (s, e) =>
                {
                    // hack around mono bug where when Process is disposed it sometimes retriggers the Exited event.
                    if (called)
                    {
                        return;
                    }
                    called = true;
                    if (this.ConversionComplete != null)
                    {
                        this.ConversionComplete(this, new MplayerEvent("Exiting File"));
                    }
                };

                MencoderInstance.WaitForExit();
            }
        }

        public Task Convert2WebMAsync(string videoToConvertFilePath, string outputFilePath)
        {
            return Task.Run(() => Convert2WebM(videoToConvertFilePath, outputFilePath));
        }

        public void Convert(VideoType vidType, AudioType audType, VideoAspectRatios aspectRatios, string videoToConvertFilePath,
            string outputFilePath)
        {
            throw new NotImplementedException();
        }

        public void Convert2WebM(string videoToConvertFilePath, string outputFilePath)
        {
            Convert(VideoType.webm, AudioType.vorbis, videoToConvertFilePath, outputFilePath);
        }

        public Task Convert2X264Async(string videoToConvertFilePath, string outputFilePath)
        {
            return Task.Run(() => Convert2X264(videoToConvertFilePath, outputFilePath));
        }

        public void Convert2X264(string videoToConvertFilePath, string outputFilePath)
        {
            Convert(VideoType.x264, AudioType.mp3, videoToConvertFilePath, outputFilePath);
        }

        public Task ConvertAsync(VideoType vidType, AudioType audType, string videoToConvertFilePath, string outputFilePath)
        {
            return Task.Run(() => Convert(vidType, audType, videoToConvertFilePath, outputFilePath));
        }

        public void Convert(VideoType vidType, AudioType audType, string videoToConvertFilePath, string outputFilePath)
        {
            // http://www.mplayerhq.hu/DOCS/HTML/en/menc-feat-selecting-codec.html

            StringBuilder cmd = new StringBuilder();
            // mencoder.exe          


            cmd.Append("-ovc"); // video codec for encoding 
            cmd.Append(" ");

            bool lavcVideoSelected = false;
            bool lavcAudioSelected = false;

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

            if (audType == AudioType.mp3)
            {
                cmd.Append("mp3lame");
            }
            else
            {
                lavcAudioSelected = true;
                cmd.Append("lavc"); // use one of libavcodec's audio codecs
            }
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


            if (lavcAudioSelected == true || lavcVideoSelected == true)
            {
                cmd.Append("-lavcopts");
                cmd.Append(" ");
            }

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

            if (lavcAudioSelected)
            {
                // setup the selected audio format
                if (audType == AudioType.vorbis)
                {
                    cmd.Append("acodec=libvorbis");
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
            }

            cmd.Append("-vf harddup"); // avoid audio/video sync issues
            cmd.Append(" ");

            cmd.Append("-o");
            cmd.Append(" ");
            cmd.Append('"' + outputFilePath + '"');

            Convert(cmd.ToString());

        }

        public Task Convert2DvdMpegAsync(RegionType regType, string videoToConvertFilePath, string outputFilePath)
        {
            return Task.Run(() => Convert2DvdMpeg(regType, videoToConvertFilePath, outputFilePath));
        }

        public void Convert2DvdMpeg(RegionType regType, string videoToConvertFilePath, string outputFilePath)
        {
            // http://www.mplayerhq.hu/DOCS/HTML/en/menc-feat-vcd-dvd.html

            StringBuilder cmd = new StringBuilder();
            // mencoder.exe          

            cmd.Append("-srate");
            cmd.Append(" ");
            cmd.Append("48000");
            cmd.Append(" ");

            cmd.Append("-af");
            cmd.Append(" ");
            cmd.Append("lavcresample=48000");
            cmd.Append(" ");

            cmd.Append("-noautosub");
            cmd.Append(" ");

            cmd.Append("-oac"); // audio codec option
            cmd.Append(" ");
            cmd.Append("lavc"); // use builtin audio codec
            cmd.Append(" ");

            cmd.Append("-aid");
            cmd.Append(" ");
            cmd.Append("0");
            cmd.Append(" ");

            cmd.Append("-ovc"); // video codec option
            cmd.Append(" ");
            cmd.Append("lavc"); // use builtin video codec
            cmd.Append(" ");

            cmd.Append("-of");
            cmd.Append(" ");
            cmd.Append("mpeg");
            cmd.Append(" ");

            cmd.Append("-mpegopts");
            cmd.Append(" ");
            cmd.Append("format=dvd:tsaf");
            cmd.Append(" ");

            cmd.Append("-ofps");
            cmd.Append(" ");
            if (regType == RegionType.PAL)
            {
                cmd.Append("25");
            }
            else if (regType == RegionType.NTSC)
            {
                cmd.Append("30000/1001");
            }
            cmd.Append(" ");

            cmd.Append("-vf");
            cmd.Append(" ");
            if (regType == RegionType.PAL)
            {
                cmd.Append("scale=720:576,harddup");
            }
            else if (regType == RegionType.NTSC)
            {
                cmd.Append("scale=720:480,harddup");
            }
            cmd.Append(" ");

            cmd.Append("-lavcopts");
            cmd.Append(" ");

            if (regType == RegionType.PAL)
            {
                cmd.Append("vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=5000:keyint=15:vstrict=0:acodec=ac3:abitrate=192:aspect=16/9");
            }
            else if (regType == RegionType.NTSC)
            {
                cmd.Append("vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=5000:keyint=18:vstrict=0:acodec=ac3:abitrate=192:aspect=16/9");
            }
            cmd.Append(" ");

            cmd.Append("-o");
            cmd.Append(" ");
            cmd.Append('"' + outputFilePath + '"');
            cmd.Append(" ");
            cmd.Append('"' + videoToConvertFilePath + '"');

            Convert(cmd.ToString());

        }


        public void Convert2X265(string videoToConvertFilePath, string outputFilePath)
        {
            throw new NotImplementedException();
        }

        public void Convert2Av1(string videoToConvertFilePath, string outputFilePath)
        {
            throw new NotImplementedException();
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
                        int percent = Globals.IntParse(line.Substring(21, 3).Replace("%", "").Trim());
                        if (percent != _currentPercent)
                        {
                            // Only riase this event once the percent has changed
                            if (this.PercentCompleted != null)
                            {
                                this.PercentCompleted(this, new MplayerEvent(percent));
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    Majorsilence.Media.Videos.Logging.Instance.WriteLine(ex);
                }
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
                Console.Error.WriteLine(e.Data);
            }
        }
    }




}
