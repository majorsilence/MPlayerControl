/*
Copyright 2012 (C) Peter Gill <peter@majorsilence.com>

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MediaPlayer
{
    public partial class PlayerProperties : Form
    {
        public PlayerProperties()
        {
            InitializeComponent();
        }

        private void PlayerProperties_Load(object sender, EventArgs e)
        {
            textBox1.Text = MediaPlayer.Properties.Settings.Default.MPlayerPath;
        }

        private void btnMPlayerPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MediaPlayer.Properties.Settings.Default.MPlayerPath = textBox1.Text.Trim();
            MediaPlayer.Properties.Settings.Default.Save();
        }

    }
}
