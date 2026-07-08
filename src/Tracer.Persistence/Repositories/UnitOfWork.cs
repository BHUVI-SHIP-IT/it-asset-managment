using Tracer.Application.Common.Interfaces;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Repositories;

/// <summary>
/// Unit of Work wrapping TracerDbContext.SaveChangesAsync (Doc 10 §4.3).
/// Ensures atomicity across multiple repository operations within a single request.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TracerDbContext _context;

    public UnitOfWork(TracerDbContext context) => _context = context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
