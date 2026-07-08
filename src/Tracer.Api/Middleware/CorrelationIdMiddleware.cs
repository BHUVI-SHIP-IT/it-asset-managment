namespace Tracer.Api.Middleware;

/// <summary>
/// Reads (or generates) a Correlation ID per request and propagates it through the
/// Serilog LogContext and the response header for end-to-end tracing.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString("N");

        // Echo it back so the caller can correlate client-side logs.
        context.Response.Headers[HeaderName] = correlationId;

        // Push into Serilog's ambient context — all log entries in this request carry it.
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
