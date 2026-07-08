using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

public class Department : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    
    // Navigation
    public Company Company { get; set; } = null!;

    public Department(Guid id) : base(id) { }
    private Department() { }
}
