namespace eShop.Domain.Responses.API.Auth;

public record FindUserResponse
{
    public AccountDataDto AccountDataDto { get; set; } = null!;
    public PersonalDataDto PersonalDataDtoEntity { get; set; } = null!;
    public PermissionsDataDto PermissionsDataDto {  get; set; } = null!; 
}