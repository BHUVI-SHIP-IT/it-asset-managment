using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.StatusLabels.Commands;

public record CreateStatusLabelCommand(
    string Name,
    bool IsDeployable = false,
    bool IsPending = false,
    bool IsArchived = false) : IRequest<int>;

public class CreateStatusLabelCommandHandler : IRequestHandler<CreateStatusLabelCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateStatusLabelCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateStatusLabelCommand request, CancellationToken cancellationToken)
    {
        var entity = new StatusLabel(0)
        {
            Name = request.Name.Trim(),
            IsDeployable = request.IsDeployable,
            IsPending = request.IsPending,
            IsArchived = request.IsArchived
        };

        _context.StatusLabels.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
