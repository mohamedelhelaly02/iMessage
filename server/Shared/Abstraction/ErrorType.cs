namespace server.Shared.Abstraction;

public enum ErrorType
{
    NotFound,
    Validation,
    Unauthorized,
    Forbidden,
    Conflict,
    InternalServerError
}
