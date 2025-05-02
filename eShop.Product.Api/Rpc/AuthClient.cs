using Auth;

namespace eShop.Product.Api.Rpc;

public class AuthClient
{
    public AuthClient(IConfiguration configuration, ILogger<AuthClient> logger)
    {
        this.logger = logger;
        var uri = configuration["Configuration:Grpc:Servers:AuthServer:Uri"] ??
                  throw new NullReferenceException("Not specified RPC server uri");
        var channel = GrpcChannel.ForAddress(uri);
        client = new AuthService.AuthServiceClient(channel);
    }

    private readonly AuthService.AuthServiceClient client;
    private readonly ILogger<AuthClient> logger;

    public async ValueTask<GetUserResponse> GetUserAsync(Guid userId)
    {
        logger.LogInformation("Calling RPC GetUser");
        var response = await client.GetUserAsync(new GetUserRequest { UserId = userId.ToString() });
        logger.LogInformation("Called RPC GetUser");
        return response;
    }

    public async ValueTask<InitiateSellerResponse> InitiateSellerAsync(Guid userId)
    {
        logger.LogInformation("Calling RPC InitiateSeller");
        var response = await client.InitiateSellerAsync(new InitiateSellerRequest { UserId = userId.ToString() });
        logger.LogInformation("Called RPC InitiateSeller");
        return response;
    }
}