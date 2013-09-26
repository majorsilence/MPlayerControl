using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMPlayerCommon
{
    /// <summary>
    /// The seek type that is used when seeking a new position in the video stream.
    /// </summary>
    public enum Seek
    {
        Relative = 0,
        Percentage = 1,
        Absolute = 2
    }
}
