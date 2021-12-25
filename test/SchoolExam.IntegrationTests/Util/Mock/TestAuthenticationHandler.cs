using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SchoolExam.IntegrationTests.Util.Mock;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static IEnumerable<Claim> _claims = null!;
    public static string AuthenticationScheme => "Test";

    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public static void SetClaims(params Claim[] claims)
    {
        _claims = claims;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(_claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}