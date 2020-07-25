using System;
using System.Runtime.InteropServices;
using LibMPlayerCommon;


namespace MPlayerGtkWidget
{
    static class LinuxNative
    {
        [DllImport("libgdk-x11-2.0",
            EntryPoint = "gdk_x11_drawable_get_xid")]
        [return: MarshalAs(UnmanagedType.I8)]
        public static extern long GdkX11DrawableGetXID(IntPtr drawable);
    }

    static class WindowsNative
    {
        [DllImport("libgdk-win32-2.0-0.dll",
            EntryPoint = "gdk_win32_drawable_get_handle",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GdkWindowToWin32Window(IntPtr raw);
    }

    [System.ComponentModel.ToolboxItem(true)]
    public partial class GtkMPlayerWidget : Gtk.Bin
    {
        MPlayer _play;

        public GtkMPlayerWidget()
        {
            this.Build();
        }

        private static long GetNativeWidgetHandle(Gtk.Widget widget)
        {
            var platform = Environment.OSVersion.Platform;
            if (PlatformID.Unix == platform)
            {
                widget.GdkWindow.GetInternalPaintInfo(
                    out var realDrawable, out var x, out var y);
                return LinuxNative.GdkX11DrawableGetXID(realDrawable.Handle);
            }

            if (PlatformID.Win32NT == platform)
            {
                var hWnd = WindowsNative.GdkWindowToWin32Window(widget.Handle);
                return hWnd.ToInt64();
            }

            throw new PlatformNotSupportedException(
                "Only tested on Windows and Linux");
        }

        protected void OnButtonPlayClicked(object sender, System.EventArgs e)
        {
            if (_play != null)
            {
                _play.Stop();
            }

            if (System.IO.File.Exists(VideoPath) == false && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            var handle = GetNativeWidgetHandle(drawingareaVideo);

            _play = new MPlayer(handle, MplayerBackends.X11, MPlayerPath);
            _play.Play(VideoPath);
        }

        protected void OnButtonStopClicked(object sender, System.EventArgs e)
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