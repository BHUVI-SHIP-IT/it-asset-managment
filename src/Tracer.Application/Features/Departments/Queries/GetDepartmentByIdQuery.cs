using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Departments.DTOs;

namespace Tracer.Application.Features.Departments.Queries;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto?>;

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly IApplicationDbContext _context;
    public GetDepartmentByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new DepartmentDto { Id = entity.Id, Name = entity.Name };
    }
}
