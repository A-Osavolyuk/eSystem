using eSecurity.Idp.Security.Authorization.Authorize.Par;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Binding;

namespace eSecurity.Idp.Common.Binding.Binders;

public sealed class ParBinder : IFormBinder<PushedAuthorizationRequest>
{
    public async Task<TypedResult<PushedAuthorizationRequest>> BindAsync(
        IFormCollection form, CancellationToken cancellationToken = default)
    {
        if (!form.TryGetValue("response_type", out var responseTypeStringValue))
        {
            return TypedResult<PushedAuthorizationRequest>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "response_type is required"
            });
        }

        var responseTypeString = responseTypeStringValue.ToString();
        if (!EnumHelper.TryParseFromString<ResponseType>(responseTypeString, out var responseType))
        {
            return TypedResult<PushedAuthorizationRequest>.Fail(new Error
            {
                Code = ErrorCode.UnsupportedResponseType,
                Description = "response_type is invalid"
            });
        }

        if (!form.TryGetValue("client_id", out var clientId))
        {
            return TypedResult<PushedAuthorizationRequest>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "client_id is required"
            });
        }
        
        if (!form.TryGetValue("scope", out var scope))
        {
            return TypedResult<PushedAuthorizationRequest>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "scope is required"
            });
        }

        var redirectUri = form["redirect_uri"];
        var nonce = form["nonce"];
        var state = form["state"];
        var prompt = form["prompt"];
        var codeChallenge = form["code_challenge"];
        var codeChallengeMethodString = form["code_challenge_method"].ToString();
        var codeChallengeMethod = EnumHelper.ParseFromString<ChallengeMethod>(codeChallengeMethodString);

        var request = new PushedAuthorizationRequest
        {
            ResponseType = responseType.Value,
            ClientId = clientId.ToString(),
            RedirectUri = redirectUri.ToString(),
            Scope = scope.ToString(),
            Nonce = nonce.ToString(),
            State = state.ToString(),
            Prompt = prompt.ToString(),
            CodeChallenge = codeChallenge.ToString(),
            CodeChallengeMethod = codeChallengeMethod?.Value
        };

        return TypedResult<PushedAuthorizationRequest>.Success(request);
    }
}