using eShop.Domain.Common.Api;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Commands;

internal sealed record DeleteUserAvatarCommand(Guid UserId) : IRequest<Result>;

internal sealed class DeleteUserAvatarCommandHandler(
    IStoreService service) : IRequestHandler<DeleteUserAvatarCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(DeleteUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        await service.DeleteUserAvatarAsync(request.UserId);

        return Result.Success("User avatar was deleted successfully");
    }
}