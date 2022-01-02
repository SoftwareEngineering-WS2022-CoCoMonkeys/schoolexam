using System.Security.Claims;

namespace SchoolExam.Web.Extensions;

public static class ClaimsPrincipalsExtension
{
    public static string? GetClaim(this ClaimsPrincipal claimsPrincipal, string type)
    {
        return claimsPrincipal.Claims.SingleOrDefault(x => x.Type.Equals(type))?.Value;
    }
}