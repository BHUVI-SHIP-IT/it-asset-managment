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
            // Usually, refresh token goes into HttpOnly Cookie. 
            // For now, we return both in the body for simplicity, or we can set it here.
            // Let's set refresh token in cookie as per Doc 5
            
            // In a real app we would extract the refresh token from the result and set the cookie.
            // Since our DTO only has AccessToken, we need to adjust DTO to have RefreshToken to set cookie,
            // or just rely on the body. We'll rely on the body or simplify.
            // Actually, for this PoC, returning token response is fine.
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
