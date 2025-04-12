using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Seller;

namespace eShop.Infrastructure.Services;

public class SellerService(IHttpClientService httpClient, IConfiguration configuration) : ISellerService
{
    private readonly IHttpClientService httpClient = httpClient;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> RegisterSellerAsync(RegisterSellerRequest request) =>
        await httpClient.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Seller/register-seller",
            Methods: HttpMethods.Post,
            Data: request));
}