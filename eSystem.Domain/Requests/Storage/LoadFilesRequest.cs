using eSystem.Domain.Common.Http;

namespace eSystem.Domain.Requests.Storage;

public class LoadFilesRequest
{
    public required Metadata Metadata { get; set; }
}