using eSecurity.Client.Common.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Security.Authentication;

public sealed class SignInManager(
    NavigationManager navigationManager,
    IOptions<BackendOptions> backendOptions)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly BackendOptions _backendOptions = backendOptions.Value;

    public void SignIn()
    {
        var url = $"{_backendOptions.Uri}/bff/login";
        _navigationManager.NavigateTo(url, true);
    }

    public void SignOut()
    {
        var url = $"{_backendOptions.Uri}/bff/logout";
        _navigationManager.NavigateTo(url, true);
    }
}