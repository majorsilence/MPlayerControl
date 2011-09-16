/*

Copyright 2011 (C) Peter Gill <peter@majorsilence.com>

This file is part of SlideShow.

SlideShow is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

SlideShow is distributed in the hope that it will be useful,
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

namespace SlideShow
{
    public partial class MainForm : Form
    {
        delegate void SlideShowRun(string outputMpeg, string audioFilePath, int lengthInSecondsBetweenPhotos, List<string> photoList);
        private List<string> _photoList;

        public MainForm()
        {
            InitializeComponent();

            _photoList = new List<string>();
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int i;
            for (i = 0; i < s.Length; i++)
            {
                this.AddPhotoToString(s[i]);
            }

            this.LoadPhotosIntoListView();
        }

        private void AddPhotoToString(string str)
        {
            bool keep = true;
            foreach (string s in _photoList)
            {
                if (str == s)
                {
                    keep = false;
                }
            }
            if (keep == true && ValidFileType(str))
            {
                _photoList.Add(str);
            }
        }

        private bool ValidFileType(string filepath)
        {
            string extentionType = System.IO.Path.GetExtension(filepath).ToLower();

            if (extentionType == ".jpg" || extentionType == ".bmp" || extentionType == ".png")
            {
                return true;
            }
            return false;
        }

        private void LoadPhotosIntoListView()
        {

            if (_photoList.Count <= 0)
            {
                MessageBox.Show("Please add photos to Lowercase");
                return;
            }

            listView1.Items.Clear();
            List<ListViewItem> itemList = new List<ListViewItem>();

            foreach (string str in _photoList)
            {
                ListViewItem item = new ListViewItem(System.IO.Path.GetFileName(str));
                item.SubItems.Add(str);
                itemList.Add(item);
            }
            listView1.Items.AddRange(itemList.ToArray());
        }


        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }



        private void AudioFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AudioFile.Text = openFileDialog1.FileName;
            }
        }

        private void CreateVideo_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            string saveFile = saveFileDialog1.FileName;

            if (System.IO.File.Exists(saveFile))
            {
                if (MessageBox.Show("File with same name already exists.  Do you want to overwrite?", "Overwrite",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
            }

            DisableControls();

            SlideShowRun runner = StartConversion;
            runner.BeginInvoke(saveFile, AudioFile.Text, (int)numericUpDown1.Value, _photoList, Callback, runner);
        }

        private void EnableControls()
        {
            panel1.Enabled = true;
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Visible = false;
            toolStripStatusLabel2.Visible = false;
            MessageBox.Show("Slideshow created.", "Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisableControls()
        {
            panel1.Enabled = false;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel2.Visible = true; 
       }

        private void StartConversion(string outputMpeg, string audioFilePath, int lengthInSecondsBetweenPhotos, List<string> photoList)
        {
            LibMPlayerCommon.SlideShow a = new LibMPlayerCommon.SlideShow();
            List<LibMPlayerCommon.SlideShowInfo> b = new List<LibMPlayerCommon.SlideShowInfo>();

            var options = LibMPlayerCommon.SlideShowEffect.Flip | LibMPlayerCommon.SlideShowEffect.Moire | LibMPlayerCommon.SlideShowEffect.Normal |
                LibMPlayerCommon.SlideShowEffect.Pixelate | LibMPlayerCommon.SlideShowEffect.RandomJitter | LibMPlayerCommon.SlideShowEffect.Swirl |
                LibMPlayerCommon.SlideShowEffect.TimeWarp | LibMPlayerCommon.SlideShowEffect.Water;

            // Add each photo in the list with a random picture change effect.
            foreach (string filePath in photoList)
            {

                if (System.IO.File.Exists(filePath))
                {

                    var matching = Enum.GetValues(typeof(LibMPlayerCommon.SlideShowEffect))
                           .Cast<LibMPlayerCommon.SlideShowEffect>()
                           .Where(c => (options & c) == c)
                           .ToArray();

                    LibMPlayerCommon.SlideShowEffect randomEffect = matching[new Random().Next(matching.Length)];

                    b.Add(new LibMPlayerCommon.SlideShowInfo(filePath, randomEffect));
                }
            }

            a.CreateSlideShow(b , outputMpeg,
                audioFilePath,
                lengthInSecondsBetweenPhotos);
        }

        private void Callback (IAsyncResult r)
        {
            this.BeginInvoke(new Action(EnableControls), null);
        }

    }
}
