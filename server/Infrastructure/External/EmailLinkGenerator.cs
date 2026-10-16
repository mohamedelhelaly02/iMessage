using server.Application.Interfaces;

namespace server.Infrastructure.External;

public sealed class EmailLinkGenerator(IConfiguration configuration) : IEmailLinkGenerator
{
    private readonly string clientUrl = configuration["AngularClient"]
         ?? throw new KeyNotFoundException("clientUrl is not configured");

    public string GenerateConfirmEmailLink(string userId, string encodedToken)
        => $"{clientUrl}/auth/confirm-email?userId={userId}&token=${encodedToken}";

}
