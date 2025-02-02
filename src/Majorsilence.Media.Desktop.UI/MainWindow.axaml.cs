using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.Desktop.UI
{
    public partial class MainWindow : Window
    {
        Player _play;

        public MainWindow()
        {
            InitializeComponent();
            this.Opened += MainWindow_Opened;
        }

        private async void MainWindow_Opened(object sender, EventArgs e)
        {
            var b = new BackendPrograms();
            if (System.IO.File.Exists(Properties.Settings.Default.MPlayerPath) == false
                && System.IO.File.Exists(b.MPlayer) == false)
            {
                var dlg = new PlayerProperties();
                await dlg.ShowDialog(this);
            }

            this._play = PlayerFactory.Get(videoWidget.Handle.ToInt64(), Properties.Settings.Default.MPlayerPath);
            this._play.VideoExited += new MplayerEventHandler(play_VideoExited);
        }

        public async void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this._play.CurrentStatus != MediaStatus.Stopped)
            {
                this._play.Stop();
            }

            var openFileDialog = new OpenFileDialog
            {
                AllowMultiple = false
            };

            var result = await openFileDialog.ShowAsync(this);
            if (result == null || result.Length == 0)
            {
                return;
            }

            string filePath = result[0].ToString();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            this._play.Play(filePath);
        }

        private void play_VideoExited(object sender, MplayerEvent e)
        {
            this._play.Stop();
        }
    }
}