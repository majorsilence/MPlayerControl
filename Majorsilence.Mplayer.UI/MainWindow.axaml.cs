using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Majorsilence.Mplayer.UI
{
    public partial class MainWindow : Window
    {
        LibMPlayerCommon.Player _play;
        //public string MPlayerPath { get; set; } = @"mplayer";
        public string MPlayerPath { get; set; } = "/opt/homebrew/bin/mpv";
        public string VideoPath { get; set; } = @"/Users/petergill/Downloads/test.mp4";

        public MainWindow()
        {
            InitializeComponent();
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            // Change button text when button is clicked.
            // var button = (Button)sender;
            //button.Content = "Hello, Avalonia!";

            if (_play != null)
            {
                _play.Stop();
            }

            if (System.IO.File.Exists(VideoPath) == false && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            // var xxx = videoControl.CreateNativeControlCore(parent);
            var handle = videoWidget.Handle.ToInt64();
            
            _play = LibMPlayerCommon.PlayerFactory.Get(handle, MPlayerPath);
            
            _play.Play(VideoPath);
        }
    }
}