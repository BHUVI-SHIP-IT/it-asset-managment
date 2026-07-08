using MediatR;
using Tracer.Shared.Results;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Reports.Queries.GetReportStatus;

public sealed record GetReportStatusQuery(Guid ReportId) : IRequest<Result<ReportStatusDto>>;

public record ReportStatusDto(Guid Id, string ReportName, ReportStatus Status, DateTime CreatedAtUtc, DateTime? CompletedAtUtc);
