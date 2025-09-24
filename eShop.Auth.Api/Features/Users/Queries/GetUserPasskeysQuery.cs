using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserPasskeysQuery(Guid UserId) : IRequest<Result>;

public class GetUserPasskeysQueryHandler(IUserManager userManager) : IRequestHandler<GetUserPasskeysQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserPasskeysQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = user.Passkeys.Select(passkey =>
            new UserPasskeyDto()
            {
                Id = passkey.Id,
                DisplayName = passkey.DisplayName,
                CreateDate = passkey.CreateDate
            }).ToList();
        
        return Result.Success(response);
    }
}