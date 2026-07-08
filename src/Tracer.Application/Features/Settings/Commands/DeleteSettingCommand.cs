using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Settings.Commands;

/// <summary>
/// Soft-deletes a tenant setting by key within the current user's company (M6).
/// Returns <c>false</c> when the key does not exist (idempotent).
/// </summary>
public record DeleteSettingCommand(string Key) : IRequest<bool>;

public class DeleteSettingCommandHandler : IRequestHandler<DeleteSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteSettingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteSettingCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;

        var entity = await _context.TenantSettings
            .FirstOrDefaultAsync(
                s => s.Key == request.Key && (!companyId.HasValue || s.CompanyId == companyId.Value),
                cancellationToken);

        if (entity is null) return false;

        // Interceptor converts Remove to soft-delete (Doc 4 §1.2).
        _context.TenantSettings.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
