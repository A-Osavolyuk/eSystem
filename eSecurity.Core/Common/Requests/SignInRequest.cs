using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Core.Common.Requests;

public class SignInRequest
{
    public required SignInPayload Payload { get; set; }
}