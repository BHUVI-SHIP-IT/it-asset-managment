using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Application.Features.CustomFields.Commands;

/// <summary>
/// Creates a new custom field definition for the current tenant (M6, Doc 4 §3.20).
/// </summary>
public record CreateCustomFieldCommand(
    string Name,
    CustomFieldType FieldType,
    bool IsRequired,
    string? Options = null) : IRequest<Guid>;

public class CreateCustomFieldCommandHandler : IRequestHandler<CreateCustomFieldCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCustomFieldCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateCustomFieldCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new InvalidOperationException("CompanyId is required to create custom fields.");

        var field = CustomField.Create(companyId, request.Name, request.FieldType, request.IsRequired, request.Options);

        _context.CustomFields.Add(field);
        await _context.SaveChangesAsync(cancellationToken);
        return field.Id;
    }
}
