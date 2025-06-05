using eShop.Domain.Common.API;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

internal sealed record DeleteUserAvatarCommand(Guid UserId) : IRequest<Result>;

internal sealed class DeleteUserAvatarCommandHandler(
    IStoreService service) : IRequestHandler<DeleteUserAvatarCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(DeleteUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.UserId.ToString(), Container.Avatar);

        return Result.Success("User avatar was deleted successfully");
    }
}