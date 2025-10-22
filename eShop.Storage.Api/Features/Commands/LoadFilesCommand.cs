using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Storage;
using eShop.Domain.Responses.Storage;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

public record LoadFilesCommand(LoadFilesRequest Request) : IRequest<Result>;

public class LoadFilesCommandHandler(IStorageManager storageManager) : IRequestHandler<LoadFilesCommand, Result>
{
    private readonly IStorageManager storageManager = storageManager;

    public async Task<Result> Handle(LoadFilesCommand request, CancellationToken cancellationToken)
    {
        var metadata = request.Request.Metadata;
        var files = await storageManager.LoadAsync(metadata);
        var response = new LoadFilesResponse() { Files = files };
        return Result.Success(response);
    }
}