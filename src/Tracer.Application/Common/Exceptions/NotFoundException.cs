namespace Tracer.Application.Common.Exceptions;

/// <summary>Thrown when a requested aggregate does not exist or is outside RLS scope (Doc 2 §2.1.7 → 404).</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
