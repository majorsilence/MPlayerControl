using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		MPlayerGtkWidget.GtkMPlayerWidget video = new MPlayerGtkWidget.GtkMPlayerWidget();
		video.MPlayerPath = "mplayer";
		video.VideoPath = @"/home/peter/Videos/The Solar System Song (with lyrics).mp4";
		this.Add(video);
		this.ShowAll();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
