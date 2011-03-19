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

namespace MPlayerControlExample
{
    partial class MainForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblVideoLength = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPlayerProperties = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnFastforward = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.lblVideoPosition = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelVideo
            // 
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.BackColor = System.Drawing.Color.Black;
            this.panelVideo.Location = new System.Drawing.Point(-15, -5);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(795, 378);
            this.panelVideo.TabIndex = 3;
            this.panelVideo.DoubleClick += new System.EventHandler(this.panelVideo_DoubleClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Video Files|*.asf;*.asx;*.avi;*.mkv;*.wmv;*.mov;*.mpeg;*.mpg;*.mp2;*.mp4;*.flv;*." +
                "webm;*.ogv;*.ogm;*.ogg;*.swf;*.vob;*.xvid;*.yuv;*.divx|All Files|*.*";
            // 
            // lblVideoLength
            // 
            this.lblVideoLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoLength.AutoSize = true;
            this.lblVideoLength.BackColor = System.Drawing.Color.Transparent;
            this.lblVideoLength.ForeColor = System.Drawing.Color.White;
            this.lblVideoLength.Location = new System.Drawing.Point(391, 11);
            this.lblVideoLength.Name = "lblVideoLength";
            this.lblVideoLength.Size = new System.Drawing.Size(35, 13);
            this.lblVideoLength.TabIndex = 6;
            this.lblVideoLength.Text = "label1";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(75, 3);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(310, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            this.trackBar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseDown);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.btnPlayerProperties);
            this.panel1.Controls.Add(this.btnRewind);
            this.panel1.Controls.Add(this.btnFastforward);
            this.panel1.Controls.Add(this.lblVideoLength);
            this.panel1.Controls.Add(this.btnLoadFile);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Controls.Add(this.lblVideoPosition);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.btnPlay);
            this.panel1.Location = new System.Drawing.Point(1, 379);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 55);
            this.panel1.TabIndex = 2;
            // 
            // btnPlayerProperties
            // 
            this.btnPlayerProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlayerProperties.BackColor = System.Drawing.Color.Transparent;
            this.btnPlayerProperties.FlatAppearance.BorderSize = 0;
            this.btnPlayerProperties.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayerProperties.Image = global::MPlayerControlExample.Properties.Resources.config;
            this.btnPlayerProperties.Location = new System.Drawing.Point(3, 3);
            this.btnPlayerProperties.Name = "btnPlayerProperties";
            this.btnPlayerProperties.Size = new System.Drawing.Size(18, 40);
            this.btnPlayerProperties.TabIndex = 10;
            this.btnPlayerProperties.UseVisualStyleBackColor = false;
            this.btnPlayerProperties.Click += new System.EventHandler(this.btnPlayerProperties_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRewind.BackColor = System.Drawing.Color.Transparent;
            this.btnRewind.FlatAppearance.BorderSize = 0;
            this.btnRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btnRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRewind.Image = global::MPlayerControlExample.Properties.Resources.rewind;
            this.btnRewind.Location = new System.Drawing.Point(582, 3);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(45, 44);
            this.btnRewind.TabIndex = 9;
            this.btnRewind.UseVisualStyleBackColor = false;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            this.btnRewind.MouseLeave += new System.EventHandler(this.btnRewind_MouseLeave);
            this.btnRewind.MouseHover += new System.EventHandler(this.btnRewind_MouseHover);
            // 
            // btnFastforward
            // 
            this.btnFastforward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFastforward.BackColor = System.Drawing.Color.Transparent;
            this.btnFastforward.FlatAppearance.BorderSize = 0;
            this.btnFastforward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnFastforward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btnFastforward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFastforward.Image = global::MPlayerControlExample.Properties.Resources.fastforward;
            this.btnFastforward.Location = new System.Drawing.Point(631, 3);
            this.btnFastforward.Name = "btnFastforward";
            this.btnFastforward.Size = new System.Drawing.Size(45, 44);
            this.btnFastforward.TabIndex = 8;
            this.btnFastforward.UseVisualStyleBackColor = false;
            this.btnFastforward.Click += new System.EventHandler(this.btnFastforward_Click);
            this.btnFastforward.MouseLeave += new System.EventHandler(this.btnFastforward_MouseLeave);
            this.btnFastforward.MouseHover += new System.EventHandler(this.btnFastforward_MouseHover);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadFile.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadFile.FlatAppearance.BorderSize = 0;
            this.btnLoadFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnLoadFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btnLoadFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadFile.Image = global::MPlayerControlExample.Properties.Resources.document_open;
            this.btnLoadFile.Location = new System.Drawing.Point(446, 6);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(75, 42);
            this.btnLoadFile.TabIndex = 5;
            this.btnLoadFile.UseVisualStyleBackColor = false;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            this.btnLoadFile.MouseLeave += new System.EventHandler(this.btnLoadFile_MouseLeave);
            this.btnLoadFile.MouseHover += new System.EventHandler(this.btnLoadFile_MouseHover);
            // 
            // lblVideoPosition
            // 
            this.lblVideoPosition.AutoSize = true;
            this.lblVideoPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblVideoPosition.ForeColor = System.Drawing.Color.White;
            this.lblVideoPosition.Location = new System.Drawing.Point(27, 11);
            this.lblVideoPosition.Name = "lblVideoPosition";
            this.lblVideoPosition.Size = new System.Drawing.Size(35, 13);
            this.lblVideoPosition.TabIndex = 2;
            this.lblVideoPosition.Text = "label1";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::MPlayerControlExample.Properties.Resources.stop;
            this.btnStop.Location = new System.Drawing.Point(682, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(45, 44);
            this.btnStop.TabIndex = 1;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.MouseLeave += new System.EventHandler(this.btnStop_MouseLeave);
            this.btnStop.MouseHover += new System.EventHandler(this.btnStop_MouseHover);
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlay.BackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Image = global::MPlayerControlExample.Properties.Resources.play;
            this.btnPlay.Location = new System.Drawing.Point(731, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(45, 44);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlay.MouseLeave += new System.EventHandler(this.btnPlay_MouseLeave);
            this.btnPlay.MouseHover += new System.EventHandler(this.btnPlay_MouseHover);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(784, 428);
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "MPlayerControlExample";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MainForm_DragOver);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnFastforward;
        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblVideoLength;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblVideoPosition;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnPlayerProperties;
    }
}
