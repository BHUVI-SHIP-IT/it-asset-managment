using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Domain.Errors;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.Token, cancellationToken);

        if (user is null || user.RefreshTokenExpiryUtc < DateTime.UtcNow || !user.IsActive)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Auth.InvalidRefreshToken);
        }

        // We need permissions for the new token
        user = await _userRepository.GetUserWithPermissionsAsync(user.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Auth.InvalidRefreshToken);
        }

        var permissions = user.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList();

        var token = _jwtProvider.GenerateAccessToken(user, user.Role, permissions);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryUtc = DateTime.UtcNow.AddDays(7);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new TokenResponse(token, 900));
    }
}
