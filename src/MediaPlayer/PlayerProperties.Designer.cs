namespace MediaPlayer
{
    partial class PlayerProperties
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
            this.label1 = new Majorsilence.Forms.Label();
            this.textBox1 = new Majorsilence.Forms.TextBox();
            this.openFileDialog1 = new Majorsilence.Forms.OpenFileDialog();
            this.btnMPlayerPath = new Majorsilence.Forms.Button();
            this.statusStrip1 = new Majorsilence.Forms.StatusStrip();
            this.lblStatus = new Majorsilence.Forms.ToolStripStatusLabel();
            this.btnSave = new Majorsilence.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MPlayer/libmpv Path:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(403, 20);
            this.textBox1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnMPlayerPath
            // 
            this.btnMPlayerPath.Location = new System.Drawing.Point(504, 9);
            this.btnMPlayerPath.Name = "btnMPlayerPath";
            this.btnMPlayerPath.Size = new System.Drawing.Size(114, 23);
            this.btnMPlayerPath.TabIndex = 2;
            this.btnMPlayerPath.Text = "Select MPlayer/libmpv Path";
            this.btnMPlayerPath.UseVisualStyleBackColor = true;
            this.btnMPlayerPath.Click += new System.EventHandler(this.btnMPlayerPath_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new Majorsilence.Forms.ToolStripItem[]
                {
                    this.lblStatus
                });
            this.statusStrip1.Location = new System.Drawing.Point(0, 106);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(630, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(504, 80);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PlayerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = Majorsilence.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 128);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnMPlayerPath);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "PlayerProperties";
            this.Text = "Player Properties";
            this.Load += new System.EventHandler(this.PlayerProperties_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Majorsilence.Forms.Label label1;
        private Majorsilence.Forms.TextBox textBox1;
        private Majorsilence.Forms.OpenFileDialog openFileDialog1;
        private Majorsilence.Forms.Button btnMPlayerPath;
        private Majorsilence.Forms.StatusStrip statusStrip1;
        private Majorsilence.Forms.ToolStripStatusLabel lblStatus;
        private Majorsilence.Forms.Button btnSave;
    }
}