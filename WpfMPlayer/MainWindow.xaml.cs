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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MPlayer _play;

        public MainWindow()
        {
            InitializeComponent();


            //System.Windows.Forms.Panel = new System.Windows.Forms.Panel();
            


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

            _play = new MPlayer(handle, MplayerBackends.Direct3D, @"C:\Users\Peter\Desktop\MPlayer-rtm-svn-34401\mplayer.exe");
            _play.Play(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");

        }

   
    }
}
