using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class ProfileService(
    IApiClient pipe,
    IConfiguration configuration) : ApiService(configuration, pipe), IProfileService
{


}