using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tracer.Domain.Common;

namespace Tracer.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that auto-fills audit fields (CreatedAtUtc, CreatedBy, UpdatedAtUtc, UpdatedBy)
/// on every SaveChanges call (Doc 10 §3.3). Also handles soft-delete by converting Remove to Update.
/// Applies to both <see cref="AuditableEntity{Guid}"/> and <see cref="AuditableEntity{Int32}"/>.
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var utcNow = DateTime.UtcNow;
        var sub = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = Guid.TryParse(sub, out var id) ? id : (Guid?)null;

        foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity<Guid>>())
            ApplyAudit(entry, utcNow, userId);

        foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity<int>>())
            ApplyAudit(entry, utcNow, userId);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyAudit<TId>(EntityEntry<AuditableEntity<TId>> entry, DateTime utcNow, Guid? userId)
        where TId : notnull
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
}
