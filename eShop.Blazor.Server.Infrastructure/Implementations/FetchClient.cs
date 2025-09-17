using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class FetchClient(
    IJSRuntime jSRuntime,
    IConfiguration configuration) : IFetchClient
{
    private readonly IJSRuntime jSRuntime = jSRuntime;
    private readonly IConfiguration configuration = configuration;
    private const string GatewayKey = "services:proxy:http:0";

    public async ValueTask<HttpResponse> FetchAsync(FetchOptions options)
    {
        var gatewayUrl = configuration[GatewayKey];
        var url = $"{gatewayUrl}{options.Url}";
        var headers = new Dictionary<string, string>()
        {
            { "Accept", "application/json" },
            { "Content-Type", "application/json" }
        };
        
        var result = await jSRuntime.InvokeAsync<HttpResponse>("fetchApi", new
        {
            url,
            headers,
            method = options.Method.Method,
            credentials = options.Credentials,
            body = options.Body,
        });
        return result;
    }
}