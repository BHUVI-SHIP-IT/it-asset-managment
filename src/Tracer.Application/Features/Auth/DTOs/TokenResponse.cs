namespace Tracer.Application.Features.Auth.DTOs;

public record TokenResponse(string AccessToken, int ExpiresIn);
