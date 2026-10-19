using Riok.Mapperly.Abstractions;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;

namespace TaskManager.Service.Mapping;

// SV-01, DT-06: never PasswordHash
[Mapper]
public static partial class UserMapper
{
    public static partial UserResponse ToResponse(this User user);
}
