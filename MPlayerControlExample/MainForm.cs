/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of MPlayerControlExample.

MPlayerControlExample is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

MPlayerControlExample is distributed in the hope that it will be useful,
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

namespace MPlayerControlExample
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {

        private Discover _videoSettings;
        private MPlayer play;
        private string filePath;
        private bool trackBarMousePushedDown = false;
        private int currentTime = 0;

		private MainForm() {}

        public MainForm(string url, bool playNow, bool fullScreen)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            this.filePath = url.Trim();

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



            this.play = new MPlayer(panelVideo.Handle.ToInt32(), backend);
            this.play.VideoExited += new MplayerEventHandler(play_VideoExited);

            // Set fullscreen
            if (fullScreen == true && (this.WindowState != FormWindowState.Maximized))
            {
                this.ToggleFormFullScreen();
            }

            // start playing mmediately
            if (playNow == true && this.filePath != "")
            {
                btnPlay_Click(new object(), new EventArgs());
            }

        }
		

        private void MainForm_Load(object sender, EventArgs e)
        {


        }




        private void play_VideoExited(object sender, MplayerEvent e)
        {
            timer1.Stop();
            this.ResetTime();
        }



        private void btnPlay_Click(object sender, EventArgs e)
        {

            if (this.filePath == String.Empty || this.filePath == null)
            {

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.filePath = openFileDialog1.FileName;
                }
                else
                {
                    MessageBox.Show("You must select a video file.", "Select a file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            _videoSettings = new Discover(this.filePath);

            this.play.Play(this.filePath);

            Discover file = new Discover(this.filePath);
            lblVideoLength.Text = TimeConversion.ConvertTimeHHMMSS(file.Length);

            this.currentTime = play.CurrentPosition();

            timer1.Start();

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.play != null)
            {
                this.play.Stop();

                this.ResetTime();
                timer1.Stop();

            }
        }


        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = openFileDialog1.FileName;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            this.SetTime(1);


            float videoLength = (float)this.play.CurrentPlayingFileLength();
            if (videoLength == 0f)
            {
                return;
            }

            int percent = (int)(((float)this.currentTime / videoLength) * 100);

            if (percent >= 100)
            {
                percent = 100;
                timer1.Stop();
            }

            if (this.trackBarMousePushedDown == false)
            {
                trackBar1.Value = percent;
            }

        }


        private void SetTime(int timeInSecondsAdded)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.currentTime += timeInSecondsAdded;
                lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this.currentTime);
            });

            
        }

        private void ResetTime()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.currentTime = 0;
                lblVideoPosition.Text = TimeConversion.ConvertTimeHHMMSS(this.currentTime);
                trackBar1.Value = 0;
            });
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            this.play.Pause();

            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }

        }

        private void btnFastforward_Click(object sender, EventArgs e)
        {
            this.play.Seek(60, Seek.Relative);
            this.SetTime(60);                
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            this.play.Seek(-60, Seek.Relative);
            this.SetTime(-60);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            trackBarMousePushedDown = true;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {

            int length = this.play.CurrentPlayingFileLength();
            if (length == 0)
            {
                return;
            }

            int percentNew = trackBar1.Value;
            int newPositionInSeconds = (int)(((float)percentNew / 100.0f) * (float)length);
            int changeInSeconds = newPositionInSeconds - this.currentTime;

            this.play.Seek(changeInSeconds, Seek.Relative);
            this.SetTime(changeInSeconds);
            
            trackBarMousePushedDown = false;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if ((e.KeyChar.ToString().ToLower() == Keys.F.ToString().ToLower()) || (e.KeyChar == (char)Keys.F11))
            {
                this.ToggleFormFullScreen();
                //this.play.ToggleFullScreen(); // it is already toggled to fullscreen on the control it is attached to.  
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
                    
                    this.play.Seek(10, Seek.Relative);
                    this.SetTime(10);
                    break;
                case (int)Keys.Left:
                    this.play.Seek(-10, Seek.Relative);
                    this.SetTime(-10);
                    break;
                case (int)Keys.Up:
                    this.play.Seek(60, Seek.Relative);
                    this.SetTime(60);
                    break;
                case (int)Keys.Down:
                    this.play.Seek(-60, Seek.Relative);
                    this.SetTime(-60);
                    break;
            }

            return false;
        }



    }
}
