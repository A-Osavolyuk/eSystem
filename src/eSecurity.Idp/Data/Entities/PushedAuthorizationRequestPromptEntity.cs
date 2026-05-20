using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class PushedAuthorizationRequestPromptEntity : Entity
{
    public Guid Id { get; set; }

    public PromptType Prompt { get; set; }

    public Guid RequestId { get; set; }
    public PushedAuthorizationRequestEntity Request { get; set; } = null!;
}