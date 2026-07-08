using Tracer.Shared.Results;

namespace Tracer.Domain.Errors;

public static class DomainErrors
{
    public static class Auth
    {
        public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Validation);
        public static readonly Error AccountInactive = new("Auth.AccountInactive", "This account is inactive.", ErrorType.Validation);
        public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "Invalid or expired refresh token.", ErrorType.Validation);
    }
}
