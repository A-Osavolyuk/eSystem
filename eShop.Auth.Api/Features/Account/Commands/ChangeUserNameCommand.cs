namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangeUserNameCommand(ChangeUserNameRequest Request)
    : IRequest<Result<ChangeUserNameResponse>>;

internal sealed class ChangeUserNameCommandHandler(
    ITokenHandler tokenHandler,
    AppManager appManager,
    AuthDbContext context) : IRequestHandler<ChangeUserNameCommand, Result<ChangeUserNameResponse>>
{
    private readonly ITokenHandler tokenHandler = tokenHandler;
    private readonly AppManager appManager = appManager;
    private readonly AuthDbContext context = context;

    public async Task<Result<ChangeUserNameResponse>> Handle(ChangeUserNameCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var result = await appManager.UserManager.SetUserNameAsync(user, request.Request.UserName);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot change username of user with email {request.Request.Email} " +
                $"due to error: {result.Errors.First().Description}."));
        }

        user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var roles = (await appManager.UserManager.GetRolesAsync(user)).ToList();
        var permissions = (await appManager.PermissionManager.GetUserPermissionsAsync(user)).ToList();
        var tokens = await tokenHandler.GenerateTokenAsync(user!, roles, permissions);

        return new(new ChangeUserNameResponse()
        {
            Message = "Your user name was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}