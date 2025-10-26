namespace eAccount.Domain.DTOs;

public class ClaimsIdentityDto
{
    public List<ClaimDto> Claims { get; set; } = [];
    public string Scheme { get; set; } = string.Empty;
}