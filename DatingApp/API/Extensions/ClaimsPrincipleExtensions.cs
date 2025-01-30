using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
    {
        var username = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (username == null) throw new Exception("Could not find username!");
        return username;
    }
}