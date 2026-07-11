using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;
using Tracer.Shared.Models;

namespace Tracer.Application.Features.Companies;

public sealed record CompanyDto(Guid Id, string Name);

public sealed record GetCompaniesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortColumn = null,
    string? SortDirection = null) : IRequest<PagedList<CompanyDto>>;

public sealed record CreateCompanyCommand(string Name) : IRequest<Guid>;
public sealed record UpdateCompanyCommand(Guid Id, string Name) : IRequest;
public sealed record DeleteCompanyCommand(Guid Id) : IRequest;

public sealed class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, PagedList<CompanyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedList<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var size = request.PageSize < 1 ? 10 : request.PageSize;
        var query = _context.Companies.AsQueryable();

        query = request.SortDirection?.ToLowerInvariant() == "desc"
            ? query.OrderByDescending(x => x.Name)
            : query.OrderBy(x => x.Name);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new CompanyDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return new PagedList<CompanyDto>(items, page, size, total);
    }
}

public sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Company(Guid.NewGuid()) { Name = request.Name.Trim() };
        _context.Companies.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public sealed class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Company {request.Id} was not found.");

        entity.Name = request.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Company {request.Id} was not found.");

        _context.Companies.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
