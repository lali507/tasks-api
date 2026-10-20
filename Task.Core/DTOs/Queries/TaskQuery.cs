using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enums;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Queries;

// GN-02, FN-MB-03: filters for GET /api/tasks, combined with AND (FN-MB-03-R4)
public record TaskQuery : PagingQuery
{
    // Only Open / Taken / Done; a number outside the enum is rejected too
    [EnumDataType(typeof(Status), ErrorMessage = ValidationMessages.InvalidValue)]
    public Status? Status { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.InvalidValue)]
    public int? ProjectId { get; init; }

    // FN-MB-03-R3: only tasks taken by the current user
    public bool Mine { get; init; }
}
