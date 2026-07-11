using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Exceptions;
using Tracer.Domain.Exceptions;

namespace Tracer.Api.Middleware;

/// <summary>
/// Global exception handler implementing .NET 9 IExceptionHandler (Doc 10 §3.5).
/// Maps known exception types to RFC 7807 Problem Details responses (Doc 5 §1.3).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Unwrap common wrappers so JsonException from model binding surfaces clearly.
        var root = exception;
        while (root.InnerException is not null
               && root is not JsonException
               && root is not ValidationException
               && root is not NotFoundException
               && root is not DomainException
               && root is not DbUpdateException)
        {
            root = root.InnerException;
        }

        var (statusCode, title, detail, errors) = root switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                validationEx.Message,
                validationEx.Errors as object),

            JsonException jsonEx => (
                StatusCodes.Status400BadRequest,
                "Invalid JSON",
                jsonEx.Path is { Length: > 0 } path
                    ? $"Invalid JSON at '{path}': {jsonEx.Message}"
                    : jsonEx.Message,
                null as object),

            DbUpdateException dbEx when IsUniqueConstraintViolation(dbEx) => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "A record with the same unique value already exists.",
                null as object),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFoundEx.Message,
                null as object),

            DomainException domainEx => (
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                domainEx.Message,
                null as object),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.",
                null as object)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Handled exception ({StatusCode}): {Message}", statusCode, exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{statusCode}"
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains("unique index", StringComparison.OrdinalIgnoreCase)
               || message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase)
               || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase);
    }
}
