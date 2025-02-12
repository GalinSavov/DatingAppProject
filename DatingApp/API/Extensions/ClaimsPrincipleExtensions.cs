using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static int GetId(this ClaimsPrincipal claimsPrincipal)
    {
        var id = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ??
         throw new Exception("Could not find ID!"));
        return id;
    }
    public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
    {
        var username = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        if (username == null) throw new Exception("Could not find username!");
        return username;
    }
}