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
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email: {request.Request.Email}"
            });
        }

        var entity = Mapper.ToPersonalDataEntity(request.Request);
        var result = await appManager.ProfileManager.SetAsync(user, entity, cancellationToken);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Failed to setting personal data with message: {result.Errors.First().Description}"
            });
        }

        return Result.Success("Personal data was successfully set");
    }
}