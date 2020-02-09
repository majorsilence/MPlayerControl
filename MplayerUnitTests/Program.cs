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
            try
            {
                init.RunBeforeAnyTests();


                /*
                var instSlide = new SlideShow_Test();
                instSlide.Test1();
    */
                // tests that you want to manually run

                //var inst = new MPlayer_Test();
                //inst.MemoryLeak_Test();


                // DebugMpvTests ();

                DebugMencoderTests();
                DebugMplayerTests();

            }
            catch (Exception)
            {
                init.FinalTearDown();
            }

        }

        private static void DebugMencoderTests()
        {
            var mcoder = new Mencoder_Test();
            mcoder.Convert2DvdMpegNtscAsyncTest().GetAwaiter().GetResult();
            mcoder.Convert2DvdMpegNtscTest();
            mcoder.Convert2DvdMpegPalAsyncTest().GetAwaiter().GetResult();
            mcoder.Convert2DvdMpegPalTest();
            mcoder.Convert2WebMAsyncTest().GetAwaiter().GetResult();
            mcoder.Convert2WebMTest();
            mcoder.Convert2X264AsyncTest().GetAwaiter().GetResult();
            mcoder.Convert2X264Test();

            var mcoder2 = new Mencoder2_Test();
            mcoder2.Convert2DvdMpegNtscAsyncTest().GetAwaiter().GetResult();
            mcoder2.Convert2DvdMpegNtscTest();
            mcoder2.Convert2DvdMpegPalAsyncTest().GetAwaiter().GetResult();
            mcoder2.Convert2DvdMpegPalTest();
            mcoder2.Convert2WebMAsyncTest().GetAwaiter().GetResult();
            mcoder2.Convert2WebMTest();
            mcoder2.Convert2X264AsyncTest().GetAwaiter().GetResult();
            mcoder2.Convert2X264Test();
        }

        private static void DebugMplayerTests()
        {
            var mplayerDiscover = new MPlayerDiscover_Test();

            mplayerDiscover.AspectRatio_Test1();
            mplayerDiscover.AspectRatio_Test2();
            //mpvDiscover.AudioBitrate_Test1();
            //mpvDiscover.AudioBitrate_Test2();
            mplayerDiscover.AudioList_Test1();
            mplayerDiscover.AudioList_Test2();
            mplayerDiscover.AudioSampleRate_Test1();
            mplayerDiscover.AudioSampleRate_Test2();
            //mpvDiscover.FPS_Test1();
            mplayerDiscover.FPS_Test2();
            mplayerDiscover.Height_Test1();
            mplayerDiscover.Height_Test2();
            //mpvDiscover.IsAudio_Test1();
            //mpvDiscover.IsAudio_Test2();
            mplayerDiscover.IsVideo_Test1();
            mplayerDiscover.IsVideo_Test2();
            mplayerDiscover.Length_Test1();
            mplayerDiscover.Length_Test2();
            //mpvDiscover.VideoBitrate_Test1();
            //mpvDiscover.VideoBitrate_Test2();

            mplayerDiscover.Width_Test1();
            mplayerDiscover.Width_Test2();

            var mplayerTest = new MPlayer_Test();
            mplayerTest.MemoryLeak_Test();

        }

        private static void DebugMpvTests()
        {
            var mpvDiscover = new MpvDiscover_Test();

            mpvDiscover.AspectRatio_Test1();
            mpvDiscover.AspectRatio_Test2();
            //mpvDiscover.AudioBitrate_Test1();
            //mpvDiscover.AudioBitrate_Test2();
            mpvDiscover.AudioList_Test1();
            mpvDiscover.AudioList_Test2();
            mpvDiscover.AudioSampleRate_Test1();
            mpvDiscover.AudioSampleRate_Test2();
            //mpvDiscover.FPS_Test1();
            mpvDiscover.FPS_Test2();
            mpvDiscover.Height_Test1();
            mpvDiscover.Height_Test2();
            //mpvDiscover.IsAudio_Test1();
            //mpvDiscover.IsAudio_Test2();
            mpvDiscover.IsVideo_Test1();
            mpvDiscover.IsVideo_Test2();
            mpvDiscover.Length_Test1();
            mpvDiscover.Length_Test2();
            //mpvDiscover.VideoBitrate_Test1();
            //mpvDiscover.VideoBitrate_Test2();

            mpvDiscover.Width_Test1();
            mpvDiscover.Width_Test2();
        }

    }
}
