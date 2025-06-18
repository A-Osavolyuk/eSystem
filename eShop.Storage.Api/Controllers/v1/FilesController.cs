using eShop.Application.Utilities;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Storage;
using eShop.Storage.Api.Features.Commands;
using eShop.Storage.Api.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;

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
    public async ValueTask<ActionResult<Response>> UploadFilesAsync(IFormFileCollection files, [FromForm] string metadata)
    {
        var metadataObject = JsonConvert.DeserializeObject<Metadata>(metadata)!;
        var response = await sender.Send(new UploadFilesCommand(files, metadataObject));
        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Load files")]
    [EndpointDescription("Load files uris")]
    [ProducesResponseType(200)]
    [HttpPost("load")]
    public async ValueTask<ActionResult<Response>> LoadFilesAsync([FromBody] LoadFilesRequest request)
    {
        var response = await sender.Send(new LoadFilesCommand(request));
        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}