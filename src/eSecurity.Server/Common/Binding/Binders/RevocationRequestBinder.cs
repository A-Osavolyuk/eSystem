using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Revocation;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class RevocationRequestBinder : IFormBinder<RevocationRequest>
{
    public Task<TypedResult<RevocationRequest>> BindAsync(
        IFormCollection form, CancellationToken cancellationToken = default)
    {
        if (!form.TryGetValue("token", out var token))
        {
            return Task.FromResult(TypedResult<RevocationRequest>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "token is required"
            }));
        }

        var result = new RevocationRequest()
        {
            Token = token.ToString(),
            TokenTypeHint = EnumHelper.FromString<TokenTypeHint>(form["token_type_hint"].ToString())
        };
        
        return Task.FromResult(TypedResult<RevocationRequest>.Success(result));
    }
}