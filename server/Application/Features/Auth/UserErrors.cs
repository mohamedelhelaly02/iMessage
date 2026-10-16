using server.Shared.Abstraction;

namespace server.Application.Features.Auth;

public static class UserErrors
{
    public static Error EmailInUse => Error.Conflict("Email is already in use.");
    public static Error AccountLocked => Error.Conflict("Your account has been temporarily locked due to too many failed login attempts. Please try again in 15 minutes.");
    public static Error EmailNotConfirmed => Error.Conflict("Email is not confirmed");
    public static Error RegistrationFailed(Dictionary<string, string[]> errors) => Error.Validation("User.REGISTRATION_FAILED", "User registration failed.", errors);
    public static Error RoleAssignmentFailed(Dictionary<string, string[]> errors) => Error.Validation("User.ROLE_ASSIGNMENT_FAILED", "Role assignment failed.", errors);
    public static Error InvalidCredentials => Error.Unauthorized("User.INVALID_CREDENTIALS");

    public static Error UserNotFound => Error.NotFound(message: "The specified user does not exist.");
}
