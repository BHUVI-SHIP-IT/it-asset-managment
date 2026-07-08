using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

public class StatusLabel : AuditableEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeployable { get; set; }
    public bool IsPending { get; set; }
    public bool IsArchived { get; set; }
    
    public StatusLabel(int id) : base(id) { }
    private StatusLabel() { }
}
