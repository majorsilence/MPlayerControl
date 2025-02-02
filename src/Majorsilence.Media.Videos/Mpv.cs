﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Majorsilence.Media.Videos.Exceptions;

namespace Majorsilence.Media.Videos;

public class Mpv : IDisposable
{
    private const int MpvFormatString = (int)MpvFormat.MPV_FORMAT_STRING;
    private IntPtr _libMpvDll;
    private readonly string _libMpvPath;

    private MpvCommand _mpvCommand;
    private MpvCreate _mpvCreate;
    private MpvFree _mpvFree;
    private MpvGetPropertyString _mpvGetPropertyString;
    private readonly IntPtr _mpvHandle;
    private MpvInitialize _mpvInitialize;
    private MpvObserveProperty _mpvObserveProperty;
    private MpvSetOption _mpvSetOption;
    private MpvSetOptionString _mpvSetOptionString;
    private MpvSetProperty _mpvSetProperty;
    private MpvTerminateDestroy _mpvTerminateDestroy;
    private bool disposed;

    public Mpv(string libMpvPath)
    {
        _libMpvPath = libMpvPath;

        if (_mpvHandle != IntPtr.Zero)
            _mpvTerminateDestroy(_mpvHandle);

        LoadMpvDynamic();
        if (_libMpvDll == IntPtr.Zero)
        {
            Console.WriteLine("libmpvdll null");
            return;
        }

        _mpvHandle = _mpvCreate.Invoke();
        Thread.Sleep(1000);
        if (_mpvHandle == IntPtr.Zero)
        {
            Console.WriteLine("mpvhandle null");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Initialize()
    {
        _mpvInitialize.Invoke(_mpvHandle);
        _mpvSetOptionString(_mpvHandle, GetUtf8Bytes("keep-open"), GetUtf8Bytes("always"));
    }

    ~Mpv()
    {
        if (_mpvHandle != IntPtr.Zero) _mpvTerminateDestroy(_mpvHandle);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
            if (_mpvHandle != IntPtr.Zero)
                _mpvTerminateDestroy(_mpvHandle);

        disposed = true;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
    internal static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    public int SetProperty(string name, MpvFormat format, string data)
    {
        if (_mpvHandle == IntPtr.Zero) return -1;

        var bytes = GetUtf8Bytes(data);
        return _mpvSetProperty(_mpvHandle, GetUtf8Bytes(name), (int)format, ref bytes);
    }

    public string GetProperty(string name)
    {
        string data;
        var returnValue = InternalGetProperty(name, out data);

        if (returnValue < 0)
        {
            var error = (MpvError)returnValue;
            throw new MPlayerControlException($"GetProperty ({name}) error {error.ToString()}", returnValue);
        }

        return data;
    }

    private int InternalGetProperty(string name, out string value)
    {
        var lpBuffer = IntPtr.Zero;
        var returnValue = _mpvGetPropertyString(_mpvHandle, GetUtf8Bytes(name), (int)MpvFormat.MPV_FORMAT_STRING, ref lpBuffer);
        var data = Marshal.PtrToStringUTF8(lpBuffer);
        _mpvFree(lpBuffer);

        value = data;
        return returnValue;
    }

    public bool TryGetProperty(string name, out string value)
    {
        value = "";
        try
        {
            string data;
            var returnValue = InternalGetProperty(name, out data);

            if (returnValue < 0) return false;

            value = data;
        }
        catch
        {
            return false;
        }

        return true;
    }

    public float GetPropertyFloat(string name)
    {
        var data = GetProperty(name);
        return Globals.FloatParse(data);
    }

    public bool TryGetPropertyFloat(string name, out float value)
    {
        value = 0f;
        try
        {
            string data;
            var returnValue = InternalGetProperty(name, out data);

            if (returnValue < 0) return false;

            value = Globals.FloatParse(data);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public int GetPropertyInt(string name)
    {
        var data = GetProperty(name);
        return (int)Globals.FloatParse(data);
    }

    public bool TryGetPropertyInt(string name, out int value)
    {
        value = 0;
        try
        {
            string data;
            var returnValue = InternalGetProperty(name, out data);

            if (returnValue < 0) return false;

            value = (int)Globals.FloatParse(data);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public int SetOption(string name, MpvFormat format, long data)
    {
        return _mpvSetOption(_mpvHandle, GetUtf8Bytes(name), (int)format, ref data);
    }

    public int SetOption(string name, MpvFormat format, string data)
    {
        return _mpvSetOptionString(_mpvHandle, GetUtf8Bytes(name), GetUtf8Bytes(data));
    }

    private object GetDllType(Type type, string name)
    {
        IntPtr address;
        var platform = PlatformCheck.RunningPlatform();
        if (platform == Platform.Windows)
            address = GetProcAddress(_libMpvDll, name);
        else if (platform == Platform.Linux)
            address = dlsym(_libMpvDll, name);
        else
            throw new NotImplementedException();

        if (address != IntPtr.Zero)
            return Marshal.GetDelegateForFunctionPointer(address, type);
        return null;
    }

    private void LoadMpvDynamic()
    {
        var platform = PlatformCheck.RunningPlatform();
        if (platform == Platform.Windows)
            _libMpvDll = LoadLibrary(_libMpvPath);
        else if (platform == Platform.Linux)
            _libMpvDll = dlopen(_libMpvPath, RTLD_NOW);
        else
            throw new NotImplementedException();

        _mpvCreate = (MpvCreate)GetDllType(typeof(MpvCreate), "mpv_create");
        _mpvInitialize = (MpvInitialize)GetDllType(typeof(MpvInitialize), "mpv_initialize");
        _mpvTerminateDestroy = (MpvTerminateDestroy)GetDllType(typeof(MpvTerminateDestroy), "mpv_terminate_destroy");
        _mpvCommand = (MpvCommand)GetDllType(typeof(MpvCommand), "mpv_command");
        _mpvSetOption = (MpvSetOption)GetDllType(typeof(MpvSetOption), "mpv_set_option");
        _mpvSetOptionString = (MpvSetOptionString)GetDllType(typeof(MpvSetOptionString), "mpv_set_option_string");
        _mpvGetPropertyString = (MpvGetPropertyString)GetDllType(typeof(MpvGetPropertyString), "mpv_get_property");
        _mpvSetProperty = (MpvSetProperty)GetDllType(typeof(MpvSetProperty), "mpv_set_property");
        _mpvFree = (MpvFree)GetDllType(typeof(MpvFree), "mpv_free");
        _mpvObserveProperty = (MpvObserveProperty)GetDllType(typeof(MpvObserveProperty), "mpv_observe_property");
    }

    public void DoMpvCommand(params string[] args)
    {
        if (_mpvHandle == IntPtr.Zero) return;

        IntPtr[] byteArrayPointers;
        var mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out byteArrayPointers);
        _mpvCommand(_mpvHandle, mainPtr);
        foreach (var ptr in byteArrayPointers) Marshal.FreeHGlobal(ptr);
        Marshal.FreeHGlobal(mainPtr);
    }

    private static IntPtr AllocateUtf8IntPtrArrayWithSentinel(string[] arr, out IntPtr[] byteArrayPointers)
    {
        var numberOfStrings = arr.Length + 1;
        byteArrayPointers = new IntPtr[numberOfStrings];
        var rootPointer = Marshal.AllocCoTaskMem(IntPtr.Size * numberOfStrings);
        for (var index = 0; index < arr.Length; index++)
        {
            var bytes = GetUtf8Bytes(arr[index]);
            var unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
            byteArrayPointers[index] = unmanagedPointer;
        }

        Marshal.Copy(byteArrayPointers, 0, rootPointer, numberOfStrings);
        return rootPointer;
    }

    private static byte[] GetUtf8Bytes(string s)
    {
        return Encoding.UTF8.GetBytes(s + "\0");
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr MpvCreate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvInitialize(IntPtr mpvHandle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvCommand(IntPtr mpvHandle, IntPtr strings);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvTerminateDestroy(IntPtr mpvHandle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvSetOption(IntPtr mpvHandle, byte[] name, int format, ref long data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvSetOptionString(IntPtr mpvHandle, byte[] name, byte[] value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvGetPropertyString(IntPtr mpvHandle, byte[] name, int format, ref IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvSetProperty(IntPtr mpvHandle, byte[] name, int format, ref byte[] data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void MpvFree(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvObserveProperty(IntPtr mpvHandle, ulong reply_userdata, [MarshalAs(UnmanagedType.LPStr)] string name, int format);

    #region Linux

    [DllImport("libdl.so")]
    protected static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so")]
    protected static extern IntPtr dlsym(IntPtr handle, string symbol);

    private const int RTLD_NOW = 2;

    #endregion
}

/// <summary>
///     https://github.com/mpv-player/mpv/blob/master/libmpv/client.h#L239
/// </summary>
internal enum MpvError
{
    /**
     * No error happened (used to signal successful operation).
     * Keep in mind that many API functions returning error codes can also
     * return positive values, which also indicate success. API users can
     * hardcode the fact that ">= 0" means success.
     */
    MPV_ERROR_SUCCESS = 0,

    /**
     * The event ringbuffer is full. This means the client is choked, and can't
     * receive any events. This can happen when too many asynchronous requests
     * have been made, but not answered. Probably never happens in practice,
     * unless the mpv core is frozen for some reason, and the client keeps
     * making asynchronous requests. (Bugs in the client API implementation
     * could also trigger this, e.g. if events become "lost".)
     */
    MPV_ERROR_EVENT_QUEUE_FULL = -1,

    /**
     * Memory allocation failed.
     */
    MPV_ERROR_NOMEM = -2,

    /**
     * The mpv core wasn't configured and initialized yet. See the notes in
     * mpv_create().
     */
    MPV_ERROR_UNINITIALIZED = -3,

    /**
     * Generic catch-all error if a parameter is set to an invalid or
     * unsupported value. This is used if there is no better error code.
     */
    MPV_ERROR_INVALID_PARAMETER = -4,

    /**
     * Trying to set an option that doesn't exist.
     */
    MPV_ERROR_OPTION_NOT_FOUND = -5,

    /**
     * Trying to set an option using an unsupported MPV_FORMAT.
     */
    MPV_ERROR_OPTION_FORMAT = -6,

    /**
     * Setting the option failed. Typically this happens if the provided option
     * value could not be parsed.
     */
    MPV_ERROR_OPTION_ERROR = -7,

    /**
     * The accessed property doesn't exist.
     */
    MPV_ERROR_PROPERTY_NOT_FOUND = -8,

    /**
     * Trying to set or get a property using an unsupported MPV_FORMAT.
     */
    MPV_ERROR_PROPERTY_FORMAT = -9,

    /**
     * The property exists, but is not available. This usually happens when the
     * associated subsystem is not active, e.g. querying audio parameters while
     * audio is disabled.
     */
    MPV_ERROR_PROPERTY_UNAVAILABLE = -10,

    /**
     * Error setting or getting a property.
     */
    MPV_ERROR_PROPERTY_ERROR = -11,

    /**
     * General error when running a command with mpv_command and similar.
     */
    MPV_ERROR_COMMAND = -12,

    /**
     * Generic error on loading (usually used with mpv_event_end_file.error).
     */
    MPV_ERROR_LOADING_FAILED = -13,

    /**
     * Initializing the audio output failed.
     */
    MPV_ERROR_AO_INIT_FAILED = -14,

    /**
     * Initializing the video output failed.
     */
    MPV_ERROR_VO_INIT_FAILED = -15,

    /**
     * There was no audio or video data to play. This also happens if the
     * file was recognized, but did not contain any audio or video streams,
     * or no streams were selected.
     */
    MPV_ERROR_NOTHING_TO_PLAY = -16,

    /**
     * When trying to load the file, the file format could not be determined,
     * or the file was too broken to open it.
     */
    MPV_ERROR_UNKNOWN_FORMAT = -17,

    /**
     * Generic error for signaling that certain system requirements are not
     * fulfilled.
     */
    MPV_ERROR_UNSUPPORTED = -18,

    /**
     * The API function which was called is a stub only.
     */
    MPV_ERROR_NOT_IMPLEMENTED = -19,

    /**
     * Unspecified error.
     */
    MPV_ERROR_GENERIC = -20
}