namespace DddBookstore.Improved.Domain;

/// <summary>
/// Thrown when a business rule of the domain is violated.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
