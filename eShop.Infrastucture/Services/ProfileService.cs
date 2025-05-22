using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class ProfileService(
    IApiClient pipe,
    IConfiguration configuration) : ApiService(configuration, pipe), IProfileService
{
    public async ValueTask<Response> GetPhoneNumberAsync(string email) => await ApiClient.SendAsync(
        new HttpRequest(Url: $"{Configuration[Key]}/api/v1/Profile/get-phone-number/{email}",
            Method: HttpMethod.Get),
        new HttpOptions() { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> GetPersonalDataAsync(string email) => await ApiClient.SendAsync(
        new HttpRequest(Url: $"{Configuration[Key]}/api/v1/Profile/get-personal-data/{email}",
            Method: HttpMethod.Get),
        new HttpOptions() { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> ChangeUserNameAsync(ChangeUserNameRequest request) => await ApiClient.SendAsync(
        new HttpRequest(Url: $"{Configuration[Key]}/api/v1/Profile/change-user-name",
            Method: HttpMethod.Patch, Data: request),
        new HttpOptions() { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> ChangePersonalDataAsync(ChangePersonalDataRequest request) => await ApiClient.SendAsync(
            new HttpRequest(Url: $"{Configuration[Key]}/api/v1/Profile/change-personal-data",
                Method: HttpMethod.Put, Data: request),
            new HttpOptions() { ValidateToken = true, WithBearer = true });
}