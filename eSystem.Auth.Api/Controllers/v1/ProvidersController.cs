namespace eSystem.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ProvidersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
}