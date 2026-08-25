using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Majorsilence.Forms;
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.PlayerControls
{
    public partial class WinFormMPlayerControl : UserControl
    {
        Player _play;

        private readonly VideoView _videoView;

        public WinFormMPlayerControl()
        {
            InitializeComponent();
            _videoView = AttachVideoView();
        }

        public WinFormMPlayerControl(Player play)
            : this()
        {
            SetPlayer(play);
        }

        public void SetPlayer(Player play)
        {
            _play = play;
            _videoView.SetPlayer(play);
        }

        // Majorsilence.Forms composites all its controls into one drawn surface, so panelVideo has no
        // native window for mpv to draw into. The video is painted into the panel instead, from the
        // frames mpv renders into memory -- see VideoView.
        private VideoView AttachVideoView()
        {
            var view = new VideoView { Dock = DockStyle.Fill };
            panelVideo.Controls.Add(view);
            return view;
        }

        /// <summary>
        /// The platform window id to embed a player into. Always zero on Majorsilence.Forms, which has
        /// no per-control native windows; players handed a zero id render their frames back to us
        /// instead (see <see cref="VideoView"/>), so passing this on is harmless and kept for callers
        /// ported from the WinForms build.
        /// </summary>
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MPlayerPath { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string VideoPath { get; set; }

        private void WinFormMPlayerControl_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
