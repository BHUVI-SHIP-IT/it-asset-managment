using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Departments.DTOs;

namespace Tracer.Application.Features.Departments.Queries;

public record GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllDepartmentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .Select(x => new DepartmentDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
