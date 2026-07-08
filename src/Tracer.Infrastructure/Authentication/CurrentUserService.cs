using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Infrastructure.Authentication;

/// <summary>
/// Resolves the authenticated user's identity from JWT claims in the current HTTP context (Doc 7 §5.4).
/// Returns null for unauthenticated requests (e.g., health checks).
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public Guid? CompanyId
    {
        get
        {
            var company = _httpContextAccessor.HttpContext?.User.FindFirstValue("company_id");
            return Guid.TryParse(company, out var id) ? id : null;
        }
    }
}
