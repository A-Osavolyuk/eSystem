using System.Text.Json;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Storage;
using eSystem.Storage.Api.Features.Commands;

namespace eSystem.Storage.Api.Controllers.v1;

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
            s => Ok(HttpResponseBuilder.Create().Succeeded().WithResult(s.Value).WithMessage(s.Message).Build()),
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
            s => Ok(HttpResponseBuilder.Create().Succeeded().WithResult(s.Value).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}