using MediatR;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>;
