/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerCommon.

LibMPlayerCommon is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

LibMPlayerCommon is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;


using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;


namespace LibMPlayerCommon
{

    public class MpvPlayer : IDisposable, Player
    {

        private const int MpvFormatString = 1;
        private IntPtr _libMpvDll;
        private IntPtr _mpvHandle;

        #region Linux

        [DllImport ("libdl.so")]
        protected static extern IntPtr dlopen (string filename, int flags);

        [DllImport ("libdl.so")]
        protected static extern IntPtr dlsym (IntPtr handle, string symbol);

        const int RTLD_NOW = 2;
        // for dlopen's flags

        #endregion

        [DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary (string dllToLoad);

        [DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr GetProcAddress (IntPtr hModule, string procedureName);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate IntPtr MpvCreate ();

        private MpvCreate _mpvCreate;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvInitialize (IntPtr mpvHandle);

        private MpvInitialize _mpvInitialize;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvCommand (IntPtr mpvHandle, IntPtr strings);

        private MpvCommand _mpvCommand;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvTerminateDestroy (IntPtr mpvHandle);

        private MpvTerminateDestroy _mpvTerminateDestroy;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvSetOption (IntPtr mpvHandle, byte[] name, int format, ref long data);

        private MpvSetOption _mpvSetOption;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvSetOptionString (IntPtr mpvHandle, byte[] name, byte[] value);

        private MpvSetOptionString _mpvSetOptionString;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvGetPropertystring (IntPtr mpvHandle, byte[] name, int format, ref IntPtr data);

        private MpvGetPropertystring _mpvGetPropertyString;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate int MpvSetProperty (IntPtr mpvHandle, byte[] name, int format, ref byte[] data);

        private MpvSetProperty _mpvSetProperty;

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        private delegate void MpvFree (IntPtr data);

        private MpvFree _mpvFree;


        private object GetDllType (Type type, string name)
        {
            IntPtr address;
            var platform = PlatformCheck.RunningPlatform ();
            if (platform == Platform.Windows) {
                address = GetProcAddress (_libMpvDll, name);
            } else if (platform == Platform.Linux) {
                address = dlsym (_libMpvDll, name);
            } else {
                throw new NotImplementedException ();
            }

            if (address != IntPtr.Zero)
                return Marshal.GetDelegateForFunctionPointer (address, type);
            return null;
        }

        private void LoadMpvDynamic ()
        {
            var platform = PlatformCheck.RunningPlatform ();
            if (platform == Platform.Windows) {
                _libMpvDll = LoadLibrary (_libMpvPath); // "mpv-1.dll"); // The dll is included in the DEV builds by lachs0r: https://mpv.srsfckn.biz/
            } else if (platform == Platform.Linux) {
                _libMpvDll = dlopen (_libMpvPath, RTLD_NOW); //("/usr/lib/x86_64-linux-gnu/libmpv.so.1", RTLD_NOW);
            } else {
                throw new NotImplementedException ();
            }


            _mpvCreate = (MpvCreate)GetDllType (typeof(MpvCreate), "mpv_create");
            _mpvInitialize = (MpvInitialize)GetDllType (typeof(MpvInitialize), "mpv_initialize");
            _mpvTerminateDestroy = (MpvTerminateDestroy)GetDllType (typeof(MpvTerminateDestroy), "mpv_terminate_destroy");
            _mpvCommand = (MpvCommand)GetDllType (typeof(MpvCommand), "mpv_command");
            _mpvSetOption = (MpvSetOption)GetDllType (typeof(MpvSetOption), "mpv_set_option");
            _mpvSetOptionString = (MpvSetOptionString)GetDllType (typeof(MpvSetOptionString), "mpv_set_option_string");
            _mpvGetPropertyString = (MpvGetPropertystring)GetDllType (typeof(MpvGetPropertystring), "mpv_get_property");
            _mpvSetProperty = (MpvSetProperty)GetDllType (typeof(MpvSetProperty), "mpv_set_property");
            _mpvFree = (MpvFree)GetDllType (typeof(MpvFree), "mpv_free");
        }

        // *******************************************

        private long _wid;
        private bool _fullscreen;

        // Current position in seconds in stream.
        private float _currentPosition = 0;

        // The total length that the video is in seconds.
        private int _totalTime = 0;

        private string currentFilePath;
        private string _libMpvPath;


        public event MplayerEventHandler VideoExited;
        /// <summary>
        /// This event is the most accurate way to get the current position of the current playing file.
        /// Whenever the postion changes this event will notify that the positon has changed with the value
        /// being the new position (seconds into the file).
        /// </summary>
        public event MplayerEventHandler CurrentPosition;

        private System.Timers.Timer _currentPostionTimer;


        public MpvPlayer (long wid)
            : this (wid, "")
        {
        }

        public MpvPlayer (long wid, string libMpvPath)
            : this (wid, libMpvPath, false, TimeSpan.FromMilliseconds (1000))
        {
        }

        /// <summary>
        /// Create a new instance of mplayer class.
        /// </summary>
        /// <param name="wid">Window ID that mplayer should attach itself</param>
        /// <param name="libMpvPath">The full filepath to libmpv. </param>
        /// <param name="loadMplayer">If true mplayer will immediately be loaded and you should not attempt to 
        /// play any files until MplayerRunning is true.</param>
        /// <param name="positionUpdateInterval">Interval of periodical position updates</param>
        public MpvPlayer (long wid, string libMpvPath, bool loadMplayer, TimeSpan positionUpdateInterval)
        {
            System.Environment.SetEnvironmentVariable ("LC_NUMERIC", "C");

            this._wid = wid;
            this._fullscreen = false;
            this.MplayerRunning = false;
            this._libMpvPath = libMpvPath;
            this.CurrentStatus = MediaStatus.Stopped;

            // This timer will send an event every second with the current video postion when video
            // is in play mode.
            this._currentPostionTimer = new System.Timers.Timer (positionUpdateInterval.TotalMilliseconds);
            this._currentPostionTimer.Elapsed += new ElapsedEventHandler (_currentPostionTimer_Elapsed);
            this._currentPostionTimer.Enabled = false;

            if (loadMplayer) {
                Action caller = new Action (InitializeMplayer);
                caller.BeginInvoke (null, null);
            }

        }

        /// <summary>
        /// Cleanup resources.  Currently this means that mplayer is closed if it is still running.
        /// </summary>
        /// <remarks>Alternatively call the Dispose method.</remarks>
        ~MpvPlayer ()
        {
            // Cleanup

            if (_mpvHandle != IntPtr.Zero) {
                _mpvTerminateDestroy (_mpvHandle);
            }

        }

        bool disposed = false;

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposed)
                return;

            if (disposing) {
                if (_mpvHandle != IntPtr.Zero) {
                    _mpvTerminateDestroy (_mpvHandle);
                }
            }

            disposed = true;
        }

