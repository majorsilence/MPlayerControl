using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Majorsilence.Media.Videos;

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

        public void SetPlayer(Player play)
        {
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

            if (_play == null)
            {
                throw new InvalidDataException("The player is not set.  You must set it with the constructor or through the SetPlayer method.");
            }
            else if (System.IO.File.Exists(MPlayerPath) == false && _play is Majorsilence.Media.Videos.MPlayer)
            {
                throw new System.IO.FileNotFoundException("File not found", MPlayerPath);
            }
            else if (System.IO.File.Exists(MPlayerPath) == false && _play is Majorsilence.Media.Videos.MpvPlayer)
            {
                throw new System.IO.FileNotFoundException("File not found", MPlayerPath);
            }


            if (System.IO.File.Exists(VideoPath) == false && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
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
