using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Responses;

// DT-03; Role is serialized as a string (DT-00-R5)
public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public Role Role { get; init; }
}
