using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Storage;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Queries;

internal sealed record GetUserAvatarQuery(Guid UserId) : IRequest<Result>;

internal sealed class GetUserAvatarQueryHandler(IStoreService service)
    : IRequestHandler<GetUserAvatarQuery, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(GetUserAvatarQuery request, CancellationToken cancellationToken)
    {
        var key = request.UserId.ToString();
        var file = await service.FindAsync(key, Container.Avatar);

        if (string.IsNullOrWhiteSpace(file))
        {
            return Results.NotFound($"Cannot get avatar of user with ID {request.UserId}");
        }

        var response = new LoadFiledResponse() { Files = [file] };

        return Result.Success(file);
    }
}