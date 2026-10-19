using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-02
public record LoginRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    [MaxLength(256, ErrorMessage = ValidationMessages.MaxLength)]
    public string Email { get; init; } = string.Empty;

    // DT-02-R1: no length or complexity check; a wrong password is a 401
    [Required(ErrorMessage = ValidationMessages.Required)]
    public string Password { get; init; } = string.Empty;
}
