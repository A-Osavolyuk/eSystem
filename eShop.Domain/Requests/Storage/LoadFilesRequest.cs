using eShop.Domain.Common.Http;

namespace eShop.Domain.Requests.Storage;

public class LoadFilesRequest
{
    public required Metadata Metadata { get; set; }
}