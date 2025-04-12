using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class ProfileService(
    IHttpClientService clientService,
    IConfiguration configuration) : Api(configuration, clientService), IProfileService
{
    public async ValueTask<Response> GetPhoneNumberAsync(string email) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Profile/get-phone-number/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> GetPersonalDataAsync(string email) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Profile/get-personal-data/{email}",
            Methods: HttpMethods.Get));

    public async ValueTask<Response> ChangeUserNameAsync(ChangeUserNameRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Profile/change-user-name",
            Methods: HttpMethods.Patch, Data: request));

    public async ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Profile/change-personal-data",
            Methods: HttpMethods.Put, Data: request));
}