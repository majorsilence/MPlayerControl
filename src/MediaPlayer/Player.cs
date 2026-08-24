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
using Majorsilence.Forms;

using Majorsilence.Media.Videos;

namespace MediaPlayer
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class Player : Form
    {

        // The track bar's Value only ranges 0..TrackBarResolution, not 0..100 -- a plain percentage
        // only has 100 distinct thumb positions, so on anything but a short video most position events
        // land on the same whole percent as the last one and the thumb visibly jumps instead of gliding.
        private const int TrackBarResolution = 10000;

        private Majorsilence.Media.Videos.Discover _videoSettings;
        private Majorsilence.Media.Videos.Player _play;
        private LibMPlayerWinform.VideoView _videoView;
        private string _filePath;
        private bool _trackBarMousePushedDown = false;
        private int _currentTime = 0;
        private float _currentPositionSeconds = 0f;
        private bool _fullscreen = false;
        private bool _playNow = false;

        private bool _playOnceAndClose;

        private Player()
        {
        }

        public Player(string url, bool playNow, bool fullScreen)
            : this(url, playNow, fullScreen, false)
        {
        }

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
            // KeyPreview lets the form act on a key before the focused control does, which is what
            // makes MainForm_KeyDown's seeking work while the track bar has focus.
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            // Overrides the designer's 0-100 range -- see TrackBarResolution.
            trackBar1.Maximum = TrackBarResolution;

            Majorsilence.Media.Videos.BackendPrograms b = new Majorsilence.Media.Videos.BackendPrograms();
            if (System.IO.File.Exists(MediaPlayer.Properties.Settings.Default.MPlayerPath) == false
                && System.IO.File.Exists(b.MPlayer) == false)
            {
                // Nothing configured yet: look in the usual install locations before bothering the user.
                var discovered = Majorsilence.Media.Videos.PlayerDiscovery.FindPlayerPath();
                if (discovered != null)
                {
                    MediaPlayer.Properties.Settings.Default.MPlayerPath = discovered;
                    MediaPlayer.Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Cannot find mplayer or libmpv.  Loading properties form to select.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnPlayerProperties_Click(sender, e);
                }
            }

            // Majorsilence.Forms composites every control into a single drawn surface, so panelVideo
            // has no native window of its own to embed into -- Handle is always zero. Passing that on
            // would leave mpv drawing into a separate top-level window. Instead the video is painted
            // into the form by _videoView, from frames mpv renders into memory (see VideoView).
            this._videoView = new LibMPlayerWinform.VideoView { Dock = DockStyle.Fill };
            panelVideo.Controls.Add(this._videoView);

            this._play = Majorsilence.Media.Videos.PlayerFactory.Get(panelVideo.Handle.ToInt64(), MediaPlayer.Properties.Settings.Default.MPlayerPath);
            //this._play = new MPlayer(panelVideo.Handle.ToInt64(), backend, MediaPlayer.Properties.Settings.Default.MPlayerPath);
            if (this._play == null)
            {
                // PlayerFactory returns null for a path that names neither mplayer nor libmpv — i.e. the
                // properties dialog was cancelled without picking one. Leave the UI up rather than crash.
                MessageBox.Show("No mplayer or libmpv path is configured, so playback is disabled.  Set one in the properties form.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this._videoView.SetPlayer(this._play);

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

            if (this._playOnceAndClose != true) return;
            try
            {

                this.Invoke(new MethodInvoker(Close));
            }
            catch (Exception ex)
            {
                Logging.Instance.WriteLine(ex);
            }

        }

        private void _play_CurrentPosition(object sender, MplayerEvent e)
        {
            // handle current postion event.  Display the current postion and update trackbar.
            SetExactTime(e.Value);
        }



        private async void btnPlay_Click(object sender, EventArgs e)
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

            if (string.IsNullOrEmpty(this._filePath))
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

            _videoSettings = Majorsilence.Media.Videos.DiscoverFactory.Get(this._filePath, MediaPlayer.Properties.Settings.Default.MPlayerPath);
            await _videoSettings.ExecuteAsync();
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
            if (this._play == null) return;
            btnPlay.Image = MediaPlayer.Properties.Resources.play;

            this._play.Stop();

            this.ResetTime();
        }


        private void SetExactTime(float newTime)
        {
            this.Invoke((MethodInvoker)delegate
                {
                    this._currentPositionSeconds = Math.Max(0f, newTime);
                    this._currentTime = (int)this._currentPositionSeconds;
                    lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this._currentTime);
                    SyncTrackBarToCurrentTime();
                });


        }

        private void SetTime(int timeInSecondsAdded)
        {
            this.Invoke((MethodInvoker)delegate
                {
                    this._currentTime = Math.Max(0, this._currentTime + timeInSecondsAdded);
                    this._currentPositionSeconds = this._currentTime;
                    lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this._currentTime);
                    SyncTrackBarToCurrentTime();
                });


        }

        /// <summary>
        /// Moves the track bar thumb to wherever <see cref="_currentTime"/> now is.
        /// </summary>
        /// <remarks>
        /// Every seek runs through SetTime/SetExactTime, but only the once-a-second position event
        /// used to move the thumb -- so a seek left it sitting at the old spot until the next tick,
        /// and not at all while paused. That was invisible while the arrow keys were nudging the
        /// thumb by themselves (and seeking nothing); now that they really seek, the thumb has to
        /// follow the position rather than the keystroke. The Fast-forward and Rewind buttons take
        /// the same path and get the same fix.
        ///
        /// Must be called on the UI thread -- the callers are already inside their Invoke.
        /// </remarks>
        private void SyncTrackBarToCurrentTime()
        {
            // Leave the thumb alone while it is being dragged; the drag is the source of truth then.
            if (this._trackBarMousePushedDown) return;

            int videoLength = this._play == null ? 0 : this._play.CurrentPlayingFileLength();
            if (videoLength <= 0) return;

            int value = (int)((this._currentPositionSeconds / videoLength) * TrackBarResolution);
            trackBar1.Value = Math.Max(trackBar1.Minimum, Math.Min(trackBar1.Maximum, value));
        }

        private void ResetTime()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                    {
                        this._currentTime = 0;
                        this._currentPositionSeconds = 0f;
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

            int newPositionInSeconds = (int)(((float)trackBar1.Value / TrackBarResolution) * (float)length);
            int changeInSeconds = newPositionInSeconds - this._currentTime;

            this._play.Seek(changeInSeconds, Seek.Relative);
            this.SetTime(changeInSeconds);

            _trackBarMousePushedDown = false;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Console.WriteLine("key press: " + e.KeyChar.ToString());
            if ((e.KeyChar.ToString().ToLower() == Keys.F.ToString().ToLower()) || (e.KeyChar == (char)Keys.F11))
            {
                this.ToggleFormFullScreen();
            }
        }


        private Majorsilence.Forms.FormBorderStyle _border = FormBorderStyle.Sizable;
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

                this.FormBorderStyle = Majorsilence.Forms.FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }

            
        }

        private void panelVideo_DoubleClick(object sender, EventArgs e)
        {
            this.ToggleFormFullScreen();
        }



        // Arrow-key seeking. This used to be a ProcessKeyPreview(ref Message) override, which is a
        // Win32 message-pump hook: Majorsilence.Forms has no message pump, so it declares the method
        // only so ported code compiles and never calls it. The override was silently dead, which is
        // why the arrow keys nudged the track bar (TrackBar handles them itself) without ever moving
        // the video. KeyPreview + KeyDown is the portable equivalent, and it is set in MainForm_Load.
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._play == null) return;

            int seconds;
            switch (e.KeyCode)
            {
                case Keys.Right: seconds = 10; break;
                case Keys.Left: seconds = -10; break;
                case Keys.Up: seconds = 60; break;
                case Keys.Down: seconds = -60; break;
                default: return;
            }

            this._play.Seek(seconds, Seek.Relative);
            this.SetTime(seconds);

            // Handled here, so the focused control (typically the track bar, which moves its own
            // thumb on the arrow keys) does not also act on the same keystroke and fight the
            // position that _play_CurrentPosition is about to report back.
            e.Handled = true;
        }

        private void btnPlayerProperties_Click(object sender, EventArgs e)
        {
            PlayerProperties dlg = new PlayerProperties();
            dlg.ShowDialog();
        }



        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            btnStop_Click(sender, e);
            btnPlay.Image = MediaPlayer.Properties.Resources.play;
            this._filePath = openFileDialog1.FileName;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (s.Length <= 0) return;
            // Stop current playing and start new file.
            btnStop_Click(sender, e);
            this._filePath = s[0];
            btnPlay_Click(sender, e);

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
            _play?.SetSize(this.panelVideo.Width, this.panelVideo.Height);
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

        private void buttonMute_Click(object sender, EventArgs e)
        {
            this._play.Mute();
        }

        private void Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            _videoSettings?.Dispose ();
            _play?.Dispose();
        }

        private void Player_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
