using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Review;

namespace eShop.Infrastructure.Services;

public class ReviewService(
    IApiClient httpClient,
    IConfiguration configuration) : ApiService(configuration, httpClient), IReviewService
{
    public async Task<Response> CreateReviewAsync(CreateReviewRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Reviews/create-review", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async Task<Response> DeleteReviewsWithProductIdAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Reviews/delete-reviews-with-product-id/{id}", Method = HttpMethod.Delete },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async Task<Response> GetReviewListByProductIdAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Reviews/get-reviews-by-product-id/{id}", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async Task<Response> UpdateReviewAsync(UpdateReviewRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Reviews/update-review", Method = HttpMethod.Put, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

}