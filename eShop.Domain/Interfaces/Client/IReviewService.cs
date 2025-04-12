using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Review;

namespace eShop.Domain.Interfaces.Client;

public interface IReviewService
{
    public Task<Response> GetReviewListByProductIdAsync(Guid id);
    public Task<Response> CreateReviewAsync(CreateReviewRequest request);
    public Task<Response> DeleteReviewsWithProductIdAsync(Guid id);
    public Task<Response> UpdateReviewAsync(UpdateReviewRequest request);
}