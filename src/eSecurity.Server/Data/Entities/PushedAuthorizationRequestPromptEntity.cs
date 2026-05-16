using eSystem.Core.Server.Data.Entities;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class PushedAuthorizationRequestPromptEntity : Entity
{
    public Guid Id { get; set; }

    public PromptType Prompt { get; set; }

    public Guid RequestId { get; set; }
    public PushedAuthorizationRequestEntity Request { get; set; } = null!;
}