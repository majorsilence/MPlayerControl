namespace Test
{
    partial class Form1
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
            this.winFormMPlayerControl1 = new LibMPlayerWinform.WinFormMPlayerControl();
            this.SuspendLayout();
            // 
            // winFormMPlayerControl1
            // 
            this.winFormMPlayerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winFormMPlayerControl1.Location = new System.Drawing.Point(0, 0);
            this.winFormMPlayerControl1.MPlayerPath = null;
            this.winFormMPlayerControl1.Name = "winFormMPlayerControl1";
            this.winFormMPlayerControl1.Size = new System.Drawing.Size(944, 572);
            this.winFormMPlayerControl1.TabIndex = 0;
            this.winFormMPlayerControl1.VideoPath = null;
            this.winFormMPlayerControl1.Load += new System.EventHandler(this.winFormMPlayerControl1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 572);
            this.Controls.Add(this.winFormMPlayerControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private LibMPlayerWinform.WinFormMPlayerControl winFormMPlayerControl1;
    }
}

