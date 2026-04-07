using System;

namespace Aethos.Domain.Exceptions;

public class DivergenceException : Exception
{
    public DivergenceException(string message) : base(message) { }
    
    public DivergenceException(string message, string expectedHash, string computedHash) 
        : base($"{message} - Esperado: {expectedHash}, Computado: {computedHash}") { }
}
