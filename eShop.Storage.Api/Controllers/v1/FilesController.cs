using eShop.Application.Utilities;
using eShop.Domain.Common.API;
using eShop.Storage.Api.Features.Commands;
using eShop.Storage.Api.Interfaces;
using Newtonsoft.Json;

namespace eShop.Storage.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FilesController(
    IStoreService storeService,
    ISender sender) : ControllerBase
{
    private readonly IStoreService storeService = storeService;
    private readonly ISender sender = sender;

    [EndpointSummary("Upload files")]
    [EndpointDescription("Uploads files")]
    [ProducesResponseType(200)]
    [HttpPost("upload")]
    public async ValueTask<ActionResult<Response>> UploadProductImagesAsync(IFormFileCollection files, [FromForm] string metadata)
    {
        var metadataObject = JsonConvert.DeserializeObject<Metadata>(metadata)!;
        var response = await sender.Send(new UploadFilesCommand(files, metadataObject));
        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}