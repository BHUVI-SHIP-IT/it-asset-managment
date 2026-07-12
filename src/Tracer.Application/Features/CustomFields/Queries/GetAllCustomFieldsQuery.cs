using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.CustomFields.DTOs;

namespace Tracer.Application.Features.CustomFields.Queries;

/// <summary>
/// Returns all custom field definitions for the current tenant (M6, Doc 4 §3.20).
/// </summary>
public record GetAllCustomFieldsQuery : IRequest<List<CustomFieldDto>>;

public class GetAllCustomFieldsQueryHandler : IRequestHandler<GetAllCustomFieldsQuery, List<CustomFieldDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllCustomFieldsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<CustomFieldDto>> Handle(GetAllCustomFieldsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;

        var query = _context.CustomFields.AsQueryable();

        if (companyId.HasValue)
            query = query.Where(f => f.CompanyId == companyId.Value);

        return await query
            .OrderBy(f => f.Name)
            .Select(f => new CustomFieldDto
            {
                Id = f.Id,
                CompanyId = f.CompanyId,
                Name = f.Name,
                FieldType = f.FieldType.ToString().ToLowerInvariant(),
                IsRequired = f.IsRequired,
                Options = f.Options,
                CreatedAtUtc = f.CreatedAtUtc,
                UpdatedAtUtc = f.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
