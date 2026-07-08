using MediatR;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token) : IRequest<Result<TokenResponse>>;
