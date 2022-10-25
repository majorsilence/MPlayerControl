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
            var player = Majorsilence.Media.Videos.PlayerFactory.Get(winFormMPlayerControl1.Handle, "/usr/bin/mplayer");
            winFormMPlayerControl1.SetPlayer(player);
            winFormMPlayerControl1.MPlayerPath = @"/usr/bin/mplayer";
            winFormMPlayerControl1.VideoPath = @"/home/peter/Downloads/20200717_183033.mp4";
        }
    }
}
