using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-10
public record LabelRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(30, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string Name { get; init; } = string.Empty;
}
