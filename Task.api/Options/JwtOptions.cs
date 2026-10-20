using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Options;

// GN-04-R2, IN-02: token settings from appsettings; the key only from User Secrets or Jwt__Key (DP-04-R1)
public class JwtOptions
{
    public const string Section = "Jwt";

    // HMAC-SHA256 needs at least 256 bits
    [Required, MinLength(32)]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    // GN-04: 60 minutes
    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; } = 60;
}
