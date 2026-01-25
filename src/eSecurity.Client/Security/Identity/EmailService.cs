using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public class EmailService(IApiClient apiClient) : IEmailService
{
    private readonly IApiClient _apiClient = apiClient;
    
        public async ValueTask<ApiResponse> AddEmailAsync(AddEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/add"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> CheckEmailAsync(CheckEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/check"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/change"
            }, new ApiOptions
            {

                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> VerifyEmailAsync(VerifyEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> ManageEmailAsync(ManageEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/manage"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/remove"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Email/reset"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}