using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-07
public record ProjectRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(1000, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Description { get; init; }
}
