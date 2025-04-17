using eShop.Domain.Common.API;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record GetPersonalDataQuery(string Email) : IRequest<Result>;

internal sealed class GetPersonalDataQueryHandler(
    AppManager appManager,
    ICacheService cacheService) : IRequestHandler<GetPersonalDataQuery, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(GetPersonalDataQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"personal-data-{request.Email}";
        var data = await cacheService.GetAsync<PersonalData>(key);

        if (data is null)
        {
            var user = await appManager.UserManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"Cannot find user with email {request.Email}."
                });
            }

            var personalData = await appManager.ProfileManager.FindAsync(user, cancellationToken);

            if (personalData is null)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"Cannot find or user with email {user.Email} has no personal data."
                });
            }

            await cacheService.SetAsync(key, personalData, TimeSpan.FromHours(6));

            return Result.Success(Mapper.ToPersonalDataResponse(personalData));
        }

        return Result.Success(Mapper.ToPersonalDataResponse(data));
    }
}