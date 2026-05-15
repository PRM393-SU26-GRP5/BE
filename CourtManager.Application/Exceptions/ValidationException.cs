namespace CourtManager.Application.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
