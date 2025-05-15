namespace eShop.Domain.DTOs;

public class UserDto
{
    public AccountDataDto AccountDataDto { get; set; } = null!;
    public PersonalDataDto PersonalDataDto { get; set; } = null!;
    public PermissionsDataDto PermissionsDataDto { get; set; } = null!;
}