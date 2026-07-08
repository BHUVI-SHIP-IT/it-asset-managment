using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

// ---------------------------------------------------------------------------
// Minimal FK-target stubs for aggregates owned by not-yet-built modules
// (IAM: Company/ApplicationUser; Master Data: AssetModel/StatusLabel/Location).
//
// Per DDD (Doc 10 §4.3), the Asset aggregate references these by Guid FK only.
// These stubs exist so the Asset module compiles and its EF relationships are
// valid today; the full aggregates from Docs 2/4/7 replace them without changing
// the Asset code (the FK columns and navigations stay identical).
// ---------------------------------------------------------------------------

/// <summary>Tenant boundary (Doc 4 §1.3 CompanyId; Doc 7 tenant isolation). Master module: IAM.</summary>
public class Company : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    
    // IAM navigation
    public ICollection<ApplicationUser> Users { get; private set; } = new List<ApplicationUser>();

    public Company(Guid id) : base(id) { }
    private Company() { }
}

/// <summary>System user / asset assignee (Doc 4, Doc 7). Master module: IAM.</summary>
public class ApplicationUser : Entity<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryUtc { get; set; }

    public Guid CompanyId { get; set; }
    public int RoleId { get; set; }

    // Navigations
    public Company? Company { get; set; }
    public Role Role { get; set; } = null!;

    public ApplicationUser(Guid id) : base(id) { }
    private ApplicationUser() { }
}
