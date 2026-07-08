using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, Guid CompanyId) : IRequest<Guid>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category(Guid.NewGuid())
        {
            Name = request.Name
            , CompanyId = request.CompanyId
        };
        
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
