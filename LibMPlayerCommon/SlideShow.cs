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
        private int _secondsBetweenImages=3;

        private BackendPrograms _backend;

        public SlideShow() : this("") { }
        public SlideShow(string mencoderPath)
        {
            _backend = new BackendPrograms("", mencoderPath);
            _workingDirectory = System.IO.Path.Combine(Globals.MajorSilenceMEncoderLocalAppDataDirectory, "SlideShow");
        }

        public void CreateSlideShow(List<SlideShowInfo> files, string outputFilePath, string audioFile, int secondsBetweenImages)
        {
            this._files = files;
            this._outputFilePath = outputFilePath;
            this._audioFile = audioFile;
            this._secondsBetweenImages = secondsBetweenImages;

            if (System.IO.Directory.Exists(this._workingDirectory) == false)
            {
                System.IO.Directory.CreateDirectory(this._workingDirectory);  
            }
            else
            {
                // Always make sure to clean up any previous conversion
                System.IO.Directory.Delete(this._workingDirectory, true);  
            }

            foreach (SlideShowInfo x in files)
            {

                this._fileCounter++;
                string firstPart = this._fileCounter.ToString().PadLeft(6, '0');

                string filename = System.IO.Path.Combine(this._workingDirectory, firstPart + ".jpg");
      
                System.Drawing.Image picOriginal = System.Drawing.Image.FromFile(x.FilePath);

                picOriginal.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                picOriginal.Dispose();

                // Create 30 (FPS) images for every second for each image
                for (int i = 0; i < (this._secondsBetweenImages * (int)SlideShow.FPS); i++)
                {
                    System.Drawing.Bitmap pic = (Bitmap)System.Drawing.Image.FromFile(x.FilePath);
                             
                    string secondPart = i.ToString().PadLeft(6, '0');
                    

                    if (x.Effect== SlideShowEffect.Swirl)
                    {
                        BitmapFilter.Swirl(pic, i, true);
                    }
                    else if(x.Effect== SlideShowEffect.Water)
                    {
                        BitmapFilter.Water(pic, (short)i, true);
                    }

                    filename = System.IO.Path.Combine(this._workingDirectory, firstPart + "-a-" + secondPart + ".jpg");

                    pic.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    pic.Dispose();

                }

            }

            string currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this._workingDirectory;
            CreateVideo();
            AddAudio();
            System.Environment.CurrentDirectory = currentDirectory;

            // Always make sure to clean up conversion files
            System.IO.Directory.Delete(this._workingDirectory, true);  
            
        }

        private void CreateVideo()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            

            p.StartInfo.FileName = this._backend.MEncoder;
            // harddup - use this so duplicate pictures are not removed, if they are removed there will be major audio video sync problems. 
            p.StartInfo.Arguments = "mf://*.jpg -mf fps=" + SlideShow.FPS + " -ovc lavc harddup -lavcopts vcodec=mpeg4:mbd=2:trell";
            p.StartInfo.Arguments += " -o output.avi";

            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();


            

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
            p.StartInfo.Arguments = "output.avi -o \"" + this._outputFilePath + "\" -vf harddup -ovc copy -oac mp3lame -audiofile \"" + this._audioFile + '"';
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
        Sphere,
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
