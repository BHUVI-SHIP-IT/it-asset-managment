using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Departments.Commands;

public record CreateDepartmentCommand(string Name, Guid CompanyId) : IRequest<Guid>;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateDepartmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = new Department(Guid.NewGuid())
        {
            Name = request.Name
            , CompanyId = request.CompanyId
        };
        
        _context.Departments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
