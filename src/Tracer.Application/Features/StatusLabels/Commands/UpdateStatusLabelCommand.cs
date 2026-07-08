using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.StatusLabels.Commands;

public record UpdateStatusLabelCommand(int Id, string Name) : IRequest<bool>;

public class UpdateStatusLabelCommandHandler : IRequestHandler<UpdateStatusLabelCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateStatusLabelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateStatusLabelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.StatusLabels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
