
using System;
using System.Windows.Forms;
using Majorsilence.Media.Videos;


namespace Test
{
    static class Test2
    {
        [STAThread]
        static void Main()
        {

            var player = Majorsilence.Media.Videos.PlayerFactory.Get(-1, "/usr/lib/x86_64-linux-gnu/libmpv.so.1");


            System.Windows.Forms.Form frm = new System.Windows.Forms.Form();
            frm.Height = 600;
            frm.Width = 800;

            var playerControl = new LibMPlayerWinform.WinFormMPlayerControl(player);
            player.SetHandle(playerControl.Handle);
            playerControl.Dock = DockStyle.Fill;

            //playerControl.MPlayerPath = @"C:\path\to\mplayer.exe";
            playerControl.VideoPath = @"/home/peter/Downloads/Die Hard 2 (1990) [1080p] {5.1}/Die.Hard.2.BluRay.1080p.x264.5.1.Judas.mp4";

            frm.Controls.Add(playerControl);

            Application.Run(frm);
        }
    }


}