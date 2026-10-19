using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Services;

// CO-04. currentUserId is checked on every write (GN-09-R1).
public interface ILabelService
{
    // FN-MB-05, FN-AD-05: not paged (FN-MB-05-R1)
    Task<ServiceResult<List<LabelResponse>>> GetAllAsync(CancellationToken ct);

    // FN-AD-12
    Task<ServiceResult<LabelResponse>> CreateAsync(int currentUserId, LabelRequest request, CancellationToken ct);

    // FN-AD-13
    Task<ServiceResult<LabelResponse>> UpdateAsync(int currentUserId, int id, LabelRequest request, CancellationToken ct);

    // FN-AD-14
    Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct);
}
