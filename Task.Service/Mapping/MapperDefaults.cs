using Riok.Mapperly.Abstractions;

// SV-01: check that every DTO (target) member is mapped; entity members such as PasswordHash may stay unmapped
[assembly: MapperDefaults(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
