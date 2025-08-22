using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class CredentialService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), ICredentialService
{
    public async ValueTask<Response> CreateKeyAsync(CreatePasskeyRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/WebAuthN/credential/options", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> VerifyKeyAsync(VerifyPasskeyRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/WebAuthN/credential/verification", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> CreateAssertionOptionsAsync(
        PasskeySignInRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/WebAuthN/assertion/options", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> VerifyAssertionResponseAsync(
        VerifyPasskeySignInRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/WebAuthN/assertion/verification", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });
}