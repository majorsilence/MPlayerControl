using System;

namespace LibMPlayerCommon
{
    public class MPlayerControlException : Exception
    {

        public MPlayerControlException (string msg)
            : base (msg)
        {
        }

        public MPlayerControlException (string msg, Exception ex)
            : base (msg, ex)
        {
        }
            
    }
}

