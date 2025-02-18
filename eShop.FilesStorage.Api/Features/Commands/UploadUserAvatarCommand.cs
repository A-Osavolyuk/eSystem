using eShop.Domain.Exceptions;
using eShop.Domain.Responses.Api.Files;
using eShop.FilesStorage.Api.Interfaces;

namespace eShop.FilesStorage.Api.Features.Commands;

internal sealed record UploadUserAvatarCommand(IFormFile File, Guid UserId) : IRequest<Result<UploadAvatarResponse>>;

internal sealed class UploadUserAvatarCommandHandler(
    IStoreService service) : IRequestHandler<UploadUserAvatarCommand, Result<UploadAvatarResponse>>
{
    private readonly IStoreService service = service;

    public async Task<Result<UploadAvatarResponse>> Handle(UploadUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var response = await service.UploadUserAvatarAsync(request.File, request.UserId);

        if (string.IsNullOrEmpty(response))
        {
            return new(new FailedOperationException($"Cannot upload avatar of user with ID {request.UserId}"));
        }

        return new(new UploadAvatarResponse()
        {
            Message = "User avatar was uploaded successfully",
            Uri = response
        });
    }
}