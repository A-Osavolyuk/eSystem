using eSystem.Core.Http.Results;
using eSystem.Core.Requests.Storage;
using eSystem.Core.Responses.Storage;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Features.Commands;

public record LoadFilesCommand(LoadFilesRequest Request) : IRequest<Result>;

public class LoadFilesCommandHandler(IStorageManager storageManager) : IRequestHandler<LoadFilesCommand, Result>
{
    private readonly IStorageManager _storageManager = storageManager;

    public async Task<Result> Handle(LoadFilesCommand request, CancellationToken cancellationToken)
    {
        var metadata = request.Request.Metadata;
        var files = await _storageManager.LoadAsync(metadata);
        var response = new LoadFilesResponse { Files = files };
        return Results.Ok(response);
    }
}