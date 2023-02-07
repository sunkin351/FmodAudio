using System;

namespace FmodAudio;

public class FmodException : Exception
{
    public Result? Result { get; } = null;

    public FmodException(string message) : base(message)
    {
    }

    public FmodException(string message, Exception inner) : base(message, inner)
    {
    }
    
    public FmodException(Result ErrorResult) : base()
    {
        Result = ErrorResult;
    }

    public FmodException(Result ErrorResult, string message) : base(message)
    {
        Result = ErrorResult;
    }

    public FmodException(Result ErrorResult, string message, Exception InnerException) : base(message, InnerException)
    {
        Result = ErrorResult;
    }
}
