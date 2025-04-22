using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Account;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record SetPersonalDataCommand(SetPersonalDataRequest Request)
    : IRequest<Result>;

internal sealed record SetPersonalDataCommandHandler(
    AppManager appManager) : IRequestHandler<SetPersonalDataCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(SetPersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email: {request.Request.Email}");
        }

        var entity = Mapper.ToPersonalDataEntity(request.Request);
        var result = await appManager.ProfileManager.SetAsync(user, entity, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed to setting personal data with message: {result.Errors.First().Description}");
        }

        return Result.Success("Personal data was successfully set");
    }
}