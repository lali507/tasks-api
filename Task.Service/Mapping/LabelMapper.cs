using Riok.Mapperly.Abstractions;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;

namespace TaskManager.Service.Mapping;

// SV-01, DT-11
[Mapper]
public static partial class LabelMapper
{
    public static partial LabelResponse ToResponse(this Label label);
}
