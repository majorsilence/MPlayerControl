using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace LibMPlayerCommon
{

    /// <summary>
    /// With this class you can create a slideshow from many different types of images.  All operations with this class are
    /// blocking so you should call it in a background thread.
    /// </summary>
    /// <example>
    /// To use this class create a new instance.  Also create a new list of SlideShowInfo.  Add all images you want
    /// in the slide show to the SlideShowImage list then call the CreateSlideShow method of the SlideShow class.
    /// 
    /// If you do not want any audio then leave the audio parameter as an empty string.
    /// <code>
    /// LibMPlayerCommon.SlideShow a = new LibMPlayerCommon.SlideShow();
    /// 
    /// List<LibMPlayerCommon.SlideShowInfo> b = new  List<LibMPlayerCommon.SlideShowInfo>();
    /// b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Blue hills.jpg",  LibMPlayerCommon.SlideShowEffect.Swirl));
    /// b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Sunset.jpg",  LibMPlayerCommon.SlideShowEffect.Normal));
    /// b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Water lilies.jpg",  LibMPlayerCommon.SlideShowEffect.Normal));
    /// b.Add(new LibMPlayerCommon.SlideShowInfo(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Winter.jpg",  LibMPlayerCommon.SlideShowEffect.Normal));
    /// 
    /// a.CreateSlideShow(b, @"C:\Documents and Settings\Peter\Desktop\hellworld.mpg", 
    ///    @"C:\Documents and Settings\All Users\Documents\My Music\Magnatune Compilation\Rock\16. TranceVision_ Alpha.mp3", 
    ///    15);
    /// </code>
    /// </example>
    public class SlideShow
    {
        private List<SlideShowInfo> _files;
        private const float FPS = 29.97f;
        private string _outputFilePath;
        private string _audioFile;

        private int _fileCounter = 0;
        private string _workingDirectory = "";
        private string _imageDirectory = "";
        private string _videoDirectory = "";
        private int _secondsBetweenImages=3;

        private BackendPrograms _backend;

        public SlideShow() : this("") { }
        public SlideShow(string mencoderPath)
        {
            _backend = new BackendPrograms("", mencoderPath);
            _workingDirectory = System.IO.Path.Combine(Globals.MajorSilenceMEncoderLocalAppDataDirectory, "SlideShow");
            _imageDirectory = System.IO.Path.Combine(_workingDirectory, "Images");
            _videoDirectory = System.IO.Path.Combine(_workingDirectory, "Videos");
        }

        public void CreateSlideShow(List<SlideShowInfo> files, string outputFilePath, string audioFile, int secondsBetweenImages)
        {
            this._files = files;
            this._outputFilePath = outputFilePath;
            this._audioFile = audioFile;
            this._secondsBetweenImages = secondsBetweenImages;

            if (System.IO.File.Exists(outputFilePath))
            {
                System.IO.File.Delete(outputFilePath);
            }

            if (System.IO.Directory.Exists(this._workingDirectory))
            {
                System.IO.Directory.Delete(this._workingDirectory, true);  
                
            }
            System.IO.Directory.CreateDirectory(this._workingDirectory);
            string currentDirectory = System.Environment.CurrentDirectory;

            SetupVideoDirectory();
            foreach (SlideShowInfo x in files)
            {
                SetupImageDirectory();
                

                this._fileCounter++;
                string firstPart = this._fileCounter.ToString().PadLeft(6, '0');

                string firstFilename = System.IO.Path.Combine(_imageDirectory, firstPart + ".jpg");
      
                System.Drawing.Image picOriginal = System.Drawing.Image.FromFile(x.FilePath);
                picOriginal = LibImages.ImageResize.ResizeBlackBar(picOriginal, 720, 480); // NTSC - PAL would be 720x576;

                picOriginal.Save(firstFilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                picOriginal.Dispose();

                // Create 30 (FPS) images for every second for each image
                int imageCountPerImage = this._secondsBetweenImages * (int)SlideShow.FPS;
                for (int i = 0; i < imageCountPerImage; i++)
                {

                    System.Drawing.Bitmap pic = (Bitmap)System.Drawing.Image.FromFile(firstFilename);
                             
                    string secondPart = i.ToString().PadLeft(6, '0');

                    pic = SetImageEffect(x.Effect, pic, imageCountPerImage, i);

                    string filename = System.IO.Path.Combine(_imageDirectory, firstPart + "-a-" + secondPart + ".jpg");

                    pic.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    pic.Dispose();

                }

                // Create video of current images
                
                System.Environment.CurrentDirectory = _imageDirectory;

                // output-temp.avi
                string videoName = string.Format("output-temp{0}.avi", _fileCounter.ToString().PadLeft(6, '0'));
                CreateVideo(videoName);       
                System.Environment.CurrentDirectory = currentDirectory;


                // delete temporary images
                DeleteImageDirectory();

                  
                // delete temporary video

            }

            System.Environment.CurrentDirectory = _videoDirectory;
            AppendVideo();
            System.Environment.CurrentDirectory = currentDirectory;

            DeleteVideoDirectory();

            AddAudio();

            // Always make sure to clean up conversion files
            System.IO.Directory.Delete(this._workingDirectory, true);  
            
        }

        private void SetupImageDirectory()
        {
            DeleteImageDirectory();
            System.IO.Directory.CreateDirectory(_imageDirectory);
        }

        private void DeleteImageDirectory()
        {
            if (System.IO.Directory.Exists(_imageDirectory))
            {
                System.IO.Directory.Delete(_imageDirectory, true);
            }
        }

        private void SetupVideoDirectory()
        {
            DeleteVideoDirectory();
            System.IO.Directory.CreateDirectory(_videoDirectory);
        }

        private void DeleteVideoDirectory()
        {
            if (System.IO.Directory.Exists(_videoDirectory))
            {
                System.IO.Directory.Delete(_videoDirectory, true);
            }
        }


        private System.Drawing.Bitmap SetImageEffect(SlideShowEffect effect, System.Drawing.Bitmap pic, int imageCountPerImage, int loopCount)
        {
            if (effect == SlideShowEffect.Swirl && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.Swirl(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.Water && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.Water(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.Moire && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.Moire(pic, EffectValue(imageCountPerImage, loopCount));
            }
            else if (effect == SlideShowEffect.Pixelate && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.Pixelate(pic, EffectValue(imageCountPerImage, loopCount), true);
            }
            else if (effect == SlideShowEffect.RandomJitter && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.RandomJitter(pic, EffectValue(imageCountPerImage, loopCount));
            }
            else if (effect == SlideShowEffect.TimeWarp && EffectImage(imageCountPerImage, loopCount))
            {
                LibImages.BitmapFilter.TimeWarp(pic, (byte)EffectValue(imageCountPerImage, loopCount), true);
            }

            return pic;
        }

        private short EffectValue(int imageCountPerImage, int loopCountPosition)
        {
            return (short)((imageCountPerImage / 2) - loopCountPosition);
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
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = this._backend.MEncoder;
            // harddup - use this so duplicate pictures are not removed, if they are removed there will be major audio video sync problems. 
            p.StartInfo.Arguments = "mf://*.jpg -mf fps=" + SlideShow.FPS + " -ovc lavc harddup -lavcopts vcodec=mpeg4:mbd=2:trell";
            p.StartInfo.Arguments += string.Format(" -o {0}", videoName);

            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            System.IO.File.Move(System.IO.Path.Combine(System.Environment.CurrentDirectory, videoName),
                System.IO.Path.Combine(_videoDirectory, videoName));
            

        }

        private void AppendVideo()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = this._backend.MEncoder;
            // -oac copy -ovc copy -o 'combined_clip.avi' 'clip1.avi' 'clip2.avi'
            p.StartInfo.Arguments = "-oac copy -ovc copy -idx -o output.avi";

            foreach (string file in System.IO.Directory.GetFiles(System.Environment.CurrentDirectory, "*.avi"))
            {
                p.StartInfo.Arguments += string.Format(" \"{0}\"", file);
            }

            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            System.IO.File.Move(System.IO.Path.Combine(_videoDirectory, "output.avi"),
                System.IO.Path.Combine("../", "output.avi"));
        }

        private void AddAudio()
        {

            if (System.IO.File.Exists(this._audioFile) == false)
            {
                System.IO.File.Copy("output.avi", this._outputFilePath);
                return;
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = this._backend.MEncoder;

            // -vf harddup is needed to keep duplicates of the video when adding the audio.  Else you are going to have a huge audio/video sync problem.
            p.StartInfo.Arguments = "\"" + System.IO.Path.Combine(_workingDirectory,"output.avi") +  "\" -o \"" + this._outputFilePath + "\" -vf harddup -ovc copy -oac mp3lame -audiofile \"" + this._audioFile + '"';
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();


        }


    }


    public enum SlideShowEffect
    {
        Normal,
        Flip,
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
