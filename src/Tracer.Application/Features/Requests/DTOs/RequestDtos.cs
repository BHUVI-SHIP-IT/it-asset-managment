namespace Tracer.Application.Features.Requests.DTOs;

public sealed record RequestDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public Guid RequestedByUserId { get; init; }
    public string RequestedByName { get; init; } = string.Empty;
    public string? ItemId { get; init; }
    public string? ItemName { get; init; }
    public int? Quantity { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAtUtc { get; init; }
    public Guid? ResolvedByUserId { get; init; }
    public string? ResolvedByName { get; init; }
    public DateTime? ResolvedAtUtc { get; init; }
    public string? Notes { get; init; }
    public string? ResolutionNotes { get; init; }
}

public sealed record RequestCatalogItemDto(string Id, string Name, string? Extra);
