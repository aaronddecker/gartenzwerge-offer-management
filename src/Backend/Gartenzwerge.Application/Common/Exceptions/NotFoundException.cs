using Gartenzwerge.Application.Common.Exceptions;

namespace Gartenzwerge.Application.Common.Exceptions;
/// <summary>
/// Represents an application-level error when a requested resource could not be found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}