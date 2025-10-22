using eShop.Auth.Api.Security.Authentication.SignIn;
using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Security.Authentication.SignIn;

namespace eShop.Auth.Api.Features.Security.Commands;

public record SignInCommand(SignInRequest Request) : IRequest<Result>;

public class SignInCommandHandler(ISignInResolver signInResolver) : IRequestHandler<SignInCommand, Result>
{
    private readonly ISignInResolver signInResolver = signInResolver;

    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var type = request.Request.Type;
        if (type == SignInType.LinkedAccount) return Results.BadRequest("Unsupported for manual call");
        
        var credentials = request.Request.Credentials;
        var strategy = signInResolver.Resolve(type);
        return await strategy.SignInAsync(credentials, cancellationToken);
    }
}