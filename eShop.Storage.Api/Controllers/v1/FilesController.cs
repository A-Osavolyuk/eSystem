using System.Text.Json;
using eShop.Application.Utilities;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Storage;
using eShop.Storage.Api.Features.Commands;

namespace eShop.Storage.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FilesController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Upload files")]
    [EndpointDescription("Uploads files")]
    [ProducesResponseType(200)]
    [HttpPost("upload")]
    public async ValueTask<IActionResult> UploadFilesAsync(IFormFileCollection files, [FromForm] string metadata)
    {
        var metadataObject = JsonSerializer.Deserialize<Metadata>(metadata)!;
        var response = await sender.Send(new UploadFilesCommand(files, metadataObject));
        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Load files")]
    [EndpointDescription("Load files uris")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<IActionResult> LoadFilesAsync([FromBody] LoadFilesRequest request)
    {
        var response = await sender.Send(new LoadFilesCommand(request));
        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}