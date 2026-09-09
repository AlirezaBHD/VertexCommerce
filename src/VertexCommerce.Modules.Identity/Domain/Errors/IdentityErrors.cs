using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Identity.Domain.Errors;

public static class IdentityErrors
{
    public static Error UserNotFound(Guid userId) =>
        Error.NotFound("User", userId);

    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Invalid phone number or password.");

    public static readonly Error AccountInactive =
        Error.Unauthorized("Your account has been deactivated.");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized("Invalid or expired refresh token.");

    public static readonly Error TokenExpired =
        Error.Conflict("Token is expired.");

    public static readonly Error OptNotVerified =
        Error.Conflict("You need to verify OPT first.");

    public static readonly Error PhoneAlreadyRegistered =
        Error.Conflict("Phone number already registered.");

    public static readonly Error OtpExpired =
        Error.Conflict("Otp is expired.");

    public static readonly Error TooManyAttempts =
        Error.Conflict("Too many attempts.");

    public static readonly Error OtpWrong =
        Error.Conflict("Otp is wrong.");

    public static readonly Error IncorrectCurrentPassword =
        Error.Validation("Current password is incorrect.");
}
