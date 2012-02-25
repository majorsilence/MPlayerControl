using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibMPlayerCommon;

namespace LibMPlayerWinform
{
    public partial class WinFormMPlayerControl : UserControl
    {
        MPlayer _play;

        public WinFormMPlayerControl()
        {
            InitializeComponent();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_play != null)
            {
                _play.Stop();
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (_play != null)
            {
                _play.Stop();
            }

            int handle = (int)this.panelVideo.Handle;

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
