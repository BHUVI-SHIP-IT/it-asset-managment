namespace Tracer.Application.Features.Consumables.DTOs;

public sealed class ConsumableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public int TotalQuantity { get; set; }
    public decimal PurchaseCost { get; set; }
    public int ReorderThreshold { get; set; }
    public Guid? AssignedUserId { get; set; }
    public DateTime? AssignedAtUtc { get; set; }
}
