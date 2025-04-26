using eShop.Domain.Requests.API.Account;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record SetPersonalDataCommand(SetPersonalDataRequest Request)
    : IRequest<Result>;

internal sealed record SetPersonalDataCommandHandler(
    IProfileManager profileManager,
    UserManager<UserEntity> userManager) : IRequestHandler<SetPersonalDataCommand, Result>
{
    private readonly IProfileManager profileManager = profileManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    public async Task<Result> Handle(SetPersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email: {request.Request.Email}");
        }

        var entity = Mapper.Map(request.Request);
        var result = await profileManager.SetAsync(user, entity, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed to setting personal data with message: {result.Errors.First().Description}");
        }

        return Result.Success("Personal data was successfully set");
    }
}