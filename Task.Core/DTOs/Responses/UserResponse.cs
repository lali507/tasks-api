using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Responses;

// DT-06; never PasswordHash
public record UserResponse
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public Role Role { get; init; }
    public DateTime CreatedAt { get; init; }
}
