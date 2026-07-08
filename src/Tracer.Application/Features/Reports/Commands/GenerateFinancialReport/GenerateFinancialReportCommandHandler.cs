using Hangfire;
using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Reports.Commands.GenerateFinancialReport;

public sealed class GenerateFinancialReportCommandHandler : IRequestHandler<GenerateFinancialReportCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public GenerateFinancialReportCommandHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUser,
        IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _currentUser = currentUser;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Result<Guid>> Handle(GenerateFinancialReportCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("No user context is available.");

        var reportExport = ReportExport.Create(request.ReportName, companyId, userId);
        
        _context.ReportExports.Add(reportExport);
        await _context.SaveChangesAsync(cancellationToken);

        // Enqueue the background job to generate the report content
        _backgroundJobClient.Enqueue<IFinancialReportJob>(
            job => job.ExecuteAsync(reportExport.Id, CancellationToken.None));

        return Result.Success(reportExport.Id);
    }
}
