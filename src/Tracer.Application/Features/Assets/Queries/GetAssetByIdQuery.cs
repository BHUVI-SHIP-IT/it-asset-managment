using MediatR;
using Tracer.Application.Features.Assets.DTOs;

namespace Tracer.Application.Features.Assets.Queries;

public record GetAssetByIdQuery(Guid Id) : IRequest<AssetDetailDto?>;
