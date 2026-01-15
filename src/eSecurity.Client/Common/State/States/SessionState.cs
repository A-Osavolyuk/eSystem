using eSecurity.Core.Security.Cookies;

namespace eSecurity.Client.Common.State.States;

public class SessionState : State
{
    public SessionCookie? Session { get; set; }
}