using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserVerificationMethodsQuery(Guid UserId) : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserVerificationMethodsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserVerificationMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = user.VerificationMethods.Select(method =>
            new UserVerificationMethodDto()
            {
                Method = method.Method,
                Preferred = method.Preferred
            });

        return Result.Success(response);
    }
}