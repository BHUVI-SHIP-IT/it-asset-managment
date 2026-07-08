using FluentValidation.Results;

namespace Tracer.Application.Common.Exceptions;

/// <summary>
/// Aggregates FluentValidation failures into a per-field dictionary that the API maps to an
/// RFC 7807 validation problem+json response (Doc 5 §1.3).
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException() : base("One or more validation failures have occurred.")
        => Errors = new Dictionary<string, string[]>();

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        => Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

    public IDictionary<string, string[]> Errors { get; }
}
