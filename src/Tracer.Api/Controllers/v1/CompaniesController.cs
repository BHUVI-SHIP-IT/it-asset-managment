using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Companies;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/companies")]
[Authorize]
public sealed class CompaniesController : ControllerBase
{
    private readonly ISender _sender;

    public CompaniesController(ISender sender) => _sender = sender;

    [HttpGet]
    [Authorize(Policy = Permissions.Settings.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetCompaniesQuery(pageNumber, pageSize, sortColumn, sortDirection),
            cancellationToken);

        return Ok(new
        {
            items = result.Items,
            totalCount = result.TotalCount,
            pageNumber = result.Page,
            pageSize = result.PageSize
        });
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Settings.Manage)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.Settings.Manage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCompanyCommand body, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCompanyCommand(id, body.Name), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.Settings.Manage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCompanyCommand(id), cancellationToken);
        return NoContent();
    }
}
