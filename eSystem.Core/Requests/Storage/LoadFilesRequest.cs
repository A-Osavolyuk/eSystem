using eSystem.Core.Common.Http;

namespace eSystem.Core.Requests.Storage;

public class LoadFilesRequest
{
    public required Metadata Metadata { get; set; }
}