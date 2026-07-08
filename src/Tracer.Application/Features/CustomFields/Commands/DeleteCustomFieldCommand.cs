using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Soft-deletes a custom field definition (Doc 4 §1.2). Cascades to CustomFieldValues via EF config.
/// Returns <c>false</c> when the field does not exist (idempotent).
/// </summary>
public record DeleteCustomFieldCommand(Guid Id) : IRequest<bool>;

public class DeleteCustomFieldCommandHandler : IRequestHandler<DeleteCustomFieldCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomFieldCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteCustomFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _context.CustomFields
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (field is null) return false;

        // Interceptor converts Remove to soft-delete (Doc 4 §1.2).
        _context.CustomFields.Remove(field);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
