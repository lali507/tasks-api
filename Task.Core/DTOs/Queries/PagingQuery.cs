using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Validation;

namespace TaskManager.Core.DTOs.Queries;

// GN-02: page from 1, pageSize 1–50; out of range -> 400
public record PagingQuery
{
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.InvalidValue)]
    public int Page { get; init; } = 1;

    [Range(1, 50, ErrorMessage = ValidationMessages.InvalidValue)]
    public int PageSize { get; init; } = 10;
}
