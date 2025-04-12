using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Product;
using eShop.Product.Api.Features.Products.Commands;
using eShop.Product.Api.Features.Products.Queries;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Get products")]
    [EndpointDescription("Getting all products")]
    [ProducesResponseType(200)]
    [HttpGet("get-products")]
    public async ValueTask<ActionResult<Response>> GetProductsAsync()
    {
        var result = await sender.Send(new GetProductsQuery());

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get product by name")]
    [EndpointDescription("Getting product by name")]
    [ProducesResponseType(200)]
    [HttpGet("get-product-by-name/{name}")]
    public async ValueTask<ActionResult<Response>> GetProductByNameAsync(string name)
    {
        var result = await sender.Send(new GetProductByNameQuery(name));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get product by article")]
    [EndpointDescription("Getting product by article")]
    [ProducesResponseType(200)]
    [HttpGet("get-product-by-article/{article}")]
    public async ValueTask<ActionResult<Response>> GetProductByArticleAsync(string article)
    {
        var result = await sender.Send(new GetProductByArticleQuery(article));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get product by id")]
    [EndpointDescription("Gets product by id")]
    [ProducesResponseType(200)]
    [HttpGet("get-product-by-id/{id:guid}")]
    public async ValueTask<ActionResult<Response>> GetProductByIdAsync(Guid id)
    {
        var result = await sender.Send(new GetProductByIdQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Create product")]
    [EndpointDescription("Creates a product")]
    [ProducesResponseType(200)]
    [HttpPost("create-product")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> CreateProductAsync([FromBody] CreateProductRequest request)
    {
        var result = await sender.Send(new CreateProductCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Update product")]
    [EndpointDescription("Updates the product")]
    [ProducesResponseType(200)]
    [HttpPut("update-product")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> UpdateProductAsync([FromBody] UpdateProductRequest request)
    {
        var result = await sender.Send(new UpdateProductCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete product")]
    [EndpointDescription("Deletes the product")]
    [ProducesResponseType(200)]
    [HttpDelete("delete-product")]
    public async ValueTask<ActionResult<Response>> DeleteProductAsync([FromBody] DeleteProductRequest request)
    {
        var result = await sender.Send(new DeleteProductCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}