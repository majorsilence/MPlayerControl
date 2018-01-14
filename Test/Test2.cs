
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

            var player = LibMPlayerCommon.PlayerFactory.Get(-1, "/usr/lib/x86_64-linux-gnu/libmpv.so.1");


            System.Windows.Forms.Form frm = new System.Windows.Forms.Form();
            frm.Height = 600;
            frm.Width = 800;

            var playerControl = new LibMPlayerWinform.WinFormMPlayerControl(player);
            player.SetHandle(playerControl.Handle);
            playerControl.Dock = DockStyle.Fill;

            //playerControl.MPlayerPath = @"C:\path\to\mplayer.exe";
            playerControl.VideoPath = @"C:\path\to\video\sintel_trailer-720p.mp4";

            frm.Controls.Add(playerControl);

            Application.Run(frm);
        }
    }


}