namespace server.Application.Interfaces;

public interface IEmailLinkGenerator
{
    string GenerateConfirmEmailLink(string userId, string encodedToken);
}
