using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    int RoleId) : IRequest<Result<Guid>>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.RoleId).GreaterThan(0);
    }
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _users = users;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await _users.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            return Result.Failure<Guid>(Error.Conflict("User.DuplicateEmail", $"A user with email '{email}' already exists."));

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken);
        if (!roleExists)
            return Result.Failure<Guid>(Error.Validation("User.InvalidRole", "The specified role does not exist."));

        var user = new ApplicationUser(Guid.NewGuid())
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CompanyId = companyId,
            RoleId = request.RoleId,
            IsActive = true
        };

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
