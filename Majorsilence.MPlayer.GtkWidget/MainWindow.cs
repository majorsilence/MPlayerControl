using Gtk;
using System;
using System.Runtime.InteropServices;
using UI = Gtk.Builder.ObjectAttribute;
using LibMPlayerCommon;
using System.Security;

namespace Majorsilence.MPlayer.GtkWidget
{
    internal class MainWindow : Window
    {
        [UI] private DrawingArea _drawingArea = null;
        [UI] private Button _buttonPlay = null;

        private int _counter;
        LibMPlayerCommon.Player _play;
        public string MPlayerPath { get; set; } = @"C:\Users\peter\Downloads\MPlayer-x86_64-r38363+g4fbf3c828b\mplayer.exe";
        public string VideoPath { get; set; } = @"C:\Users\peter\Downloads\20220716_104936.mp4";

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _buttonPlay.Clicked += ButtonPlay_Clicked;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            _play?.Dispose();
            Application.Quit();
        }

        private void ButtonPlay_Clicked(object sender, EventArgs a)
        {
  
            if (_play != null)
            {
                _play.Stop();
            }

            if (System.IO.File.Exists(VideoPath) == false && VideoPath.StartsWith("http") == false)
            {
                throw new System.IO.FileNotFoundException("File not found", VideoPath);
            }

            var handle = GetNativeWidgetHandle(_drawingArea.Window);

            _play = LibMPlayerCommon.PlayerFactory.Get(handle, MPlayerPath);


            _play.Play(VideoPath);
        }

        private  long GetNativeWidgetHandle(Gdk.Window widget)
        {
            var platform = Environment.OSVersion.Platform;
            if (PlatformID.Unix == platform)
            {
                //widget.GdkWindow.GetInternalPaintInfo(
                //    out var realDrawable, out var x, out var y);
                //return LinuxNative.GdkX11DrawableGetXID(realDrawable.Handle);
            }

            if (PlatformID.Win32NT == platform)
            {
                //IntPtr windowHandle = gdk_win32_window_get_handle(this.Window.Handle);
                var hWnd = WindowsNative.gdk_win32_window_get_handle(widget.Handle);
                return hWnd.ToInt64();
            }

            throw new PlatformNotSupportedException(
                "Only tested on Windows and Linux");
        }
    }

    static class LinuxNative
    {
        [DllImport("libgdk-x11-2.0",
            EntryPoint = "gdk_x11_drawable_get_xid")]
        [return: MarshalAs(UnmanagedType.I8)]
        public static extern long GdkX11DrawableGetXID(IntPtr drawable);
    }

    static class WindowsNative
    {
        //[DllImport("libgdk-3-0.dll",
        //    EntryPoint = "gdk_win32_window_get_handle ",
        //    CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr GdkWindowToWin32Window(IntPtr raw);

        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-3-0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gdk_win32_window_get_handle(IntPtr w);
    }
}
