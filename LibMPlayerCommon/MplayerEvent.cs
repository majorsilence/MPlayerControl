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

namespace LibMPlayerCommon
{
    /// <summary>
    /// Delegatefor use with mplayer control events.  Uses MplayerEvent.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MplayerEventHandler(object sender, MplayerEvent e);

    /// <summary>
    /// Event class that is used with the mplayer control.  Can send String or Integer messages.
    /// </summary>
    public class MplayerEvent : System.EventArgs
    {
         private string _msg;
         private int _value;

        public MplayerEvent(string m)
        {
            _msg = m;
            _value = 0;
        }

        public MplayerEvent(int v)
        {
            _msg = "";
            _value = v;
        }

        /// <summary>
        /// Event Message.
        /// </summary>
        public string Message
        {
            get { return _msg; }
        }

        public int Value
        {
            get { return _value; }
        }
    }

}
