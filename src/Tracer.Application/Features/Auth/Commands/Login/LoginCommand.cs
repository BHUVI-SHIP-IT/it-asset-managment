using MediatR;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;
