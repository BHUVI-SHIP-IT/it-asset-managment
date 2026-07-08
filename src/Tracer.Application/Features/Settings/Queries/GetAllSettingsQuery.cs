using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Settings.DTOs;

namespace Tracer.Application.Features.Settings.Queries;

/// <summary>
/// Returns all tenant settings for the current user's company (M6, Doc 5 §settings).
/// </summary>
public record GetAllSettingsQuery : IRequest<List<TenantSettingDto>>;

public class GetAllSettingsQueryHandler : IRequestHandler<GetAllSettingsQuery, List<TenantSettingDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllSettingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<TenantSettingDto>> Handle(GetAllSettingsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;

        var query = _context.TenantSettings.AsQueryable();

        if (companyId.HasValue)
            query = query.Where(s => s.CompanyId == companyId.Value);

        return await query
            .OrderBy(s => s.Key)
            .Select(s => new TenantSettingDto
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                Key = s.Key,
                Value = s.Value,
                CreatedAtUtc = s.CreatedAtUtc,
                UpdatedAtUtc = s.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
