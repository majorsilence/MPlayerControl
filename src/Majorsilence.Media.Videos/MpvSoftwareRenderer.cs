/*

Copyright 2026 (C) Peter Gill <peter@majorsilence.com>

This file is part of Majorsilence.Media.Videos.

Majorsilence.Media.Videos is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

Majorsilence.Media.Videos is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Runtime.InteropServices;

namespace Majorsilence.Media.Videos;

/// <summary>
///     Renders mpv's video output into a caller-supplied pixel buffer, instead of handing mpv a
///     native window to draw into itself.
/// </summary>
/// <remarks>
///     mpv normally embeds by being given a platform window id (its <c>wid</c> option). A UI toolkit
///     that composites everything itself has no such id to give — its "controls" are regions of one
///     drawn surface, not OS windows — so <c>wid</c> embedding cannot work there and mpv opens a
///     window of its own instead.
///
///     libmpv's render API is the supported way out: with the software renderer, mpv writes each
///     frame into memory the caller owns, and the caller draws it wherever it likes. That makes the
///     video just another thing painted into the UI's own surface, which is exactly what a
///     composited toolkit needs, and it needs no windowing-system integration at all.
/// </remarks>
public sealed class MpvSoftwareRenderer : IDisposable
{
    // From libmpv's render.h. Only the software-rendering subset is declared; the OpenGL and
    // platform-display parameters have no use here.
    private const int MpvRenderParamInvalid = 0;
    private const int MpvRenderParamApiType = 1;
    private const int MpvRenderParamSwSize = 17;
    private const int MpvRenderParamSwFormat = 18;
    private const int MpvRenderParamSwStride = 19;
    private const int MpvRenderParamSwPointer = 20;

    // mpv writes premultiplied BGRA in memory order on little-endian, which is what Skia's
    // default 32-bit surface expects, so frames can be blitted with no conversion.
    private const string SwFormat = "bgra";

    private readonly object _sync = new();
    private readonly MpvRenderContextRender _render;
    private readonly MpvRenderContextFree _free;
    private readonly MpvRenderContextSetUpdateCallback _setUpdateCallback;

    // Held for the lifetime of the context: mpv keeps the pointer and calls it from its own
    // threads, so letting the delegate be collected would leave it calling freed memory.
    private readonly MpvRenderUpdateFn _updateCallback;

    private IntPtr _context;
    private bool _disposed;

    private MpvSoftwareRenderer(IntPtr context, MpvRenderContextRender render, MpvRenderContextFree free,
        MpvRenderContextSetUpdateCallback setUpdateCallback)
    {
        _context = context;
        _render = render;
        _free = free;
        _setUpdateCallback = setUpdateCallback;

        _updateCallback = _ => FrameAvailable?.Invoke(this, EventArgs.Empty);
        _setUpdateCallback(_context, _updateCallback, IntPtr.Zero);
    }

    /// <summary>
    ///     Raised by mpv when a new frame is ready to be rendered. Raised on an mpv thread, so a UI
    ///     caller has to marshal to its own thread before painting.
    /// </summary>
    public event EventHandler FrameAvailable;

    /// <summary>
    ///     Builds a software render context for <paramref name="mpv" />, or returns null when this
    ///     build of libmpv has no render API (the caller then falls back to whatever it did before).
    ///     The mpv instance must already have been initialized with <c>vo=libmpv</c>.
    /// </summary>
    internal static MpvSoftwareRenderer TryCreate(Mpv mpv)
    {
        if (mpv == null || mpv.Handle == IntPtr.Zero)
        {
            return null;
        }

        var create = mpv.ResolveExport(typeof(MpvRenderContextCreate), "mpv_render_context_create") as MpvRenderContextCreate;
        var render = mpv.ResolveExport(typeof(MpvRenderContextRender), "mpv_render_context_render") as MpvRenderContextRender;
        var free = mpv.ResolveExport(typeof(MpvRenderContextFree), "mpv_render_context_free") as MpvRenderContextFree;
        var setUpdate = mpv.ResolveExport(typeof(MpvRenderContextSetUpdateCallback), "mpv_render_context_set_update_callback")
            as MpvRenderContextSetUpdateCallback;

        if (create == null || render == null || free == null || setUpdate == null)
        {
            return null;
        }

        var apiType = Marshal.StringToHGlobalAnsi("sw");
        try
        {
            var parameters = new[]
            {
                new MpvRenderParam { Type = MpvRenderParamApiType, Data = apiType },
                new MpvRenderParam { Type = MpvRenderParamInvalid, Data = IntPtr.Zero }
            };

            if (create(out var context, mpv.Handle, parameters) < 0 || context == IntPtr.Zero)
            {
                return null;
            }

            return new MpvSoftwareRenderer(context, render, free, setUpdate);
        }
        finally
        {
            Marshal.FreeHGlobal(apiType);
        }
    }

    /// <summary>
    ///     Draws the current frame into <paramref name="buffer" />, which must hold at least
    ///     <paramref name="stride" /> * <paramref name="height" /> bytes of BGRA pixels. Returns
    ///     false if the frame could not be rendered, in which case the buffer is left alone.
    /// </summary>
    public bool RenderTo(IntPtr buffer, int width, int height, int stride)
    {
        if (buffer == IntPtr.Zero || width <= 0 || height <= 0 || stride < width * 4)
        {
            return false;
        }

        lock (_sync)
        {
            if (_disposed || _context == IntPtr.Zero)
            {
                return false;
            }

            // Every pointer handed to mpv has to stay alive for the duration of the call, so the
            // size, stride and format all get pinned/allocated here rather than passed inline.
            var size = Marshal.AllocHGlobal(sizeof(int) * 2);
            var stridePointer = Marshal.AllocHGlobal(IntPtr.Size);
            var format = Marshal.StringToHGlobalAnsi(SwFormat);

            try
            {
                Marshal.WriteInt32(size, 0, width);
                Marshal.WriteInt32(size, sizeof(int), height);
                // MPV_RENDER_PARAM_SW_STRIDE is a size_t*, so it is pointer-sized, not int-sized.
                Marshal.WriteIntPtr(stridePointer, new IntPtr(stride));

                var parameters = new[]
                {
                    new MpvRenderParam { Type = MpvRenderParamSwSize, Data = size },
                    new MpvRenderParam { Type = MpvRenderParamSwFormat, Data = format },
                    new MpvRenderParam { Type = MpvRenderParamSwStride, Data = stridePointer },
                    new MpvRenderParam { Type = MpvRenderParamSwPointer, Data = buffer },
                    new MpvRenderParam { Type = MpvRenderParamInvalid, Data = IntPtr.Zero }
                };

                return _render(_context, parameters) >= 0;
            }
            finally
            {
                Marshal.FreeHGlobal(size);
                Marshal.FreeHGlobal(stridePointer);
                Marshal.FreeHGlobal(format);
            }
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_context == IntPtr.Zero)
            {
                return;
            }

            // Detach the callback before tearing the context down so mpv cannot call back into a
            // half-freed object from one of its own threads.
            try { _setUpdateCallback(_context, null, IntPtr.Zero); } catch { /* already gone */ }

            _free(_context);
            _context = IntPtr.Zero;
        }
    }

    // mpv_render_param is { int type; void *data; } -- the int is followed by padding to the
    // pointer's alignment, which is what the default sequential layout produces.
    [StructLayout(LayoutKind.Sequential)]
    private struct MpvRenderParam
    {
        public int Type;
        public IntPtr Data;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvRenderContextCreate(out IntPtr context, IntPtr mpvHandle, MpvRenderParam[] parameters);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int MpvRenderContextRender(IntPtr context, MpvRenderParam[] parameters);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void MpvRenderContextFree(IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void MpvRenderUpdateFn(IntPtr callbackContext);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void MpvRenderContextSetUpdateCallback(IntPtr context, MpvRenderUpdateFn callback, IntPtr callbackContext);
}
