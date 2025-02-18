namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserRoleCommand(RemoveUserRoleRequest Request) : IRequest<Result<RemoveUserRoleResponse>>;

internal sealed class RemoveUserRoleCommandHandler(
    AppManager appManager) : IRequestHandler<RemoveUserRoleCommand, Result<RemoveUserRoleResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<RemoveUserRoleResponse>> Handle(RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        var isInRole = await appManager.UserManager.IsInRoleAsync(user, request.Request.Role);

        if (!isInRole)
        {
            return new(new BadRequestException(
                $"User with ID {user.Id} not in role {request.Request.Role}"));
        }

        var result = await appManager.UserManager.RemoveFromRoleAsync(user, request.Request.Role);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot remove user with ID {user.Id} from role {request.Request.Role}."));
        }

        return new(new RemoveUserRoleResponse()
        {
            Succeeded = true,
            Message = "User was successfully removed from role."
        });
    }
}