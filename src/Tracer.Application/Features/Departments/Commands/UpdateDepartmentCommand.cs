using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Departments.Commands;

public record UpdateDepartmentCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateDepartmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
