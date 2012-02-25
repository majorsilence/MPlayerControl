using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibMPlayerCommon;

namespace WpfMPlayer
{
    /// <summary>
    /// Interaction logic for WpfMPlayerControl.xaml
    /// </summary>
    public partial class WpfMPlayerControl : UserControl
    {
        MPlayer _play;

        public WpfMPlayerControl()
        {
            InitializeComponent();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            if (_play != null)
            {
                _play.Stop();
            }
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_play != null)
            {
                _play.Stop();
            }

            int handle = (int)this.windowsFormsHost1.Handle;

            if (System.IO.File.Exists(MPlayerPath) == false)
            {
                throw new System.IO.FileNotFoundException("File not found", MPlayerPath);
            }

            if (System.IO.File.Exists(VideoPath) == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            _play = new MPlayer(handle, MplayerBackends.Direct3D, MPlayerPath);
            _play.Play(VideoPath);

        }

        public string MPlayerPath { get; set; }
        public string VideoPath { get; set; }
    }
}
