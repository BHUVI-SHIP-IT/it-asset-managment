using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

public class Location : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    
    // Navigation
    public Company Company { get; set; } = null!;

    public Location(Guid id) : base(id) { }
    private Location() { }
}