        /// <summary>
        /// Is mplayer alreadying running?  True or False.
        /// </summary>
        public bool MplayerRunning { get; set; }

        /// <summary>
        /// The current status of the player.
        /// </summary>
        public MediaStatus CurrentStatus { get; set; }

        public bool HardwareAccelerated { get; set; }


        private void InitializeMplayer ()
        {
            this._currentPostionTimer.Start ();
            this.MplayerRunning = true;
        }

        /// <summary>
        /// Load and start playing a video.
        /// </summary>
        /// <param name="filePath"></param>
        public void Play (string filePath)
        {
            LoadFile (filePath);

            if (_mpvHandle == IntPtr.Zero) {
                return;
            }

            var bytes = GetUtf8Bytes ("no");
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("pause"), MpvFormatString, ref bytes);

            this.CurrentStatus = MediaStatus.Playing;
        }



        private void DoMpvCommand (params string[] args)
        {
            // https://github.com/mpv-player/mpv/blob/master/DOCS/man/input.rst

            IntPtr[] byteArrayPointers;
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel (args, out byteArrayPointers);
            _mpvCommand (_mpvHandle, mainPtr);
            foreach (var ptr in byteArrayPointers) {
                Marshal.FreeHGlobal (ptr);
            }
            Marshal.FreeHGlobal (mainPtr);
        }

        public static IntPtr AllocateUtf8IntPtrArrayWithSentinel (string[] arr, out IntPtr[] byteArrayPointers)
        {
            int numberOfStrings = arr.Length + 1; // add extra element for extra null pointer last (sentinel)
            byteArrayPointers = new IntPtr [numberOfStrings];
            IntPtr rootPointer = Marshal.AllocCoTaskMem (IntPtr.Size * numberOfStrings);
            for (int index = 0; index < arr.Length; index++) {
                var bytes = GetUtf8Bytes (arr [index]);
                IntPtr unmanagedPointer = Marshal.AllocHGlobal (bytes.Length);
                Marshal.Copy (bytes, 0, unmanagedPointer, bytes.Length);
                byteArrayPointers [index] = unmanagedPointer;
            }
            Marshal.Copy (byteArrayPointers, 0, rootPointer, numberOfStrings);
            return rootPointer;
        }


