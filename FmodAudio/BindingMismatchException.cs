using System;

namespace FmodAudio;

public sealed class BindingMismatchException : Exception
{
    // TODO: Add Message
    public BindingMismatchException()
    {
    }

    public BindingMismatchException(string message) : base(message)
    {
    }
}
