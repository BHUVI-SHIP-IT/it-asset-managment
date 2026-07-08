namespace Tracer.Application.Features.Auth.DTOs;

public record CurrentUserDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    List<string> Permissions,
    Guid CompanyId
);
