namespace TaskManager.Api.Auth;

// RL-01 to RL-04: role names for [Authorize(Roles = ...)], the same text as the Role enum in the token
public static class Roles
{
    public const string Admin = nameof(Core.Enums.Role.Admin);
    public const string Member = nameof(Core.Enums.Role.Member);
}
