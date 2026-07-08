using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.AssetModels.Commands;

public record DeleteAssetModelCommand(Guid Id) : IRequest<bool>;

public class DeleteAssetModelCommandHandler : IRequestHandler<DeleteAssetModelCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteAssetModelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteAssetModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AssetModels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.AssetModels.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
