using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.StatusLabels.Commands;

public record DeleteStatusLabelCommand(int Id) : IRequest<bool>;

public class DeleteStatusLabelCommandHandler : IRequestHandler<DeleteStatusLabelCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteStatusLabelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteStatusLabelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.StatusLabels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.StatusLabels.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
