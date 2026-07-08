using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Reports.Commands.GenerateFinancialReport;

public sealed record GenerateFinancialReportCommand(string ReportName) : IRequest<Result<Guid>>;
