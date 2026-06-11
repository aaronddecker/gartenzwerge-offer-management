namespace Gartenzwerge.Application.Common.Exceptions;

/// <summary>
/// Represents an application-level error when the current resource state conflicts with the requested operation.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}