using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.LicenseAggregate;

namespace Tracer.Application.Features.Licenses;

public sealed record LicenseDto(
    Guid Id,
    string Name,
    Guid CompanyId,
    Guid? ManufacturerId,
    int TotalSeats,
    decimal PurchaseCost,
    DateTime? ExpirationDate,
    string? Notes);

public sealed record GetLicensesQuery : IRequest<List<LicenseDto>>;
public sealed record GetLicenseByIdQuery(Guid Id) : IRequest<LicenseDto?>;
public sealed record CreateLicenseCommand(
    string Name,
    Guid? ManufacturerId,
    int TotalSeats,
    decimal PurchaseCost,
    DateTime? ExpirationDate,
    string? Notes) : IRequest<Guid>;

public sealed record UpdateLicenseCommand(
    Guid Id,
    string Name,
    Guid? ManufacturerId,
    int TotalSeats,
    decimal PurchaseCost,
    DateTime? ExpirationDate,
    string? Notes) : IRequest;

public sealed record DeleteLicenseCommand(Guid Id) : IRequest;

public sealed class GetLicensesQueryHandler : IRequestHandler<GetLicensesQuery, List<LicenseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetLicensesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<LicenseDto>> Handle(GetLicensesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SoftwareLicenses.AsQueryable();
        if (_currentUser.CompanyId is Guid companyId)
            query = query.Where(x => x.CompanyId == companyId);

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new LicenseDto(
                x.Id, x.Name, x.CompanyId, x.ManufacturerId, x.TotalSeats, x.PurchaseCost, x.ExpirationDate, x.Notes))
            .ToListAsync(cancellationToken);
    }
}

public sealed class GetLicenseByIdQueryHandler : IRequestHandler<GetLicenseByIdQuery, LicenseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLicenseByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LicenseDto?> Handle(GetLicenseByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.SoftwareLicenses
            .Where(x => x.Id == request.Id)
            .Select(x => new LicenseDto(
                x.Id, x.Name, x.CompanyId, x.ManufacturerId, x.TotalSeats, x.PurchaseCost, x.ExpirationDate, x.Notes))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public sealed class CreateLicenseCommandHandler : IRequestHandler<CreateLicenseCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateLicenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateLicenseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available.");

        var license = SoftwareLicense.Create(
            request.Name,
            companyId,
            request.ManufacturerId,
            request.TotalSeats,
            request.PurchaseCost,
            request.ExpirationDate);

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            license.Update(
                license.Name,
                license.ManufacturerId,
                license.TotalSeats,
                license.PurchaseCost,
                license.ExpirationDate,
                request.Notes);
        }

        _context.SoftwareLicenses.Add(license);
        await _context.SaveChangesAsync(cancellationToken);
        return license.Id;
    }
}

public sealed class UpdateLicenseCommandHandler : IRequestHandler<UpdateLicenseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateLicenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateLicenseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available.");

        var license = await _context.SoftwareLicenses
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new KeyNotFoundException($"License {request.Id} was not found.");

        license.Update(
            request.Name,
            request.ManufacturerId,
            request.TotalSeats,
            request.PurchaseCost,
            request.ExpirationDate,
            request.Notes);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DeleteLicenseCommandHandler : IRequestHandler<DeleteLicenseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteLicenseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available.");

        var license = await _context.SoftwareLicenses
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.CompanyId == companyId, cancellationToken)
            ?? throw new KeyNotFoundException($"License {request.Id} was not found.");

        license.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
