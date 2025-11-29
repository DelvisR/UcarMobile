using System;

namespace UcarMobileApi.Core.Exceptions;

public class NotFoundException(string message) : Exception(message) { }

public class BusinessException : Exception
{
    public BusinessException()
        : base("A business rule was violated.") { }

    public BusinessException(string message)
        : base(message) { }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException) { }
}

