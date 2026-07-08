using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.AssetModels.Commands;

public record CreateAssetModelCommand(string Name, Guid CompanyId) : IRequest<Guid>;

public class CreateAssetModelCommandHandler : IRequestHandler<CreateAssetModelCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateAssetModelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateAssetModelCommand request, CancellationToken cancellationToken)
    {
        var entity = new AssetModel(Guid.NewGuid())
        {
            Name = request.Name
            , CompanyId = request.CompanyId
        };
        
        _context.AssetModels.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
