using System;

namespace Majorsilence.Media.Videos.Exceptions
{
    public class MPlayerControlException : Exception
    {

        public int ErrorCode = 0;

        public MPlayerControlException (string msg)
            : base (msg)
        {
        }

        public MPlayerControlException (string msg, int errorCode)
            : base (msg)
        {
            ErrorCode = errorCode;
        }

        public MPlayerControlException (string msg, Exception ex)
            : base (msg, ex)
        {
        }

        public MPlayerControlException (string msg, Exception ex, int errorCode)
            : base (msg, ex)
        {
            this.ErrorCode = errorCode;
        }
    }
}

