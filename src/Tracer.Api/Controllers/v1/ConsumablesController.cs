using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Consumables.DTOs;
using Tracer.Application.Features.Consumables.Commands.CheckoutConsumable;
using Tracer.Application.Features.Consumables.Commands.CreateConsumable;
using Tracer.Application.Features.Consumables.Queries;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/consumables")]
[Produces("application/json")]
[Authorize]
public class ConsumablesController : ControllerBase
{
    private readonly ISender _sender;

    public ConsumablesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Consumables.View)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsumableDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllConsumablesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Consumables.Create)]
    public async Task<ActionResult<int>> Create(CreateConsumableCommand command)
    {
        return await _sender.Send(command);
    }

    [HttpPost("{id}/checkout")]
    [Authorize(Policy = Permissions.Consumables.Checkout)]
    public async Task<ActionResult> Checkout(int id, CheckoutConsumableCommand command)
    {
        if (id != command.ConsumableId)
        {
            return BadRequest();
        }

        await _sender.Send(command);
        return NoContent();
    }
}
