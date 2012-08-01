
using System;
using System.Windows.Forms;
using LibMPlayerCommon;


namespace Test
{
    static class Test2
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Form frm = new System.Windows.Forms.Form();
            frm.Height = 600;
            frm.Width = 800;

            LibMPlayerWinform.WinFormMPlayerControl playerControl = new LibMPlayerWinform.WinFormMPlayerControl();
            playerControl.Dock = DockStyle.Fill;
            playerControl.MPlayerPath = @"C:\Users\Peter\Desktop\mplayer.exe";
            playerControl.VideoPath = @"C:\Users\Peter\Downloads\sintel_trailer-720p.mp4";
            frm.Controls.Add(playerControl);

            Application.Run(frm);
        }
    }


}