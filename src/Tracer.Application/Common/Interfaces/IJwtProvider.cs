using Tracer.Domain.Entities;

namespace Tracer.Application.Common.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(ApplicationUser user, Role role, IEnumerable<string> permissions);
    string GenerateRefreshToken();
}
