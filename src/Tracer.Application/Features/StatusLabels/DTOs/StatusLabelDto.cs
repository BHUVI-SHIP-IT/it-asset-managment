namespace Tracer.Application.Features.StatusLabels.DTOs;

public class StatusLabelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeployable { get; set; }
    public bool IsPending { get; set; }
    public bool IsArchived { get; set; }
}
