using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Comments;

namespace eShop.Infrastructure.Services;

public class CommentService(
    IHttpClientService httpClient, 
    IConfiguration configuration) : ICommentService, IApi
{
    private readonly IHttpClientService httpClient = httpClient;
    private readonly IConfiguration configuration = configuration;

    public async ValueTask<Response> GetCommentsAsync(Guid productId) => await httpClient.SendAsync(
        new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Comments/get-comments/{productId}", Methods: HttpMethods.Get));

    public async ValueTask<Response> CreateCommentAsync(CreateCommentRequest request) =>
        await httpClient.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Comments/create-comment", Methods: HttpMethods.Post, Data: request));

    public async ValueTask<Response> UpdateCommentAsync(UpdateCommentRequest request) =>
        await httpClient.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Comments/update-comment", Methods: HttpMethods.Put, Data: request));

    public async ValueTask<Response> DeleteCommentAsync(DeleteCommentsRequest request) =>
        await httpClient.SendAsync(new Request(
            Url: $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Comments/delete-comment", Methods: HttpMethods.Delete, Data: request));
}