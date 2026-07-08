using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Updates the name, type, and options of an existing custom field definition (M6).
/// Returns <c>false</c> when the field does not exist.
/// </summary>
public record UpdateCustomFieldCommand(
    Guid Id,
    string Name,
    CustomFieldType FieldType,
    bool IsRequired,
    string? Options = null) : IRequest<bool>;

public class UpdateCustomFieldCommandHandler : IRequestHandler<UpdateCustomFieldCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateCustomFieldCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateCustomFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _context.CustomFields
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (field is null) return false;

        field.Update(request.Name, request.FieldType, request.IsRequired, request.Options);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
