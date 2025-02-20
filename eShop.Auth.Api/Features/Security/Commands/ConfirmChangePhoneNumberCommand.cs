namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request)
    : IRequest<Result<ConfirmChangePhoneNumberResponse>>;

internal sealed class ConfirmChangePhoneNumberCommandHandler(
    AppManager appManager,
    ITokenHandler tokenHandler)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result<ConfirmChangePhoneNumberResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly ITokenHandler tokenHandler = tokenHandler;

    public async Task<Result<ConfirmChangePhoneNumberResponse>> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return new(new NotFoundException(
                $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}."));
        }

        var result =
            await appManager.SecurityManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber,
                request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return new(
                new FailedOperationException(
                    $"Failed on phone number change with message: {result.Errors.First().Description}"));
        }

        user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.NewPhoneNumber);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with phone number {request.Request.NewPhoneNumber}."));
        }

        var roles = (await appManager.UserManager.GetRolesAsync(user)).ToList();
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user);
        var tokens = await tokenHandler.GenerateTokenAsync(user!, roles, permissions);

        return new(new ConfirmChangePhoneNumberResponse()
        {
            Message = "Your phone number was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}