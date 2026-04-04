using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Introspection;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class IntrospectionRequestBinder : IFormBinder<IntrospectionRequest>
{
    public Task<TypedResult<IntrospectionRequest>> BindAsync(
        IFormCollection form, CancellationToken cancellationToken = default)
    {
        if (!form.TryGetValue("token", out var token))
        {
            return Task.FromResult(TypedResult<IntrospectionRequest>.Fail(new Error()
            {
                Code = ErrorType.OAuth.InvalidRequest,
                Description = "token is required"
            }));
        }

        var result = new IntrospectionRequest()
        {
            Token = token.ToString(),
            TokenTypeHint = EnumHelper.FromString<TokenTypeHint>(form["token_type_hint"].ToString())
        };
        
        return Task.FromResult(TypedResult<IntrospectionRequest>.Success(result));
    }
}