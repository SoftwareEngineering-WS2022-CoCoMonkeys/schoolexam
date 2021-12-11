using System.Security.Claims;

namespace SchoolExam.SharedKernel.Extensions;

public static class ClaimsPrincipalsExtension
{
    public static string? GetClaim(this ClaimsPrincipal claimsPrincipal, string type)
    {
        return claimsPrincipal.Claims.SingleOrDefault(x => x.Type.Equals(type))?.Value;
    }
}