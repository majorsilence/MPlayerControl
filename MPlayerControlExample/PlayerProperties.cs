using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPlayerControlExample
{
    public partial class PlayerProperties : Form
    {
        public PlayerProperties()
        {
            InitializeComponent();
        }

        private void PlayerProperties_Load(object sender, EventArgs e)
        {
            textBox1.Text = MPlayerControlExample.Properties.Settings.Default.MPlayerPath;
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
            MPlayerControlExample.Properties.Settings.Default.MPlayerPath = textBox1.Text.Trim();
        }

    }
}
