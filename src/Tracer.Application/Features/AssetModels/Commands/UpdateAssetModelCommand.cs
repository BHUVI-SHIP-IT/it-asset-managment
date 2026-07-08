using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.AssetModels.Commands;

public record UpdateAssetModelCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateAssetModelCommandHandler : IRequestHandler<UpdateAssetModelCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateAssetModelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateAssetModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AssetModels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
