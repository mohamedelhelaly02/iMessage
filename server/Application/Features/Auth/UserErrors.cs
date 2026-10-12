using System;
using server.Shared.Abstraction;

namespace server.Application.Features.Auth;

public static class UserErrors
{
    public static Error EmailInUse => Error.Conflict("Email is already in use.");
    public static Error RegistrationFailed(Dictionary<string, string[]> errors) => Error.Validation("User.REGISTRATION_FAILED", "User registration failed.", errors);
    public static Error RoleAssignmentFailed(Dictionary<string, string[]> errors) => Error.Validation("User.ROLE_ASSIGNMENT_FAILED", "Role assignment failed.", errors);
    public static Error InvalidCredentails => Error.Unauthorized("User.INVALID_CREDENTIALS");
}
