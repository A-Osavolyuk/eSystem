using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Storage;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

internal sealed record UploadFilesCommand(IFormFileCollection Files, Metadata Metadata)
    : IRequest<Result>;

internal sealed class UploadFilesCommandHandler(
    IStoreService service) : IRequestHandler<UploadFilesCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        var metadata = request.Metadata;
        var list = await service.UploadRangeAsync(request.Files, metadata.Identifier, metadata.Type);

        if (list.Count == 0)
        {
            return Results.InternalServerError($"Cannot upload files with identifier {metadata.Identifier}.");
        }

        var response = new UploadFiledResponse() { Files = list };
        
        return Result.Success(response);
    }
}