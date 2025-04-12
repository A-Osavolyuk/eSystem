using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request)
    : IRequest<Result>;

internal sealed class ChangePersonalDataCommandHandler(
    AppManager appManager,
    AuthDbContext context) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly AuthDbContext context = context;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var entity = Mapper.ToPersonalDataEntity(request.Request);
        var result = await appManager.ProfileManager.ChangePersonalDataAsync(user, entity);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Failed on changing personal data with message: {result.Errors.First().Description}"
            });
        }

        return Result.Success("Personal data was successfully updated");
    }
}