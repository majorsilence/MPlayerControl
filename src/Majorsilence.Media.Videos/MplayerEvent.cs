/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

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

namespace Majorsilence.Media.Videos;

/// <summary>
///     Delegatefor use with mplayer control events.  Uses MplayerEvent.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void MplayerEventHandler(object sender, MplayerEvent e);

/// <summary>
///     Event class that is used with the mplayer control.  Can send String or Integer messages.
/// </summary>
public class MplayerEvent : EventArgs
{
    public MplayerEvent(string m)
    {
        Message = m;
        Value = 0;
    }

    public MplayerEvent(float v)
    {
        Message = "";
        Value = v;
    }

    /// <summary>
    ///     Event Message.
    /// </summary>
    public string Message { get; }

    public float Value { get; }
}