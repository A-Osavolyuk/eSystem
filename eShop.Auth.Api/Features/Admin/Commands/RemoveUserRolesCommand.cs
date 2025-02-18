namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserRolesCommand(RemoveUserRolesRequest Request)
    : IRequest<Result<RemoveUserRolesResponse>>;

internal sealed class RemoveUserRolesCommandHandler(
    AppManager appManager)
    : IRequestHandler<RemoveUserRolesCommand, Result<RemoveUserRolesResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<RemoveUserRolesResponse>> Handle(RemoveUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        foreach (var role in request.Request.Roles)
        {
            var isInRole = await appManager.UserManager.IsInRoleAsync(user, role);

            if (!isInRole)
            {
                return new(new BadRequestException(
                    $"User with ID {request.Request.UserId} is not in role {role}."));
            }

            var result = await appManager.UserManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return new(new FailedOperationException(
                    $"Cannot remove role from user with ID {request.Request.UserId} " +
                    $"due to server error: {result.Errors.First().Description}."));
            }
        }

        return new(new RemoveUserRolesResponse()
        {
            Succeeded = true,
            Message = "Roles were successfully removed from user"
        });
    }
}