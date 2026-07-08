using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Depreciations.Commands.CreateDepreciation;

public sealed record CreateDepreciationCommand(
    string Name,
    int Months,
    decimal MinimumValue) : IRequest<Result<Guid>>;
