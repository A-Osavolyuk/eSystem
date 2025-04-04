namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record CreateRoleCommand(CreateRoleRequest Request) : IRequest<Result>;

internal sealed class CreateRoleCommandHandler(
    AppManager appManager) : IRequestHandler<CreateRoleCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var isRoleExists = await appManager.RoleManager.FindByNameAsync(request.Request.Name);

        if (isRoleExists is not null)
        {
            return Result.Success("Role already exists.");
        }

        var result = await appManager.RoleManager.CreateAsync(new AppRole() { Name = request.Request.Name });

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot create role due to server error: {result.Errors.First().Description}"
            });
        }

        return Result.Success("Role was successfully created");
    }
}