using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Domain.Errors;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            // If they reach here, Authorize attribute probably failed anyway, but just in case.
            return Result.Failure<CurrentUserDto>(DomainErrors.Auth.InvalidCredentials);
        }

        var user = await _userRepository.GetUserWithPermissionsAsync(userId.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(DomainErrors.Auth.InvalidCredentials);
        }

        var dto = new CurrentUserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList(),
            user.CompanyId
        );

        return Result.Success(dto);
    }
}
