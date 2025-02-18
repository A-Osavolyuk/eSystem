namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record DeleteRoleCommand(DeleteRoleRequest Request) : IRequest<Result<DeleteRoleResponse>>;

internal sealed class DeleteRoleCommandHandler(
    AppManager appManager) : IRequestHandler<DeleteRoleCommand, Result<DeleteRoleResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<DeleteRoleResponse>> Handle(DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await appManager.RoleManager.FindByIdAsync(request.Request.Id.ToString());

        if (role is null || role.Id != request.Request.Id.ToString())
        {
            return new(new NotFoundException($"Cannot find role with ID {request.Request.Id}."));
        }

        if (role.Name != request.Request.Name)
        {
            return new Result<DeleteRoleResponse>(new BadRequestException("Role name is different."));
        }

        var result = await appManager.RoleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return new Result<DeleteRoleResponse>(
                new FailedOperationException(
                    $"Cannot delete role due to server error: {result.Errors.First()}."));
        }

        return new(new DeleteRoleResponse() { Message = "Role was successfully deleted.", Succeeded = true });
    }
}