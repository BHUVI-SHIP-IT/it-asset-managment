using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Sets (creates or updates) a custom field value for a specific entity instance (M6, Doc 4 §3.21).
/// Upsert semantics: creates if absent, updates Value if already present.
/// </summary>
public record SetCustomFieldValueCommand(Guid CustomFieldId, Guid EntityId, string? Value) : IRequest<Guid>;

public class SetCustomFieldValueCommandHandler : IRequestHandler<SetCustomFieldValueCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SetCustomFieldValueCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(SetCustomFieldValueCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.CustomFieldValues
            .FirstOrDefaultAsync(
                v => v.CustomFieldId == request.CustomFieldId && v.EntityId == request.EntityId,
                cancellationToken);

        if (existing is not null)
        {
            existing.SetValue(request.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var value = CustomFieldValue.Create(request.CustomFieldId, request.EntityId, request.Value);
        _context.CustomFieldValues.Add(value);
        await _context.SaveChangesAsync(cancellationToken);
        return value.Id;
    }
}
