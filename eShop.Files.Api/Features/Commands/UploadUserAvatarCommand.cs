using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Files;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Commands;

internal sealed record UploadUserAvatarCommand(IFormFile File, Guid UserId) : IRequest<Result>;

internal sealed class UploadUserAvatarCommandHandler(
    IStoreService service) : IRequestHandler<UploadUserAvatarCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(UploadUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var response = await service.UploadUserAvatarAsync(request.File, request.UserId);

        if (string.IsNullOrEmpty(response))
        {
            return Results.InternalServerError($"Cannot upload avatar of user with ID {request.UserId}");
        }

        return Result.Success(new UploadAvatarResponse()
        {
            Message = "User avatar was uploaded successfully",
            Uri = response
        });
    }
}