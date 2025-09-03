using System;

namespace UcarMobileApi.Core.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
