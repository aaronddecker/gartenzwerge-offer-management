namespace Gartenzwerge.Application.Common.Exceptions;

/// <summary>
/// This exception is thrown when a user attempts to access a resource or perform an action for which they do not have the necessary permissions or authentication.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}