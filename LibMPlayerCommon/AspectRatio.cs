/*

Copyright 2010 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibMPlayerCommon.

LibMPlayerCommon is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

LibMPlayerCommon is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Text;

namespace LibMPlayerCommon
{

    /// <summary>
    /// This class is used to hold different video aspect ratio values.
    /// </summary>
    public class ScreenAspectRatio
    {
        /// <summary>
        /// The float value of 4/3 aspect ratio.
        /// </summary>
        public static float FourThree
        {
            get { return 4.0f / 3.0f; }
        }

        /// <summary>
        /// The float value of 16/9 aspect ratio.
        /// </summary>
        public static float SixteenNine
        {
            get { return 16.0f / 9.0f; }
        }
    }


}

