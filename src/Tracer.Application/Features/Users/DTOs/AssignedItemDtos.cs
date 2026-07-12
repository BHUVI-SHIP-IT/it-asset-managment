namespace Tracer.Application.Features.Users.DTOs;

public sealed record UserAssignedItemsDto
{
    public IReadOnlyList<AssignedItemDto> Assets { get; init; } = [];
    public IReadOnlyList<AssignedItemDto> Consumables { get; init; } = [];
    public IReadOnlyList<AssignedItemDto> Components { get; init; } = [];
    public IReadOnlyList<AssignedItemDto> Accessories { get; init; } = [];
    public IReadOnlyList<AssignedItemDto> Licenses { get; init; } = [];
}

public sealed record AssignedItemDto
{
    public string ItemType { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Identifier { get; init; }
    public DateTime? AssignedAtUtc { get; init; }
    public string? Status { get; init; }
    public string? DetailPath { get; init; }
}
