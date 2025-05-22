using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Comments;

namespace eShop.Infrastructure.Services;

public class CommentService(
    IApiClient apiClient, 
    IConfiguration configuration) : ApiService(configuration, apiClient), ICommentService
{
    public async ValueTask<Response> GetCommentsAsync(Guid productId) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Comments/get-comments/{productId}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> CreateCommentAsync(CreateCommentRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Comments/create-comment", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> UpdateCommentAsync(UpdateCommentRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Comments/update-comment", Method = HttpMethod.Put, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> DeleteCommentAsync(DeleteCommentsRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Comments/delete-comment", Method = HttpMethod.Delete, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true });

}