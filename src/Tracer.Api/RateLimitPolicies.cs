namespace Tracer.Api;

/// <summary>
/// Centralises rate-limit policy names to avoid magic strings across controllers.
/// </summary>
public static class RateLimitPolicies
{
    /// <summary>Brute-force protection for login / refresh endpoints (20 req/min/user).</summary>
    public const string Auth = "AuthPolicy";

    /// <summary>High-throughput GET endpoints (1,000 req/min/user with a queue of 100).</summary>
    public const string Read = "ReadPolicy";

    /// <summary>Mutation endpoints — POST / PUT / DELETE (200 req/min/user, queue 20).</summary>
    public const string Write = "WritePolicy";

    /// <summary>CPU-heavy export / report endpoints (10 req/min/user, queue 5).</summary>
    public const string Reports = "ReportsPolicy";
}
