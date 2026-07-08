namespace Tracer.Application.Common.Interfaces;

public interface IFinancialReportJob
{
    Task ExecuteAsync(Guid reportExportId, CancellationToken cancellationToken);
}
