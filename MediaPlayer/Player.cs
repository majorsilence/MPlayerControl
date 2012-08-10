/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of MediaPlayer.

MediaPlayer is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

MediaPlayer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using LibMPlayerCommon;

namespace MediaPlayer
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class Player : Form
    {

        private Discover _videoSettings;
        private MPlayer _play;
        private string _filePath;
        private bool _trackBarMousePushedDown = false;
        private int _currentTime = 0;
        private bool _fullscreen=false;
        private bool _playNow = false;

        private bool _playOnceAndClose;

		private Player() {}

        public Player(string url, bool playNow, bool fullScreen) : this(url, playNow, fullScreen, false){}

        public Player(string url, bool playNow, bool fullScreen, bool playOnceAndClose)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            _fullscreen = fullScreen;
            _playNow = playNow;

            this._filePath = url.Trim();
            _playOnceAndClose = playOnceAndClose;

        }
		
        
        private void MainForm_Load(object sender, EventArgs e)
        {

            LibMPlayerCommon.BackendPrograms b = new LibMPlayerCommon.BackendPrograms();
            if (System.IO.File.Exists(MediaPlayer.Properties.Settings.Default.MPlayerPath) == false
                && System.IO.File.Exists(b.MPlayer) == false && BackendPrograms.OSPlatform() == "windows")
            {
                MessageBox.Show("Cannot find mplayer.  Loading properties form to select.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnPlayerProperties_Click(sender, e);
            }


            MplayerBackends backend;
            System.PlatformID runningPlatform = System.Environment.OSVersion.Platform;
            if (runningPlatform == System.PlatformID.Unix)
            {
                backend = MplayerBackends.GL2;
            }
            else if (runningPlatform == PlatformID.MacOSX)
            {
                backend = MplayerBackends.OpenGL;
            }
            else
            {
                backend = MplayerBackends.Direct3D;
            }


            this._play = new MPlayer(panelVideo.Handle.ToInt32(), backend, MediaPlayer.Properties.Settings.Default.MPlayerPath);
            this._play.VideoExited += new MplayerEventHandler(play_VideoExited);
            this._play.CurrentPosition += new MplayerEventHandler(_play_CurrentPosition);


            // Set fullscreen
            if (_fullscreen == true && (this.WindowState != FormWindowState.Maximized))
            {
                this.ToggleFormFullScreen();
            }

            // start playing mmediately
            if (_playNow == true && this._filePath != "")
            {
                btnPlay_Click(new object(), new EventArgs());
            }

        }


        private void play_VideoExited(object sender, MplayerEvent e)
        {
            btnPlay.Image = MediaPlayer.Properties.Resources.play;
            this._play.Stop();
            this.ResetTime();

            if (this._playOnceAndClose == true)
            {
                try
                {

                    this.Invoke(new MethodInvoker(Close));
                }
                catch (Exception ex)
                {
                    Logging.Instance.WriteLine(ex);
                }
            }

        }

        private void _play_CurrentPosition(object sender, MplayerEvent e)
        {
            // handle current postion event.  Display the current postion and update trackbar.

            SetExactTime(e.Value);

            float videoLength = (float)this._play.CurrentPlayingFileLength();
            if (videoLength == 0f)
            {
                return;
            }

            int percent = (int)(((float)this._currentTime / videoLength) * 100);

            if (percent >= 100)
            {
                percent = 100;
            }

            if (this._trackBarMousePushedDown == false)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    trackBar1.Value = percent;
                });
            }

        }



        private void btnPlay_Click(object sender, EventArgs e)
        {

            if (this._play.CurrentStatus != MediaStatus.Stopped)
            {
                if (this._play.CurrentStatus == MediaStatus.Paused)
                {
                    // Is currently paused so start playing file and set the image to the pause image.
                    btnPlay.Image = MediaPlayer.Properties.Resources.pause;
                }
                if (this._play.CurrentStatus == MediaStatus.Playing)
                {
                    // Is currently playing a file so pause it and set the image to the play image.
                    btnPlay.Image = MediaPlayer.Properties.Resources.play;
                }

                this._play.Pause();

                return;
                
            }

            if (this._filePath == String.Empty || this._filePath == null)
            {

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this._filePath = openFileDialog1.FileName;
                }
                else
                {
                    MessageBox.Show("You must select a video file.", "Select a file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            _videoSettings = new Discover(this._filePath, MediaPlayer.Properties.Settings.Default.MPlayerPath);
            this._play.Play(this._filePath);
            lblVideoLength.Text = TimeConversion.ConvertTimeHHMMSS(_videoSettings.Length);

            btnPlay.Image = MediaPlayer.Properties.Resources.pause;

            comboBoxAudioTracks.DisplayMember = "Name";
            comboBoxAudioTracks.ValueMember = "ID";
            comboBoxAudioTracks.DataSource = _videoSettings.AudioTracks;


            comboBoxSubtitles.DisplayMember = "Name";
            comboBoxSubtitles.ValueMember = "ID";
            comboBoxSubtitles.DataSource = _videoSettings.SubtitleList;

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this._play != null)
            {
                btnPlay.Image = MediaPlayer.Properties.Resources.play;

                this._play.Stop();

                this.ResetTime();

            }
        }


        private void SetExactTime(int newTime)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this._currentTime = newTime;
                lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this._currentTime);
            });


        }

        private void SetTime(int timeInSecondsAdded)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this._currentTime += timeInSecondsAdded;
                lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this._currentTime);
            });

            
        }

        private void ResetTime()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this._currentTime = 0;
                    lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this._currentTime);
                    trackBar1.Value = 0;
                });
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(ex);
            }
        }

        private void btnFastforward_Click(object sender, EventArgs e)
        {
            this._play.Seek(60, Seek.Relative);
            this.SetTime(60);                
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            this._play.Seek(-60, Seek.Relative);
            this.SetTime(-60);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            _trackBarMousePushedDown = true;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            // Move the video to the new selected postion.

            int length = this._play.CurrentPlayingFileLength();
            if (length == 0)
            {
                return;
            }

            int percentNew = trackBar1.Value;
            int newPositionInSeconds = (int)(((float)percentNew / 100.0f) * (float)length);
            int changeInSeconds = newPositionInSeconds - this._currentTime;

            this._play.Seek(changeInSeconds, Seek.Relative);
            this.SetTime(changeInSeconds);

            _trackBarMousePushedDown = false;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if ((e.KeyChar.ToString().ToLower() == Keys.F.ToString().ToLower()) || (e.KeyChar == (char)Keys.F11))
            {
                this.ToggleFormFullScreen();
            }
        }


        private System.Windows.Forms.FormBorderStyle _border = FormBorderStyle.Sizable;
        private FormWindowState _windowstate = FormWindowState.Normal;
        private void ToggleFormFullScreen()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = this._border;
                this.WindowState = this._windowstate;
            }
            else
            {
                this._border = this.FormBorderStyle;
                this._windowstate = this.WindowState;

                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            
        }

        private void panelVideo_DoubleClick(object sender, EventArgs e)
        {
            this.ToggleFormFullScreen();
        }



        protected override bool ProcessKeyPreview(ref System.Windows.Forms.Message m)
        {
            switch (m.WParam.ToInt32())
            {
                // TODO: Fix so move commands are sent to correct carousel
                case (int)Keys.Right:

                    this._play.Seek(10, Seek.Relative);
                    this.SetTime(10);
                    break;
                case (int)Keys.Left:
                    this._play.Seek(-10, Seek.Relative);
                    this.SetTime(-10);
                    break;
                case (int)Keys.Up:
                    this._play.Seek(60, Seek.Relative);
                    this.SetTime(60);
                    break;
                case (int)Keys.Down:
                    this._play.Seek(-60, Seek.Relative);
                    this.SetTime(-60);
                    break;
            }

            return false;
        }

        private void btnPlayerProperties_Click(object sender, EventArgs e)
        {
            PlayerProperties dlg = new PlayerProperties();
            dlg.ShowDialog();
        }



        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                btnStop_Click(sender, e);
                btnPlay.Image = MediaPlayer.Properties.Resources.play;
                this._filePath = openFileDialog1.FileName;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (s.Length > 0)
            {
                // Stop current playing and start new file.
                btnStop_Click(sender, e);
                this._filePath = s[0];
                btnPlay_Click(sender, e);
            }

        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void MainForm_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        #region Button Style Changes
        private void btnLoadFile_MouseHover(object sender, EventArgs e)
        {
            btnLoadFile.FlatStyle = FlatStyle.Popup;
        }

        private void btnLoadFile_MouseLeave(object sender, EventArgs e)
        {
            btnLoadFile.FlatStyle = FlatStyle.Flat;
        }

        private void btnRewind_MouseHover(object sender, EventArgs e)
        {
            btnRewind.FlatStyle = FlatStyle.Popup;
        }

        private void btnRewind_MouseLeave(object sender, EventArgs e)
        {
            btnRewind.FlatStyle = FlatStyle.Flat;
        }

        private void btnFastforward_MouseHover(object sender, EventArgs e)
        {
            btnFastforward.FlatStyle = FlatStyle.Popup;
        }

        private void btnFastforward_MouseLeave(object sender, EventArgs e)
        {
            btnFastforward.FlatStyle = FlatStyle.Flat;
        }

        private void btnStop_MouseHover(object sender, EventArgs e)
        {
            btnStop.FlatStyle = FlatStyle.Popup;
        }

        private void btnStop_MouseLeave(object sender, EventArgs e)
        {
            btnStop.FlatStyle = FlatStyle.Flat;
        }

        private void btnPlay_MouseHover(object sender, EventArgs e)
        {
            btnPlay.FlatStyle = FlatStyle.Popup;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            btnPlay.FlatStyle = FlatStyle.Flat;
        }

        #endregion Button Style Change

        private void panelVideo_Resize(object sender, EventArgs e)
        {
			if (this._play == null)
			{
				return;
			}
            this._play.SetSize(this.panelVideo.Width, this.panelVideo.Height);
        }

        private void comboBoxAudioTracks_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxAudioTracks.SelectedIndex == -1)
            {
                return;
            }
            AudioTrackInfo trackInfo = (AudioTrackInfo)comboBoxAudioTracks.SelectedItem;
            
            this._play.SwitchAudioTrack(trackInfo.ID);
        }

        private void comboBoxSubtitles_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxSubtitles.SelectedIndex == -1)
            {
                return;
            }
            SubtitlesInfo info = (SubtitlesInfo)comboBoxSubtitles.SelectedItem;

            this._play.SwitchSubtitle(info.ID);
        }

    }
}
