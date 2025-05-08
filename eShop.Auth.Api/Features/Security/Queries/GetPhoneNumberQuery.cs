using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record GetPhoneNumberQuery(string Email) : IRequest<Result>;

internal sealed class GetPhoneNumberQueryHandler(
    IUserManager userManager,
    ICacheService cacheService) : IRequestHandler<GetPhoneNumberQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(GetPhoneNumberQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"phone-number-{request.Email}";
        var phoneNumber = await cacheService.GetAsync<string>(key);

        if (string.IsNullOrEmpty(phoneNumber))
        {
            var user = await userManager.FindByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                return Results.NotFound($"Cannot find user with email {request.Email}.");
            }

            await cacheService.SetAsync(key, user.PhoneNumber!, TimeSpan.FromHours(6));

            return Result.Success(new GetPhoneNumberResponse()
            {
                PhoneNumber = user.PhoneNumber!
            });
        }

        return Result.Success(new GetPhoneNumberResponse()
        {
            PhoneNumber = phoneNumber
        });
    }
}