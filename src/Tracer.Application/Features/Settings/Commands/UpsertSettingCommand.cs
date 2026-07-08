using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.SettingAggregate;

namespace Tracer.Application.Features.Settings.Commands;

/// <summary>
/// Creates or updates a tenant setting key/value pair (upsert semantics).
/// Scoped to the current user's CompanyId (M6, Doc 5 §settings).
/// </summary>
public record UpsertSettingCommand(string Key, string? Value) : IRequest<Guid>;

public class UpsertSettingCommandHandler : IRequestHandler<UpsertSettingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpsertSettingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(UpsertSettingCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new InvalidOperationException("CompanyId is required to upsert settings.");

        var existing = await _context.TenantSettings
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.Key == request.Key, cancellationToken);

        if (existing is not null)
        {
            existing.UpdateValue(request.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var setting = TenantSetting.Create(companyId, request.Key, request.Value);
        _context.TenantSettings.Add(setting);
        await _context.SaveChangesAsync(cancellationToken);
        return setting.Id;
    }
}
