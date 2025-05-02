using Cart;
using eShop.Application;
using Grpc.Net.Client;

namespace eShop.Auth.Api.Rpc;

public class CartClient
{
    public CartClient(ILogger<CartClient> logger, IConfiguration configuration)
    {
        this.logger = logger;
        var uri = configuration["Configuration:Grpc:Servers:CartServer:Uri"] ??
                  throw new NullReferenceException("Not specified RPC server uri");
        var channel = GrpcChannel.ForAddress(uri);
        client = new CartService.CartServiceClient(channel);
    }

    private readonly ILogger<CartClient> logger;
    private readonly CartService.CartServiceClient client;

    public async ValueTask<InitiateUserResponse> InitiateUserAsync(InitiateUserRequest request)
    {
        logger.LogInformation("Calling RPC InitiateUser");

        var response = await client.InitiateUserAsync(request);

        logger.LogInformation("Called RPC InitiateUser");
        return response;
    }
}