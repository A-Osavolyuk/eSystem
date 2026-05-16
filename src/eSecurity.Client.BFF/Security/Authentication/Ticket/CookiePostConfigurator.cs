using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Security.Authentication.Ticket;

public class CookiePostConfigurator(ITicketStore ticketStore) : IPostConfigureOptions<CookieAuthenticationOptions>
{
    private readonly ITicketStore _ticketStore = ticketStore;

    public void PostConfigure(string? name, CookieAuthenticationOptions options)
    {
        if (name == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            options.SessionStore = _ticketStore;
        }
    }
}