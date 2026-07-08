using System.Text;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Api.BackgroundServices;

public class FinancialReportJob : IFinancialReportJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<FinancialReportJob> _logger;

    public FinancialReportJob(IApplicationDbContext context, ILogger<FinancialReportJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid reportExportId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting FinancialReportJob for export ID: {reportExportId}");

        var export = await _context.ReportExports.FindAsync(new object[] { reportExportId }, cancellationToken);
        if (export == null)
        {
            _logger.LogError($"ReportExport {reportExportId} not found.");
            return;
        }

        try
        {
            var assets = await _context.Assets
                .Include(a => a.Depreciation)
                .Where(a => a.CompanyId == export.CompanyId)
                .ToListAsync(cancellationToken);

            var sb = new StringBuilder();
            // CSV Header
            sb.AppendLine("AssetTag,Name,Status,PurchaseDate,PurchaseCost,DepreciationSchedule,CurrentValue");

            foreach (var asset in assets)
            {
                var tag = EscapeCsv(asset.AssetTag);
                var name = EscapeCsv(asset.Name);
                var status = asset.Status.ToString();
                var pDate = asset.PurchaseDate?.ToString("yyyy-MM-dd") ?? "";
                var pCost = asset.PurchaseCost.ToString("F2");
                var depName = EscapeCsv(asset.Depreciation?.Name ?? "None");
                var cValue = asset.CurrentValue.ToString("F2");

                sb.AppendLine($"{tag},{name},{status},{pDate},{pCost},{depName},{cValue}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            
            export.MarkCompleted(bytes);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"FinancialReportJob completed successfully for {reportExportId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"FinancialReportJob failed for {reportExportId}");
            export.MarkFailed();
            await _context.SaveChangesAsync(cancellationToken);
            throw; // Rethrow for Hangfire to handle retries
        }
    }

    private string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
