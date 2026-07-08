using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Common;

namespace Tracer.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that auto-fills audit fields (CreatedAtUtc, CreatedBy, UpdatedAtUtc, UpdatedBy)
/// on every SaveChanges call (Doc 10 §3.3). Also handles soft-delete by converting Remove to Update.
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditableEntityInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var utcNow = DateTime.UtcNow;
        var userId = _currentUser.UserId;

        foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted:
                    // Convert hard-delete to soft-delete (Doc 4 §1.2).
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = utcNow;
                    entry.Entity.UpdatedAtUtc = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
