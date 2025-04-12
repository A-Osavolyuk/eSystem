using eShop.Domain.Common.Api;
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
        var response = await service.GetUserAvatarAsync(request.UserId);

        if (string.IsNullOrWhiteSpace(response))
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot get avatar of user with ID {request.UserId}"
            });
        }

        return Result.Success(response);
    }
}