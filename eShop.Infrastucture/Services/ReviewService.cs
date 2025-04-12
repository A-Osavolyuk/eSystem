using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Review;

namespace eShop.Infrastructure.Services;

public class ReviewService(
    IHttpClientService httpClient, 
    IConfiguration configuration) : ApiService(configuration, httpClient), IReviewService
{

    public async Task<Response> CreateReviewAsync(CreateReviewRequest request) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Reviews/create-review",
            Methods: HttpMethods.Post, Data: request));

    public async Task<Response> DeleteReviewsWithProductIdAsync(Guid id) => await HttpClientService.SendAsync(new Request(
        Url:
        $"{Configuration[Key]}/api/v1/Reviews/delete-reviews-with-product-id/{id}",
        Methods: HttpMethods.Delete));

    public async Task<Response> GetReviewListByProductIdAsync(Guid id) => await HttpClientService.SendAsync(new Request(
        Url: $"{Configuration[Key]}/api/v1/Reviews/get-reviews-by-product-id/{id}",
        Methods: HttpMethods.Get));

    public async Task<Response> UpdateReviewAsync(UpdateReviewRequest request) => await HttpClientService.SendAsync(
        new Request(
            Url: $"{Configuration[Key]}/api/v1/Reviews/update-review",
            Methods: HttpMethods.Put, Data: request));
}