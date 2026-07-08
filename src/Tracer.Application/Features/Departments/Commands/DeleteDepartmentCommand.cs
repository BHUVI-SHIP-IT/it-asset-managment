using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest<bool>;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteDepartmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.Departments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
