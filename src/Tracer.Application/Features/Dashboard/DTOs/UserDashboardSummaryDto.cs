namespace Tracer.Application.Features.Dashboard.DTOs;

public sealed class UserDashboardSummaryDto
{
    public AssignedCountsDto AssignedCounts { get; init; } = new();
    public RequestCountsDto RequestCounts { get; init; } = new();
    public IReadOnlyList<AttentionItemDto> AttentionItems { get; init; } = [];
}

public sealed class AssignedCountsDto
{
    public int Assets { get; init; }
    public int Consumables { get; init; }
    public int Components { get; init; }
    public int Licenses { get; init; }
    public int Accessories { get; init; }
}

public sealed class RequestCountsDto
{
    public int Pending { get; init; }
    public int Approved { get; init; }
    public int Rejected { get; init; }
}

public sealed class AttentionItemDto
{
    public string Kind { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public DateTime? DueAtUtc { get; init; }
}
