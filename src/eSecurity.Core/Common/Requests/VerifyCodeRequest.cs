using eSecurity.Core.Security.Authorization.Access;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Core.Common.Requests;

public class VerifyCodeRequest
{
    public required string Subject { get; set; }
    public required string Code { get; set; }
}