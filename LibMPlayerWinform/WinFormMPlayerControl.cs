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
        Player _play;

        public WinFormMPlayerControl()
        {
            InitializeComponent();
        }

        public WinFormMPlayerControl(Player play)
        {
            InitializeComponent();

            _play = play;
        }

        public long Handle
        {
            get
            {
                return this.panelVideo.Handle.ToInt64();
            }
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
          
            if (System.IO.File.Exists(MPlayerPath) == false && _play is LibMPlayerCommon.MPlayer)
            {
                throw new System.IO.FileNotFoundException("File not found", MPlayerPath);
            }

            if (System.IO.File.Exists(VideoPath) == false && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            if (_play == null)
            {
                //_play = new MPlayer(Handle, MplayerBackends.Direct3D, MPlayerPath);
            }
            _play.Play(VideoPath);
        }

        public string MPlayerPath { get; set; }

        public string VideoPath { get; set; }

        private void WinFormMPlayerControl_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
