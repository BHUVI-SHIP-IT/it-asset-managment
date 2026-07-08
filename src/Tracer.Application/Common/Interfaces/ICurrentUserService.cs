namespace Tracer.Application.Common.Interfaces;

/// <summary>
/// Ambient accessor for the authenticated principal, resolved from JWT claims (Doc 7 §5.4).
/// Used to stamp audit fields and enforce tenant scoping.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? CompanyId { get; }
}
