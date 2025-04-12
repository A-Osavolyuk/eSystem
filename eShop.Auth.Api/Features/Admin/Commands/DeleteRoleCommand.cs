using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record DeleteRoleCommand(DeleteRoleRequest Request) : IRequest<Result>;

internal sealed class DeleteRoleCommandHandler(
    AppManager appManager) : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await appManager.RoleManager.FindByIdAsync(request.Request.Id.ToString());

        if (role is null || role.Id != request.Request.Id)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find role with ID {request.Request.Id}."
            });
        }

        if (role.Name != request.Request.Name)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.BadRequest,
                Message = "Bad request.",
                Details = "Role name is different."
            });
        }

        var result = await appManager.RoleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot delete role due to server error: {result.Errors.First()}."
            });
        }

        return Result.Success("Role was successfully deleted.");
    }
}