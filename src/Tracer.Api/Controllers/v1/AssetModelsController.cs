using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Shared.Authorization;
using Tracer.Application.Features.AssetModels.Commands;
using Tracer.Application.Features.AssetModels.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/asset-models")]
[Authorize(Policy = Permissions.AssetModels.View)]
public class AssetModelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetModelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllAssetModelsQuery());
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
        var result = await _mediator.Send(new GetAssetModelByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = Permissions.AssetModels.Create)]
    public async Task<IActionResult> Create(CreateAssetModelCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.AssetModels.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateAssetModelCommand command)
    {
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.AssetModels.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteAssetModelCommand(id));
        return success ? NoContent() : NotFound();
    }
}
