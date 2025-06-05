using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Storage;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

internal sealed record UploadUserAvatarCommand(IFormFile File, Guid UserId) : IRequest<Result>;

internal sealed class UploadUserAvatarCommandHandler(
    IStoreService service) : IRequestHandler<UploadUserAvatarCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(UploadUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var key = $"avatar_{request.UserId}";
        var uri = await service.UploadAsync(request.File, key, Container.Avatar);

        if (string.IsNullOrEmpty(uri))
        {
            return Results.InternalServerError($"Cannot upload avatar of user with ID {request.UserId}");
        }

        var response = new UploadFiledResponse() { Files = [uri] };
        
        return Result.Success(response);
    }
}