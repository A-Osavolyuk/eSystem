using System.Security.Claims;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Identity.Email;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Authorization.Scopes.Handlers;

public sealed class EmailScopeHandler(IEmailQueryService emailQueryService) : IScopeHandler
{
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    
    public bool CanHandle(string scope) => scope == ScopeTypes.Email;

    public async ValueTask<TypedResult<List<Claim>>> HandleAsync(ScopeContext context, 
        CancellationToken cancellationToken = default)
    {
        var email = await _emailQueryService.GetByTypeAsync(context.UserId, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return TypedResult<List<Claim>>.Fail(new Error()
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid email"
            });
        }

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Email, email.Email),
            new(AppClaimTypes.EmailVerified, email.IsVerified.ToString())
        };

        return TypedResult<List<Claim>>.Success(claims);
    }
}