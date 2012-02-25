using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void winFormMPlayerControl1_Load(object sender, EventArgs e)
        {
            winFormMPlayerControl1.MPlayerPath = @"C:\Users\Peter\Desktop\mplayer.exe";
            winFormMPlayerControl1.VideoPath = @"C:\Users\Peter\Downloads\big_buck_bunny_480p_surround-fix.avi";
        }
    }
}
