using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
