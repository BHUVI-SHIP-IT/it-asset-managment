namespace Tracer.Application.Features.Dashboard.DTOs;

public sealed class DashboardMetricsDto
{
    public int TotalAssets { get; init; }
    public int ActiveAssets { get; init; }
    public int PendingCheckouts { get; init; }
    public int OverdueCheckins { get; init; }
    /// <summary>Org-wide inventory requests awaiting approval (Pending status).</summary>
    public int PendingApprovals { get; init; }
}
