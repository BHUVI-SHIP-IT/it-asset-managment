using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Shared.Authorization;
using Tracer.Application.Features.Categories.Commands;
using Tracer.Application.Features.Categories.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/categories")]
[Authorize(Policy = Permissions.Categories.View)]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
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
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Categories.Create)]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.Categories.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        // Route id is authoritative — body may omit Id (frontend historically sent only name fields).
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = "Route ID does not match body Id.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var effective = command with { Id = id };
        if (string.IsNullOrWhiteSpace(effective.Name))
        {
            var problem = new ProblemDetails
            {
                Title = "Validation Failed",
                Detail = "Name is required.",
                Status = StatusCodes.Status400BadRequest
            };
            problem.Extensions["errors"] = new Dictionary<string, string[]>
            {
                ["name"] = ["Name is required."]
            };
            return BadRequest(problem);
        }

        var success = await _mediator.Send(effective);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.Categories.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand(id));
        return success ? NoContent() : NotFound();
    }
}
