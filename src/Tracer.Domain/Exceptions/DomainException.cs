namespace Tracer.Domain.Exceptions;

/// <summary>
/// Base class for domain rule violations (Doc 10 §3.1). Surfaced as 422 Unprocessable Entity
/// by the global exception handler (Doc 3 §4.2, Doc 5 §1.3).
/// </summary>
public abstract class DomainException : Exception
{
    public abstract string Code { get; }

    protected DomainException(string message) : base(message) { }
}
