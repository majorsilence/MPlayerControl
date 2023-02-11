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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Majorsilence.Media.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Majorsilence.Media.Videos
{
    /// <summary>
    ///     With this class you can create a slideshow from many different types of images.  All operations with this class are
    ///     blocking so you should call it in a background thread.
    /// </summary>
    /// <example>
    ///     To use this class create a new instance.  Also create a new list of SlideShowInfo.  Add all images you want
    ///     in the slide show to the SlideShowImage list then call the CreateSlideShow method of the SlideShow class.
    ///     If you do not want any audio then leave the audio parameter as an empty string.
    ///     <code>
    /// Majorsilence.Media.Videos.SlideShow a = new Majorsilence.Media.Videos.SlideShow();
    /// 
    /// List<Majorsilence.Media.Videos.SlideShowInfo>
    ///             b = new  List
    ///             <Majorsilence.Media.Videos.SlideShowInfo>
    ///                 ();
    ///                 b.Add(new Majorsilence.Media.Videos.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My
    ///                 Pictures\Sample Pictures\Blue hills.jpg",  Majorsilence.Media.Videos.SlideShowEffect.Swirl));
    ///                 b.Add(new Majorsilence.Media.Videos.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My
    ///                 Pictures\Sample Pictures\Sunset.jpg",  Majorsilence.Media.Videos.SlideShowEffect.Normal));
    ///                 b.Add(new Majorsilence.Media.Videos.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My
    ///                 Pictures\Sample Pictures\Water lilies.jpg",  Majorsilence.Media.Videos.SlideShowEffect.Normal));
    ///                 b.Add(new Majorsilence.Media.Videos.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My
    ///                 Pictures\Sample Pictures\Winter.jpg",  Majorsilence.Media.Videos.SlideShowEffect.Normal));
    ///                 // Synchronous
    ///                 a.CreateSlideShow(b, @"C:\Documents and Settings\Peter\Desktop\hellworld.mpg",
    ///                 @"C:\Documents and Settings\All Users\Documents\My Music\Magnatune Compilation\Rock\16. TranceVision_
    ///                 Alpha.mp3",
    ///                 15);
    ///                 // asynchronous
    ///                 await a.CreateSlideShowAsync(b, @"C:\Documents and Settings\Peter\Desktop\hellworld.mpg",
    ///                 @"C:\Documents and Settings\All Users\Documents\My Music\Magnatune Compilation\Rock\16. TranceVision_
    ///                 Alpha.mp3",
    ///                 15);
    /// </code>
    /// </example>
    public class SlideShow
    {
        private const float FPS = 29.97f;
        private string _audioFile;

        private int _fileCounter = 0;
        private List<SlideShowInfo> _files;
        private string _imageDirectory = "";
        private string _outputFilePath;
        private int _secondsBetweenImages = 3;
        private string _videoDirectory = "";
        private string _workingDirectory = "";
        private string mencoderPath;

        public SlideShow()
            : this("")
        {
        }

        public SlideShow(string mencoderPath)
        {
            this.mencoderPath = mencoderPath;
            _workingDirectory = Path.Combine(Globals.MajorSilenceMEncoderLocalAppDataDirectory, "SlideShow");
            _imageDirectory = Path.Combine(_workingDirectory, "Images");
            _videoDirectory = Path.Combine(_workingDirectory, "Videos");
        }

        public Task CreateSlideShowAsync(List<SlideShowInfo> files, string outputFilePath, string audioFile,
            int secondsBetweenImages)
        {
            return Task.Run(() => CreateSlideShow(files, outputFilePath, audioFile, secondsBetweenImages));
        }

        public void CreateSlideShow(List<SlideShowInfo> files, string outputFilePath, string audioFile,
            int secondsBetweenImages)
        {
            this._files = files;
            this._outputFilePath = outputFilePath;
            this._audioFile = audioFile;
            this._secondsBetweenImages = secondsBetweenImages;

            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            if (Directory.Exists(this._workingDirectory))
            {
                Directory.Delete(this._workingDirectory, true);
            }

            Directory.CreateDirectory(this._workingDirectory);

            string currentDirectory = Environment.CurrentDirectory;

            SetupVideoDirectory();
            foreach (SlideShowInfo x in files)
            {
                SetupImageDirectory();


                this._fileCounter++;
                string firstPart = this._fileCounter.ToString().PadLeft(6, '0');

                string firstFilename = Path.Combine(_imageDirectory, firstPart + ".jpg");

                using (var picOriginal =
                       ImageResize.ResizeBlackBar(x.FilePath, 720, 480)) // NTSC - PAL would be 720x576;
                {
                    var jpgEncoder = new JpegEncoder();
                    picOriginal.SaveAsJpeg(firstFilename, jpgEncoder);
                }

                // Create 30 (FPS) images for every second for each image
                int imageCountPerImage = this._secondsBetweenImages * (int)FPS;
                for (int i = 0; i < imageCountPerImage; i++)
                {
                    using (var pic = Image.Load(firstFilename))
                    {
                        string secondPart = i.ToString().PadLeft(6, '0');

                        using (var picResult = SetImageEffect(x.Effect, pic, imageCountPerImage, i))
                        {
                            string filename = Path.Combine(_imageDirectory, firstPart + "-a-" + secondPart + ".jpg");

                            var jpgEncoder = new JpegEncoder();
                            picResult.SaveAsJpeg(filename, jpgEncoder);
                        }
                    }
                }

                // Create video of current images
                // output-temp.avi
                string videoName = string.Format("output-temp{0}.avi", _fileCounter.ToString().PadLeft(6, '0'));
                CreateVideo(videoName);

                // delete temporary images
                DeleteImageDirectory();


                // delete temporary video
            }

            AppendVideo();

            DeleteVideoDirectory();

            AddAudio();

            // Always make sure to clean up conversion files
            Directory.Delete(this._workingDirectory, true);
        }

        private void SetupImageDirectory()
        {
            DeleteImageDirectory();
            Directory.CreateDirectory(_imageDirectory);
        }

        private void DeleteImageDirectory()
        {
            if (Directory.Exists(_imageDirectory))
            {
                Directory.Delete(_imageDirectory, true);
            }
        }

        private void SetupVideoDirectory()
        {
            DeleteVideoDirectory();
            Directory.CreateDirectory(_videoDirectory);
        }

        private void DeleteVideoDirectory()
        {
            if (Directory.Exists(_videoDirectory))
            {
                Directory.Delete(_videoDirectory, true);
            }
        }


        private Image SetImageEffect(SlideShowEffect effect, Image pic, int imageCountPerImage, int loopCount)
        {
            if (effect == SlideShowEffect.Swirl && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.Swirl(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.Water && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.Water(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.Moire && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.Moire(pic, EffectValue(imageCountPerImage, loopCount));
            }
            else if (effect == SlideShowEffect.Pixelate && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.Pixelate(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.RandomJitter && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.RandomJitter(pic, EffectValue(imageCountPerImage, loopCount));
            }
            else if (effect == SlideShowEffect.TimeWarp && EffectImage(imageCountPerImage, loopCount))
            {
                Filter.TimeWarp(pic, (byte)EffectValue(imageCountPerImage, loopCount), true);
            }

            return pic;
        }

        private short EffectValue(int imageCountPerImage, int loopCountPosition)
        {
            float x = (imageCountPerImage / 2.0f) - loopCountPosition;
            return (short)(Math.Round(x, MidpointRounding.AwayFromZero));
        }


        private bool EffectImage(int imageCountPerImage, int loopCountPosition)
        {
            if ((imageCountPerImage - loopCountPosition) > (imageCountPerImage / 2))
            {
                return true;
            }

            return false;
        }

        private void CreateVideo(string videoName)
        {
            using (var mencod = new Mencoder(mencoderPath))
            {
                mencod.ConversionComplete += (object sender, MplayerEvent e) =>
                {
                    File.Move(Path.Combine(_imageDirectory, videoName),
                        Path.Combine(_videoDirectory, videoName));
                };

                mencod.Convert(
                    $"mf://*.jpg -mf fps={FPS} -ovc lavc harddup -lavcopts vcodec=mpeg4:mbd=2:trell scale=1920:1080 -o {videoName}",
                    _imageDirectory);
            }
        }

        private void AppendVideo()
        {
            using (var mencod = new Mencoder(mencoderPath))
            {
                // -oac copy -ovc copy -o 'combined_clip.avi' 'clip1.avi' 'clip2.avi'
                var cmd = new StringBuilder();
                cmd.Append("-oac copy -ovc copy -idx -o output.avi");

                foreach (string file in Directory.GetFiles(_videoDirectory, "*.avi"))
                {
                    cmd.Append(string.Format(" \"{0}\"", file));
                }

                mencod.ConversionComplete += (object sender, MplayerEvent e) =>
                {
                    File.Move(Path.Combine(_videoDirectory, "output.avi"),
                        Path.Combine(_workingDirectory, "output.avi"));
                };
                mencod.Convert(cmd.ToString(), _videoDirectory);
            }
        }

        private void AddAudio()
        {
            if (File.Exists(this._audioFile) == false)
            {
                File.Copy(Path.Combine(_workingDirectory, "output.avi"), this._outputFilePath);
                return;
            }

            using (var mencod = new Mencoder(mencoderPath))
            {
                // -vf harddup is needed to keep duplicates of the video when adding the audio.  Else you are going to have a huge audio/video sync problem.

                mencod.Convert("\"" + Path.Combine(_workingDirectory, "output.avi") + "\" -o \"" +
                               this._outputFilePath + "\" -vf harddup -ovc copy -oac mp3lame -audiofile \"" +
                               this._audioFile + '"');
            }
        }
    }


    public enum SlideShowEffect
    {
        Normal,
        RandomJitter,
        Swirl,
        TimeWarp,
        Moire,
        Water,
        Pixelate
    }

    public class SlideShowInfo
    {
        public SlideShowInfo(string filepath, SlideShowEffect effect)
        {
            this.FilePath = filepath;
            this.Effect = effect;
        }

        public string FilePath { get; set; }

        public SlideShowEffect Effect { get; set; }
    }
}