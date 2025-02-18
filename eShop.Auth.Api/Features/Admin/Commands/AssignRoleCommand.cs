namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record AssignRoleCommand(AssignRoleRequest Request) : IRequest<Result<AssignRoleResponse>>;

internal sealed class AssignRoleCommandHandler(
    AppManager appManager,
    ILogger<AssignRoleCommandHandler> logger) : IRequestHandler<AssignRoleCommand, Result<AssignRoleResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly ILogger<AssignRoleCommandHandler> logger = logger;

    public async Task<Result<AssignRoleResponse>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await appManager.RoleManager.FindByNameAsync(request.Request.RoleName);
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId.ToString());

        if (role is null)
        {
            return new(new NotFoundException($"Cannot find role with name {request.Request.RoleName}."));
        }

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        var result = await appManager.UserManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot assign role due to server error: {result.Errors.First().Description}"));
        }

        return new(new AssignRoleResponse() { Succeeded = true, Message = "Role was successfully assigned" });
    }
}