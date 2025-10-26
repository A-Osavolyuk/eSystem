using System.Net;
using eAccount.Infrastructure.Http;
using eAccount.Infrastructure.Security;
using eSystem.Core.Responses.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using HttpRequest = eSystem.Core.Common.Http.HttpRequest;
using HttpResponse = eSystem.Core.Common.Http.HttpResponse;

namespace eAccount.Infrastructure.Implementations;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    TokenProvider tokenProvider,
    NavigationManager navigationManager,
    AuthenticationManager authenticationManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationManager authenticationManager = authenticationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private const string Key = "services:proxy:http:0";

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var message = new HttpRequestMessage();
            
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
            message.RequestUri = new Uri(httpRequest.Url);
            message.Method = httpRequest.Method;
            message.IncludeUserAgent(httpContext);
            message.IncludeCookies(httpContext);
            message.AddContent(httpRequest, httpOptions);

            var httpClient = clientFactory.CreateClient("eShop.Client");
            var httpResponseMessage = await httpClient.SendAsync(message);

            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await authenticationManager.RefreshTokenAsync();
                
                if (!result.Success)
                {
                    tokenProvider.Clear();
                    navigationManager.NavigateTo("/account/login");
                }
                else
                {
                    var response = result.Get<RefreshTokenResponse>()!;
                    tokenProvider.AccessToken = response.AccessToken;
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
                    
                    httpResponseMessage = await httpClient.SendAsync(message);
                }
            }
            
            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
            var httpResponse = JsonSerializer.Deserialize<HttpResponse>(responseJson, serializationOptions)!;

            return httpResponse;
        }
        catch (Exception ex)
        {
            var response = new ResponseBuilder()
                .Failed()
                .WithMessage(ex.Message)
                .Build();

            return response;
        }
    }
}