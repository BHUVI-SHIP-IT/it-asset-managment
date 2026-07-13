using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Auth.DTOs;
using Tracer.Domain.Errors;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithPermissionsAsync(Guid.Empty, cancellationToken); // Dummy call first to construct logic
        
        // Let's actually fetch the user by email
        user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Auth.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Auth.AccountInactive);
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
        if (!isPasswordValid)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Auth.InvalidCredentials);
        }

        // To generate token, we need permissions. 
        // GetByEmailAsync currently only includes Role, we need the permissions.
        // Let's reload with permissions.
        user = await _userRepository.GetUserWithPermissionsAsync(user.Id, cancellationToken);
        if (user is null)
        {
             return Result.Failure<TokenResponse>(DomainErrors.Auth.InvalidCredentials);
        }

        var permissions = user.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList();

        var token = _jwtProvider.GenerateAccessToken(user, user.Role, permissions);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryUtc = DateTime.UtcNow.AddDays(7); // Should be config driven
        user.LastLoginAtUtc = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // JWT expiration comes from settings, we'll just hardcode the DTO for now or add to IJwtProvider.
        // Actually, we can just return it. Let's return 15 mins for now (900 seconds)
        return Result.Success(new TokenResponse(token, refreshToken, 900));
    }
}
