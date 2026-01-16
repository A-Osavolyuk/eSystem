using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public class EmailService(IApiClient apiClient) : IEmailService
{
    private readonly IApiClient _apiClient = apiClient;
    
        public async ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/add"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/check"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/change"
            }, new ApiOptions()
            {

                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/verify"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/manage"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/remove"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Email/reset"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}