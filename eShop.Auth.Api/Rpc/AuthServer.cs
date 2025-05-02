using Auth;
using eShop.Application;
using Grpc.Core;

namespace eShop.Auth.Api.Rpc;

internal sealed class AuthServer(
    ILogger<AuthServer> logger,
    UserManager<UserEntity> userManager) : AuthService.AuthServiceBase
{
    private readonly UserManager<UserEntity> userManager = userManager;
    private const string SellerRole = "Seller";

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogInformation("Executing RPC GetUser");

            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                logger.LogInformation("Executed RPC GetUser with exception");

                return new GetUserResponse()
                {
                    IsSucceeded = false,
                    Message = $"Cannot find user with ID {request.UserId}",
                    User = null
                };
            }

            logger.LogInformation("Executed RPC GetUser");

            return new GetUserResponse()
            {
                IsSucceeded = true,
                User = new UserData()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("Executed RPC GetUser with exception");
            return new GetUserResponse()
            {
                IsSucceeded = false,
                Message = ex.Message,
                User = null
            };
        }
    }

    public override async Task<InitiateSellerResponse> InitiateSeller(InitiateSellerRequest request,
        ServerCallContext context)
    {
        try
        {
            logger.LogInformation("Executing RPC InitiateSeller");
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                logger.LogInformation("Executed RPC InitiateSeller with exception");
                return new InitiateSellerResponse()
                {
                    IsSucceeded = false,
                    Message = $"Cannot find user with ID {request.UserId}"
                };
            }

            var result = await userManager.AddToRoleAsync(user, SellerRole);

            if (!result.Succeeded)
            {
                logger.LogInformation("Executed RPC InitiateSeller with exception");
                return new InitiateSellerResponse()
                {
                    IsSucceeded = false,
                    Message = result.Errors.FirstOrDefault()?.Description
                };
            }

            logger.LogInformation("Executed RPC InitiateSeller");
            return new InitiateSellerResponse()
            {
                IsSucceeded = true,
                Message = "Selle was successfully initiated"
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("Executed RPC InitiateSeller with exception");
            return new InitiateSellerResponse()
            {
                IsSucceeded = false,
                Message = ex.Message,
            };
        }
    }
}