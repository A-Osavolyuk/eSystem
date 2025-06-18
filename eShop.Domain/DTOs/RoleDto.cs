using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public class RoleDto
{
    public Guid Id { get; init; }
    public string Name  { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}