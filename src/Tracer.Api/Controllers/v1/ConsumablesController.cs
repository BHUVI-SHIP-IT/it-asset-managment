using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Consumables.Commands.CheckoutConsumable;
using Tracer.Application.Features.Consumables.Commands.CreateConsumable;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ConsumablesController : ControllerBase
{
    private readonly ISender _sender;

    public ConsumablesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Policy = "CONSUMABLES.CREATE")]
    public async Task<ActionResult<int>> Create(CreateConsumableCommand command)
    {
        return await _sender.Send(command);
    }

    [HttpPost("{id}/checkout")]
    [Authorize(Policy = "CONSUMABLES.CHECKOUT")]
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