        private static byte [] GetUtf8Bytes (string s)
        {
            return Encoding.UTF8.GetBytes (s + "\0");
        }

        /// <summary>
        /// Starts a new video/audio file immediatly.  Requires that Play has been called.
        /// </summary>
        /// <param name="filePath">string</param>
        public void LoadFile (string filePath)
        {
            this.currentFilePath = filePath;

            if (_mpvHandle != IntPtr.Zero)
                _mpvTerminateDestroy (_mpvHandle);

            LoadMpvDynamic ();
            if (_libMpvDll == IntPtr.Zero)
                return;

            _mpvHandle = _mpvCreate.Invoke ();
            if (_mpvHandle == IntPtr.Zero)
                return;

            _mpvInitialize.Invoke (_mpvHandle);
            _mpvSetOptionString (_mpvHandle, GetUtf8Bytes ("keep-open"), GetUtf8Bytes ("always"));
            int mpvFormatInt64 = 4;
            _mpvSetOption (_mpvHandle, GetUtf8Bytes ("wid"), mpvFormatInt64, ref _wid);
            DoMpvCommand ("loadfile", filePath);

            this.LoadCurrentPlayingFileLength ();
        }

        public void SetHandle (long wid)
        {
            _wid = wid;
            //int mpvFormatInt64 = 4;
            //_mpvSetOption (_mpvHandle, GetUtf8Bytes ("wid"), mpvFormatInt64, ref _wid);
        }

        /// <summary>
        /// Prepare filepaths to be used witht the loadfile command.  
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// For some reason it strips the DirectorySeperatorChar so we double it up here.
        /// </remarks>
        private string PrepareFilePath (string filePath)
        {
            string preparedPath = filePath.Replace ("" + System.IO.Path.DirectorySeparatorChar, "" + System.IO.Path.DirectorySeparatorChar + System.IO.Path.DirectorySeparatorChar);

            return preparedPath;
        }


        /// <summary>
        /// Move to a new position in the video.
        /// </summary>
        /// <param name="timePosition">Seconds.  The position to seek move to.</param>
        public void MovePosition (int timePosition)
        {
            if (_mpvHandle == IntPtr.Zero) {
                return;
            }

            DoMpvCommand ("seek", timePosition.ToString (CultureInfo.InvariantCulture), "absolute");
        }



        /// <summary>
        /// Seek a new postion.
        /// Seek to some place in the movie.
        /// Seek.Relative is a relative seek of +/- value seconds (default).
        /// Seek.Percentage is a seek to value % in the movie.
        /// Seek.Absolute is a seek to an absolute position of value seconds.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void Seek (int value, Seek type)
        {
            if (_mpvHandle == IntPtr.Zero) {
                return;
            }

            DoMpvCommand ("seek", value.ToString (CultureInfo.InvariantCulture), type.ToString ().ToLower ());
        }

        public void SetSize (int width, int height)
        {
            if (this.CurrentStatus != MediaStatus.Playing) {
                return;
            }

            if (_mpvHandle == IntPtr.Zero) {
                return;
            }

            var widthBytes = GetUtf8Bytes (width.ToString (CultureInfo.InvariantCulture));
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("width"), MpvFormatString, ref widthBytes);

