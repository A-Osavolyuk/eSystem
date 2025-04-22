using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request)
    : IRequest<Result>;

internal sealed class ChangePersonalDataCommandHandler(
    AppManager appManager) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var entity = Mapper.Map(request.Request);
        var result = await appManager.ProfileManager.UpdateAsync(user, entity, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed on changing personal data with message: {result.Errors.First().Description}");
        }

        return Result.Success("Personal data was successfully updated");
    }
}