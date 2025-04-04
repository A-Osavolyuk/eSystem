namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record AssignRoleCommand(AssignRoleRequest Request) : IRequest<Result>;

internal sealed class AssignRoleCommandHandler(
    AppManager appManager,
    ILogger<AssignRoleCommandHandler> logger) : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly ILogger<AssignRoleCommandHandler> logger = logger;

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await appManager.RoleManager.FindByNameAsync(request.Request.RoleName);
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId.ToString());

        if (role is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = "$Cannot find role with name {request.Request.RoleName}"
            });
        }

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Details = $"Cannot find user with ID {request.Request.UserId}.",
                Message = "Not found"
            });
        }

        var result = await appManager.UserManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Message = "Server error",
                Code = ErrorCode.InternalServerError,
                Details = $"Cannot assign role due to server error: {result.Errors.First().Description}"
            });
        }

        return Result.Success("Role was successfully assigned");
    }
}