using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserEmailsQuery(Guid UserId) : IRequest<Result>;

public class GetUserEmailsQueryHandler(IUserManager userManager) : IRequestHandler<GetUserEmailsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var response = user.Emails.Select(email => new UserEmailDto()
        {
            Id = email.Id,
            Email = email.Email,
            NormalizedEmail = email.NormalizedEmail,
            Type = email.Type,
            IsVerified = email.IsVerified,
            VerifiedDate = email.VerifiedDate,
            UpdateDate = email.UpdateDate
        });

        return Result.Success(response);
    }
}