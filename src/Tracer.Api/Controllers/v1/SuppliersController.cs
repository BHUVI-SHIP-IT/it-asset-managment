using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Shared.Authorization;
using Tracer.Application.Features.Suppliers.Commands;
using Tracer.Application.Features.Suppliers.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/suppliers")]
[Authorize(Policy = Permissions.Suppliers.View)]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllSuppliersQuery());
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 10 : pageSize;
        var items = result.Skip((page - 1) * size).Take(size).ToList();
        return Ok(new
        {
            items,
            totalCount = result.Count,
            pageNumber = page,
            pageSize = size
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSupplierByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Suppliers.Create)]
    public async Task<IActionResult> Create(CreateSupplierCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.Suppliers.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.Suppliers.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteSupplierCommand(id));
        return success ? NoContent() : NotFound();
    }
}
