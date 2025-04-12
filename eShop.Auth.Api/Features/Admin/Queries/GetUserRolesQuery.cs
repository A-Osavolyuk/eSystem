using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

internal sealed class GetUserRolesQueryHandler(
    AppManager appManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Id);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with ID {request.Id}."
            });
        }

        var roleList = await appManager.UserManager.GetRolesAsync(user);

        if (!roleList.Any())
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find roles for user with ID {request.Id}."
            });
        }

        var result = new UserRolesResponse() with { UserId = user.Id };

        foreach (var role in roleList)
        {
            var roleInfo = await appManager.RoleManager.FindByNameAsync(role);

            if (roleInfo is null)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"Cannot find role {role}"
                });
            }

            result.Roles.Add(new RoleData()
            {
                Id = roleInfo.Id,
                Name = roleInfo.Name!,
                NormalizedName = roleInfo.NormalizedName!
            });
        }

        return Result.Success(result);
    }
}