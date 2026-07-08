using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

public class AssetModel : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public Guid ManufacturerId { get; set; }
    public Guid CategoryId { get; set; }
    
    // Navigation
    public Company Company { get; set; } = null!;
    public Manufacturer Manufacturer { get; set; } = null!;
    public Category Category { get; set; } = null!;

    public AssetModel(Guid id) : base(id) { }
    private AssetModel() { }
}
