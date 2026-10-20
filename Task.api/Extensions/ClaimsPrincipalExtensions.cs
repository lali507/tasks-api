using System.Security.Claims;

namespace TaskManager.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    // GN-04: the user id is the sub claim (JwtBearer maps it to NameIdentifier).
    // A missing id is 0, which no user has, so the Service returns 401 (GN-09-R1).
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"), out var id) ? id : 0;
}
