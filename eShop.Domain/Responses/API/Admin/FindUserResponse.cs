namespace eShop.Domain.Responses.API.Admin;

public record FindUserResponse
{
    public AccountData AccountData { get; set; } = null!;
    public PersonalData PersonalDataEntity { get; set; } = null!;
    public PermissionsData PermissionsData {  get; set; } = null!; 
}