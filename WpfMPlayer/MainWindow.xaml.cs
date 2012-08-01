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
    /// Load an d
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            videoControl.MPlayerPath = @"C:\Users\Peter\Desktop\mplayer.exe";
            videoControl.VideoPath = @"C:\Users\Peter\Downloads\big_buck_bunny_480p_surround-fix.avi";
        }

  
    }
}
