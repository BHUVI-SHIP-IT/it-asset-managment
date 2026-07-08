namespace Tracer.Application.Common.Interfaces;

/// <summary>Atomic commit boundary across repositories (Doc 10 §4.3).</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
