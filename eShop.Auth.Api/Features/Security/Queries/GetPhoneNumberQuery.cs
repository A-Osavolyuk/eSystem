using eShop.Domain.Interfaces.API;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record GetPhoneNumberQuery(string Email) : IRequest<Result<GetPhoneNumberResponse>>;

internal sealed class GetPhoneNumberQueryHandler(
    AppManager appManager,
    ICacheService cacheService) : IRequestHandler<GetPhoneNumberQuery, Result<GetPhoneNumberResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result<GetPhoneNumberResponse>> Handle(GetPhoneNumberQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"phone-number-{request.Email}";
        var phoneNumber = await cacheService.GetAsync<string>(key);

        if (string.IsNullOrEmpty(phoneNumber))
        {
            var user = await appManager.UserManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new(new NotFoundException($"Cannot find user with email {request.Email}."));
            }

            await cacheService.SetAsync(key, user.PhoneNumber!, TimeSpan.FromHours(6));

            return new(new GetPhoneNumberResponse()
            {
                PhoneNumber = user.PhoneNumber!
            });
        }

        return new(new GetPhoneNumberResponse()
        {
            PhoneNumber = phoneNumber
        });
    }
}