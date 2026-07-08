using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Manufacturers.Commands;

public record CreateManufacturerCommand(string Name, Guid CompanyId) : IRequest<Guid>;

public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateManufacturerCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Manufacturer(Guid.NewGuid())
        {
            Name = request.Name
            , CompanyId = request.CompanyId
        };
        
        _context.Manufacturers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
