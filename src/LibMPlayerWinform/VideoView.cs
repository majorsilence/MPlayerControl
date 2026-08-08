/*

Copyright 2026 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerWinform.

LibMPlayerWinform is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

LibMPlayerWinform is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using Majorsilence.Forms;
using Majorsilence.Media.Videos;
using SkiaSharp;

namespace LibMPlayerWinform
{
    /// <summary>
    ///     Displays mpv's video inside the form, by painting the frames mpv renders into memory.
    /// </summary>
    /// <remarks>
    ///     This is the alternative to handing mpv a native window id. Majorsilence.Forms composites
    ///     every control into one drawn surface, so a control has no window of its own to give mpv --
    ///     <c>Control.Handle</c> is always zero -- and mpv would open a separate window instead of
    ///     embedding. Here mpv writes each frame into a bitmap this control owns
    ///     (<see cref="MpvSoftwareRenderer" />), and the control paints it like any other content, so
    ///     the video really is inside the form and moves, resizes and clips with it.
    /// </remarks>
    public class VideoView : Control
    {
        private readonly object _sync = new object();
        private SKBitmap _frame;
        private MpvSoftwareRenderer _renderer;
        private Majorsilence.Forms.Timer _repaintTimer;

        public VideoView()
        {
            // mpv's update callback arrives on one of its own threads and can fire far more often
            // than there is any point repainting, so frames are pulled on a UI-thread timer instead.
            // ~60fps is past the point of visible improvement for playback.
            _repaintTimer = new Majorsilence.Forms.Timer { Interval = 16 };
            _repaintTimer.Tick += (s, e) => PullFrame();
        }

        /// <summary>
        ///     The player to display. Pass null to detach. A player that embeds through a real window
        ///     id has no renderer, in which case this control simply stays blank and stays out of the
        ///     way.
        /// </summary>
        public void SetPlayer(Majorsilence.Media.Videos.Player player)
        {
            lock (_sync)
            {
                _renderer = (player as MpvPlayer)?.Renderer;
            }

            if (_renderer == null)
            {
                _repaintTimer.Enabled = false;
                return;
            }

            _repaintTimer.Enabled = true;
        }

        private void PullFrame()
        {
            MpvSoftwareRenderer renderer;
            lock (_sync)
            {
                renderer = _renderer;
            }

            if (renderer == null)
            {
                return;
            }

            var width = ScaledSize.Width;
            var height = ScaledSize.Height;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            lock (_sync)
            {
                if (_frame == null || _frame.Width != width || _frame.Height != height)
                {
                    _frame?.Dispose();
                    // Bgra8888/Premul matches the "bgra" mpv writes, so the frame can be drawn as-is.
                    _frame = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
                }

                if (!renderer.RenderTo(_frame.GetPixels(), _frame.Width, _frame.Height, _frame.RowBytes))
                {
                    return;
                }
            }

            Invalidate();
        }

        /// <inheritdoc />
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (_sync)
            {
                if (_frame == null)
                {
                    return;
                }

                e.Canvas.DrawBitmap(_frame, new SKRect(0, 0, ScaledSize.Width, ScaledSize.Height));
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repaintTimer.Enabled = false;
                _repaintTimer.Dispose();

                lock (_sync)
                {
                    _frame?.Dispose();
                    _frame = null;
                    _renderer = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
