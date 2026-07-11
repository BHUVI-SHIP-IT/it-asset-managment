using Tracer.Domain.Entities;
using Tracer.Application.Features.Users.DTOs;

namespace Tracer.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    // Loads the user along with their Role and Role.RolePermissions.Permission
    Task<ApplicationUser?> GetUserWithPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserDto>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    void Update(ApplicationUser user);
}
