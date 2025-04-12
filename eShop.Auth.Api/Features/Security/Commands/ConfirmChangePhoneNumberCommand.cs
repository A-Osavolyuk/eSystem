using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangePhoneNumberCommandHandler(
    AppManager appManager,
    ITokenHandler tokenHandler)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly ITokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}."
            });
        }

        var result =
            await appManager.SecurityManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber,
                request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error.",
                Details = $"Failed on phone number change with message: {result.Errors.First().Description}"
            });
        }

        user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.NewPhoneNumber);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find user with phone number {request.Request.NewPhoneNumber}."
            });
        }

        var roles = (await appManager.UserManager.GetRolesAsync(user)).ToList();
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user);
        var tokens = await tokenHandler.GenerateTokenAsync(user!, roles, permissions);

        return Result.Success(new ConfirmChangePhoneNumberResponse()
        {
            Message = "Your phone number was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}