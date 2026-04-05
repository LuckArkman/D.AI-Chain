using System;

namespace Aethos.Domain.Exceptions;

public class Int128OverflowException : Exception
{
    public Int128OverflowException(string message) : base(message) { }
}
