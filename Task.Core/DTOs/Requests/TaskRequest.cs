using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Requests;

// DT-12; no Status or TakenByUserId (DT-12-R3)
public record TaskRequest
{
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(1000, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Description { get; init; }

    // A missing int arrives as 0, so Range is what enforces "required"
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.Required)]
    public int ProjectId { get; init; }

    public List<int>? LabelIds { get; init; }
}
