using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.CustomFields.DTOs;

namespace Tracer.Application.Features.CustomFields.Queries;

/// <summary>
/// Returns all custom field values for a specific entity instance (e.g., an Asset Guid).
/// Joins to the field definition to include name and type metadata (M6, Doc 4 §3.21).
/// </summary>
public record GetCustomFieldsByEntityQuery(Guid EntityId) : IRequest<List<CustomFieldValueDto>>;

public class GetCustomFieldsByEntityQueryHandler : IRequestHandler<GetCustomFieldsByEntityQuery, List<CustomFieldValueDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomFieldsByEntityQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CustomFieldValueDto>> Handle(GetCustomFieldsByEntityQuery request, CancellationToken cancellationToken)
    {
        return await _context.CustomFieldValues
            .Where(v => v.EntityId == request.EntityId)
            .Select(v => new CustomFieldValueDto
            {
                Id = v.Id,
                CustomFieldId = v.CustomFieldId,
                FieldName = v.CustomField!.Name,
                FieldType = v.CustomField!.FieldType.ToString(),
                EntityId = v.EntityId,
                Value = v.Value
            })
            .ToListAsync(cancellationToken);
    }
}
