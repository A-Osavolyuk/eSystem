namespace eShop.Infrastructure.Services;

public class ProfileService(
    IHttpClientService clientService,
    IConfiguration configuration) : IProfileService
{
    public async ValueTask<Response> GetPhoneNumberAsync(string email) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Profile/get-phone-number/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> GetPersonalDataAsync(string email) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Profile/get-personal-data/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> ChangeUserNameAsync(ChangeUserNameRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Profile/change-user-name",
            Methods: HttpMethods.Patch, Data: request));

    public async ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await clientService.SendAsync(
        new Request(Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Profile/change-personal-data",
            Methods: HttpMethods.Put, Data: request));
}