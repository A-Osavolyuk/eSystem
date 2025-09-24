using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserPasswordDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserPasswordDataQueryHandler(IUserManager userManager) : IRequestHandler<GetUserPasswordDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserPasswordDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserPasswordDto()
        {
            HasPassword = user.HasPassword(),
            PasswordChangeDate = user.PasswordChangeDate
        };
        
        return Result.Success(response);
    }
}