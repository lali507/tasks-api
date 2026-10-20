using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Results;
using TaskManager.Service.Mapping;
using TaskManager.Service.Validation;

namespace TaskManager.Service.Services;

// Writes check in this order: user exists (401) -> resource exists (404) -> fields (400) -> business rules (409)
public class LabelService(ILabelRepository labels, IUserRepository users) : ILabelService
{
    // FN-MB-05, FN-AD-05: ordered by name, not paged (FN-MB-05-R1)
    public async Task<ServiceResult<List<LabelResponse>>> GetAllAsync(CancellationToken ct)
    {
        var items = await labels.GetAllAsync(ct);
        return ServiceResult<List<LabelResponse>>.Success(items.Select(l => l.ToResponse()).ToList());
    }

    // FN-AD-12
    public async Task<ServiceResult<LabelResponse>> CreateAsync(int currentUserId, LabelRequest request, CancellationToken ct)
    {
        // GN-09-R1
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<LabelResponse>.Unauthorized();

        var errors = new FieldErrors();
        var name = errors.Text(nameof(request.Name), request.Name, min: 2, max: 30);
        if (errors.HasErrors)
            return ServiceResult<LabelResponse>.Validation(errors.ToDictionary());

        // EN-03-R1
        if (await labels.NameExistsAsync(name, null, ct))
            return ServiceResult<LabelResponse>.Conflict(ErrorMessages.LabelNameTaken);

        var label = new Label { Name = name };
        // GN-12: a parallel request with the same name fails on the unique index
        if (await labels.AddAsync(label, ct) == WriteResult.Duplicate)
            return ServiceResult<LabelResponse>.Conflict(ErrorMessages.LabelNameTaken);

        return ServiceResult<LabelResponse>.Success(label.ToResponse());
    }

    // FN-AD-13: the new name shows on every linked task
    public async Task<ServiceResult<LabelResponse>> UpdateAsync(int currentUserId, int id, LabelRequest request, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<LabelResponse>.Unauthorized();

        var label = await labels.FindAsync(id, ct);
        if (label is null)
            return ServiceResult<LabelResponse>.NotFound();

        var errors = new FieldErrors();
        var name = errors.Text(nameof(request.Name), request.Name, min: 2, max: 30);
        if (errors.HasErrors)
            return ServiceResult<LabelResponse>.Validation(errors.ToDictionary());

        // The same name for the same label is allowed
        if (await labels.NameExistsAsync(name, id, ct))
            return ServiceResult<LabelResponse>.Conflict(ErrorMessages.LabelNameTaken);

        label.Name = name;
        if (await labels.UpdateAsync(label, ct) == WriteResult.Duplicate)
            return ServiceResult<LabelResponse>.Conflict(ErrorMessages.LabelNameTaken);

        return ServiceResult<LabelResponse>.Success(label.ToResponse());
    }

    // FN-AD-14
    public async Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult.Unauthorized();

        var label = await labels.FindAsync(id, ct);
        if (label is null)
            return ServiceResult.NotFound();

        // RE-03: the join table cascades, so the tasks stay without the label
        await labels.DeleteAsync(label, ct);
        return ServiceResult.Success();
    }
}
