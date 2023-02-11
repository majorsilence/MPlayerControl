using System;

namespace Majorsilence.Media.Videos.Exceptions;

public class MPlayerControlException : Exception
{
    public int ErrorCode;

    public MPlayerControlException(string msg)
        : base(msg)
    {
    }

    public MPlayerControlException(string msg, int errorCode)
        : base(msg)
    {
        ErrorCode = errorCode;
    }

    public MPlayerControlException(string msg, Exception ex)
        : base(msg, ex)
    {
    }

    public MPlayerControlException(string msg, Exception ex, int errorCode)
        : base(msg, ex)
    {
        ErrorCode = errorCode;
    }
}