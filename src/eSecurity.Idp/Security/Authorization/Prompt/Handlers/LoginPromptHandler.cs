using eSecurity.Idp.Security.Authentication.Session;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authorization.Prompt.Handlers;

public sealed class LoginPromptHandler(
    IOptions<OpenIdConfiguration> options,
    ISessionAccessor sessionAccessor,
    IPromptStateFactory stateFactory) : IPromptHandler
{
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly IPromptStateFactory _stateFactory = stateFactory;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.Login;

    public async ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        var cookie = _sessionAccessor.GetCookie();
        if (cookie is not null)
            return PromptResult.Next();

        context.Prompts.Remove(PromptType.Login);
        if (context.Prompts.Count == 0)
            context.Prompts.Add(PromptType.None);

        try
        {
            var state = _stateFactory.CreateState(context);
            var url = QueryBuilder.Create()
                .WithUri("https://localhost:6521/login")
                .WithQueryParam("state", state)
                .Build();

            return PromptResult.Success(
                Results.Redirect(RedirectionCode.Found, url)
            );
        }
        catch (Exception)
        {
            var uri = QueryBuilder.Create()
                .WithUri(context.RedirectUri)
                .WithQueryParam("error", ErrorCode.ServerError)
                .WithQueryParam("error_description", "Server error")
                .Build();
            
            return PromptResult.Success(
                Results.Redirect(RedirectionCode.Found, uri)
            );
        }
    }
}