            var heightBytes = GetUtf8Bytes (height.ToString (CultureInfo.InvariantCulture));
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("height"), MpvFormatString, ref heightBytes);
        }

        /// <summary>
        /// Pause the current video.  If paused it will unpause.
        /// </summary>
        public void Pause ()
        {
            if (this.CurrentStatus != MediaStatus.Playing && this.CurrentStatus != MediaStatus.Paused) {
                return;
            }

            try {
                if (_mpvHandle == IntPtr.Zero) {
                    return;
                }

                var bytes = GetUtf8Bytes ("yes");
                _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("pause"), MpvFormatString, ref bytes);

            } catch (Exception ex) {
                Logging.Instance.WriteLine (ex);
                return;
            }

            if (this.CurrentStatus == MediaStatus.Paused) {
                this.CurrentStatus = MediaStatus.Playing;
            } else {
                this.CurrentStatus = MediaStatus.Paused;
            }
        }

        /// <summary>
        /// Stop the current video.
        /// </summary>
        public void Stop ()
        {
            if (this.CurrentStatus != MediaStatus.Stopped) {
                Pause ();
                Seek (0, LibMPlayerCommon.Seek.Absolute);
            }
            this.CurrentStatus = MediaStatus.Stopped;
        }


        /// <summary>
        /// Retrieves the number of seconds of the current playing video.
        /// </summary>
        public int CurrentPlayingFileLength ()
        {
            return this._totalTime;
        }
        // Sets in motions events to set this._totalTime.  Is called as soon as the video starts.
        private void LoadCurrentPlayingFileLength ()
        {
            var lpBuffer = IntPtr.Zero;
            _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes ("duration"), MpvFormatString, ref lpBuffer);

            var duration = Marshal.PtrToStringAnsi (lpBuffer);
            _mpvFree (lpBuffer);

            this._totalTime = Globals.IntParse (duration);
        }

        private void _currentPostionTimer_Elapsed (object sender, ElapsedEventArgs e)
        {
            if (this.CurrentStatus == MediaStatus.Playing) {
                var lpBuffer = IntPtr.Zero;
                _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes ("time-pos"), MpvFormatString, ref lpBuffer);

                var pos = Marshal.PtrToStringAnsi (lpBuffer);
                _mpvFree (lpBuffer);

                this._currentPosition = Globals.FloatParse (pos);

                this.CurrentPosition?.Invoke (this, new MplayerEvent (this._currentPosition));
            }
        }


        /// <summary>
        /// Get if the video is full is screen or not.  Set video to play in fullscreen.
        /// </summary>
        public bool FullScreen {
            get { return _fullscreen; }
            set {
                _fullscreen = value;

                var bytes = _fullscreen == true ? GetUtf8Bytes ("yes") : GetUtf8Bytes ("no");
                _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("fullscreen"), MpvFormatString, ref bytes);
            }
        }

        /// <summary>
        /// Toggle Fullscreen.
        /// </summary>
        public void ToggleFullScreen ()
        {
            if (this.MplayerRunning) {
                var lpBuffer = IntPtr.Zero;
                _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes ("fullscreen"), MpvFormatString, ref lpBuffer);

                var isFullScreen = Marshal.PtrToStringAnsi (lpBuffer);
                _mpvFree (lpBuffer);

                if (isFullScreen == "yes") {
                    var bytes = GetUtf8Bytes ("no");
                    _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("fullscreen"), MpvFormatString, ref bytes);
                } else {
                    var bytes = GetUtf8Bytes ("yes");
                    _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("fullscreen"), MpvFormatString, ref bytes);
                }

            }
        }

        /// <summary>
        /// Toggle Mute.  
        /// </summary>
        public void Mute ()
        {
            var lpBuffer = IntPtr.Zero;
            _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes ("ao-mute"), MpvFormatString, ref lpBuffer);

            var isMuted = Marshal.PtrToStringAnsi (lpBuffer);
            _mpvFree (lpBuffer);

            if (isMuted == "yes") {
                var bytes = GetUtf8Bytes ("no");
                _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("ao-mute"), MpvFormatString, ref bytes);
            } else {
                var bytes = GetUtf8Bytes ("yes");
                _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("ao-mute"), MpvFormatString, ref bytes);
            }

        }


        /// <summary>
        /// Accepts a volume value of 0 - 100.
        /// </summary>
        /// <param name="volume"></param>
        public void Volume (int volume)
        {
            Debug.Assert (volume >= 0 && volume <= 100);


            var bytes = GetUtf8Bytes (volume.ToString ());
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("ao-volume"), MpvFormatString, ref bytes);

        }

        public void SwitchAudioTrack (int track)
        {
            var bytes = GetUtf8Bytes (track.ToString ());
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("audio-reload"), MpvFormatString, ref bytes);
        }

        public void SwitchSubtitle (int sub)
        {
            var bytes = GetUtf8Bytes (sub.ToString ());
            _mpvSetProperty (_mpvHandle, GetUtf8Bytes ("sub-reload"), MpvFormatString, ref bytes);
        }

    }
}
