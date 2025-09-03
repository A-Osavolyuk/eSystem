using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Requests.API.Storage;

public class LoadFilesRequest
{
    public required Metadata Metadata { get; set; }
}