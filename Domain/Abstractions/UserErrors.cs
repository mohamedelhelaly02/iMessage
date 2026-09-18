namespace Domain.Abstractions;

public static class UserErrors
{
    public static Error NotFound =>
        Error.NotFound(
            "User.NotFound",
            "The requested user was not found.");

    public static Error EmailAlreadyExists
        => Error.Conflict(
            "User.EmailAlreadyExists",
            "A user with this email address already exists.");

    public static Error InvalidCredentials
        => Error.Unauthorized(
            "User.InvalidCredentials",
            "Invalid email or password.");

    public static Error RegisterationValidation(Dictionary<string, string[]>? errors)
        => Error.Validation(
            "User.RegisterationValidation",
            errors: errors);

    public static Error RoleAssignFailed(string message)
       => Error.Validation(
           "User.RoleAssignFailed",
           message);
}