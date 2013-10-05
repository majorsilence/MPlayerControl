using System;
using LibMPlayerCommon;


namespace MPlayerGtkWidget
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class GtkMPlayerWidget : Gtk.Bin
	{
		MPlayer _play;
		
		public GtkMPlayerWidget ()
		{
			this.Build ();
		}

		protected void OnButtonPlayClicked (object sender, System.EventArgs e)
		{
			if (_play != null)
            {
                _play.Stop();
            }

            int handle = (int)this.drawingareaVideo.Handle;

			if (System.IO.File.Exists(VideoPath) == false  && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            _play = new MPlayer(handle, MplayerBackends.GL, MPlayerPath);
            _play.Play(VideoPath);
		}

		protected void OnButtonStopClicked (object sender, System.EventArgs e)
		{
			if (_play != null)
            {
                _play.Stop();
            }
		}
		
		public string MPlayerPath { get; set; }
        public string VideoPath { get; set; }
	}
}

