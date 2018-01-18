using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LibMPlayerCommon
{
    public class Mpv : IDisposable
    {
        public Mpv (string _libMpvPath)
        {
            this._libMpvPath = _libMpvPath;
        }

        ~Mpv ()
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

        private const int MpvFormatString = (int)MpvFormat.MPV_FORMAT_STRING;
        private IntPtr _libMpvDll;
        private IntPtr _mpvHandle;
        private string _libMpvPath;

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

     
        public int SetProperty (string name, MpvFormat format, string data)
        {
            if (_mpvHandle == IntPtr.Zero) {
                return -1;
            }

            var bytes = GetUtf8Bytes (data);
            return _mpvSetProperty (_mpvHandle, GetUtf8Bytes (name), (int)format, ref bytes);

        }

        public string GetProperty (string name)
        {
            var lpBuffer = IntPtr.Zero;
            _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes (name), (int)MpvFormat.MPV_FORMAT_STRING, ref lpBuffer);
            var data = Marshal.PtrToStringAuto (lpBuffer);
            _mpvFree (lpBuffer);
            return data;
        }

        public float GetPropertyFloat (string name)
        {
            var lpBuffer = IntPtr.Zero;
            _mpvGetPropertyString (_mpvHandle, GetUtf8Bytes (name), (int)MpvFormat.MPV_FORMAT_STRING, ref lpBuffer);
            var data = Marshal.PtrToStringAuto (lpBuffer);
            _mpvFree (lpBuffer);
            return Globals.FloatParse (data);
        }

        public int SetOption (string name, MpvFormat format, long data)
        {
            return _mpvSetOption (_mpvHandle, GetUtf8Bytes (name), (int)format, ref data);
        }

        public void Initialize ()
        {
            System.Environment.SetEnvironmentVariable ("LC_NUMERIC", "C");
            if (_mpvHandle != IntPtr.Zero)
                _mpvTerminateDestroy (_mpvHandle);

            LoadMpvDynamic ();
            if (_libMpvDll == IntPtr.Zero) {
                Console.WriteLine ("libmpvdll null");
                return;
            }

            _mpvHandle = _mpvCreate.Invoke ();
            System.Threading.Thread.Sleep (1000);
            if (_mpvHandle == IntPtr.Zero) {
                Console.WriteLine ("mpvhandle null");
                return;
            }

            _mpvInitialize.Invoke (_mpvHandle);

            _mpvSetOptionString (_mpvHandle, GetUtf8Bytes ("keep-open"), GetUtf8Bytes ("always"));

        }

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

        public void DoMpvCommand (params string[] args)
        {
            // https://github.com/mpv-player/mpv/blob/master/DOCS/man/input.rst

            if (_mpvHandle == IntPtr.Zero) {
                return;
            }

            IntPtr[] byteArrayPointers;
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel (args, out byteArrayPointers);
            _mpvCommand (_mpvHandle, mainPtr);
            foreach (var ptr in byteArrayPointers) {
                Marshal.FreeHGlobal (ptr);
            }
            Marshal.FreeHGlobal (mainPtr);
        }

        private static IntPtr AllocateUtf8IntPtrArrayWithSentinel (string[] arr, out IntPtr[] byteArrayPointers)
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
    }
}

