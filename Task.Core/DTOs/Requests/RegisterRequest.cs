using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-01; no Role field, always Member (DT-01-R2)
public record RegisterRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    [MaxLength(256, ErrorMessage = ValidationMessages.MaxLength)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(50, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string DisplayName { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(64, MinimumLength = 8, ErrorMessage = ValidationMessages.Length)]
    [RegularExpression(ValidationMessages.PasswordPattern, ErrorMessage = ValidationMessages.PasswordComplexity)]
    public string Password { get; init; } = string.Empty;
}
