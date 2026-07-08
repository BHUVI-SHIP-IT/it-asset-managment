using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

/// <summary>
/// Tracks asynchronous financial report generation.
/// </summary>
public sealed class ReportExport : Entity<Guid>
{
    private ReportExport() { }

    private ReportExport(Guid id, string reportName, Guid companyId, Guid requestedBy)
        : base(id)
    {
        ReportName = reportName;
        CompanyId = companyId;
        RequestedBy = requestedBy;
        Status = ReportStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string ReportName { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid RequestedBy { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public byte[]? FileContent { get; private set; }

    public static ReportExport Create(string reportName, Guid companyId, Guid requestedBy)
    {
        return new ReportExport(Guid.NewGuid(), reportName, companyId, requestedBy);
    }

    public void MarkCompleted(byte[] content)
    {
        Status = ReportStatus.Completed;
        FileContent = content;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = ReportStatus.Failed;
        CompletedAtUtc = DateTime.UtcNow;
    }
}

public enum ReportStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}
