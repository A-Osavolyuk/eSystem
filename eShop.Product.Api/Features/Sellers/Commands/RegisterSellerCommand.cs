using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;
using eShop.Product.Api.Entities;
using Results = eShop.Domain.Common.API.Results;

namespace eShop.Product.Api.Features.Sellers.Commands;

internal sealed record RegisterSellerCommand(RegisterSellerRequest Request) : IRequest<Result>;

internal sealed class RegisterSellerCommandHandler(
    AppDbContext context,
    AuthClient client) : IRequestHandler<RegisterSellerCommand, Result>
{
    private readonly AppDbContext context = context;
    private readonly AuthClient client = client;

    public async Task<Result> Handle(RegisterSellerCommand request,
        CancellationToken cancellationToken)
    {
        var userResponse = await client.GetUserAsync(request.Request.UserId);

        if (!userResponse.IsSucceeded)
        {
            return Results.NotFound(userResponse.Message);
        }

        var initiateSellerResponse = await client.InitiateSellerAsync(request.Request.UserId);

        if (!initiateSellerResponse.IsSucceeded)
        {
            return Results.InternalServerError(initiateSellerResponse.Message);
        }

        var entity = new SellerEntity()
        {
            UserId = Guid.Parse(userResponse.User.Id),
            Name = userResponse.User.Username,
            Email = userResponse.User.Email,
            PhoneNumber = userResponse.User.PhoneNumber,
        };

        await context.Sellers.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Seller was successfully registered");
    }
}