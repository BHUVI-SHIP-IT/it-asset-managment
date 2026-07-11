namespace Tracer.Application.Features.Users.DTOs;

public sealed record UserDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public Guid CompanyId { get; init; }
}

public sealed record RoleDto(int Id, string Name);
