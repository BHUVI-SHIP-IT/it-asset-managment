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

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.Licenses.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLicenseBody body, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateLicenseCommand(
            id,
            body.Name,
            body.ManufacturerId,
            body.TotalSeats,
            body.PurchaseCost,
            body.ExpirationDate,
            body.Notes), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.Licenses.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteLicenseCommand(id), cancellationToken);
        return NoContent();
    }

    public sealed record UpdateLicenseBody(
        string Name,
        Guid? ManufacturerId,
        int TotalSeats,
        decimal PurchaseCost,
        DateTime? ExpirationDate,
        string? Notes);
}
