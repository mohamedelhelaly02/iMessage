namespace server.Application.Interfaces;

public interface IEmailLinkGenerator
{
    string GenerateConfirmEmailLink(string encodedEmail, string encodedToken);
}
