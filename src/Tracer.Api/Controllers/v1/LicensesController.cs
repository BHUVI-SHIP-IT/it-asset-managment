using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Licenses;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/licenses")]
[Authorize]
public sealed class LicensesController : ControllerBase
{
    private readonly ISender _sender;

    public LicensesController(ISender sender) => _sender = sender;

    [HttpGet]
    [Authorize(Policy = Permissions.Licenses.View)]
    public async Task<ActionResult<List<LicenseDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetLicensesQuery(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Permissions.Licenses.View)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetLicenseByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Licenses.Create)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateLicenseCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(id);
    }
}
