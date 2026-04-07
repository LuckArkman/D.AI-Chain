using System;

namespace Aethos.Domain.Exceptions;

public class InsufficientGasException : Exception
{
    public InsufficientGasException(string message) : base(message) { }
}
