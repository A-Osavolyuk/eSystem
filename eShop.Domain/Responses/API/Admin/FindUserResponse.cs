namespace eShop.Domain.Responses.Api.Admin;

public record class FindUserResponse
{
    public AccountData AccountData { get; set; } = null!;
    public PersonalData PersonalDataEntity { get; set; } = null!;
    public PermissionsData PermissionsData {  get; set; } = null!; 
}