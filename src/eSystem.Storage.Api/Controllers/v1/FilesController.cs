using System.Text.Json;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Requests;
using eSystem.Core.Requests.Storage;
using eSystem.Storage.Api.Features.Commands;

namespace eSystem.Storage.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FilesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [EndpointSummary("Upload files")]
    [EndpointDescription("Uploads files")]
    [ProducesResponseType(200)]
    [HttpPost("upload")]
    public async ValueTask<IActionResult> UploadFilesAsync(IFormFileCollection files, [FromForm] string metadata)
    {
        var metadataObject = JsonSerializer.Deserialize<Metadata>(metadata)!;
        var result = await _sender.Send(new UploadFilesCommand(files, metadataObject));
        return HttpContext.HandleResult(result);
    }
    
    [EndpointSummary("Load files")]
    [EndpointDescription("Load files uris")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<IActionResult> LoadFilesAsync([FromBody] LoadFilesRequest request)
    {
        var result = await _sender.Send(new LoadFilesCommand(request));
        return HttpContext.HandleResult(result);
    }
}