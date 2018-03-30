using System;


namespace MplayerUnitTests
{

    internal sealed class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // setup
            var init = new SetupInitialize();
            init.RunBeforeAnyTests();

            /*
            var instSlide = new SlideShow_Test();
            instSlide.Test1();
*/
            // tests that you want to manually run

            var inst = new MPlayer_Test ();
            inst.MemoryLeak_Test ();


            // DebugMpvTests ();

        }

        private static void DebugMpvTests(){
            var mpvDiscover = new MpvDiscover_Test ();

            mpvDiscover.AspectRatio_Test1 ();
            mpvDiscover.AspectRatio_Test2 ();
            //mpvDiscover.AudioBitrate_Test1();
            //mpvDiscover.AudioBitrate_Test2();
            mpvDiscover.AudioList_Test1 ();
            mpvDiscover.AudioList_Test2 ();
            mpvDiscover.AudioSampleRate_Test1 ();
            mpvDiscover.AudioSampleRate_Test2 ();
            //mpvDiscover.FPS_Test1();
            mpvDiscover.FPS_Test2 ();
            mpvDiscover.Height_Test1 ();
            mpvDiscover.Height_Test2 ();
            //mpvDiscover.IsAudio_Test1();
            //mpvDiscover.IsAudio_Test2();
            mpvDiscover.IsVideo_Test1 ();
            mpvDiscover.IsVideo_Test2 ();
            mpvDiscover.Length_Test1 ();
            mpvDiscover.Length_Test2 ();
            //mpvDiscover.VideoBitrate_Test1();
            //mpvDiscover.VideoBitrate_Test2();

            mpvDiscover.Width_Test1 ();
            mpvDiscover.Width_Test2 (); 
        }
        
    }
}
