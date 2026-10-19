using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-05; Role cannot be sent or changed (UM-02)
public record UpdateUserRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    [MaxLength(256, ErrorMessage = ValidationMessages.MaxLength)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(50, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string DisplayName { get; init; } = string.Empty;

    // Optional: checked like Password in DT-01 only when sent
    [StringLength(64, MinimumLength = 8, ErrorMessage = ValidationMessages.Length)]
    [RegularExpression(ValidationMessages.PasswordPattern, ErrorMessage = ValidationMessages.PasswordComplexity)]
    public string? NewPassword { get; init; }
}
