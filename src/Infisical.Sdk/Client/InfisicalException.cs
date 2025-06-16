using System;
using System.IO;

namespace Infisical.Sdk
{
  public class InfisicalException : Exception
  {
    public InfisicalException(string message) : base(message)
    {
    }

    public InfisicalException(IOException cause) : base(cause?.Message, cause)
    {
    }

    // Optional: Add additional constructors following standard .NET exception patterns
    public InfisicalException() : base()
    {
    }

    public InfisicalException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}