using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Reports.Queries.DownloadReport;

public sealed record DownloadReportQuery(Guid ReportId) : IRequest<Result<ReportDownloadDto>>;

public record ReportDownloadDto(string ReportName, byte[] FileContent);
