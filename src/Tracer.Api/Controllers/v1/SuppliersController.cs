using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Suppliers.Commands;
using Tracer.Application.Features.Suppliers.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "Suppliers.View")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllSuppliersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSupplierByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = "Suppliers.Create")]
    public async Task<IActionResult> Create(CreateSupplierCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Suppliers.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Suppliers.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteSupplierCommand(id));
        return success ? NoContent() : NotFound();
    }
}
