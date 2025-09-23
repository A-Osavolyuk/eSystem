namespace eShop.Blazor.Server.Domain.DTOs;

public class ClaimIdentityDto
{
    public List<ClaimDto> Claims { get; set; } = [];
    public string Scheme { get; set; } = string.Empty;
}