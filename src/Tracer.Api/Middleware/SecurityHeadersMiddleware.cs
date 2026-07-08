namespace Tracer.Api.Middleware;

/// <summary>
/// Adds OWASP-recommended HTTP security headers to every response (M7 Hardening).
/// Registered early in the pipeline so headers are present even on error responses.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent MIME-type sniffing attacks.
        headers["X-Content-Type-Options"] = "nosniff";

        // Block the page from being embedded in iframes (clickjacking protection).
        headers["X-Frame-Options"] = "DENY";

        // Control how much referrer information is sent.
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Restrict access to powerful browser features.
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";

        // Content-Security-Policy: API only — no HTML/JS served, so this is a tight policy.
        headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";

        // Remove the Server header to avoid fingerprinting.
        headers.Remove("Server");

        // Remove X-Powered-By (Kestrel default).
        headers.Remove("X-Powered-By");

        await _next(context);
    }
}
