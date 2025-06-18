using eShop.Domain.Common.API;

namespace eShop.Domain.Requests.API.Storage;

public class LoadFilesRequest
{
    public required Metadata Metadata { get; set; }
}