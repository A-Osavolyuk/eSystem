using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request)
    : IRequest<Result>;

internal sealed class ChangePersonalDataCommandHandler(
    AuthDbContext context,
    IUserManager userManager,
    IProfileManager profileManager) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly AuthDbContext context = context;
    private readonly IUserManager userManager = userManager;
    private readonly IProfileManager profileManager = profileManager;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var entity = Mapper.Map(request.Request);
        var result = await profileManager.UpdateAsync(user, entity, cancellationToken);

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