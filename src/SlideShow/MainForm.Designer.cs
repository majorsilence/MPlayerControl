namespace SlideShow
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new Majorsilence.Forms.ListView();
            this.columnFileName = ((Majorsilence.Forms.ColumnHeader)(new Majorsilence.Forms.ColumnHeader()));
            this.columnFilePath = ((Majorsilence.Forms.ColumnHeader)(new Majorsilence.Forms.ColumnHeader()));
            this.CreateVideo = new Majorsilence.Forms.Button();
            this.groupBox2 = new Majorsilence.Forms.GroupBox();
            this.AudioFile = new Majorsilence.Forms.Button();
            this.openFileDialog1 = new Majorsilence.Forms.OpenFileDialog();
            this.label1 = new Majorsilence.Forms.Label();
            this.numericUpDown1 = new Majorsilence.Forms.NumericUpDown();
            this.panel1 = new Majorsilence.Forms.Panel();
            this.statusStrip1 = new Majorsilence.Forms.StatusStrip();
            this.toolStripProgressBar1 = new Majorsilence.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new Majorsilence.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new Majorsilence.Forms.SaveFileDialog();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Anchor = ((Majorsilence.Forms.AnchorStyles)((((Majorsilence.Forms.AnchorStyles.Top | Majorsilence.Forms.AnchorStyles.Bottom) 
            | Majorsilence.Forms.AnchorStyles.Left) 
            | Majorsilence.Forms.AnchorStyles.Right)));
            // Majorsilence.Forms.ListView.ColumnHeaderCollection has no AddRange overload.
            this.listView1.Columns.Add(this.columnFileName);
            this.listView1.Columns.Add(this.columnFilePath);
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(14, 166);
            this.listView1.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(806, 458);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = Majorsilence.Forms.View.Details;
            this.listView1.DragDrop += new Majorsilence.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.DragEnter += new Majorsilence.Forms.DragEventHandler(this.listView1_DragEnter);
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "Filename";
            this.columnFileName.Width = 115;
            // 
            // columnFilePath
            // 
            this.columnFilePath.Text = "Filepath";
            this.columnFilePath.Width = 347;
            // 
            // CreateVideo
            // 
            this.CreateVideo.Location = new System.Drawing.Point(598, 122);
            this.CreateVideo.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.CreateVideo.Name = "CreateVideo";
            this.CreateVideo.Size = new System.Drawing.Size(201, 35);
            this.CreateVideo.TabIndex = 6;
            this.CreateVideo.Text = "Create Slideshow";
            this.CreateVideo.UseVisualStyleBackColor = true;
            this.CreateVideo.Click += new System.EventHandler(this.CreateVideo_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AudioFile);
            this.groupBox2.Location = new System.Drawing.Point(4, 17);
            this.groupBox2.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(802, 65);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Audio";
            // 
            // AudioFile
            // 
            this.AudioFile.Location = new System.Drawing.Point(9, 25);
            this.AudioFile.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.AudioFile.Name = "AudioFile";
            this.AudioFile.Size = new System.Drawing.Size(784, 35);
            this.AudioFile.TabIndex = 1;
            this.AudioFile.Text = "Select Audio File";
            this.AudioFile.UseVisualStyleBackColor = true;
            this.AudioFile.Click += new System.EventHandler(this.AudioFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 122);
            this.label1.Margin = new Majorsilence.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Time in seconds between picture change:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(334, 118);
            this.numericUpDown1.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(180, 26);
            this.numericUpDown1.TabIndex = 9;
            this.numericUpDown1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.CreateVideo);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = Majorsilence.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(831, 665);
            this.panel1.TabIndex = 10;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new Majorsilence.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 620);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new Majorsilence.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(831, 45);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 37);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(383, 38);
            this.toolStripStatusLabel2.Text = "This may take several minutes or much longer. ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = Majorsilence.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 665);
            this.Controls.Add(this.panel1);
            this.Margin = new Majorsilence.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Slide show";
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Majorsilence.Forms.ListView listView1;
        private Majorsilence.Forms.ColumnHeader columnFileName;
        private Majorsilence.Forms.ColumnHeader columnFilePath;
        private Majorsilence.Forms.Button CreateVideo;
        private Majorsilence.Forms.GroupBox groupBox2;
        private Majorsilence.Forms.Button AudioFile;
        private Majorsilence.Forms.OpenFileDialog openFileDialog1;
        private Majorsilence.Forms.Label label1;
        private Majorsilence.Forms.NumericUpDown numericUpDown1;
        private Majorsilence.Forms.Panel panel1;
        private Majorsilence.Forms.StatusStrip statusStrip1;
        private Majorsilence.Forms.ToolStripProgressBar toolStripProgressBar1;
        private Majorsilence.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private Majorsilence.Forms.SaveFileDialog saveFileDialog1;
    }
}

