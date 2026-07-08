import os

def scaffold(entity, plural, has_company_id=True, id_type="Guid"):
    base_dir = f"src/Tracer.Application/Features/{plural}"
    api_dir = f"src/Tracer.Api/Controllers/v1"
    os.makedirs(f"{base_dir}/DTOs", exist_ok=True)
    os.makedirs(f"{base_dir}/Queries", exist_ok=True)
    os.makedirs(f"{base_dir}/Commands", exist_ok=True)
    
    dto = f"""namespace Tracer.Application.Features.{plural}.DTOs;

public class {entity}Dto
{{
    public {id_type} Id {{ get; set; }}
    public string Name {{ get; set; }} = string.Empty;
}}
"""
    with open(f"{base_dir}/DTOs/{entity}Dto.cs", "w") as f: f.write(dto)

    get_all = f"""using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.{plural}.DTOs;

namespace Tracer.Application.Features.{plural}.Queries;

public record GetAll{plural}Query : IRequest<List<{entity}Dto>>;

public class GetAll{plural}QueryHandler : IRequestHandler<GetAll{plural}Query, List<{entity}Dto>>
{{
    private readonly IApplicationDbContext _context;
    public GetAll{plural}QueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<{entity}Dto>> Handle(GetAll{plural}Query request, CancellationToken cancellationToken)
    {{
        return await _context.{plural}
            .Select(x => new {entity}Dto {{ Id = x.Id, Name = x.Name }})
            .ToListAsync(cancellationToken);
    }}
}}
"""
    with open(f"{base_dir}/Queries/GetAll{plural}Query.cs", "w") as f: f.write(get_all)

    get_by_id = f"""using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.{plural}.DTOs;

namespace Tracer.Application.Features.{plural}.Queries;

public record Get{entity}ByIdQuery({id_type} Id) : IRequest<{entity}Dto?>;

public class Get{entity}ByIdQueryHandler : IRequestHandler<Get{entity}ByIdQuery, {entity}Dto?>
{{
    private readonly IApplicationDbContext _context;
    public Get{entity}ByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<{entity}Dto?> Handle(Get{entity}ByIdQuery request, CancellationToken cancellationToken)
    {{
        var entity = await _context.{plural}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new {entity}Dto {{ Id = entity.Id, Name = entity.Name }};
    }}
}}
"""
    with open(f"{base_dir}/Queries/Get{entity}ByIdQuery.cs", "w") as f: f.write(get_by_id)
    
    create_cmd = f"""using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.{plural}.Commands;

public record Create{entity}Command(string Name{', Guid CompanyId' if has_company_id else ''}) : IRequest<{id_type}>;

public class Create{entity}CommandHandler : IRequestHandler<Create{entity}Command, {id_type}>
{{
    private readonly IApplicationDbContext _context;
    public Create{entity}CommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<{id_type}> Handle(Create{entity}Command request, CancellationToken cancellationToken)
    {{
        var entity = new {entity}({f"{id_type}.NewGuid()" if id_type == 'Guid' else "0"})
        {{
            Name = request.Name
            {f", CompanyId = request.CompanyId" if has_company_id else ""}
        }};
        
        _context.{plural}.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }}
}}
"""
    with open(f"{base_dir}/Commands/Create{entity}Command.cs", "w") as f: f.write(create_cmd)
    
    update_cmd = f"""using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.{plural}.Commands;

public record Update{entity}Command({id_type} Id, string Name) : IRequest<bool>;

public class Update{entity}CommandHandler : IRequestHandler<Update{entity}Command, bool>
{{
    private readonly IApplicationDbContext _context;
    public Update{entity}CommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(Update{entity}Command request, CancellationToken cancellationToken)
    {{
        var entity = await _context.{plural}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }}
}}
"""
    with open(f"{base_dir}/Commands/Update{entity}Command.cs", "w") as f: f.write(update_cmd)

    delete_cmd = f"""using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.{plural}.Commands;

public record Delete{entity}Command({id_type} Id) : IRequest<bool>;

public class Delete{entity}CommandHandler : IRequestHandler<Delete{entity}Command, bool>
{{
    private readonly IApplicationDbContext _context;
    public Delete{entity}CommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(Delete{entity}Command request, CancellationToken cancellationToken)
    {{
        var entity = await _context.{plural}.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.{plural}.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }}
}}
"""
    with open(f"{base_dir}/Commands/Delete{entity}Command.cs", "w") as f: f.write(delete_cmd)
    
    controller = f"""using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.{plural}.Commands;
using Tracer.Application.Features.{plural}.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "{plural}.View")]
public class {plural}Controller : ControllerBase
{{
    private readonly IMediator _mediator;

    public {plural}Controller(IMediator mediator)
    {{
        _mediator = mediator;
    }}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {{
        var result = await _mediator.Send(new GetAll{plural}Query());
        return Ok(result);
    }}

    [HttpGet("{{id}}")]
    public async Task<IActionResult> GetById({id_type} id)
    {{
        var result = await _mediator.Send(new Get{entity}ByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }}

    [HttpPost]
    [Authorize(Policy = "{plural}.Create")]
    public async Task<IActionResult> Create(Create{entity}Command command)
    {{
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new {{ id }}, new {{ id }});
    }}

    [HttpPut("{{id}}")]
    [Authorize(Policy = "{plural}.Update")]
    public async Task<IActionResult> Update({id_type} id, Update{entity}Command command)
    {{
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }}

    [HttpDelete("{{id}}")]
    [Authorize(Policy = "{plural}.Delete")]
    public async Task<IActionResult> Delete({id_type} id)
    {{
        var success = await _mediator.Send(new Delete{entity}Command(id));
        return success ? NoContent() : NotFound();
    }}
}}
"""
    with open(f"{api_dir}/{plural}Controller.cs", "w") as f: f.write(controller)


scaffold("Category", "Categories")
scaffold("Location", "Locations")
scaffold("Manufacturer", "Manufacturers")
scaffold("Supplier", "Suppliers")
scaffold("Department", "Departments")
scaffold("AssetModel", "AssetModels")
scaffold("StatusLabel", "StatusLabels", has_company_id=False, id_type="int")
