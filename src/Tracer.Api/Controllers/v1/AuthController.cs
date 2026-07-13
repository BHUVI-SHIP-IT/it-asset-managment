using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tracer.Application.Features.Auth.Commands.Login;
using Tracer.Application.Features.Auth.Commands.RefreshToken;
using Tracer.Application.Features.Auth.Queries.GetCurrentUser;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            // Refresh token is returned in the TokenResponse body for the SPA to persist
            // and send back on POST /api/v1/auth/refresh.
            return Ok(result.Value);
        }

        return Unauthorized(result.Error);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Unauthorized(result.Error);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Unauthorized(result.Error);
    }
}
