using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Comments;

namespace eShop.Infrastructure.Services;

public class CommentService(
    IHttpClientService httpClientService, 
    IConfiguration configuration) : ApiService(configuration, httpClientService), ICommentService
{
    public async ValueTask<Response> GetCommentsAsync(Guid productId) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Comments/get-comments/{productId}", 
            Method: HttpMethod.Get));

    public async ValueTask<Response> CreateCommentAsync(CreateCommentRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Comments/create-comment", 
            Method: HttpMethod.Post, Data: request));

    public async ValueTask<Response> UpdateCommentAsync(UpdateCommentRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Comments/update-comment", 
            Method: HttpMethod.Put, Data: request));

    public async ValueTask<Response> DeleteCommentAsync(DeleteCommentsRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Comments/delete-comment", 
            Method: HttpMethod.Delete, Data: request));
}