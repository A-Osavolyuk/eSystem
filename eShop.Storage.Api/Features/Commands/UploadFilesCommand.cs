using eShop.Domain.Common.Http;
using eShop.Domain.Common.Results;
using eShop.Domain.Responses.Storage;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

internal sealed record UploadFilesCommand(IFormFileCollection Files, Metadata Metadata)
    : IRequest<Result>;

internal sealed class UploadFilesCommandHandler(
    IStorageManager service) : IRequestHandler<UploadFilesCommand, Result>
{
    private readonly IStorageManager service = service;

    public async Task<Result> Handle(UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        var metadata = request.Metadata;
        var list = await service.UploadAsync(request.Files, metadata);

        if (list.Count == 0)
        {
            return Results.InternalServerError($"Cannot upload files of type '{metadata.Type}' with identifier '{metadata.Identifier}'.");
        }

        var response = new UploadFiledResponse() { Files = list };
        
        return Result.Success(response);
    }
}