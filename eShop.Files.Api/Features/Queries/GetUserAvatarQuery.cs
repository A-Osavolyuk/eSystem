using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Queries;

internal sealed record GetUserAvatarQuery(Guid UserId) : IRequest<Result>;

internal sealed class GetUserAvatarQueryHandler(IStoreService service)
    : IRequestHandler<GetUserAvatarQuery, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(GetUserAvatarQuery request, CancellationToken cancellationToken)
    {
        var key = request.UserId.ToString();
        var response = await service.FindAsync(key, Container.Avatar);

        if (string.IsNullOrWhiteSpace(response))
        {
            return Results.NotFound($"Cannot get avatar of user with ID {request.UserId}");
        }

        return Result.Success(response);
    }
}