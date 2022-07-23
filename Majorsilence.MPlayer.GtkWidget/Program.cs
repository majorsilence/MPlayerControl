using Gtk;
using System;

namespace Majorsilence.MPlayer.GtkWidget
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.Majorsilence.MPlayer.GtkWidget.Majorsilence.MPlayer.GtkWidget", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
