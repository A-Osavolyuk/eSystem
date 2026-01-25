using eSystem.Core.Http.Results;
using eSystem.Core.Requests;
using eSystem.Core.Responses.Storage;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Features.Commands;

internal sealed record UploadFilesCommand(IFormFileCollection Files, Metadata Metadata)
    : IRequest<Result>;

internal sealed class UploadFilesCommandHandler(
    IStorageManager service) : IRequestHandler<UploadFilesCommand, Result>
{
    private readonly IStorageManager _service = service;

    public async Task<Result> Handle(UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        var metadata = request.Metadata;
        var list = await _service.UploadAsync(request.Files, metadata);

        if (list.Count == 0)
        {
            return Results.InternalServerError($"Cannot upload files of type '{metadata.Type}' with identifier '{metadata.Identifier}'.");
        }

        var response = new UploadFiledResponse { Files = list };
        
        return Results.Ok(response);
    }
}